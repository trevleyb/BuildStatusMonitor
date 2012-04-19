using System;
using System.IO;
using System.Reflection;
using System.Xml.Schema;
using System.Xml.Serialization;
using BuildStatusMonitor.Utilities;

namespace BuildStatusMonitor.Configuration {
    [Serializable]
    public class BuildStatusConfig {
        private const string DefaultConfigFileName = "BuildStatusMonitor.Settings.xml";

        public BuildStatusConfig() {
            Settings    = new Settings();
            Schedules   = new Schedules();
            Controllers = new Controllers();
            Monitors    = new Monitors();
            Visualisers = new Visualisers();
            Transitions = new Transitions();
        }

        #region Data elements stored in the Configuration File
        [XmlElement("Settings", typeof (Settings))]
        public Settings Settings { get; set; }

        [XmlElement("Schedules", typeof (Schedules))]
        public Schedules Schedules { get; set; }

        [XmlElementAttribute("Controllers", typeof (Controllers), Form = XmlSchemaForm.Unqualified)]
        public Controllers Controllers { get; set; }

        [XmlElementAttribute("Monitors", typeof (Monitors), Form = XmlSchemaForm.Unqualified)]
        public Monitors Monitors { get; set; }

        [XmlElementAttribute("Visualisers", typeof (Visualisers), Form = XmlSchemaForm.Unqualified)]
        public Visualisers Visualisers { get; set; }

        [XmlElementAttribute("Transitions", typeof (Transitions), Form = XmlSchemaForm.Unqualified)]
        public Transitions Transitions { get; set; }
        #endregion

        #region Load and Save the Settings through Serialization

        /// <summary>
        /// Saves the settings Data to the serialised File.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public void Save(string fileName = null) {
            try {
                using (var fs = new FileStream(fileName ?? GetConfigFileName(fileName), FileMode.Create)) {
                    var serializer = new XmlSerializer(typeof (BuildStatusConfig));
                    serializer.Serialize(fs, this);
                }
            } catch (Exception ex) {
                throw new LogApplicationException("Could not Save Configuration Instance Data.", ex);
            }
        }

