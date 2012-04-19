using System;
using System.Collections.Generic;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BuildStatusMonitor.Configuration {
    [Serializable]
    [XmlType(AnonymousType = true)]
    public class Transition {
        public Transition() {}
        public Transition(string name) {
            _name = name;
        }

        private string _name;
        private List<State> _states = new List<State>();
            
        [XmlAttribute]
        public string Name {
            get { return _name; }
            set { _name = value; }
        }

        [XmlElement("State", Form = XmlSchemaForm.Unqualified)]
        public List<State> States {
            get { return _states; }
            set { _states = value; }
        }

        #region Get/Set Data Helpers
        public State Add(State state) {
            States.Add(state);
            return state;
        }
        #endregion

    }
}