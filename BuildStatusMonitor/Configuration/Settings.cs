using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BuildStatusMonitor.Configuration {
    [Serializable]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public class Settings {
        private List<Setting> _settingField = new List<Setting>();

        [XmlElement("Setting", Form = XmlSchemaForm.Unqualified)]
        public List<Setting> Setting {
            get { return _settingField; }
            set { _settingField = value; }
        }

        #region Get/Set Data Helpers
        public Setting Add(Setting setting) {
            Setting.Add(setting);
            return setting;
        }

        public Setting Add<T>(string name, T value) {
            return Add(new Setting(name, value.ToString()));
        }

        public T Get<T>(string name, T defaultValue) {
            foreach (var setting in Setting.Where(setting => setting.Name.Equals(name))) {
                if (string.IsNullOrEmpty(setting.Value)) return defaultValue;
                return (T)Convert.ChangeType(setting.Value, typeof(T));
            }
            return defaultValue;
        }

        public object this[string name] {
            get { return Get<object>(name, null); }
        }
        #endregion 
    }
}