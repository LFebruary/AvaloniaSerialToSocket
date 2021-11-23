using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Timers;

namespace SerialPortTest
{
    internal static partial class SerialPortTools
    {
        public static string LastUsedPortID = string.Empty;

        private static string _lastSerialReading;
        public static string LastSerialReading
        {
            get => _lastSerialReading;
            set
            {
                _lastSerialReading = value;
                Extensions.CatchSerialPortException(SocketTools.SendData);
            }
        }
#nullable enable
        private static SerialPort? ComPort = null;
        #nullable restore

        public static void GetPortAndStartListening()
        {
            static void Timer_Elapsed(object sender, ElapsedEventArgs e)
            {
                Console.WriteLine(Extensions.FillBlanks("", 24, '-'));
                Console.WriteLine($"Last used COM Port: {LastUsedPortID}");
                Console.WriteLine(Extensions.FillBlanks("", 24, '-'));
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
                ComPort = new SerialPort(ComPortID, 9600, Parity.None, 8, StopBits.One);

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
                Console.WriteLine(data);
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


    }

    public class SerialPortWrapper
    {
        private SerialPortWrapper(string serialPortID, int index)
        {
            SerialPortID = serialPortID;
            Index = index;
        }

        internal string SerialPortID { get; set; }
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
