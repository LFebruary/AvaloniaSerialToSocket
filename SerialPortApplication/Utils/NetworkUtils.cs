// AvaloniaSerialToSocket https://github.com/LFebruary/AvaloniaSerialToSocket 
// (c) 2024 Lyle February 
// Released under the MIT License

using SerialToSocket.AvaloniaApp.Enums.Settings;
using SerialToSocket.AvaloniaApp.Exceptions;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SerialToSocket.AvaloniaApp.Utils
{
    /// <summary>
    /// Provides utilities for network operations such as sending and receiving data.
    /// </summary>
    internal static partial class NetworkUtils
    {
        internal static IPAddress IpAddress = IPAddress.Parse(_GetLocalIPAddress());
        internal static Socket Listener = new(IpAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        internal static int Port => CustomSettings.GetSetting(IntSetting.BroadcastPort);
        internal static IPEndPoint EndPoint = new(IpAddress, Port);
        internal static readonly int DefaultBroadcastPort = 5050;

        /// <summary>
        /// Indicates whether the server socket is open and listening for connections.
        /// </summary>
        internal static bool SocketOpen => Listener.Connected;

        /// <summary>
        /// Retrieves the local IP address using a socket connection.
        /// </summary>
        /// <returns>The local IP address as a string.</returns>
        private static string _GetLocalIPAddress()
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

        /// <summary>
        /// Stops the server by closing the listener socket.
        /// </summary>
        internal static void StopServer()
        {
            Listener?.Close();
            Listener = new(IpAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }



        /// <summary>
        /// Starts the server and begins listening for incoming connections.
        /// </summary>
        public static void StartServer()
        {
            _SetupServerSocket();
            _BindServerSocket();
            _StartListening();
        }

        /// <summary>
        /// Sets up the server socket.
        /// </summary>
        private static void _SetupServerSocket()
        {
            Debug.WriteLine($"Listening on {IpAddress}:{Port}");

            try
            {
                IpAddress = IPAddress.Parse(_GetLocalIPAddress());
                Listener = new(IpAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                EndPoint = new(IpAddress, Port);
            }
            catch (SocketException exception)
            {
                throw new SerialPortException("Could not set up the server...", exception);
            }
        }

        /// <summary>
        /// Binds the server socket to the endpoint.
        /// </summary>
        private static void _BindServerSocket()
        {
            try
            {
                Listener.Bind(EndPoint);
            }
            catch (SocketException exception)
            {
                throw new SerialPortException("Could not bind the server socket...", exception);
            }
        }

        /// <summary>
        /// Starts listening for incoming connections.
        /// </summary>
        private static void _StartListening()
        {
            try
            {
                Listener.Listen(10);
            }
            catch (SocketException exception)
            {
                throw new SerialPortException("Could not start listening on the server socket...", exception);
            }
        }

        /// <summary>
        /// Sends data over the network.
        /// </summary>
        public static void SendData()
        {
            if (Listener.LocalEndPoint == null)
            {
                throw new SerialPortException("Listening socket was never instantiated");
            }

            try
            {
                _AcceptClientAndSendData();
            }
            catch (SocketException ex)
            {
                throw new SerialPortException($"Exception occurred during SocketException data send...\n{ex.Message}", ex);
            }
        }

        /// <summary>
        /// Accepts a client connection and sends data.
        /// </summary>
        private static void _AcceptClientAndSendData()
        {
            Socket handler = Listener.Accept();
            byte[] message = Encoding.ASCII.GetBytes(SerialPortUtils.LastReading);
            handler.Send(message);
            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
        }
    }
}
