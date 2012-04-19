using System.Runtime.InteropServices;
using System.Text;

namespace BuildStatusMonitor.Utilities.DelcomLights {
    public class NativeMethods {
        // Delcom USB Devices
        public const uint USBIODS = 1;
        public const uint USBDELVI = 2;
        public const uint USBNDSPY = 3;

        // USBDELVI LED MODES
        public const byte LEDOFF = 0;
        public const byte LEDON = 1;
        public const byte LEDFLASH = 2;

        // USBDELVI LED COlORS
        public const byte GREENLED = 0;
        public const byte REDLED = 1;
        public const byte YELLOWLED = 2;

        // Device Name Maximum Length
        public const int MAXDEVICENAMELEN = 512;

        // USB Packet Structures

        // USB Data IO Packact

        // Delcom DLL Fucnctions - See the DelcomDLL.pdf for documentation
        // http://www.delcom-eng.com/productdetails.asp?PartNumber=890510

        // Gets the DelcomDLL verison
        [DllImport("delcomdll.dll", EntryPoint = "DelcomGetDLLVersion")]
        internal static extern float DelcomGetDLLVersion();

        // Sets the verbose controll - used for debugging
        [DllImport("delcomdll.dll", EntryPoint = "DelcomVerboseControl")]
        internal static extern int DelcomVerboseControl(uint Mode, StringBuilder caption);

        // Gets the DLL date
        [DllImport("delcomdll.dll", EntryPoint = "DelcomGetDLLDate")]
        internal static extern int DelcomGetDLLDate(StringBuilder DateString);

        // Generic Functions

        //Gets DeviceCount
        [DllImport("delcomdll.dll", EntryPoint = "DelcomGetDeviceCount")]
        internal static extern int DelcomGetDeviceCount(uint ProductType);

        // Gets Nth Device
        [DllImport("delcomdll.dll", EntryPoint = "DelcomGetNthDevice")]
        internal static extern int DelcomGetNthDevice(uint ProductType, uint NthDevice, StringBuilder DeviceName);

        // Open Device
        [DllImport("delcomdll.dll", EntryPoint = "DelcomOpenDevice")]
        internal static extern uint DelcomOpenDevice(StringBuilder DeviceName, int Mode);

        // Close Device
        [DllImport("delcomdll.dll", EntryPoint = "DelcomCloseDevice")]
        internal static extern int DelcomCloseDevice(uint DeviceHandle);

        // Read USB Device Version
        [DllImport("delcomdll.dll", EntryPoint = "DelcomReadDeviceVersion")]
        internal static extern int DelcomReadDeviceVersion(uint DeviceHandle);

        // Read USB Device SerialNumber
        [DllImport("delcomdll.dll", EntryPoint = "DelcomReadDeviceSerialNum")]
        internal static extern int DelcomReadDeviceSerialNum(StringBuilder DeviceName, uint DeviceHandle);

        // Send USB Packet
        [DllImport("delcomdll.dll", EntryPoint = "DelcomSendPacket")]
        internal static extern int DelcomSendPacket(uint DeviceHandle, ref PacketStructure PacketOut, out PacketStructure PacketIn);

        // USBDELVI - Visual Indicator Functions

        // Set LED Functions
        [DllImport("delcomdll.dll", EntryPoint = "DelcomLEDControl")]
        internal static extern int DelcomLEDControl(uint DeviceHandle, int Color, int Mode);

        // Set LED Freq/Duty functions
        [DllImport("delcomdll.dll", EntryPoint = "DelcomLoadLedFreqDuty")]
        internal static extern int DelcomLoadLedFreqDuty(uint DeviceHandle, byte Color, byte Low, byte High);

        // Set Auto Confirm Mode
        [DllImport("delcomdll.dll", EntryPoint = "DelcomEnableAutoConfirm")]
        internal static extern int DelcomEnableAutoConfirm(uint DeviceHandle, int Mode);

