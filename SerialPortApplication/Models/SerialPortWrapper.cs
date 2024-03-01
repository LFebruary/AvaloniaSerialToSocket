// AvaloniaSerialToSocket https://github.com/LFebruary/AvaloniaSerialToSocket 
// (c) 2024 Lyle February 
// Released under the MIT License

using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;

namespace SerialToSocket.AvaloniaApp.Models
{
    /// <summary>
    /// Represents a wrapper class for serial ports, providing information about available serial ports.
    /// </summary>
    public class SerialPortWrapper
    {
        /// <summary>
        /// Initializes a new instance of the SerialPortWrapper class with the specified serial port ID and index.
        /// </summary>
        /// <param name="serialPortID">The ID of the serial port.</param>
        /// <param name="index">The index of the serial port.</param>
        private SerialPortWrapper(string serialPortID, int index)
        {
            SerialPortID = serialPortID;
            Index = index;
        }

        /// <summary>
        /// Gets or sets the ID of the serial port.
        /// </summary>
        public string SerialPortID { get; set; }

        /// <summary>
        /// Gets the index of the serial port.
        /// </summary>
        internal int Index { get; set; }

        /// <summary>
        /// Retrieves a collection of available serial ports.
        /// </summary>
        /// <returns>An IEnumerable of SerialPortWrapper representing the available serial ports.</returns>
        public static IEnumerable<SerialPortWrapper> GetSerialPorts()
        {
            string[] portNames = SerialPort.GetPortNames();

            if (portNames.Length <= 0)
            {
                throw new InvalidOperationException("ERROR!!! No valid COM Ports found");
            }
            else
            {
                return portNames.Select((id, index) => new SerialPortWrapper(id, index + 1));
            }
        }

        /// <summary>
        /// Gets the console prompt line for the serial port.
        /// </summary>
        internal string ConsolePromptLine => $"|{Index.ToString().FillBlanks(3, '\t', true)}|{SerialPortID.FillBlanks(6, '\t', true)}|";
    }
}
