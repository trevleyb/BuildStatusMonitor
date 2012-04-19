using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace BuildStatusMonitor.Configuration {
    [Serializable]
    [XmlType(AnonymousType = true)]
    [DebuggerDisplay("{Value}",Name="{Name}")]
    public class Setting {
        public Setting() {}
        public Setting(string name, string value) {
            _nameField = name;
            _valueField = value;
        }

        private string _nameField;
        private string _valueField;

        [XmlAttribute]
        public string Name {
            get { return _nameField; }
            set { _nameField = value; }
        }

        [XmlAttribute]
        public string Value {
            get { return _valueField; }
            set { _valueField = value; }
        }

        public override string ToString() {
            return string.Format("{0}='{1}'", Name, Value);
        }
    }
}