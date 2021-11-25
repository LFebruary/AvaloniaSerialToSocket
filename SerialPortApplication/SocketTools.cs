using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SerialPortApplication
{
    internal static partial class SocketTools
    {
        private static string GetLocalIPAddress()
        {
            try
            {
                using Socket socket = new(AddressFamily.InterNetwork, SocketType.Dgram, 0);
                socket.Connect("8.8.8.8", 65530);

                if (socket.LocalEndPoint is not IPEndPoint endPoint)
                {
                    throw new SerialPortException("Could not determine local ip address via socket");
                }

                return endPoint.Address.ToString();
            }
            catch (Exception e)
            {
                throw new SerialPortException("Could not determine local ip address via socket", e);
            }
        }

        //static IPHostEntry  host        = Dns.GetHostEntry(Dns.GetHostName());
        //static IPAddress    ipAddress   = IPAddress.Parse("192.168.1.37");
        internal static IPAddress    ipAddress   = IPAddress.Parse(GetLocalIPAddress());
        internal static Socket       listener    = new(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        internal static int          port        = 5050;
        internal static IPEndPoint   endPoint    = new(ipAddress, port);
        public static void StartServer()
        {
            SerialPortTools.ConsoleCallback.Invoke($"Listening on {ipAddress}:{port}", Extensions.ConsoleAlertLevel.Info);

            try
            {
                ipAddress   = IPAddress.Parse(GetLocalIPAddress());
                listener    = new(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                port        = 5050;
                endPoint    = new(ipAddress, port);

                listener.Bind(endPoint);

                listener.Listen(10);
            }
            catch (Exception ex)
            {
                throw new SerialPortException("Could not get initial startup of server going...", ex);
            }
        }

        public static void SendData()
        {
            if (listener.LocalEndPoint == null)
            {
                throw new SerialPortException("Listening socket was never instantiated");
            }

            SerialPortTools.ConsoleCallback.Invoke("Waiting for connection...", Extensions.ConsoleAlertLevel.Info);

            try
            {
                Socket handler  = listener.Accept();
                byte[] msg      = Encoding.ASCII.GetBytes(SerialPortTools.LastSerialReading);
                handler.Send(msg);
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch (SocketException ex)
            {
                throw new SerialPortException($"Exception occurred during SocketException data send...\n{ex.Message}", ex);
            }
            
        }
    }
}
