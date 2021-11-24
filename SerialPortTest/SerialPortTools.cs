using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Timers;

namespace SerialPortTest
{
    public static partial class SerialPortTools
    {
        public static string LastUsedPortID = string.Empty;

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

        private static string _lastSerialReading;
        public static string LastSerialReading
        {
            get => _lastSerialReading;
            set
            {
                _lastSerialReading = value;
                Extensions.CatchSerialPortException(SocketTools.SendData, ConsoleCallback);
            }
        }

        
#nullable enable
        private static SerialPort? ComPort = null;
        #nullable restore

        public static void GetPortAndStartListening()
        {
            static void Timer_Elapsed(object sender, ElapsedEventArgs e)
            {
                ConsoleCallback.Invoke(Extensions.FillBlanks("", 24, '-'), Extensions.ConsoleAlertLevel.Info);
                ConsoleCallback.Invoke($"Last used COM Port: {LastUsedPortID}", Extensions.ConsoleAlertLevel.Info);
                ConsoleCallback.Invoke(Extensions.FillBlanks("", 24, '-'), Extensions.ConsoleAlertLevel.Info);
            }

            string ComPortID;
            do
            {
                ComPortID = PromptUserToSelectCOMPort();

            } while (string.IsNullOrEmpty(ComPortID) && ComPortID != "Exit");

            if (ComPortID == "Exit")
            {
                Environment.Exit(0);
            }
            else
            {
                LastUsedPortID = ComPortID;
                ComPort = new SerialPort(ComPortID, 9600, Parity.None, 8, System.IO.Ports.StopBits.One);

                Timer timer = new(30000);
                timer.Elapsed += Timer_Elapsed;
                timer.AutoReset = true;
                timer.Enabled = true;

                StartListeningOnSelectedPort();
                Console.ReadLine();
            }
        }

        private static void StartListeningOnSelectedPort()
        {
            ComPort.Open();
            SocketTools.StartServer();
            ComPort.DataReceived -= ComPort_DataReceived;
            ComPort.DataReceived += ComPort_DataReceived;
        }

        private static void ComPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var data = ComPort.ReadExisting();
            if (!string.IsNullOrWhiteSpace(data))
            {
                ConsoleCallback.Invoke($"Received data from COM port: {data}", Extensions.ConsoleAlertLevel.Info);
                LastSerialReading = data;
                //Now here is where we need to add the code to broadcast message to receiver, pre-existing code not working properly.
            }
        }

        private static string PromptUserToSelectCOMPort()
        {
            static string InvalidSelection()
            {
                Console.WriteLine("Invalid selection!!! Press enter to continue");
                _ = Console.ReadLine();
                return "";
            }

            var portNames = SerialPortWrapper.GetSerialPorts();

            Console.WriteLine("Which COM Port should be listened to? (Enter 'EXIT' to quit program)");
            Console.WriteLine(Extensions.FillBlanks("", 49, '-'));
            Console.WriteLine($"|{Extensions.FillBlanks("\tNum", 6, '\t')}|{Extensions.FillBlanks("\tCom Port", 10, '\t')}|");
            Console.WriteLine(Extensions.FillBlanks("", 49, '-'));
            foreach (var portName in portNames)
            {
                Console.WriteLine(portName.ConsolePromptLine);
            }

            Console.WriteLine(Extensions.FillBlanks("", 49, '-'));
            Console.WriteLine("You can enter either Num or Com Port value:");
            string userSelectedPort = Console.ReadLine();

            if (userSelectedPort.Contains("EXIT"))
            {
                return "Exit";
            }

            if (int.TryParse(userSelectedPort, out int intResult))
            {
                if ((intResult <= 0) && (intResult > portNames.Count()))
                {
                    return InvalidSelection();
                }
                else
                {
                    return portNames.FirstOrDefault(i => i.Index == intResult).SerialPortID;
                }
            }
            else
            {
                if (portNames.FirstOrDefault(i => i.SerialPortID == userSelectedPort, out SerialPortWrapper result))
                {
                    return result.SerialPortID;
                }
                else
                {
                    return InvalidSelection();
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
