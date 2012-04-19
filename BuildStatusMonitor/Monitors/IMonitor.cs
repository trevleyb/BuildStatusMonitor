using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BuildStatusMonitor.Components;
using BuildStatusMonitor.Configuration;

namespace BuildStatusMonitor.Monitors
{
    /// <summary>
    /// IMonitor represents a interface to a Monitor. Something that will monitor a Build Service or Service and report 
    /// upon its state. The state is passed by a controller to a IVisualiser object.
    /// </summary>
    public interface IMonitor : IComponent {
        BuildStatus Poll();
        void Sleep();   // Used when the scheduler puts the Monitor to Sleep
        void Wake();    // Used when the scheduler wakes up the Monitor
    }
}
