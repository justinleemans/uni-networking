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
        public event Action<int> ClientConnected;

        /// <summary>
        /// Event triggered when a client disconnects from the server.
        /// </summary>
        public event Action<int> ClientDisconnected;
        
        private readonly Queue<int> _idPool = new Queue<int>();
        private readonly Dictionary<int, TcpConnection> _connections = new Dictionary<int, TcpConnection>();

        private Socket _socket;
        private int _lastUserId;

        /// <summary>
        /// Gets a value indicating whether the server transport is running.
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Gets the collection of connection identifiers.
        /// </summary>
        public IReadOnlyCollection<int> ConnectionIds => _connections.Keys;

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
            if (IsRunning)
            {
                NetworkLogger.Log("Server is already running", LogLevel.Warning);
                return;
            }

            try
            {
                IPEndPoint localEndPoint = new IPEndPoint(IPAddress.IPv6Any, Port);
                _socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                _socket.Bind(localEndPoint);
                _socket.Listen(MaxConnections);

                NetworkLogger.Log("Server started");
                
                IsRunning = true;
            }
            catch (Exception exception)
            {
                NetworkLogger.Log(exception, LogLevel.Error);
                IsRunning = false;
            }
        }

        /// <summary>
        /// Stops the server transport, preventing new connections.
        /// </summary>
        public void Stop()
        {
            if (!IsRunning)
            {
                return;
            }
            
            try
            {
                var connectionIds = _connections.Keys.ToArray();
                
                foreach (var connectionId in connectionIds)
                {
                    CloseConnection(connectionId);
                }

                _idPool.Clear();
                _connections.Clear();
                _lastUserId = 0;
                
                NetworkLogger.Log("Server stopped");
                
                _socket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception exception)
            {
                NetworkLogger.Log(exception, LogLevel.Error);
                _socket.Close();
            }
            finally
            {
                IsRunning = false;
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
            if (!IsRunning)
            {
                return;
            }

            if (_socket.Poll(0, SelectMode.SelectRead))
            {
                OnNewConnection(_socket.Accept());
            }
        }

        /// <summary>
        /// Sends a payload to all connected clients.
        /// </summary>
        /// <param name="payload">The payload to send.</param>
        public void Send(Payload payload)
        {
            if (!IsRunning)
            {
                return;
            }

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
            if (!IsRunning || !_connections.TryGetValue(connectionId, out var connection))
            {
                return;
            }
            
            connection.Send(payload);
        }

        /// <summary>
        /// Receives payloads and processes them using the specified handler.
        /// </summary>
        /// <param name="onMessageReceived">The handler to process received payloads.</param>
        public void Receive(Action<Payload, int> onMessageReceived)
        {
            if (!IsRunning)
            {
                return;
            }
            
            foreach (var connection in _connections)
            {
                connection.Value.Receive(payload =>
                {
                    switch (payload.Type)
                    {
                        case PayloadType.Message:
                            onMessageReceived(payload, connection.Key);
                            break;
                
                        case PayloadType.Disconnect:
                            connection.Value.Close();
                            break;
                    }
                });
            }
        }

        private void OnNewConnection(Socket socket)
        {
            var connection = new TcpConnection(socket);
            int connectionId = GetConnectionId();
            
            connection.ConnectionClosed += HandleConnectionClosed;
            _connections.Add(connectionId, connection);
            ClientConnected?.Invoke(connectionId);

            void HandleConnectionClosed()
            {
                connection.ConnectionClosed -= HandleConnectionClosed;

                if (_connections.Remove(connectionId))
                {
                    _idPool.Enqueue(connectionId);
                }
            
                ClientDisconnected?.Invoke(connectionId);
            }
        }

        private int GetConnectionId()
        {
            if (_idPool.Count > 0)
            {
                return _idPool.Dequeue();
            }

            return ++_lastUserId;
        }
    }
}