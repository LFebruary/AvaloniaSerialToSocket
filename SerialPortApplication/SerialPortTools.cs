using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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

        private static Action<string, Extensions.ConsoleAlertLevel> _consoleCallback = (_, _) => { };
        public static Action<string, Extensions.ConsoleAlertLevel> ConsoleCallback
        {
            get => _consoleCallback;
            set
            {
                if (_consoleCallback != value)
                {
                    _consoleCallback = value;
                }
            }
        }

        private static string? _lastSerialReading;
        public static string LastSerialReading
        {
            get => _lastSerialReading ?? "";
            set
            {
                _lastSerialReading = value;
                Extensions.CatchSerialPortException(SocketTools.SendData, ConsoleCallback);
            }
        }


        private static Parity? LastUsedParity => GetSetting(StringSetting.Parity) switch
        {
            NoParity => Parity.None,
            EvenParity => Parity.Even,
            OddParity => Parity.Odd,
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
        private static SerialPort? ComPort = null;
        private static Timer? Timer = null;
#nullable restore

        public static void GetPortAndStartListening()
        {
            static void Timer_Elapsed(object sender, ElapsedEventArgs e)
            {
                ConsoleCallback.Invoke(Extensions.FillBlanks("", 24, '-'), Extensions.ConsoleAlertLevel.Info);
                ConsoleCallback.Invoke($"Last used COM Port: {LastUsedPortID}", Extensions.ConsoleAlertLevel.Info);
                ConsoleCallback.Invoke(Extensions.FillBlanks("", 24, '-'), Extensions.ConsoleAlertLevel.Info);
            }

            if (LastUsedParity != null && LastUsedParity is Parity castedParity)
            {
                if (LastUsedStopBits != null && LastUsedStopBits is StopBits castedStopBits)
                {
                    Debug.WriteLine("Before ComPort create.");
                    ComPort = new SerialPort(LastUsedPortID, LastUsedBaudRate, castedParity, LastUsedDataBits, castedStopBits);
                    Debug.WriteLine("After ComPort create.");
                }
                else
                {
                    throw new SerialPortException("Invalid value for stop bits encountered");
                }
            }
            else
            {
                throw new SerialPortException("Invalid value for parity encountered");
            }
            
            if (ComPort != null)
            {
                Timer = new(30000);
                Timer.Elapsed -= Timer_Elapsed;
                Timer.Elapsed += Timer_Elapsed;
                Timer.AutoReset = true;
                Timer.Enabled = true;

                Debug.WriteLine("Before StartListeningOnSelectedPort()");
                StartListeningOnSelectedPort();
                Debug.WriteLine("After StartListeningOnSelectedPort()");
            }
        }

        public static void StopListeningOnPort()
        {
            if (ComPort?.IsOpen == true)
            {
                ComPort.Close();
            }
        }

        private static void StartListeningOnSelectedPort()
        {
            if (ComPort != null)
            {
                Debug.WriteLine("Before ComPort.Open()");
                ComPort.Open();
                Debug.WriteLine("After ComPort.Open()");

                Debug.WriteLine("Before ComPort.DataReceived -= ComPort_DataReceived");
                ComPort.DataReceived -= ComPort_DataReceived;
                Debug.WriteLine("After ComPort.DataReceived -= ComPort_DataReceived");

                Debug.WriteLine("Before ComPort.DataReceived += ComPort_DataReceived");
                ComPort.DataReceived += ComPort_DataReceived;
                Debug.WriteLine("After ComPort.DataReceived += ComPort_DataReceived");
            }
        }

        private static void ComPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (ComPort != null)
            {
                Debug.WriteLine("Before var data = ComPort.ReadExisting();");
                var data = ComPort.ReadExisting();
                Debug.WriteLine("After var data = ComPort.ReadExisting();");

                if (!string.IsNullOrWhiteSpace(data))
                {
                    ConsoleCallback.Invoke($"Received data from COM port: {data}", Extensions.ConsoleAlertLevel.Info);
                    Debug.WriteLine($"Received data from COM port: {data}");
                    LastSerialReading = data;
                    //Now here is where we need to add the code to broadcast message to receiver, pre-existing code not working properly.
                }
            }
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

        public static readonly int DefaultStabilityIndicatorStartPosition   = 0;
        public static readonly int DefaultIdenticalReadingQuantity          = 5;
        public static readonly int DefaultScaleStringWeightStartPosition    = 1;
        public static readonly int DefaultScaleStringWeightEndPosition      = 5;
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
