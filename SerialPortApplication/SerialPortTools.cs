using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Timers;
using static SerialPortApplication.CustomSettings;

namespace SerialPortApplication
{
    public static partial class SerialPortTools
    {
        public static string    LastUsedPortID      => GetSetting(StringSetting.ComPort);
        public static int       LastUsedBaudRate    => GetSetting(IntSetting.BaudRate);

        private static string? _lastSerialReading;
        public static string LastSerialReading
        {
            get => _lastSerialReading ?? "";
            set
            {
                _lastSerialReading = value;
                ValueUpdatedCallback?.Invoke(LastSerialReading);
                Debug.WriteLine(LastSerialReading);
                //SocketTools.SendData();
            }
        }

        internal static Action<string> ValueUpdatedCallback = (_) => { };

        private static Parity? LastUsedParity => GetSetting(StringSetting.Parity) switch
        {
            NoParity    => Parity.None,
            EvenParity  => Parity.Even,
            OddParity   => Parity.Odd,
            _ => null,
        };

        public static int LastUsedDataBits => GetSetting(IntSetting.Databits);

        public static StopBits? LastUsedStopBits => GetSetting(IntSetting.Stopbits) switch
        {
            1 => System.IO.Ports.StopBits.One,
            2 => System.IO.Ports.StopBits.Two,
            _ => null,
        };

#nullable enable
        private static SerialPort? SerialPort = null;
        private static Timer? Timer = null;
#nullable restore

        public static void GetPortAndStartListening()
        {
            static void Timer_Elapsed(object sender, ElapsedEventArgs e)
            {
                Debug.WriteLine($"Last used COM Port: {LastUsedPortID}");
            }

            if (SerialPort != null && SerialPort.IsOpen)
            {
                throw new SerialPortException("SerialPortListener already instantiated. First stop listening to current port before changing ports.");
            }
            else if (LastUsedParity != null && LastUsedParity is Parity castedParity && LastUsedStopBits != null && LastUsedStopBits is StopBits castedStopBits)
            {
                SerialPort = new SerialPort(LastUsedPortID, LastUsedBaudRate, castedParity, LastUsedDataBits, castedStopBits);
            }
            else if (LastUsedParity == null || LastUsedParity is not Parity)
            {
                throw new SerialPortException("Invalid value for parity encountered");
            }
            else if (LastUsedStopBits == null || LastUsedStopBits is not System.IO.Ports.StopBits)
            {
                throw new SerialPortException("Invalid value for stop bits encountered");
            }
            
            if (SerialPort != null)
            {
                Timer           = new(30000);
                Timer.Elapsed   -= Timer_Elapsed;
                Timer.Elapsed   += Timer_Elapsed;
                Timer.AutoReset = true;
                Timer.Enabled   = true;


                if (SerialPort.IsOpen)
                {
                    throw new SerialPortException("Serial port is already open, re-select port and try again.");
                }
                try
                {
                    SerialPort.Open();
                    SerialPort.DataReceived -= ComPort_DataReceived;
                    SerialPort.DataReceived += ComPort_DataReceived;
                }
                catch (UnauthorizedAccessException e)
                {
                    throw new SerialPortException("Another process on the system already has the specified COM port open. Re-select port and try again.", e);
                }

            }
        }

        internal static bool IsPortOpen => SerialPort?.IsOpen == true;

        internal static void ForcefulClose()
        {
            SerialPort?.Close();
            SerialPort = null;
        }

        public static void StopListeningOnPort()
        {
            if (SerialPort?.IsOpen == true)
            {
                SerialPort.Close();
            }
        }

        private static void ComPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort port     = (SerialPort)sender;
            string data         = port.ReadExisting();
            LastSerialReading   = data;
        }

        public static readonly IList<int> BaudRates = new ReadOnlyCollection<int>(
            new List<int> {
                110,
                300,
                600,
                1200,
                2400,
                4800,
                9600,
                19200
            });

        public static readonly int DefaultBaudRate = 9600;

        public static readonly IList<int> DataBits = new ReadOnlyCollection<int>(
            new List<int> {
                6,
                7,
                8
            });

        public static readonly int DefaultDatabits = 8;

        public static readonly IList<int> StopBits = new ReadOnlyCollection<int>(
            new List<int> {
                1,
                2
            });

        public static readonly int DefaultStopbits                          = 1;

        public static readonly int DefaultStabilityIndicatorStartPosition   = 1;
        public static readonly int DefaultIdenticalReadingQuantity          = 5;
        public static readonly int DefaultScaleStringWeightStartPosition    = 11;
        public static readonly int DefaultScaleStringWeightEndPosition      = 19;
        public static readonly int DefaultScaleStringMinimumLength          = 17;

        public const string DefaultParity = NoParity;
        public const string DefaultStabilityIndicatorSnippet = "ST";

        public static readonly IList<string> ParityValues = new ReadOnlyCollection<string>(
            new List<string> {
                EvenParity,
                OddParity,
                NoParity
            });

        public const string EvenParity  = "Even";
        public const string OddParity   = "Odd";
        public const string NoParity    = "None";

        internal const string FlowControlCtsRts     = "CTS/RTS";
        internal const string FlowControlDsrDtr     = "DSR/DTS";
        internal const string FlowControlXonXoff    = "XON/XOFF";
        internal const string FlowControlNone       = "NONE";
        internal const string FlowControlDefault    = FlowControlNone;
        internal static void SetFlowControl(FlowControl? selectedFlowControl)
        {
            if (SerialPort == null)
            {
                return;
            }

            switch (selectedFlowControl)
            {
                case FlowControl.Ctr_Rts:
                    SerialPort.Handshake = Handshake.RequestToSend;
                    SerialPort.DtrEnable = false;
                    break;
                case FlowControl.Dsr_Dtr:
                    SerialPort.Handshake = Handshake.None;
                    SerialPort.DtrEnable = true;
                    break;
                case FlowControl.Xon_Xoff:
                    SerialPort.Handshake = Handshake.XOnXOff;
                    SerialPort.DtrEnable = false;
                    break;
                case FlowControl.None:
                    SerialPort.Handshake = Handshake.None;
                    SerialPort.DtrEnable = false;
                    break;
                default:
                    SerialPort.Handshake = Handshake.None;
                    SerialPort.DtrEnable = false;
                    break;
            }
        }
    }

    public class SerialPortWrapper
    {
        private SerialPortWrapper(string serialPortID, int index)
        {
            SerialPortID = serialPortID;
            Index = index;
        }

        public string SerialPortID { get; set; }
        internal int Index { get; set; }

        public static IEnumerable<SerialPortWrapper> GetSerialPorts()
        {
            var portNames = SerialPort.GetPortNames();

            if (portNames.Length <= 0)
            {
                throw new InvalidOperationException("ERROR!!! No valid COM Ports found");
            }
            else
            {
                return portNames.Select((id, index) => new SerialPortWrapper(id, index + 1));
            }
        }

        internal string ConsolePromptLine => $"|{Extensions.FillBlanks(Index.ToString(), 3, '\t', true)}|{Extensions.FillBlanks(SerialPortID, 6, '\t', true)}|";
    }
}
