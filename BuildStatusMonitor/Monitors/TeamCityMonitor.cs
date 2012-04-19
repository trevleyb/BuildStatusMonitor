using System;
using System.Collections.Generic;
using System.Linq;
using BuildStatusMonitor.Configuration;
using BuildStatusMonitor.Utilities;
using TeamCitySharp;

namespace BuildStatusMonitor.Monitors
{
    /// <summary>
    /// TeamCityBuild Monitor. 
    /// Needs to know the Host to talk to and connection details.
    /// </summary>
    public class TeamCityMonitor : IMonitor {
        private TeamCityClient _client;
        private List<BuildStatus> _builds;
 
        /// <summary>
        /// Initializes a new instance of the <see cref="TeamCityMonitor"/> class.
        /// </summary>
        public TeamCityMonitor() {
            IsInitialised = false;
        }

        /// <summary>
        /// Initialises this component
        /// </summary>
        /// <param name="settings">The settings collection.</param>
        public void Initialise(string name, Settings settings) {

            FileLogger.Logger.LogInformation("Initialising Monitor: {0}", name);
            if (settings == null || settings.Setting.Count == 0) throw new ApplicationException("No settings provided. This is required to Initialise the component.");
            Name = name;

            // Get the parameters for the TeamCity Connection
            // --------------------------------------------------------------------------------------
            var hostName = settings["Host"] as string;
            if (string.IsNullOrEmpty(hostName)) throw new ApplicationException("Host setting must be specified.");

            var username = settings.Get("User","guest");
            var password = settings.Get("Password","guest");
            var isGuest  = settings.Get("UseGuest",true);

            // Attempt to connect to TeamCity
            // --------------------------------------------------------------------------------------
            try {
                _client = new TeamCityClient(hostName);
                _client.Connect(username, password, isGuest);
            } catch (Exception ex) {
                throw new ApplicationException("Could not connect to TeamCity using the provided credentials.", ex);
            }

            // Build the list of Builds/Projects that we will be monitoring for THIS monitor
            // -----------------------------------------------------------------------------
            _builds = BuildMonitorProjects(settings);
            if (_builds == null || _builds.Count == 0) throw new ApplicationException("No Builds specified to Monitor.");

            IsInitialised = true;
        }

        public void Finalise() { }

        /// <summary>
        /// Builds a list of all the projects that we will be monitoring for THIS monitor. 
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <returns>A list of Build ID's to monitor</returns>
        private List<BuildStatus> BuildMonitorProjects(Settings settings) {
            var buildIDs = new List<BuildStatus>();

            foreach (var setting in settings.Setting) {
                switch (setting.Name) {
                    case "Build":
                    case "BuildID":
                        var buildConfig = _client.BuildConfigByConfigurationId(setting.Value);
                        if (buildConfig == null) throw new ArgumentNullException("Could not find the Build information for " + setting.Value);
                        buildIDs.Add(new BuildStatus(buildConfig.Id, buildConfig.Name, Status.Unknown, null, null));
                        break;
                    case "Project":
                    case "ProjectID":
                        var buildConfigs1 = _client.BuildConfigsByProjectId(setting.Value);
                        foreach (var buildConfig1 in buildConfigs1) {
                            buildIDs.Add(new BuildStatus(buildConfig1.Id, buildConfig1.Name, Status.Unknown, null, null));
                        }
                        break;
                    case "ProjectName":
                        var buildConfigs2 = _client.BuildConfigsByProjectName(setting.Value);
                        foreach (var buildConfig2 in buildConfigs2) {
                            buildIDs.Add(new BuildStatus(buildConfig2.Id, buildConfig2.Name, Status.Unknown, null, null));
                        }
                        break;
                }    
            }

            foreach (var build in buildIDs) FileLogger.Logger.LogInformation("Monitor {0} : Build ID = {1}", Name, build.ID);
            return buildIDs;
        }

        /// <summary>
        /// Returns TRUE if this instance has been initialised. If not, then there is an error
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is initialised; otherwise, <c>false</c>.
        /// </value>
        public bool IsInitialised { get; private set; }

        /// <summary>
        /// Gets the name of this Monitor
        /// </summary>
        public string Name { get; private set; }

        public BuildStatus Poll () {

            FileLogger.Logger.LogVerbose("Polling Monitor: {0}", Name);
            var runningBuilds = _client.BuildsByBuildLocator(BuildLocator.RunningBuilds());
            foreach (var build in _builds) {

                // Mark the current last state for the Build.
                // ------------------------------------------------------------------------------
                DateTime dateTime;
                try {
                    var lastbuild = _client.LastBuildByBuildConfigId(build.ID);
                    switch (lastbuild.Status) {
                        case "SUCCESS":
                            build.Status = Status.Success;
                            break;
                        case "ERROR":
                        case "FAILURE":
                            build.Status = Status.Failed;
                            break;
                        case "UNKNOWN":
                            build.Status = Status.Unknown;
                            break;
                        default:
                            Console.WriteLine(lastbuild.Status);
                            break;
                    }
                    if (!string.IsNullOrEmpty(lastbuild.FinishDate)) {
                        DateTime.TryParse(lastbuild.FinishDate, out dateTime);
                        build.DateTime = dateTime;
                    }
                } catch {
                    // LastBuildByBuildID will throw an exception if no builds have occurred. 
                    // So we need to catch this and simulate an SUCCESS state. 
                    build.Status = Status.Success;
                }

                // Update any which are "RUNNING" Now
                // --------------------------------------------------------------------
                if (runningBuilds != null) {
                    foreach (var runningBuild in runningBuilds) {
                        if (build.ID.Equals(runningBuild.BuildTypeId)) {

                            switch (build.Status) {
                                case Status.Success: 
                                    build.Status = Status.SuccessInProgress;
                                    break;
                                case Status.Failed:
                                    build.Status = Status.FailedInProgress;
                                    break;
                                default:
                                    build.Status = Status.InProgress;
                                    break;
                            }

                            if (!string.IsNullOrEmpty(runningBuild.FinishDate)) {
                                DateTime.TryParse(runningBuild.FinishDate, out dateTime);
                                build.DateTime = dateTime;
                            }
                        }
                    }
                }
            }
            return DetermineMonitorState();
        }

        /// <summary>
        /// Sleeps this instance.
        /// </summary>
        public void Sleep() { }

        /// <summary>
        /// Wakes this instance.
        /// </summary>
        public void Wake() { }

        /// <summary>
        /// Works out the state of the Monitored Builds.
        /// </summary>
        /// <returns>A BuildStatus object that respresents the overall Status of this group of Projects</returns>
        private BuildStatus DetermineMonitorState() {

            // The default result is a Positive OK
            // ---------------------------------------------------------------------
            var result = new BuildStatus(Name, Name, Status.Success, DateTime.Now, null);

            foreach (var buildStatus in _builds) {
                if (buildStatus.Status == Status.Error) {
                    result.Status   = Status.Error;
                    break;
                }

                if (buildStatus.Status != Status.Success) {
                    result.Status   = buildStatus.Status;
                    result.ID       = buildStatus.ID;
                    result.Name     = buildStatus.Name;
                    result.DateTime = buildStatus.DateTime;
                    result.User     = buildStatus.User;
                    break;
                }
            }
            FileLogger.Logger.LogVerbose("Polled: {0} = {1}", result.ID, result.Status);
            return result;
        }
    }
}
