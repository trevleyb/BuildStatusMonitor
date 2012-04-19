using System;
using System.Xml.Serialization;

namespace BuildStatusMonitor.Configuration {
    [Serializable]
    [XmlType(AnonymousType = true)]
    public class Schedule {
        public Schedule() {}
        public Schedule(string startTime, string finishTime, string state = "ON", string dayOfWeek = null) {
            _dayOfWeek = dayOfWeek;
            _startTime = startTime;
            _finishTime = finishTime;
            _state = state;
        }

        private string _dayOfWeek;
        private string _startTime;
        private string _finishTime;
        private string _state;

        [XmlAttribute]
        public string DayOfWeek {
            get { return _dayOfWeek; }
            set { _dayOfWeek = value; }
        }

        [XmlAttribute]
        public string StartTime {
            get { return _startTime; }
            set { _startTime = value; }
        }

        [XmlAttribute]
        public string FinishTime {
            get { return _finishTime; }
            set { _finishTime = value; }
        }

        [XmlAttribute]
        public string State {
            get { return _state ?? "ON"; }
            set { _state = value; }
        }
    }
}