using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BuildStatusMonitor.Monitors;
using BuildStatusMonitor.Visualisers;

namespace BuildStatusMonitor
{
    public class InstanceController
    {
        public InstanceController(string name, IMonitor monitor, IVisualiser visualiser) {
            Name = name;
            Monitor = monitor;
            Visualiser = visualiser;
        }

        public string Name { get; private set; }
        public IMonitor Monitor { get; private set; }
        public IVisualiser Visualiser { get; private set; }
    }
}
