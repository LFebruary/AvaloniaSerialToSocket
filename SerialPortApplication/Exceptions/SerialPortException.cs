// AvaloniaSerialToSocket https://github.com/LFebruary/AvaloniaSerialToSocket 
// (c) 2024 Lyle February 
// Released under the MIT License

using System;

namespace SerialToSocket.AvaloniaApp.Exceptions
{
    internal class SerialPortException : Exception
    {
        public SerialPortException()
        {
        }

        public SerialPortException(string message) : base(message)
        {
        }

        public SerialPortException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}