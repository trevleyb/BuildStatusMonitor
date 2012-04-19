using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace BuildStatusMonitor.Configuration {

    /// <summary>
    /// A Controller brings together a Monitor and a Viualiser and Manages them as a pair. 
    /// </summary>
    [Serializable]
    [XmlType(AnonymousType = true)]
    public class Controller {
        public Controller() {}
        public Controller(string nameField, string monitorField, string visualiserField, string transitionField) {
            _monitorField = monitorField;
            _nameField = nameField;
            _visualiserField = visualiserField;
            _transitionField = transitionField;
        }

        private string _monitorField;
        private string _nameField;
        private string _visualiserField;
        private string _transitionField;

        [XmlAttribute]
        public string Name {
            get { return _nameField; }
            set { _nameField = value; }
        }

        [XmlAttribute]
        public string Monitor {
            get { return _monitorField; }
            set { _monitorField = value; }
        }

        [XmlAttribute]
        public string Visualiser {
            get { return _visualiserField; }
            set { _visualiserField = value; }
        }

        [XmlAttribute]
        public string Transition {
            get { return _transitionField; }
            set { _transitionField = value; }
        }
    }
}