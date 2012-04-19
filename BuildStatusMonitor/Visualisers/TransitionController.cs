using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BuildStatusMonitor.Configuration;
using BuildStatusMonitor.Monitors;
using BuildStatusMonitor.Utilities;

namespace BuildStatusMonitor.Visualisers
{
    public class TransitionController {

        private readonly string   _visualiser;
        private DateTime? _lastChange;
        private State    _priState;
        private SubState _subState;
        
        private Transition _transition;
        public TransitionController(string visualiser, Transition transition) {
            _priState   = null;
            _subState   = null;
            _transition = transition;
            _visualiser = visualiser;
        }

        public void Clear() {
            _priState   = null;
            _subState   = null;
            _lastChange = null;
        }

        public string Transition(BuildStatus status) {

            // Find the Primary State that matches the Input State
            // ------------------------------------------------------
            var activeState = _transition.States.FirstOrDefault(state => state.Name.ToUpper().Equals(status.Status.ToString().ToUpper()));
            if (activeState == null) return null;

            // Look for a SubState which matches the conditions
            // ------------------------------------------------------
            var activeSubState = activeState.SubState.FirstOrDefault(subState => subState.Evaluate(_lastChange));

            FileLogger.Logger.LogInformation("{3}: State='{0}' and SubState='{1}' and Change='{2}' and LastDate='{4}'", activeState.Name, activeSubState == null ? "None" : activeSubState.Name, activeState != _priState || activeSubState != _subState ? "YES" : "NO", _visualiser, _lastChange);

            if (activeState != _priState || activeSubState != _subState) {
                if (activeState != _priState) _lastChange = DateTime.Now;
                _priState = activeState;
                _subState = activeSubState;
                if (activeSubState != null) return activeSubState.Action;
                return activeState.Action;
            }
            return null;
        }
    }
}
