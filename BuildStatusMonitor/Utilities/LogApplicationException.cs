using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BuildStatusMonitor.Utilities
{
    [Serializable]
    public class LogApplicationException : ApplicationException
    {
        public LogApplicationException(string message) : base(message) {
            FileLogger.Logger.LogError("Exception Occurred: {0}", message);
        }

        public LogApplicationException(string message, Exception innerException) : base(message, innerException) {
            FileLogger.Logger.LogError("Exception Occurred: {0}", message);
            FileLogger.Logger.LogError(GetExceptionStack(innerException));
        }

        protected LogApplicationException(SerializationInfo info, StreamingContext context) : base(info, context) {}

        private string GetExceptionStack(Exception exception) {
            var details = new StringBuilder();
            var ex = exception;
            while (ex != null) {
                details.AppendLine("Exception : {0}" + ex.Message);
                details.AppendLine("   Source : {0}" + ex.Source);
                details.AppendLine("    Track : {0}" + ex.StackTrace);
                details.AppendLine("     Data : {0}" + ex.Data.ToString());
                ex = ex.InnerException;
            }
            details.AppendLine();
            return details.ToString();
        }
    }
}
