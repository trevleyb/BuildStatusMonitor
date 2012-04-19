using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BuildStatusMonitor.Configuration {
    [Serializable]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public class States {
        private List<State> _stateField = new List<State>();

        public States() {}

        [XmlElement("State", Form = XmlSchemaForm.Unqualified)]
        public List<State> State {
            get { return _stateField; }
            set { _stateField = value; }
        }

        #region Get/Set Data Helpers

        public State Add(State state) {
            State.Add(state);
            return state;
        }

        #endregion

    }
}