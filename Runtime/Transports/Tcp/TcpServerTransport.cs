using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using JeeLee.UniNetworking.Logging;
using JeeLee.UniNetworking.Payloads;

namespace JeeLee.UniNetworking.Transports.Tcp
{
    /// <summary>
    /// Represents a TCP server transport for handling incoming connections.
    /// </summary>
    public class TcpServerTransport : IServerTransport
    {
        /// <summary>
        /// Event triggered when a client connects to the server.
        /// </summary>
        public Func<int> ClientConnected { get; set; }

        /// <summary>
        /// Event triggered when a client disconnects from the server.
        /// </summary>
        public Action<int> ClientDisconnected { get; set; }

        /// <summary>
        /// Event triggered when the server receives a message from a client.
        /// </summary>
        public Action<Payload, int> MessageReceived { get; set; }
        
        private readonly Dictionary<int, TcpConnection> _connections = new Dictionary<int, TcpConnection>();

        private Socket _socket;

        /// <summary>
        /// Gets or sets the port on which the server listens for incoming connections.
        /// </summary>
        public ushort Port { get; set; } = 7777;

        /// <summary>
        /// Gets or sets the maximum number of simultaneous connections allowed.
        /// </summary>
        public int MaxConnections { get; set; } = 10;

        /// <summary>
        /// Starts the server transport to listen for incoming connections.
        /// </summary>
        public void Start()
        {
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.IPv6Any, Port);
            _socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            _socket.Bind(localEndPoint);
            _socket.Listen(MaxConnections);
        }

        /// <summary>
        /// Stops the server transport, preventing new connections.
        /// </summary>
        public void Stop()
        {
            try
            {
                var connectionIds = _connections.Keys.ToArray();
                
                foreach (var connectionId in connectionIds)
                {
                    CloseConnection(connectionId);
                }

                _connections.Clear();
                
                NetworkLogger.Log("Server stopped");
                
                _socket.Shutdown(SocketShutdown.Both);
            }
            finally
            {
                _socket.Close();
            }
        }

        /// <summary>
        /// Closes the connection with the specified connection identifier.
        /// </summary>
        /// <param name="connectionId">The connection identifier to close.</param>
        public void CloseConnection(int connectionId)
        {
            if (!_connections.TryGetValue(connectionId, out var connection))
            {
                return;
            }

            Send(new Payload(PayloadType.Disconnect), connectionId);
            connection.Close();
        }

        /// <summary>
        /// Called periodically to perform any necessary actions.
        /// </summary>
        public void Tick()
        {
            if (_socket.Poll(0, SelectMode.SelectRead))
            {
                OnNewConnection(_socket.Accept());
            }

            foreach (var connection in _connections)
            {
                connection.Value.Receive(payload =>
                {
                    switch (payload.Type)
                    {
                        case PayloadType.Message:
                            MessageReceived(payload, connection.Key);
                            break;
                
                        case PayloadType.Disconnect:
                            connection.Value.Close();
                            break;
                    }
                });
            }
        }

        /// <summary>
        /// Sends a payload to all connected clients.
        /// </summary>
        /// <param name="payload">The payload to send.</param>
        public void Send(Payload payload)
        {
            foreach (var connectionId in _connections.Keys)
            {
                Send(payload, connectionId);
            }
        }

        /// <summary>
        /// Sends a payload to a specific client identified by connection ID.
        /// </summary>
        /// <param name="payload">The payload to send.</param>
        /// <param name="connectionId">The connection identifier of the client.</param>
        public void Send(Payload payload, int connectionId)
        {
            if (!_connections.TryGetValue(connectionId, out var connection))
            {
                return;
            }
            
            connection.Send(payload);
        }

        private void OnNewConnection(Socket socket)
        {
            var connection = new TcpConnection(socket);
            
            int connectionId = ClientConnected();
            _connections.Add(connectionId, connection);

            connection.ConnectionClosed += HandleConnectionClosed;

            void HandleConnectionClosed()
            {
                connection.ConnectionClosed -= HandleConnectionClosed;

                _connections.Remove(connectionId);

                ClientDisconnected?.Invoke(connectionId);
            }
        }
    }
}