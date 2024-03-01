// AvaloniaSerialToSocket https://github.com/LFebruary/AvaloniaSerialToSocket 
// (c) 2024 Lyle February 
// Released under the MIT License

using SerialToSocket.AvaloniaApp.Constants;
using SerialToSocket.AvaloniaApp.Enums.Settings;
using SerialToSocket.AvaloniaApp.Exceptions;
using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Timers;

namespace SerialToSocket.AvaloniaApp.Utils
{
    /// <summary>
    /// Provides tools and utilities for working with serial ports.
    /// </summary>
    public static partial class SerialPortUtils
    {
        /// <summary>
        /// Gets the ID of the last used serial port.
        /// </summary>
        public static string LastPortID => CustomSettings.GetSetting(StringSetting.ComPort);

        /// <summary>
        /// Gets the last used baud rate.
        /// </summary>
        public static int LastBaudRate => CustomSettings.GetSetting(IntSetting.BaudRate);

        private static string? _lastReading;

        /// <summary>
        /// Gets or sets the last serial reading.
        /// </summary>
        public static string LastReading
        {
            get => _lastReading ?? "";
            set
            {
                _lastReading = value;
                OnValueUpdated?.Invoke(LastReading);
                Debug.WriteLine(LastReading);
                //SocketTools.SendData();
            }
        }

        internal static Action<string> OnValueUpdated = (_) => { };

        private static Parity? LastParity => CustomSettings.GetSetting(StringSetting.Parity) switch
        {
            SerialConstants.NoParity => Parity.None,
            SerialConstants.EvenParity => Parity.Even,
            SerialConstants.OddParity => Parity.Odd,
            _ => null,
        };

        /// <summary>
        /// Gets the last used data bits.
        /// </summary>
        public static int LastDataBits => CustomSettings.GetSetting(IntSetting.Databits);

        /// <summary>
        /// Gets the last used stop bits.
        /// </summary>
        public static StopBits? LastStopBits => CustomSettings.GetSetting(IntSetting.Stopbits) switch
        {
            1 => StopBits.One,
            2 => StopBits.Two,
            _ => null,
        };

        private static SerialPort? port = null;
        private static Timer? timer = null;

        /// <summary>
        /// Gets the COM port and starts listening.
        /// </summary>
        public static void StartListening()
        {
            static void Timer_Elapsed(object? sender, ElapsedEventArgs e)
            {
                Debug.WriteLine($"Last used COM Port: {LastPortID}");
            }

            if (port != null && port.IsOpen)
            {
                throw new SerialPortException("SerialPortListener already instantiated. First stop listening to current port before changing ports.");
            }
            else if (LastParity != null && LastParity is Parity castedParity && LastStopBits != null && LastStopBits is StopBits castedStopBits)
            {
                port = new SerialPort(LastPortID, LastBaudRate, castedParity, LastDataBits, castedStopBits);
            }
            else if (LastParity == null || LastParity is not Parity)
            {
                throw new SerialPortException("Invalid value for parity encountered");
            }
            else if (LastStopBits == null || LastStopBits is not StopBits)
            {
                throw new SerialPortException("Invalid value for stop bits encountered");
            }

            if (port != null)
            {
                timer = new(30000);
                timer.Elapsed -= Timer_Elapsed;
                timer.Elapsed += Timer_Elapsed;
                timer.AutoReset = true;
                timer.Enabled = true;


                if (port.IsOpen)
                {
                    throw new SerialPortException("Serial port is already open, re-select port and try again.");
                }
                try
                {
                    port.Open();
                    port.DataReceived -= _DataReceived;
                    port.DataReceived += _DataReceived;
                }
                catch (UnauthorizedAccessException e)
                {
                    throw new SerialPortException("Another process on the system already has the specified COM port open. Re-select port and try again.", e);
                }

            }
        }

        /// <summary>
        /// Indicates whether the port is open.
        /// </summary>
        internal static bool IsOpen => port?.IsOpen == true;

        /// <summary>
        /// Closes the port forcefully.
        /// </summary>
        internal static void ForceClose()
        {
            port?.Close();
            port = null;
        }

        /// <summary>
        /// Stops listening on the port.
        /// </summary>
        public static void StopListening()
        {
            if (port?.IsOpen == true)
            {
                port.Close();
            }
        }

        /// <summary>
        /// Event handler for data received on the COM port.
        /// </summary>
        private static void _DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort port = (SerialPort)sender;
            string data = port.ReadExisting();
            LastReading = data;
        }

        /// <summary>
        /// Sets the flow control for the serial port.
        /// </summary>
        /// <param name="selectedFlowControl">The selected flow control.</param>
        internal static void SetFlowControl(FlowControl? selectedFlowControl)
        {
            if (port == null)
            {
                return;
            }

            switch (selectedFlowControl)
            {
                case FlowControl.Ctr_Rts:
                    port.Handshake = Handshake.RequestToSend;
                    port.DtrEnable = false;
                    break;
                case FlowControl.Dsr_Dtr:
                    port.Handshake = Handshake.None;
                    port.DtrEnable = true;
                    break;
                case FlowControl.Xon_Xoff:
                    port.Handshake = Handshake.XOnXOff;
                    port.DtrEnable = false;
                    break;
                case FlowControl.None:
                    port.Handshake = Handshake.None;
                    port.DtrEnable = false;
                    break;
                default:
                    port.Handshake = Handshake.None;
                    port.DtrEnable = false;
                    break;
            }
        }
    }
}
