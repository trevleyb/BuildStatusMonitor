using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BuildStatusMonitor.Configuration {
    [Serializable]
    [XmlType(AnonymousType = true)]
    public class Monitors : List<Monitor> {

        [XmlElement("Monitor", Form = XmlSchemaForm.Unqualified)]
        public List<Monitor> Monitor {
            get { return this; }
        }

        #region Get/Set Data Helpers
        public Monitor Add(string nameField, string assemblyField, string classField, Settings settings) {
            var result = new Monitor(nameField, assemblyField, classField, settings);
            Add(result);
            return result;
        }

        public Monitor Get(string name) {
            return this.FirstOrDefault(monitor => monitor.Name.Equals(name));
        }

        public Monitor this[string name] {
            get { return Get(name); }
        }
        #endregion 
    }
}