using System;
using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;
using BuildStatusMonitor.Utilities;

namespace BuildStatusMonitor
{
    [RunInstaller(true)]
    public class WindowsServiceInstaller : Installer {
        public WindowsServiceInstaller() {
            try {
                var serviceInstaller = new ServiceInstaller();

                //must be the same as what was set in Program's constructor             
                serviceInstaller.ServiceName = "BuildStatusMonitor";
                serviceInstaller.DisplayName = "Build Status Monitor";
                serviceInstaller.Description = "Monitors Build Servers and reports Build Events to Visualisers.";
                serviceInstaller.StartType   = ServiceStartMode.Automatic;

                var processInstaller = new ServiceProcessInstaller();
                processInstaller.Account     = ServiceAccount.LocalSystem;

                Installers.Add(serviceInstaller);
                Installers.Add(processInstaller);
            } catch (Exception ex) {
                FileLogger.Logger.LogError("Could not Install/UnInstall the service.", ex); 
            } 
        }
    }
}