using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using BuildStatusMonitor.Components;
using BuildStatusMonitor.Configuration;
using BuildStatusMonitor.Monitors;
using BuildStatusMonitor.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildStatusMonitor.Test
{
    [TestClass]
    public class ComponentFactoryTest
    {
        [TestMethod]
        public void TestMonitorInstantiation() {

            var settings = new Settings();
            settings.Add("Host", "nzakledci01:8080");
            settings.Add("ProjectID", "project8");
            var instance = ComponentFactory<IMonitor>.CreateComponent(new Monitor("TestMonitor", "BuildStatusMonitor", "BuildStatusMonitor.Monitors.TeamCityMonitor", settings));
            Assert.IsNotNull(instance);
        }
    }
}
