using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using BuildStatusMonitor.Utilities.DelcomLights;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildStatusMonitor.Test
{
    [TestClass]
    public class DelcomControllerTest
    {
        [TestMethod]
        public void TestLights() {

            var deviceID = DelcomManager.GetDevices();
            if (deviceID != null && deviceID.Count > 0) {
                var device = DelcomManager.GetController(deviceID[0]);

                device.Off();
                device.SetColor(new DelcomLight(Colors.Green, Modes.On));
                device.SetColor(new DelcomLight(Colors.Red, Modes.On));
                device.SetColor(new DelcomLight(Colors.Yellow, Modes.On));

                device.SetColor(new DelcomLight(Colors.Green, Modes.Flash));
                device.SetColor(new DelcomLight(Colors.Red, Modes.Flash));
                device.SetColor(new DelcomLight(Colors.Yellow, Modes.Flash));

                device.SetColor(new DelcomLight(Colors.Green, Modes.Off));
                device.SetColor(new DelcomLight(Colors.Red, Modes.Off));
                device.SetColor(new DelcomLight(Colors.Yellow, Modes.Off));

                device.SetColor(new DelcomLight(Colors.Green, Modes.On), new DelcomLight(Colors.Red, Modes.On));
                device.SetColor(new DelcomLight(Colors.Green, Modes.On), new DelcomLight(Colors.Yellow, Modes.On));
                device.SetColor(new DelcomLight(Colors.Red, Modes.On), new DelcomLight(Colors.Yellow, Modes.On));

                device.SetColor(new DelcomLight(Colors.Green, Modes.Flash), new DelcomLight(Colors.Red, Modes.On));
                device.SetColor(new DelcomLight(Colors.Green, Modes.Flash), new DelcomLight(Colors.Yellow, Modes.On));
                device.SetColor(new DelcomLight(Colors.Red, Modes.Flash), new DelcomLight(Colors.Yellow, Modes.On));

                device.SetColor(new DelcomLight(Colors.Green, Modes.On), new DelcomLight(Colors.Red, Modes.Flash));
                device.SetColor(new DelcomLight(Colors.Green, Modes.On), new DelcomLight(Colors.Yellow, Modes.Flash));
                device.SetColor(new DelcomLight(Colors.Red, Modes.On), new DelcomLight(Colors.Yellow, Modes.Flash));

                device.Off();
            }
        }
    }
}
