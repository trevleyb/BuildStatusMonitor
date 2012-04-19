using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BuildStatusMonitor.Configuration {

    /// <summary>
    /// A collection of Controllers. Each Controller definition defines a pair between a Monitor and a 
    /// Visualiser and is then managed by a specific management class. 
    /// </summary>
    [Serializable]
    [XmlType(AnonymousType = true)]
    public class Controllers : List<Controller> {

        [XmlElement("Controller", Form = XmlSchemaForm.Unqualified)]
        public List<Controller> Controller {
            get { return this; }
        }

        #region Get/Set Data Helpers
        public Controller Add(string name, string monitor, string visualiser, string transition) {
            var controller = new Controller(name, monitor, visualiser, transition);
            Add(controller);
            return controller;
        }

        public Controller Get(string name) {
            return this.FirstOrDefault(controller => controller.Name.Equals(name));
        }

        public Controller this[string name] {
            get { return Get(name); }
        }
        #endregion 
    }
}