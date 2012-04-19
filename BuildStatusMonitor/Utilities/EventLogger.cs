using System;
using System.Diagnostics;
using System.Text;

namespace BuildStatusMonitor.Utilities
{
    public static class EventLogger {

        private const string EventSourceName = "BuildStatusMonitorService";
        private const string EventLogName = "Service Events";

        /// <summary>
        /// Initializes the <see cref="EventLogger"/> class.
        /// Ensures that there is a EventLog defined and created for this service. 
        /// </summary>
        static EventLogger() {
            if (!EventLog.SourceExists(EventSourceName)) EventLog.CreateEventSource(EventSourceName, EventLogName);
        }

        public static void LogExtended(string data, EventLogEntryType type, byte[] rawData, int id = 0, short category = 0, params string[] options) {
            Console.WriteLine(data);
            EventLog.WriteEntry(EventSourceName, string.Format(data, options), EventLogEntryType.Information, id, category, rawData);
        }

        public static void Log(string data, EventLogEntryType type = EventLogEntryType.Information, params object[] options) {
            Console.WriteLine(data);
            EventLog.WriteEntry(EventSourceName, string.Format(data, options), EventLogEntryType.Information);
        }

        public static void LogVerbose(string data, params object[] options) {
            Console.WriteLine(data,options);
        }

        public static void LogInformation(string data, params object[] options) {
            Log(data, EventLogEntryType.Information, options);
        }
        
        public static void LogWarning(string data, params object[] options) {
            Log(data, EventLogEntryType.Warning, options);
        }

        public static void LogError(string data, params object[] options) {
            Log(data, EventLogEntryType.Error, options);
        }

        public static void LogError(string data, Exception ex, params object[] options) {
            var details = new StringBuilder();
            var exception = ex;
            while (exception != null) {
                details.AppendLine(exception.Message);
                details.AppendLine(exception.Source);
                details.AppendLine(exception.StackTrace);
                details.AppendLine(exception.Data.ToString());
                exception = exception.InnerException;
            }
            var rawData = Encoding.ASCII.GetBytes(details.ToString());
            LogExtended(data, EventLogEntryType.Error, rawData);
        }
    }
}
