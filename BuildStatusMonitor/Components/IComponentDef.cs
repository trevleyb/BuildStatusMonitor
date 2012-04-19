using System.Xml.Serialization;
using BuildStatusMonitor.Configuration;

namespace BuildStatusMonitor.Components
{
    public interface IComponentDef
    {
        [XmlElement("Settings", typeof (Settings))]
        Settings Settings { get; set; }

        [XmlAttribute]
        string Name { get; set; }

        [XmlAttribute]
        string Assembly { get; set; }

        [XmlAttribute]
        string Class { get; set; }
    }
}
