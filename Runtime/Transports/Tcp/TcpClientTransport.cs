using System.Net;
using System.Net.Sockets;

namespace JeeLee.Networking.Transports.Tcp
{
    /// <summary>
    /// Represents a TCP client transport for establishing connections to a remote server.
    /// </summary>
    public class TcpClientTransport : IClientTransport
    {
        private Socket _socket;

        /// <summary>
        /// Gets or sets the IP address of the remote server to connect to.
        /// </summary>
        public string IpAddress { get; set; } = "127.0.0.1";

        /// <summary>
        /// Gets or sets the port on which the client connects to the remote server.
        /// </summary>
        public ushort Port { get; set; } = 7777;

        /// <summary>
        /// Establishes a connection to the remote server.
        /// </summary>
        /// <returns>The connection object representing the established connection.</returns>
        public Connection Connect()
        {
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse(IpAddress), Port);
            _socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            _socket.Connect(remoteEndPoint);

            return new TcpConnection(_socket);
        }

        /// <summary>
        /// Disconnects from the current server, if connected.
        /// </summary>
        public void Disconnect()
        {
            // Implementation for disconnecting, if necessary.
        }

        /// <summary>
        /// Performs any necessary actions during each tick of the TCP client transport.
        /// </summary>
        public void Tick()
        {
            // Implementation for handling ticks, if necessary.
        }
    }
}