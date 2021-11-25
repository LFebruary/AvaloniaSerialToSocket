using System;
using System.Diagnostics;
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
            catch (SocketException e)
            {
                throw new SerialPortException("Could not determine local ip address via socket", e);
            }
        }

        internal static void StopServer()
        {
            listener?.Close();
            listener = new(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        internal static IPAddress    ipAddress   = IPAddress.Parse(GetLocalIPAddress());
        internal static Socket       listener    = new(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        internal static int          Port        => CustomSettings.GetSetting(CustomSettings.IntSetting.BroadcastPort);
        internal static IPEndPoint   endPoint    = new(ipAddress, Port);
        internal static readonly int DefaultBroadcastPort = 5050;

        internal static bool SocketOpen => listener.Connected;
        public static void StartServer()
        {
            Debug.WriteLine($"Listening on {ipAddress}:{Port}");

            try
            {
                ipAddress   = IPAddress.Parse(GetLocalIPAddress());
                listener    = new(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                endPoint    = new(ipAddress, Port);

                listener.Bind(endPoint);

                listener.Listen(10);
            }
            catch (SocketException exception)
            {
                throw new SerialPortException("Could not get initial startup of server going...", exception);
            }
        }

        public static void SendData()
        {
            if (listener.LocalEndPoint == null)
            {
                throw new SerialPortException("Listening socket was never instantiated");
            }

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
