using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BuildStatusMonitor.Configuration {
    [Serializable]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public class Schedules {
        private string _stateField;
        private List<Schedule> _scheduleField = new List<Schedule>();

        public Schedules() {}

        public Schedules(string stateField) {
            _stateField = GetState(stateField) ? "ON" : "OFF";
        }

        [XmlAttribute]
        public string DefaultState {
            get { return _stateField ?? "ON"; }
            set { _stateField = value; }
        }

        [XmlElement("Schedule", Form = XmlSchemaForm.Unqualified)]
        public List<Schedule> Schedule {
            get { return _scheduleField; }
            set { _scheduleField = value; }
        }

        #region Get/Set Data Helpers

        public Schedule Add(Schedule schedule) {
            Schedule.Add(schedule);
            return schedule;
        }

        #endregion

        #region Schedule Management Functions

        /// <summary>
        /// Based on the current Date and Time, this will determine if the Schedule
        /// is ON or OFF (ie: Are we in a ON state or a OFF state). 
        /// The system will assume it is ON unless it finds a specified entry saying it should be
        /// OFF in which case it will report that it should be OFF> 
        /// </summary>
        /// <returns>
        ///   True if the Schedule should be ON, False if it should be OFF
        /// </returns>
        public bool IsScheduleOn() {
            return IsScheduleOn(DateTime.Now);
        }

        public bool IsScheduleOn(DateTime currentDateTime) {

            var scheduleState = GetState(DefaultState);
            if (Schedule != null && Schedule.Count > 0) {
                foreach (var schedule in Schedule) {

                    DateTime startDate;
                    DateTime finishDate;
                    if (!DateTime.TryParse(schedule.StartTime, out startDate)) continue;
                    if (!DateTime.TryParse(schedule.FinishTime, out finishDate)) continue;
                    if (currentDateTime < startDate || currentDateTime > finishDate) continue;
                    if (!CheckDayOfWeek(currentDateTime, schedule)) continue;

                    // If we get to here then we have a valid Schedule Entry so return its 
                    // state for ON or OFF. 
                    // --------------------------------------------------------------------
                    scheduleState = GetState(schedule.State);
                    break;
                }
            }
            return scheduleState;
        }

        /// <summary>
        /// Given a Date, will determine if the Date matches the STRING that was provided 
        /// as the DAYS of the week that the process can RUN on. 
        /// </summary>
        /// <param name="currentDateTime">The current date time.</param>
        /// <param name="schedule">The schedule.</param>
        /// <returns></returns>
        private static bool CheckDayOfWeek(DateTime currentDateTime, Schedule schedule) {
            if (!string.IsNullOrEmpty(schedule.DayOfWeek)) {
                if (schedule.DayOfWeek.ToUpper().Equals("WEEKEND")) {
                    return IsDayAMatch(currentDateTime, new[] {"Saturday", "Sunday"});
                }
                if (schedule.DayOfWeek.ToUpper().Equals("WEEK") ||
                    schedule.DayOfWeek.ToUpper().Equals("WEEKDAY")) {
                    return !IsDayAMatch(currentDateTime, new[] {"Saturday", "Sunday"});
                }
                if (schedule.DayOfWeek.Contains(",")) {
                    return IsDayAMatch(currentDateTime, schedule.DayOfWeek.Split(','));
                }
                return IsDayAMatch(currentDateTime, new[] {schedule.DayOfWeek});
            }
            return true;
        }

        /// <summary>
        /// Convert a String ON/OFF to a TRUE/FALSE. 
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        private bool GetState(string state) {
            if (!string.IsNullOrEmpty(state)) {
                return state.ToUpper().Equals("TRUE") || state.ToUpper().Equals("ON");
            }
            return false;
        }

        /// <summary>
        /// Determines whether [is day A match] [the specified current date time].
        /// </summary>
        /// <param name="currentDateTime">The current date time.</param>
        /// <param name="checkDays">The check days.</param>
        /// <returns>
        ///   <c>true</c> if [is day A match] [the specified current date time]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsDayAMatch(DateTime currentDateTime, string[] checkDays) {
            return checkDays.Any(day => currentDateTime.DayOfWeek.ToString().ToUpper().Equals(day.ToUpper()));
        }

        #endregion
    }
}