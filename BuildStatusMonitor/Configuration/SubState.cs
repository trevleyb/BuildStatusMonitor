using System;
using System.Xml.Serialization;

namespace BuildStatusMonitor.Configuration {
    [Serializable]
    [XmlType(AnonymousType = true)]
    public class SubState {
        public SubState() {}
        public SubState(string name, string condition, string action) {
            _name = name;
            _condition = condition;
            _action = action;
        }

        private string _name;
        private string _condition;
        private string _action;

        [XmlAttribute]
        public string Name {
            get { return _name; }
            set { _name = value; }
        }

        [XmlAttribute]
        public string Condition {
            get { return _condition; }
            set { _condition = value; }
        }

        [XmlAttribute]
        public string Action {
            get { return _action; }
            set { _action = value; }
        }

        #region Evaluation Methods

        public bool Evaluate(DateTime? lastStateTime) {

            if (lastStateTime == null) return false;

            if (!string.IsNullOrEmpty(Condition)) {

                // Currently only support TIME and FailedBY as conditions
                // ------------------------------------------------------
                if (_condition.ToUpper().StartsWith("TIME:")) {
                    var timeRange = _condition.ToUpper().Remove(0, "TIME:".Length);
                    if (timeRange.Contains("-")) {
                        var timeSplit = timeRange.Split('-');
                        if (timeSplit.Length == 2) {
                            int timeFrom;
                            int timeTo;
                            if (int.TryParse(timeSplit[0], out timeFrom) && int.TryParse(timeSplit[1], out timeTo)) {
                                if ((DateTime.Now - lastStateTime.Value ).Duration().Minutes >= timeFrom &&
                                    (DateTime.Now - lastStateTime.Value ).Duration().Minutes <= timeTo) return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        #endregion 

    }
}