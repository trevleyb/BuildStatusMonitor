using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BuildStatusMonitor.Monitors
{
    public enum Status { Unknown, Success, SuccessInProgress, Failed, FailedInProgress, InProgress, Error}

    /// <summary>
    /// Used as a data transfer class to move data between a Monitor and a Visualiser
    /// </summary>
    public class BuildStatus {

        public BuildStatus(string buildID, string buildName, Status status, DateTime? lastBuilt, string user) {
            ID       = buildID;
            Name     = buildName;
            Status   = status;
            DateTime = lastBuilt;
            User     = user;
        }

        public string ID { get; set; }
        public string Name { get; set; }
        public string User { get; set; }
        public Status Status { get; set; }
        public DateTime? DateTime { get; set; }
    }
}
