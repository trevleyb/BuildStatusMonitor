using System;
using BuildStatusMonitor.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildStatusMonitor.Test {
    [TestClass]
    public class UnitTest1 {
        [TestMethod]
        public void SaveAndLoadConfigData() {
            try {
                var config = new BuildStatusConfig();
                config.Settings.Add("PollFrequency", "15");

                var monitor1 = config.Monitors.Add("Monitor1", "Assembly1", "Class1");
                monitor1.Settings.Add("Setting1", "Value1");
                var monitor2 = config.Monitors.Add("Monitor2", "Assembly2", "Class2");
                monitor2.Settings.Add("Setting2", "Value2");
                monitor2.Settings.Add("Setting3", "Value3");

                var visualiser1 = config.Visualisers.Add("Visualiser1", "Assembly1", "Class1");
                visualiser1.Settings.Add("Setting1", "Value1");
                var visualiser2 = config.Visualisers.Add("Visualiser2", "Assembly2", "Class2");
                visualiser2.Settings.Add("Setting2", "Value2");
                visualiser2.Settings.Add("Setting3", "Value3");

                config.Save("C:\\Test.xml");

                BuildStatusConfig newConfig = BuildStatusConfig.Load("C:\\Test.xml");

                Assert.AreEqual(config.Settings["PollFrequency"], newConfig.Settings["PollFrequency"]);
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }
    }
}