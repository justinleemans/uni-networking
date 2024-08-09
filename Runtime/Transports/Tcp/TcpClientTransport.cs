using System;
using System.Net;
using System.Net.Sockets;
using JeeLee.UniNetworking.Payloads;

namespace JeeLee.UniNetworking.Transports.Tcp
{
    /// <summary>
    /// Represents a TCP client transport for establishing connections to a remote server.
    /// </summary>
    public class TcpClientTransport : IClientTransport
    {
        /// <summary>
        /// Event triggered when the client disconnects from the server.
        /// </summary>
        public Action ClientDisconnected { get; set; }

        /// <summary>
        /// Event triggered when the client receives a message from the server.
        /// </summary>
        public Action<Payload, int> MessageReceived { get; set; }

        private TcpConnection _connection;

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
        public void Connect()
        {
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse(IpAddress), Port);
            Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            
            socket.Connect(remoteEndPoint);

            _connection = new TcpConnection(socket);
            _connection.ConnectionClosed += HandleConnectionClosed;

            void HandleConnectionClosed()
            {
                _connection.ConnectionClosed -= HandleConnectionClosed;
                
                _connection = null;
                
                ClientDisconnected?.Invoke();
            }
        }

        /// <summary>
        /// Disconnects from the current server, if connected.
        /// </summary>
        public void Disconnect()
        {
            Send(new Payload(PayloadType.Disconnect));
            _connection.Close();
        }

        /// <summary>
        /// Performs any necessary actions during each tick of the TCP client transport.
        /// </summary>
        public void Tick()
        {
            _connection.Receive(payload =>
            {
                switch (payload.Type)
                {
                    case PayloadType.Message:
                        MessageReceived(payload, -1);
                        break;
                
                    case PayloadType.Disconnect:
                        _connection.Close();
                        break;
                }
            });
        }

        /// <summary>
        /// Sends a payload to the connected server.
        /// </summary>
        /// <param name="payload">The payload to send.</param>
        public void Send(Payload payload)
        {
            _connection.Send(payload);
        }
    }
}