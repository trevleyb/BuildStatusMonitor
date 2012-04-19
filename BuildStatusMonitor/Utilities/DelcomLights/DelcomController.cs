using System;
using System.Collections.Generic;
using System.Text;

namespace BuildStatusMonitor.Utilities.DelcomLights
{
    [Flags]
    public enum Colors { Green = 0x0000001, Red = 0x00000010, Yellow = 0x00000100 }
    public enum Modes  { On, Off, Flash, FlashSlow, FlashFast };

    public class DelcomController {
        private static readonly object Lock = new object();
        private uint _deviceID;

        private static List<string> _availableDevices;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelcomController"/> class.
        /// On the first call to this method, a list of devices will be obtained and as each device 
        /// is initialised the devices will be removed from the list to ensure the same device is 
        /// not allocated to multiple monitors. 
        /// 
        /// If AUTO is provided as the Device Name then this will take the first available
        /// device from the list. 
        /// </summary>
        /// <param name="deviceName">Name of the device.</param>
        public DelcomController(string deviceName) {
            lock(Lock) {
                if (_availableDevices == null) {
                    _availableDevices = DelcomManager.GetDevices();
                }
                if (_availableDevices.Count > 0) {
                    if (deviceName.ToUpper().Equals("AUTO")) {
                        deviceName = _availableDevices[0];
                    }
                    else {
                        if (!_availableDevices.Contains(deviceName)) throw new LogApplicationException("Invalid DeviceID provided.");
                    }
                    DeviceName = new StringBuilder(deviceName);
                    _availableDevices.Remove(deviceName);
                } else {
                    throw new LogApplicationException("No more valid Devices exist.");
                }
            }
        }

        public StringBuilder DeviceName { get; private set; }

        /// <summary>
        /// Turns off all Light Settings (Reset)
        /// </summary>
        public void Off() {
            foreach (Colors color in Enum.GetValues(typeof(Colors))) {
                SetLight(color, Modes.Off);
            }
        }

        public void SetColor(DelcomLight light) {
            SetLight(light.Color, light.Mode);
        }

        public void SetColor(DelcomLight light1, DelcomLight light2) {
            SetLight(light1.Color, light1.Mode);
            SetLight(light2.Color, light2.Mode);
        }

        #region Private Helper Methods

        private void SetLight(Colors color, Modes mode) {
            Open();
            if (IsOpen) {
                if (mode == Modes.FlashFast || mode == Modes.FlashSlow || mode == Modes.Flash) {
                    //NativeMethods.DelcomLoadLedFreqDuty(_deviceID, ColorMap(color), )
                    NativeMethods.DelcomLEDControl(_deviceID, ColorMap(color), ModeMap(mode));
                } else {
                    NativeMethods.DelcomLEDControl(_deviceID, ColorMap(color), ModeMap(mode));
                }
            }
            Close();
        }

        /// <summary>
        /// Opens the connection to the HID device. 
        /// </summary>
        private void Open() {
            _deviceID = NativeMethods.DelcomOpenDevice(DeviceName, 0);
            if (_deviceID == 0) throw new LogApplicationException("Unable to Open the Device '" + DeviceName + "'");
        }

        /// <summary>
        /// Closes the connection to the HID Device
        /// </summary>
        private void Close() {
            if (_deviceID != 0) {
                NativeMethods.DelcomCloseDevice(_deviceID);
                _deviceID = 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is open.
        /// </summary>
        private bool IsOpen { get { return _deviceID > 0; }}

        /// <summary>
        /// Maps the ENUM defined here to the BYTES for the Delcom HID
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns></returns>
        private byte ColorMap (Colors color) {
            switch (color) {
                case Colors.Red:    return NativeMethods.REDLED;
                case Colors.Green:  return NativeMethods.GREENLED;
                case Colors.Yellow: return NativeMethods.YELLOWLED;
                default:            return NativeMethods.GREENLED;
            }
        }

        /// <summary>
        /// Maps the MODE Enum to the byte value required for the interface. 
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <returns></returns>
        private byte ModeMap (Modes mode) {
            switch (mode) {
                case Modes.On:          return NativeMethods.LEDON;
                case Modes.Off:         return NativeMethods.LEDOFF;
                case Modes.Flash:       return NativeMethods.LEDFLASH;
                case Modes.FlashSlow:   return NativeMethods.LEDFLASH;
                case Modes.FlashFast:   return NativeMethods.LEDFLASH;
                default:                return NativeMethods.LEDON;
            }
        }
        #endregion 
    }
}