        // Set Auto Clear Mode
        [DllImport("delcomdll.dll", EntryPoint = "DelcomEnableAutoClear(")]
        internal static extern int DelcomEnableAutoClear(uint DeviceHandle, int Mode);

        // Set Buzzer Function
        [DllImport("delcomdll.dll", EntryPoint = "DelcomBuzzer")]
        internal static extern int DelcomBuzzer(uint DeviceHandle, byte Mode, byte Freq, byte Repeat, byte OnTime, byte OffTime);

        // Set LED Phase Delay
        [DllImport("delcomdll.dll", EntryPoint = "DelcomLoadInitialPhaseDelay")]
        internal static extern int DelcomLoadInitialPhaseDelay(uint DeviceHandle, byte Color, byte Delay);

        // Set Led Sync Functions
        [DllImport("delcomdll.dll", EntryPoint = "DelcomSyncLeds")]
        internal static extern int DelcomSyncLeds(uint DeviceHandle);

        // Set LED PreScalar Functions
        [DllImport("delcomdll.dll", EntryPoint = "DelcomLoadPreScalar")]
        internal static extern int DelcomLoadPreScalar(uint DeviceHandle, byte PreScalar);

        // Get Button Status
        [DllImport("delcomdll.dll", EntryPoint = "DelcomGetButtonStatus")]
        internal static extern int DelcomGetButtonStatus(uint DeviceHandle);

        // Set LED Power
        [DllImport("delcomdll.dll", EntryPoint = "DelcomLEDPower")]
        internal static extern int DelcomLEDPower(uint DeviceHandle, int Color, int Power);

        // USBIODS - USB IO Functions

        // Set Ports
        [DllImport("delcomdll.dll", EntryPoint = "DelcomWritePin")]
        internal static extern int DelcomWritePin(uint DeviceHandle, byte Port, byte Pin, byte Value);

        // Set Ports
        [DllImport("delcomdll.dll", EntryPoint = "DelcomWritePorts")]
        internal static extern int DelcomWritePorts(uint DeviceHandle, byte Port0, byte Port1);

        // Get Ports 
        [DllImport("delcomdll.dll", EntryPoint = "DelcomReadPorts")]
        internal static extern int DelcomReadPorts(uint DeviceHandle, out byte Port0, out byte Port1);

        // Set 64Bit Value
        [DllImport("delcomdll.dll", EntryPoint = "DelcomWrite64Bit")]
        internal static extern int DelcomWrite64Bit(uint DeviceHandle, ref DataExtStructure DataExt);

        // Get 64Bit Value
        [DllImport("delcomdll.dll", EntryPoint = "DelcomRead64Bit")]
        internal static extern int DelcomRead64Bit(uint DeviceHandle, out DataExtStructure DataExt);

        // Write I2C Functions 
        [DllImport("delcomdll.dll", EntryPoint = "DelcomWriteI2C")]
        internal static extern int DelcomWriteI2C(uint DeviceHandle, byte CmdAdd, byte Length, DataExtStructure DataExt);

        // Read I2C Functions
        [DllImport("delcomdll.dll", EntryPoint = "DelcomReadI2C")]
        internal static extern int DelcomReadI2C(uint DeviceHandle, byte CmdAdd, byte Length, out DataExtStructure DataExt);

        // Get I2C Slect Read 
        [DllImport("delcomdll.dll", EntryPoint = "DelcomSelReadI2C")]
        internal static extern int DelcomSelReadI2C(uint DeviceHandle, byte SetAddCmd, byte Address, byte ReadCmd, byte Length, out DataExtStructure DataExt);

        // Read I2C EEPROM Functions
        [DllImport("delcomdll.dll", EntryPoint = "DelcomReadI2CEEPROM")]
        internal static extern int DelcomReadI2CEEPROM(uint DeviceHandle, uint Address, uint size, byte Ctrlcode, out StringBuilder Data);

