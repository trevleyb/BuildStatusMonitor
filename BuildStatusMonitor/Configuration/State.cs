using System;
using System.Collections.Generic;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BuildStatusMonitor.Configuration {
    [Serializable]
    [XmlType(AnonymousType = true)]
    public class State {
        public State() {}
        public State(string name, string action) {
            _name = name;
            _action = action;
        }

        public State(string name, string action, List<SubState> subState) {
            _name = name;
            _action = action;
            _subState = subState;
        }

        private string _name;
        private string _action;
        private List<SubState> _subState = new List<SubState>();

        [XmlAttribute]
        public string Name {
            get { return _name; }
            set { _name = value; }
        }

        [XmlAttribute]
        public string Action {
            get { return _action; }
            set { _action = value; }
        }

        [XmlElement("SubState", Form = XmlSchemaForm.Unqualified)]
        public List<SubState> SubState {
            get { return _subState; }
            set { _subState = value; }
        }
    }
}