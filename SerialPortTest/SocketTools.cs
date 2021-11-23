using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SerialPortTest
{
    internal static partial class SocketTools
    {
        private static string GetLocalIPAddress()
        {
            try
            {
                using Socket socket = new(AddressFamily.InterNetwork, SocketType.Dgram, 0);
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                return endPoint.Address.ToString();
            }
            catch (Exception e)
            {
                throw new SerialPortException("Could not determine local ip address via socket", e);
            }
            
            //var host = Dns.GetHostEntry(Dns.GetHostName());
            //foreach (var ip in host.AddressList)
            //{
            //    if (ip.AddressFamily == AddressFamily.InterNetwork)
            //    {
            //        return ip.ToString();
            //    }
            //}

            //throw new SerialPortException("No network adapters with an IPv4 address in the system!!!");
        }

        //static IPHostEntry  host        = Dns.GetHostEntry(Dns.GetHostName());
        //static IPAddress    ipAddress   = IPAddress.Parse("192.168.1.37");
        static IPAddress    ipAddress   = IPAddress.Parse(GetLocalIPAddress());
        static Socket       listener    = new(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        static int          port        = 5050;
        static IPEndPoint   endPoint    = new(ipAddress, port);
        public static void StartServer()
        {
            Console.WriteLine($"Listening on {ipAddress}:{port}");

            try
            {
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
            Console.WriteLine("Waiting for connection...");

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

        //private static void ListenerCallback(IAsyncResult ar)
        //{
        //    if (ar.AsyncState is Socket castedSocket && castedSocket.Connected == true)
        //    {
        //        Socket handler = castedSocket.Accept();
                
        //        handler.Send(msg);
        //        Console.WriteLine($"Sent data: {Encoding.ASCII.GetString(msg)}");
        //    }
        //}
    }
}
