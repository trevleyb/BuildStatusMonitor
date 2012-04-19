using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BuildStatusMonitor.Configuration {
    [Serializable]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public class Transitions : List<Transition> {

        public Transitions() {}

        [XmlElement("Transition", Form = XmlSchemaForm.Unqualified)]
        public List<Transition> Transition {
            get { return this; }
        }

        #region Get/Set Data Helpers

        public new Transition Add(Transition transition) {
            base.Add(transition);
            return transition;
        }

        public Transition Get(string name) {
            if (string.IsNullOrEmpty(name)) return null;
            return this.FirstOrDefault(transition => transition.Name.Equals(name));
        }

        public Transition this[string name] {
            get { return Get(name); }
        }

        #endregion

    }
}