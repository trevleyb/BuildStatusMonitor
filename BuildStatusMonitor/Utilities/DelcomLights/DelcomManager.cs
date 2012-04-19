using System.Collections.Generic;
using System.Text;

namespace BuildStatusMonitor.Utilities.DelcomLights
{
    public class DelcomManager
    {
        #region Static Members to provide access to find Lights
        /// <summary>
        /// Gets the number of Delcom Lights attached to the current computer.
        /// </summary>
        public static int DeviceCount {
            get { return NativeMethods.DelcomGetDeviceCount(NativeMethods.USBDELVI); }
        }

        /// <summary>
        /// Looks for all HID devices and loads the names of the devices into an array (List) and returns that 
        /// list. If there is an error, a NULL will be returned. 
        /// </summary>
        /// <returns>A list of found devices or NULL</returns>
        public static List<string> GetDevices() {

            FileLogger.Logger.LogVerbose("Enumerating Devices.");
            var devices = new List<string>();

            FileLogger.Logger.LogVerbose("Found '{0}' Devices.", DeviceCount);
            if (DeviceCount > 0) {
                for (uint device = 0; device < DeviceCount; device++) {
                    var deviceName = new StringBuilder(NativeMethods.MAXDEVICENAMELEN + 1);
                    var result = NativeMethods.DelcomGetNthDevice(NativeMethods.USBDELVI, device, deviceName);
                    if (result == 0) {
                        FileLogger.Logger.LogError("Error getting information on device '{0}'", device);
                    }
                    else {
                        devices.Add(deviceName.ToString());
                        FileLogger.Logger.LogVerbose("Obtained Device '{0}'", deviceName);
                    }
                }
            } else {
                FileLogger.Logger.LogWarning("No devices found.");
            }
            return devices;
        }

        /// <summary>
        /// Given the name of a Device, this will return a Controller for that Device
        /// </summary>
        /// <param name="deviceName">Name of the device.</param>
        /// <returns>A DelcomHID object to manage and control the specified device.</returns>
        public static DelcomController GetController(string deviceName) {
            return new DelcomController(deviceName);
        }
        #endregion 

    }
}
