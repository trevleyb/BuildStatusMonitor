using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using BuildStatusMonitor.Configuration;
using BuildStatusMonitor.Monitors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildStatusMonitor.Test
{
    [TestClass]
    public class TeamCityTest
    {
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void InvalidConstructorTest()
        {
            var monitor = new TeamCityMonitor();
            var setting = new Settings();
            monitor.Initialise("Test", setting);
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void MinimalConstructorTest()
        {
            var monitor = new TeamCityMonitor();
            var setting = new Settings();
            setting.Add("Host", "Host");
            monitor.Initialise("Test", setting);
            Assert.IsTrue(monitor.IsInitialised);
        }

        [TestMethod]
        public void ConnectedTest()
        {
            var monitor = new TeamCityMonitor();
            var setting = new Settings();
            setting.Add("Host", "nzakledci01:8080");
            setting.Add("Build", "bt13");
            monitor.Initialise("Test", setting);
            Assert.IsTrue(monitor.IsInitialised);
        }

        [TestMethod]
        public void ConnectedTest2()
        {
            var monitor = new TeamCityMonitor();
            var setting = new Settings();
            setting.Add("Host", "nzakledci01:8080");
            setting.Add("ProjectID", "project8");
            monitor.Initialise("Test", setting);
            Assert.IsTrue(monitor.IsInitialised);
        }

        [TestMethod]
        public void PollTest()
        {
            var monitor = new TeamCityMonitor();
            var setting = new Settings();
            setting.Add("Host", "nzakledci01:8080");
            setting.Add("ProjectID", "project8");
            monitor.Initialise("Test", setting);
            var result = monitor.Poll();
            Assert.IsNotNull(result);
        }
}
}
