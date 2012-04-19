using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using BuildStatusMonitor.Components;
using BuildStatusMonitor.Configuration;
using BuildStatusMonitor.Monitors;
using BuildStatusMonitor.Utilities;
using BuildStatusMonitor.Visualisers;
using Timer = System.Timers.Timer;

namespace BuildStatusMonitor {
    public class BuildStatusController {

        private bool _lastScheduleState = false;
        private bool _polling = false;
        private Schedules _schedules;
        private Timer _timer;
        private BuildStatusConfig _config;
        private List<InstanceController> _controllers;
        private List<string> _visualiser = new List<string>(); 

        /// <summary>
        /// Starts this Master Controller. 
        /// </summary>
        public void Initialise() {

            _controllers = new List<InstanceController>();
            try {
                _config = BuildStatusConfig.Load();
            } catch (Exception ex) {
                throw new LogApplicationException("Could not load the Configuration File.",ex);
            }

            // Using the Data, Load and Instantiate each of the Visualisers and Monitors. 
            // ---------------------------------------------------------------------------
            if (_config != null) {
                FileLogger.Logger.LogVerbose("Loading Controllers.");
                
                foreach (var controller in _config.Controllers) {

                    FileLogger.Logger.LogInformation("Controller: {0} with Monitor={1} and Visualiser={2}", controller.Name, controller.Monitor, controller.Visualiser);

                    if (_visualiser.Contains(controller.Visualiser)) throw new ApplicationException("A visualiser cannot be used more than once.");
                    if (_config.Visualisers[controller.Visualiser] == null) throw new ApplicationException("Invalid Visualiser Specified: " +controller.Visualiser);
                    if (_config.Monitors[controller.Monitor] == null) throw new ApplicationException("Invalid Monitor Specified: " +controller.Monitor);
                    if (_config.Transitions[controller.Transition] == null) throw new ApplicationException("Invalid Transition Specified: " +controller.Transition);

                    var monitorInfo     = _config.Monitors[controller.Monitor];
                    var visualiserInfo  = _config.Visualisers[controller.Visualiser];

                    if (monitorInfo != null && visualiserInfo != null) {
                        var monitor     = ComponentFactory<IMonitor>.CreateComponent(monitorInfo);
                        var visualiser  = ComponentFactory<IVisualiser>.CreateComponent(visualiserInfo);
                        
                        if (monitor != null && visualiser != null) {
                            visualiser.Transitions = _config.Transitions[controller.Transition];
                            _controllers.Add(new InstanceController(controller.Name, monitor, visualiser));
                        }
                    }
                    else {
                        FileLogger.Logger.LogError("Invalid Monitor and/or Visualiser Data in Config file.");
                        throw new ApplicationException("Invalid Monitor and/or Visualiser Data in Config File");
                    }
                }
                _schedules = _config.Schedules;
            }
        }

        /// <summary>
        /// Starts the polling.
        /// </summary>
        public void StartPolling() {

            // Start the Background Worker 
            // ---------------------------------------------------------------------------
            FileLogger.Logger.LogVerbose("Start Polling '{0}' monitors.", _controllers.Count);
            if (_controllers.Count > 0) {
                FileLogger.Logger.LogVerbose("Initialising Background Timer.");
                int pollFrequency = 0;
                try {
                    pollFrequency = Convert.ToInt16(_config.Settings["PollFrequency"]);
                    FileLogger.Logger.LogVerbose("Timer started with a Poll Frequence of {0} seconds.", pollFrequency);
                    _timer = new Timer {Interval = 1000*pollFrequency};
                    _timer.Elapsed += TimerElapsed;
                    _timer.Start();
                }
                catch (Exception ex) {
                    FileLogger.Logger.LogError("Could not start the Background Timer. Poll='{0}'", ex, pollFrequency);
                }
            }
            else {
                FileLogger.Logger.LogError("No configuration data could be loaded.");
            }
        }

        /// <summary>
        /// Timers the elapsed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Timers.ElapsedEventArgs"/> instance containing the event data.</param>
        private void TimerElapsed(object sender, ElapsedEventArgs e) {
            PollMonitors();
        }

        /// <summary>
        /// Polls the monitors and passes the data to the Visualiser.
        /// However, it also checks the schedule and makes sure that the running is within
        /// the specified times (ie: This can turn OFF the lights during the evening and Weekend)
        /// </summary>
        public void PollMonitors() {

            if (!_polling) {
                _polling = true;

                // If the schedule is ON then process the Monitor and Visualiser. If the previous
                // state was that it was OFF then wake up the attached processes. 
                // ------------------------------------------------------------------------------
                if (_schedules.IsScheduleOn()) {

                    // If we were asleep then wake up this Monitor and Visualiser
                    // ----------------------------------------------------------
                    if (!_lastScheduleState) {
                        foreach (var controller in _controllers) {
                            FileLogger.Logger.LogVerbose("Waking Monitor '{0}' and Visualiser '{1}'", controller.Monitor.Name, controller.Visualiser.Name);
                            controller.Visualiser.Wake();
                            controller.Monitor.Wake();
                        }
                        _lastScheduleState = true;
                    }

                    // Process Each Controller
                    // ----------------------------------------------------------
                    FileLogger.Logger.LogVerbose("Polling Monitors {0}", _controllers.Count);
                    foreach (var controller in _controllers) {
                        try {
                            controller.Visualiser.Update(controller.Monitor.Poll());
                        }
                        catch (Exception ex) {
                            FileLogger.Logger.LogError("Error Polling Monitors.", ex);
                        }
                    }
                }
                else {
                    FileLogger.Logger.LogVerbose("In Sleep State - No Polling");

                    // Put the Monitor and Visualiser to sleep
                    // ------------------------------------------------------
                    if (_lastScheduleState) {
                        foreach (var controller in _controllers) {
                            FileLogger.Logger.LogVerbose("Putting Monitor '{0}' and Visualiser '{1}' to sleep.", controller.Monitor.Name, controller.Visualiser.Name);
                            controller.Visualiser.Sleep();
                            controller.Monitor.Sleep();
                        }
                    }
                    _lastScheduleState = false;
                }
                _polling = false;
            } else {
                FileLogger.Logger.LogInformation("Polling called while already polling. Is Poll Frequency too short?");
            }
        }

        /// <summary>
        /// Stops this instance. It will shut down each Visualiser and Monitor and close the Controllers. 
        /// </summary>
        public void Finalise() {
            if (_controllers.Count > 0) {
                foreach (var controller in _controllers) {
                    FileLogger.Logger.LogVerbose("Stopping Controllers, Monitors and Visualisers");
                    controller.Visualiser.Finalise();
                    controller.Monitor.Finalise();
                }
            }
        }

        /// <summary>
        /// Stops this instance. It will shut down each Visualiser and Monitor and close the Controllers. 
        /// </summary>
        public void StopPolling() {
            if (_timer != null) {
                _timer.Stop();
            }
        }
    }

}
