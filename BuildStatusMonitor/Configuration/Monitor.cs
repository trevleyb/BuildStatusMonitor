using System;
using System.Xml.Serialization;
using BuildStatusMonitor.Components;

namespace BuildStatusMonitor.Configuration {
    [Serializable]
    [XmlType(AnonymousType = true)]
    public class Monitor : IComponentDef{

        private string _assemblyField;
        private string _classField;
        private string _nameField;
        private Settings _settingsField;

        public Monitor() {}
        public Monitor(string nameField, string assemblyField, string classField, Settings settingsField) {
            _assemblyField = assemblyField;
            _classField = classField;
            _nameField = nameField;
            _settingsField = settingsField;
        }

        [XmlElement("Settings", typeof (Settings))]
        public Settings Settings {
            get { return _settingsField; }
            set { _settingsField = value; }
        }

        [XmlAttribute]
        public string Name {
            get { return _nameField; }
            set { _nameField = value; }
        }

        [XmlAttribute]
        public string Assembly {
            get { return _assemblyField; }
            set { _assemblyField = value; }
        }

        [XmlAttribute]
        public string Class {
            get { return _classField; }
            set { _classField = value; }
        }
    }
}