        /// <summary>
        /// Loads the specified config file back into the class structure.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public static BuildStatusConfig Load(string fileName = null) {
            try {
                using (var fs = new FileStream(GetConfigFileName(fileName), FileMode.Open)) {
                    var serializer = new XmlSerializer(typeof (BuildStatusConfig));
                    return (BuildStatusConfig) serializer.Deserialize(fs);
                }
            } catch (Exception ex) {
                throw new LogApplicationException("Could not LOAD the Configuration Data.", ex);
            }
        }

        #endregion

        #region Helper Functions
        /// <summary>
        /// Gets the name of the config file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        private static string GetConfigFileName(string fileName) {
            if (string.IsNullOrEmpty(fileName)) {
                fileName = string.IsNullOrEmpty(Properties.Settings.Default.ConfigurationFile) ? DefaultConfigFileName : Properties.Settings.Default.ConfigurationFile;
            }

            // If we have a valid FileName at this point, return it (a filename was provided of the default is corect)
            // -------------------------------------------------------------------------------------------------------
            FileLogger.Logger.LogVerbose("Looking for Configuration File: {0} = {1}", fileName, File.Exists(fileName) ? "Found" : "Not Found");
            if (File.Exists(fileName)) return fileName;

            if (!string.IsNullOrEmpty(Path.GetFileName(fileName))) {

                // If not, use the filename provided but look for it in the current directory
                // --------------------------------------------------------------------------------------------------------
                fileName = Path.Combine(Directory.GetCurrentDirectory(), Path.GetFileName(fileName));
                FileLogger.Logger.LogVerbose("Looking for Configuration File: {0} = {1}", fileName, File.Exists(fileName) ? "Found" : "Not Found");
                if (File.Exists(fileName)) return fileName;
                
                // If not, then try to find it in the directory/folder where the application is resided 
                // ---------------------------------------------------------------------------------------------------------
                fileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Path.GetFileName(fileName));
                FileLogger.Logger.LogVerbose("Looking for Configuration File: {0} = {1}", fileName, File.Exists(fileName) ? "Found" : "Not Found");
                if (File.Exists(fileName)) return fileName;
            }
            throw new LogApplicationException("Cannot find a Valid BuildStatusMonitor configuration file. ");
        }
        #endregion 

        #region Create Default Sample Configuration File

        /// <summary>
        /// Creates the default configuration File
        /// </summary>
        public static void CreateDefaultConfigFile() {

            var config = new BuildStatusConfig();

            #region Add Default Core Data (Schedule and Controllers)
            config.Settings.Add(new Setting("PollFrequency","5"));
            config.Schedules.DefaultState = "OFF";
            config.Schedules.Add(new Schedule("6:00AM", "8:00PM", "ON", "Week"));
            config.Schedules.Add(new Schedule("12:00AM","11:59PM", "OFF", "Weekend"));
            config.Controllers.Add(new Controller("Controller1", "Monitor1","Visualiser1","Transition1"));
            config.Controllers.Add(new Controller("Controller2", "Monitor1","Visualiser2","Transition2"));
            #endregion 

            #region Add the Sample Monitors
            var settings1 = new Settings();
            settings1.Add(new Setting("Host","yourhost:8080"));
            settings1.Add(new Setting("User", "guest"));
            settings1.Add(new Setting("Password", "guest"));
            settings1.Add(new Setting("UseGuest", "true"));
            settings1.Add(new Setting("ProjectID", "project2"));
            settings1.Add(new Setting("ProjectID", "project6"));
            settings1.Add(new Setting("ProjectID","project4"));
            config.Monitors.Add(new Monitor("Monitor1", "BuildStatusMonitor", "BuildStatusMonitor.Monitors.TeamCityMonitor",settings1));
 
            var settings2 = new Settings();
            settings2.Add(new Setting("Host","yourhost:8080"));
            settings2.Add(new Setting("User", "guest"));
            settings2.Add(new Setting("Password", "guest"));
            settings2.Add(new Setting("UseGuest", "true"));
            settings2.Add(new Setting("ProjectID", "projectID"));
            settings2.Add(new Setting("BuildID", "buildID"));
            settings2.Add(new Setting("ProjectName","projectName"));
            config.Monitors.Add(new Monitor("Monitor2", "BuildStatusMonitor", "BuildStatusMonitor.Monitors.TeamCityMonitor",settings2));
            #endregion 

            #region Add Sample Visualisers
            var settings3 = new Settings();
            settings3.Add(new Setting("DeviceID",@"\\?\hid#vid_0fc5&amp;pid_b080#7&amp;141d6465&amp;0&amp;0000#{4d1e55b2-f16f-11cf-88cb-001111000030}"));
            config.Visualisers.Add(new Visualiser("DelcomLight#1","BuildStatusMonitor","BuildStatusMonitor.Visualisers.DelcomVisualiser",settings3));
        
            var settings4 = new Settings();
            settings4.Add(new Setting("DeviceID",@"\\?\hid#vid_0fc5&amp;pid_b080#7&amp;26d7150a&amp;0&amp;0000#{4d1e55b2-f16f-11cf-88cb-001111000030}"));
            config.Visualisers.Add(new Visualiser("DelcomLight#2","BuildStatusMonitor","BuildStatusMonitor.Visualisers.DelcomVisualiser",settings4));
        
            var settings5 = new Settings();
            settings5.Add(new Setting("File",@"C:\Status.Log"));
            config.Visualisers.Add(new Visualiser("FileTrace","BuildStatusMonitor","BuildStatusMonitor.Visualisers.FileTraceVisualiser",settings5));
            #endregion 

            #region Add Transitions

            var transition1 = config.Transitions.Add(new Transition("Transition1"));
            var t1s1 = transition1.Add(new State("Unknown","YELLOW:ON",null));
            var t1s2 = transition1.Add(new State("Success","GREEN:ON"));
            t1s2.SubState.Add(new SubState("Success1","TIME:0-15","GREEN:FLASH"));
            var t1s3 = transition1.Add(new State("SuccessInProgress","GREEN:ON;YELLOW:FLASH",null));
            var t1s4 = transition1.Add(new State("Failed","RED:ON"));
            t1s4.SubState.Add(new SubState("Failed1","TIME:0-15","RED:FLASH-SLOW"));
            t1s4.SubState.Add(new SubState("Failed2","TIME:15-30","RED:FLASH"));
            t1s4.SubState.Add(new SubState("Failed3","TIME:30-60","RED:FLASH-FAST"));
            var t1s5 = transition1.Add(new State("FailedInProgress","RED:ON;YELLOW:FLASH",null));
            var t1s6 = transition1.Add(new State("InProgress","YELLOW:FLASH",null));
            var t1s7 = transition1.Add(new State("Error","RED:FLASH",null));

            var transition2 = config.Transitions.Add(new Transition("Transition2"));
            var t2s1 = transition2.Add(new State("Unknown","Build State is UNKNOWN",null));
            var t2s2 = transition2.Add(new State("Success","Build was Sucessfull"));
            t2s2.SubState.Add(new SubState("Success1","TIME:0-15","Build Sucessfull - Ready For QA"));
            var t2s3 = transition2.Add(new State("SuccessInProgress","Last build was Sucessfull. New Build in Progress",null));
            var t2s4 = transition2.Add(new State("Failed","Build Failed."));
            t2s4.SubState.Add(new SubState("Failed1","TIME:0-15","Build Failed: First Warning"));
            t2s4.SubState.Add(new SubState("Failed2","TIME:15-30","Build Failed: Second Warning"));
            t2s4.SubState.Add(new SubState("Failed3","TIME:30-60","Build Failed: Third Warning"));
            var t2s5 = transition2.Add(new State("FailedInProgress","Build Failed - New Build in Progress",null));
            var t2s6 = transition2.Add(new State("InProgress","Build in Progress - Previus State is Unknown",null));
            var t2s7 = transition2.Add(new State("Error","WARNING: Unknown Error Has Occurred",null));
            #endregion

            config.Save("BuildStatusMonitor.settings.sample.xml");
        }

  







        #endregion 

    }
}