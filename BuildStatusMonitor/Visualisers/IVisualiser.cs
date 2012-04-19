using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BuildStatusMonitor.Components;
using BuildStatusMonitor.Configuration;
using BuildStatusMonitor.Monitors;

namespace BuildStatusMonitor.Visualisers
{
    /// <summary>
    /// A IVisualiser is a interface for something that can Visualise the data provided by a controller that was obtained
    /// by an IMonitor object. 
    /// </summary>
    public interface IVisualiser : IComponent {
        void Update(BuildStatus status);
        void Sleep();   // Used when the scheduler puts the Monitor to Sleep
        void Wake();    // Used when the scheduler wakes up the Monitor
        Transition Transitions { set; }
    }
}