        // Write I2C EEPROM Functions
        [DllImport("delcomdll.dll", EntryPoint = "DelcomwriteI2CEEPROM")]
        internal static extern int DelcomWriteI2CEEPROM(uint DeviceHandle, uint Address, uint size, byte Ctrlcode, byte WriteDelay, out StringBuilder Data);

        // Setup RS232 Mode
        [DllImport("delcomdll.dll", EntryPoint = "DelcomRS232Ctrl")]
        internal static extern int DelcomRS232Ctrl(uint DeviceHandle, int Mode, int Value);

        // Write RS232 Function
        [DllImport("delcomdll.dll", EntryPoint = "DelcomWriteRS232")]
        internal static extern int DelcomWriteRS232(uint DeviceHandle, int Length, ref DataExtStructure DataExt);

        // Read RS232 Function
        [DllImport("delcomdll.dll", EntryPoint = "DelcomReadRS232")]
        internal static extern int DelcomReadRS232(uint DeviceHandle, out DataExtStructure DataExt);

        // SPI Write Function
        [DllImport("delcomdll.dll", EntryPoint = "DelcomSPIWrite")]
        internal static extern int DelcomSPIWrite(uint DeviceHandle, uint ClockCount, out DataExtStructure DataExt);

        // SPI SetClock Function
        [DllImport("delcomdll.dll", EntryPoint = "DelcomSPISetClock")]
        internal static extern int DelcomSPISetClock(uint DeviceHandle, uint ClockPeriod);

        // SPI Read Function
        [DllImport("delcomdll.dll", EntryPoint = "DelcomSPIRead")]
        internal static extern int DelcomSPIRead(uint DeviceHandle, out DataExtStructure DataExt);

        // SPI Write8Read64 Function
        [DllImport("delcomdll.dll", EntryPoint = "DelcomSPIWr8Read64")]
        internal static extern int DelcomSPIWr8Read64(uint DeviceHandle, uint WeData, uint ClockCount, out DataExtStructure DataExt);

        // USBNDSPY Functions

        // Set Numeric Mode
        [DllImport("delcomdll.dll", EntryPoint = "DelcomNumericMode")]
        internal static extern int DelcomNumericMode(uint DeviceHandle, byte Mode, byte Rate);

        // Set Numeric Scan Rate
        [DllImport("delcomdll.dll", EntryPoint = "DelcomNumericScanRate")]
        internal static extern int DelcomNumericScanRate(uint DeviceHandle, byte ScanRate);

        // Setup Numeric Digits
        [DllImport("delcomdll.dll", EntryPoint = "DelcomNumericSetup")]
        internal static extern int DelcomNumericSetup(uint DeviceHandle, byte Digits);

        // Set Numeric Raw Mode
        [DllImport("delcomdll.dll", EntryPoint = "DelcomNumericRaw")]
        internal static extern int DelcomNumericRaw(uint DeviceHandle, StringBuilder Str);

        // Set Numeric Integer Mode
        [DllImport("delcomdll.dll", EntryPoint = "DelcomNumericInteger")]
        internal static extern int DelcomNumericInteger(uint DeviceHandle, int Number, int Base);

        // Set Numeric Hexdecimal Mode
        [DllImport("delcomdll.dll", EntryPoint = "DelcomNumericHexaDecimal")]
        internal static extern int DelcomNumericHexaDecimal(uint DeviceHandle, int Number, int Base);

        // Set Numeric Double Mode
        [DllImport("delcomdll.dll", EntryPoint = "DelcomNumericDouble(")]
        internal static extern int DelcomNumericDouble(uint DeviceHandle, double Number, int Base);

        #region Nested type: DataExtStructure

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct DataExtStructure {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)] internal byte[] DATA; //Data 1 .. 8
        }

        #endregion

        #region Nested type: PacketStructure

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct PacketStructure {
            internal byte Recipient;
            internal byte DeviceModel;
            internal byte MajorCmd;
            internal byte MinorCmd;
            internal byte DataLSB;
            internal byte DataMSB;
            internal byte Length;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)] internal byte[] DATA; //Data 1 .. 8
        }

        #endregion
    }
}