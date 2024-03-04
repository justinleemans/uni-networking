using System;
using System.Net;
using System.Net.Sockets;

namespace JeeLee.UniNetworking.Transports.Tcp
{
    /// <summary>
    /// Represents a TCP server transport for handling incoming connections.
    /// </summary>
    public class TcpServerTransport : IServerTransport
    {
        private Socket _socket;
        
        #region IServerTransport Members

        /// <summary>
        /// Event triggered when a new connection is established.
        /// </summary>
        public event Action<Connection> NewConnection;

        /// <summary>
        /// Gets or sets the port on which the server listens for incoming connections.
        /// </summary>
        public ushort Port { get; set; } = 7777;

        /// <summary>
        /// Gets or sets the maximum number of simultaneous connections allowed.
        /// </summary>
        public int MaxConnections { get; set; } = 10;

        /// <summary>
        /// Starts the TCP server transport, binding to the specified port and listening for incoming connections.
        /// </summary>
        public void Start()
        {
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.IPv6Any, Port);
            _socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            _socket.Bind(localEndPoint);
            _socket.Listen(MaxConnections);
        }

        /// <summary>
        /// Stops the TCP server transport, preventing new incoming connections.
        /// </summary>
        public void Stop()
        {
            _socket.Close();
        }

        /// <summary>
        /// Performs any necessary actions during each tick of the TCP server transport.
        /// </summary>
        public void Tick()
        {
            if (_socket.Poll(0, SelectMode.SelectRead))
            {
                NewConnection?.Invoke(new TcpConnection(_socket.Accept()));
            }
        }

        #endregion
    }
}