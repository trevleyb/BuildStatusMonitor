using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildStatusMonitor.Test
{
    public class RunTest
    {
        public void TestMethod1()
        {
            var controller = new BuildStatusController();
            controller.Initialise();
            controller.StartPolling();
            controller.StopPolling();
            controller.Finalise();
        }
    }
}
