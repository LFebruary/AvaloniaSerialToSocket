using System;
using System.Runtime.Serialization;

namespace SerialPortApplication
{
    [Serializable]
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

        protected SerialPortException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}