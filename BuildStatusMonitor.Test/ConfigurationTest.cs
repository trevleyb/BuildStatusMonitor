using System;
using BuildStatusMonitor.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildStatusMonitor.Test {
    [TestClass]
    public class ConfigurationTest {
        [TestMethod]
        public void SaveAndLoadConfigData() {
            try {
                var config = new BuildStatusConfig();
                config.Settings.Add("PollFrequency", "15");

                var setting = new Settings();
                setting.Add("Host", "nzakledci01:8080");
                setting.Add("ProjectID", "project8");

                var monitor1 = config.Monitors.Add("Monitor1", "Assembly1", "Class1", setting);
                var monitor2 = config.Monitors.Add("Monitor2", "Assembly2", "Class2", setting);

                var visualiser1 = config.Visualisers.Add("Visualiser1", "Assembly1", "Class1",setting);
                var visualiser2 = config.Visualisers.Add("Visualiser2", "Assembly2", "Class2",setting);

                config.Schedules.Add(new Schedule("6:00AM", "8:00PM", "ON", "Week"));
                config.Schedules.DefaultState = "OFF";

                config.Save("C:\\Test.xml");

                BuildStatusConfig newConfig = BuildStatusConfig.Load("C:\\Test.xml");

                Assert.AreEqual(config.Settings["PollFrequency"], newConfig.Settings["PollFrequency"]);
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void TestSchedule() {

            var schedules1 = new Schedules();
            schedules1.Add(new Schedule("13:00", "14:00"));
            Assert.IsTrue(schedules1.IsScheduleOn(new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day,13,23,00)));
            Assert.IsTrue(schedules1.IsScheduleOn(new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day,11,23,00)));

            var schedules2 = new Schedules();
            schedules2.Add(new Schedule("13:00", "14:00", "OFF"));
            Assert.IsFalse(schedules2.IsScheduleOn(new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day,13,23,00)));
            Assert.IsTrue(schedules2.IsScheduleOn(new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day,11,23,00)));

            var schedules4 = new Schedules("OFF");
            schedules4.Add(new Schedule("13:00", "14:00", "OFF"));
            Assert.IsFalse(schedules4.IsScheduleOn(new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day,13,23,00)));
            Assert.IsFalse(schedules4.IsScheduleOn(new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day,11,23,00)));

            var schedules5 = new Schedules("OFF");
            schedules5.Add(new Schedule("13:00", "14:00", "ON"));
            Assert.IsTrue(schedules5.IsScheduleOn(new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day,13,23,00)));
            Assert.IsFalse(schedules5.IsScheduleOn(new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day,11,23,00)));

            var schedules3 = new Schedules();
            schedules3.Add(new Schedule("13:00", "14:00", "OFF", DateTime.Now.DayOfWeek.ToString()));
            Assert.IsFalse(schedules3.IsScheduleOn(new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day,13,23,00)));
            Assert.IsTrue(schedules3.IsScheduleOn(new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day+1,13,23,00)));
            Assert.IsTrue(schedules3.IsScheduleOn(new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day,14,00,01)));
            Assert.IsTrue(schedules3.IsScheduleOn(new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day+1,14,00,01)));
        }

    }
}