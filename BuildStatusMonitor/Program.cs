using System;
using System.Configuration.Install;
using System.IO;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;
using BuildStatusMonitor.Configuration;
using BuildStatusMonitor.Utilities;
using BuildStatusMonitor.Utilities.DelcomLights;

namespace BuildStatusMonitor {
    class Program : ServiceBase {
        private static readonly BuildStatusController Controller = new BuildStatusController();

        Program() {
            InitialiseLogFile();
            FileLogger.Logger.LogInformation("Build Status Monitor Running.");
            FileLogger.Logger.LogInformation("Current Directory is '{0}'", Directory.GetCurrentDirectory());
            ServiceName = "BuildStatusMonitorService";
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main(string[] args) {
            InitialiseLogFile();
            FileLogger.Logger.LogInformation("BuildStatusMonitor Application Starting.");
            FileLogger.Logger.LogInformation("Current Directory is '{0}'", Directory.GetCurrentDirectory());
            FileLogger.Logger.LogInformation("Assembly Directory is '{0}'", Assembly.GetExecutingAssembly().Location);
            FileLogger.Logger.LogInformation("Logging the attached Delcom Lights.");
            foreach (var light in DelcomManager.GetDevices()) {
                FileLogger.Logger.LogInformation(light);
            }

            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;
            if (Environment.UserInteractive) {
                var parameter = string.Concat(args);
                try {
                    switch (parameter) {
                        case "--install":
                            FileLogger.Logger.LogInformation("Installing Service.");
                            ManagedInstallerClass.InstallHelper(new[] {Assembly.GetExecutingAssembly().Location});
                            break;
                        case "--uninstall":
                            FileLogger.Logger.LogInformation("UnInstalling Service.");
                            ManagedInstallerClass.InstallHelper(new[] {"/u", Assembly.GetExecutingAssembly().Location});
                            break;
                        case "--run":
                            FileLogger.Logger.LogInformation("Running in Test Mode.");
                            var controller = new BuildStatusController();
                            controller.Initialise();
                            controller.PollMonitors();
                            controller.Finalise();
                            break;
                        case "--default":
                            FileLogger.Logger.LogInformation("Creating DEFAULT settings file.");
                            BuildStatusConfig.CreateDefaultConfigFile();
                            break;
                        default:
                            Console.WriteLine("BuildStatusMonitor");
                            Console.WriteLine("--install     To install as a service.");
                            Console.WriteLine("--uninstall   To uninstall the service.");
                            Console.WriteLine("--run         To execute a single poll/execution.");
                            Console.WriteLine("--default     To create a sample default config file.");
                            Console.WriteLine("");
                            Console.WriteLine("Attached Lights are:");
                            foreach (var light in DelcomManager.GetDevices()) {
                                Console.WriteLine(light);
                            }
                            break;
                    }
                }
                catch (Exception ex) {
                    FileLogger.Logger.LogError("Error starting the Application", ex);
                }
            }
            else {
                Run(new Program());
            }
            FileLogger.Logger.LogInformation("BuildStatusMonitor Application Finished.");
            FileLogger.Logger.Terminate();
        }

        private static void InitialiseLogFile() {
            try {
                if (!FileLogger.Logger.IsInitialised) {
                    FileLogger.LogSeverity logSeverity;
                    if (!Enum.TryParse(Properties.Settings.Default.LogLevel, true, out logSeverity)) logSeverity = FileLogger.LogSeverity.WARNING;
                    FileLogger.Logger.Initialize(Properties.Settings.Default.LogFileName, Properties.Settings.Default.LogFileSize, logSeverity);
                }
            } catch (Exception ex) {
                EventLogger.LogError("Could not initialise the File logger", ex);
                throw new ApplicationException("Could not initialise the File logger.",ex);
            }
        }

        /// <summary>
        /// Handles and Logs a domain unhandled exception.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.UnhandledExceptionEventArgs"/> instance containing the event data.</param>
        private static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e) {
            InitialiseLogFile();
            FileLogger.Logger.LogError("Unhandled Exception", e.ExceptionObject as Exception);
        }

        /// <summary>
        /// When implemented in a derived class, executes when a Start command is sent to the service by 
        /// the Service Control Manager (SCM) or when the operating system starts (for a service that starts automatically). 
        /// Specifies actions to take when the service starts.
        /// </summary>
        /// <param name="args">Data passed by the start command.</param>
        protected override void OnStart(string[] args) {
            InitialiseLogFile();
            FileLogger.Logger.LogInformation("BuildStatusMonitor: Service Starting.");
            base.OnStart(args);
            Controller.Initialise();
            Controller.StartPolling();
        }

        /// <summary>
        /// When implemented in a derived class, executes when a Stop command is sent to the service by 
        /// the Service Control Manager (SCM). Specifies actions to take when a service stops running.
        /// </summary>
        protected override void OnStop() {
            InitialiseLogFile();
            FileLogger.Logger.LogInformation("BuildStatusMonitor: Service Stopping.");
            Controller.Finalise();
            Controller.StopPolling();
            base.OnStop();
        }
    }
}