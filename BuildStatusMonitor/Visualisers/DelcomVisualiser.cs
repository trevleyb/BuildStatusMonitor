using System;
using BuildStatusMonitor.Configuration;
using BuildStatusMonitor.Monitors;
using BuildStatusMonitor.Utilities;
using BuildStatusMonitor.Utilities.DelcomLights;

namespace BuildStatusMonitor.Visualisers
{
    /// <summary>
    /// Visualiser for the Delcom Lights
    /// </summary>
    public class DelcomVisualiser : IVisualiser {

        private TransitionController _transitionController;
        private Status? _lastStatus;
        private DelcomController _controller;

        public void Initialise(string name, Settings settings) {

            FileLogger.Logger.LogInformation("Initialising Visualiser: {0}", name);
            if (settings == null || settings.Setting.Count == 0) throw new LogApplicationException("No settings provided. This is required to Initialise the component.");
            Name = name;

            // Get the parameters for the TeamCity Connection
            // --------------------------------------------------------------------------------------
            var deviceID = settings["DeviceID"] as string;
            if (string.IsNullOrEmpty(deviceID)) throw new LogApplicationException("DeviceID setting must be specified or set to AUTO.");

            // Initialise the Light Controller
            // --------------------------------------------------------------------------------------
            FileLogger.Logger.LogInformation("Creating Controller with ID: {0}", deviceID);
            _controller = new DelcomController(deviceID);
            _controller.Off();
            IsInitialised = true;
        }

        public void Finalise() {
            _controller.Off();
        }

        public void Sleep() {
            _controller.Off();
            _lastStatus = Status.Unknown;
            _transitionController.Clear();
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
                _controller.Off();
                foreach (var action in actions.Split(';')) {
                    var light = DetermineLight(action);
                    if (light !=null) _controller.SetColor(light);
                }
            }
        }

        private DelcomLight DetermineLight(string action) {
            var settings = action.Split(':');
            if (settings.Length == 2) {
                return new DelcomLight(settings[0], settings[1]);
            }
            return null;
        }

        private void UpdateWithoutTransition(BuildStatus status) {
            if (_lastStatus == null || _lastStatus != status.Status) {
                FileLogger.Logger.LogInformation("Publishing to Visualiser: {0} with a status of {1}", Name, status.Status);
                switch (status.Status) {
                    case Status.Success:
                        _controller.SetColor(new DelcomLight(Colors.Green, Modes.On));
                        break;
                    case Status.Unknown:
                        _controller.SetColor(new DelcomLight(Colors.Yellow, Modes.Flash));
                        break;
                    case Status.InProgress:
                        _controller.SetColor(new DelcomLight(Colors.Yellow, Modes.On));
                        break;
                    case Status.Error:
                        _controller.SetColor(new DelcomLight(Colors.Red, Modes.Flash));
                        break;
                    case Status.Failed:
                        _controller.SetColor(new DelcomLight(Colors.Red, Modes.On));
                        break;
                    case Status.FailedInProgress:
                        _controller.SetColor(new DelcomLight(Colors.Red, Modes.On), new DelcomLight(Colors.Yellow, Modes.Flash));
                        break;
                    case Status.SuccessInProgress:
                        _controller.SetColor(new DelcomLight(Colors.Green, Modes.On), new DelcomLight(Colors.Yellow, Modes.Flash));
                        break;
                }
                _lastStatus = status.Status;
            }
        }
    }
}
