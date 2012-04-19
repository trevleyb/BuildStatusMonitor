using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BuildStatusMonitor.Configuration {
    [Serializable]
    [XmlType(AnonymousType = true)]
    public class Visualisers : List<Visualiser> {

        [XmlElement("Visualiser", Form = XmlSchemaForm.Unqualified)]
        public List<Visualiser> Visualiser {
            get { return this; }
        }

        #region Get/Set Data Helpers
        public Visualiser Add(string nameField, string assemblyField, string classField, Settings settings) {
            var visualiser = new Visualiser(nameField, assemblyField, classField, settings);
            Add(visualiser);
            return visualiser;
        }

        public Visualiser Get(string name) {
            return this.FirstOrDefault(visualiser=> visualiser.Name.Equals(name));
        }

        public Visualiser this[string name] {
            get { return Get(name); }
        }
        #endregion 
    }
}