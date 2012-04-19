using System;
using System.Reflection;
using System.Text;
using System.IO;
using System.Threading;

namespace BuildStatusMonitor.Utilities {
    public sealed class FileLogger : IDisposable {

        public enum LogSeverity { VERBOSE = 0, INFORMATION = 1, WARNING = 2, ERROR = 3 }

        // Members
        // -------------------------------------------------
        private static FileLogger _fileLogger;
        private static StreamWriter _logFile;
        private static readonly object _lock = new object();

        private string _logFileName;
        private LogSeverity _logLevel;
        private int _logMaxFileSizeMb;
        private const int DEFAULT_MAX_LOG_SIZE_MB = 20;

        #region Constructors

        private FileLogger() {}

        public void Initialize() {
            Initialize(null, DEFAULT_MAX_LOG_SIZE_MB, LogSeverity.WARNING);
        }

        public void Initialize(string file) {
            Initialize(file, DEFAULT_MAX_LOG_SIZE_MB, LogSeverity.WARNING);
        }

        public void Initialize(string file, LogSeverity loglevel) {
            Initialize(file, DEFAULT_MAX_LOG_SIZE_MB, loglevel);
        }

        public void Initialize(string file, int maxFileSizeMb, LogSeverity logLevel) {
            _logFileName = (string.IsNullOrEmpty(file)) ? Assembly.GetExecutingAssembly().GetName().Name + ".log" : file;
            _logMaxFileSizeMb = maxFileSizeMb == 0 ? DEFAULT_MAX_LOG_SIZE_MB : maxFileSizeMb;
            _logLevel = logLevel;
            _logFile = new StreamWriter(_logFileName, true) {AutoFlush = true};
            IsInitialised = true;
            LogInformation("Log File Initialised: {0} for severity '{1}'", _logFileName, _logLevel);
        }

        public void Terminate() {
            if (_logFile != null) {
                _logFile.Flush();
                _logFile.Close();
                IsInitialised = false;
            }
        }

        public bool IsInitialised { get; private set; }
        public LogSeverity LogLevel { get; set; }

        #endregion

        #region Signleton Management
        public static FileLogger Logger {
            get {
                lock(_lock) {
                    return _fileLogger ?? (_fileLogger = new FileLogger());
                }
            }
        }
        #endregion

        #region Write to the Log File Methods
        private void WrapLogFileOnMaxSize() {
            if (_logFile.BaseStream.Length >= (_logMaxFileSizeMb*1024*1024)) {
                _logFile.Flush();
                _logFile.Close();

                File.Move(_logFileName, _logFileName + DateTime.Now.ToString("yyyyMMddHHmmsss"));
                _logFile = new StreamWriter(_logFileName, true);
            }
        }

        public void LogVerbose(string message) {
            LogMessage(LogSeverity.VERBOSE, message);
        }

        public void LogVerbose(string message, params object[] options) {
            LogMessage(LogSeverity.VERBOSE, string.Format(message,options));
        }

        public void LogVerbose(string message, Exception ex) {
            LogMessage(LogSeverity.VERBOSE, String.Format("{0}\n{1}", message, DecomposeException(ex)));
        }

        public void LogVerbose(Exception ex) {
            LogMessage(LogSeverity.VERBOSE, DecomposeException(ex));
        }

        public void LogInformation(string message) {
            LogMessage(LogSeverity.INFORMATION, message);
        }

        public void LogInformation(string message, params object[] options) {
            LogMessage(LogSeverity.INFORMATION, string.Format(message,options));
        }

        public void LogInformation(string message, Exception ex) {
            LogMessage(LogSeverity.INFORMATION, String.Format("{0}\n{1}", message, DecomposeException(ex)));
        }

        public void LogInformation(Exception ex) {
            LogMessage(LogSeverity.INFORMATION, DecomposeException(ex));
        }

        public void LogWarning(string message) {
            LogMessage(LogSeverity.WARNING, message);
        }

        public void LogWarning(string message, params object[] options) {
            LogMessage(LogSeverity.WARNING, string.Format(message,options));
        }

        public void LogWarning(string message, Exception ex) {
            LogMessage(LogSeverity.WARNING, String.Format("{0}\n{1}", message, DecomposeException(ex)));
        }

        public void LogWarning(Exception ex) {
            LogMessage(LogSeverity.WARNING, DecomposeException(ex));
        }

        public void LogError(string message) {
            LogMessage(LogSeverity.ERROR, message);
        }

        public void LogError(string message, params object[] options) {
            LogMessage(LogSeverity.ERROR, string.Format(message,options));
        }

        public void LogError(string message, Exception ex) {
            LogMessage(LogSeverity.ERROR, String.Format("{0}\n{1}", message, DecomposeException(ex)));
        }

        public void LogError(Exception ex) {
            LogMessage(LogSeverity.ERROR, DecomposeException(ex));
        }

        /// <summary>
        /// Decomposes the exception into printable text
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>a text representation of the Exception.</returns>
        private string DecomposeException(Exception exception) {
            var message = new StringBuilder();
            var ex = exception;
            while (ex != null) {
                message.AppendFormat("Exception   : {0}\n", ex.Message);
                message.AppendFormat("Source      : {0}\n", ex.Source);
                message.AppendFormat("Data        : {0}\n", ex.Data);
                ex = ex.InnerException;
            }
            return message.ToString();
        }

        private void LogMessage(LogSeverity logSeverity, string message) {
            lock(_lock) {
                try {
                    if (_logFile == null) Initialize();
                    if (logSeverity >= _logLevel) {
                        var logMessage = String.Format("{0},[{1}]\t{2}\t:{3}", DateTime.Now.ToString(), Thread.CurrentThread.ManagedThreadId.ToString(), logSeverity, message);
                            WrapLogFileOnMaxSize();
                            _logFile.WriteLine(logMessage);
                            _logFile.Flush();
                        }
                    }
                catch (Exception ex) {
                    throw new ApplicationException("Error with the Logger.\n" + ex.Message, ex );
                }
            }
        }
        #endregion

        #region Implement IDisposable
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // The bulk of the clean-up code is implemented in Dispose(bool)
        private void Dispose(bool disposing)
        {
            if (disposing) {
                if (_logFile != null) {
                    _logFile.Flush();
                    _logFile.Close();
                    _logFile.Dispose();
                }
            }
        }
        #endregion 
    }
}
