using System;
using System.Globalization;
using System.IO;
using BuildStatusMonitor.Configuration;
using BuildStatusMonitor.Monitors;
using BuildStatusMonitor.Utilities;
using BuildStatusMonitor.Utilities.DelcomLights;

namespace BuildStatusMonitor.Visualisers
{
    public sealed class FileTraceVisualiser : IVisualiser, IDisposable {

        private TransitionController _transitionController;
        private TextWriter _traceFile;
        private Status? _lastStatus = null;

        public void Initialise(string name, Settings settings) {

            FileLogger.Logger.LogInformation("Initialising Visualiser: {0}", name);
            if (settings == null || settings.Setting.Count == 0) throw new LogApplicationException("No settings provided. This is required to Initialise the component.");
            Name = name;

            var fileName = settings["File"] as string;
            if (string.IsNullOrEmpty(fileName)) throw new LogApplicationException("File setting must be specified.");

            _traceFile = new StreamWriter(fileName, true);
            IsInitialised = true;
        }

        public void Finalise() {
            _traceFile.Flush();
            _traceFile.Close();
        }

        public void Sleep() {
            _traceFile.Flush();
            _lastStatus = Status.Unknown;
        }

        public void Wake() { }

        public Transition Transitions { 
            set { _transitionController = new TransitionController(Name, value);}
        }

        public bool IsInitialised { get; private set; }

        public string Name { get; private set; }

        public void Update(BuildStatus status) {
            if (_transitionController != null) UpdateWithTransition(status); else UpdateWithoutTransition(status);
        }

        private void UpdateWithTransition(BuildStatus status) {
            var actions = _transitionController.Transition(status);
            if (actions != null) {
                _traceFile.WriteLine("{0}\t {1}\tStatus = '{2}'\tAction='{3}'", DateTime.Now.ToString(CultureInfo.InvariantCulture), status.Name, status.Status, actions);
                _traceFile.Flush();
            }
        }

        private void UpdateWithoutTransition(BuildStatus status) {
            if (_lastStatus == null || _lastStatus != status.Status) {
                FileLogger.Logger.LogInformation("{0}\t {1}\tStatus = '{2}'", DateTime.Now.ToString(CultureInfo.InvariantCulture), status.Name, status.Status);
                _traceFile.WriteLine("{0}\t {1}\tStatus = '{2}'", DateTime.Now.ToString(CultureInfo.InvariantCulture), status.Name, status.Status);
                _traceFile.Flush();
                _lastStatus = status.Status;
            }
        }

        #region Implement IDisposable
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // The bulk of the clean-up code is implemented in Dispose(bool)
        private void Dispose(bool disposing)
        {
            if (disposing) {
                if (_traceFile != null) {
                    _traceFile.Flush();
                    _traceFile.Close();
                    _traceFile.Dispose();
                }
            }
        }
        #endregion 

    }
}
