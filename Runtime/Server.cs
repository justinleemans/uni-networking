using System;
using System.Collections.Generic;
using JeeLee.UniNetworking.Logging;
using JeeLee.UniNetworking.Messages;
using JeeLee.UniNetworking.Messages.Streams;
using JeeLee.UniNetworking.Transports;
using JeeLee.UniNetworking.Transports.Tcp;

namespace JeeLee.UniNetworking
{
    /// <summary>
    /// Server peer. Runs the general server code which allows clients to connect to it.
    /// </summary>
    public sealed class Server : Peer, IServer
    {
        /// <summary>
        /// Event triggered when a client connects to the server.
        /// </summary>
        public event Action<int> ClientConnected;

        /// <summary>
        /// Event triggered when a client disconnects from the server.
        /// </summary>
        public event Action<int> ClientDisconnected;
        
        private readonly IServerTransport _transport;
        private readonly Queue<int> _idPool = new Queue<int>();
        private readonly Dictionary<int, Connection> _connections = new Dictionary<int, Connection>();

        private int _lastUserId;
        
        /// <summary>
        /// Gets a value indicating whether the server is running.
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Server"/> class using the default TCP server transport.
        /// </summary>
        public Server() : this(new TcpServerTransport())
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Server"/> class with a specified server transport.
        /// </summary>
        /// <param name="transport">The server transport to use.</param>
        public Server(IServerTransport transport)
        {
            _transport = transport;
            _transport.NewConnection += OnNewConnection;
        }

        #region Peer Members

        /// <summary>
        /// Sends a data stream to the peer.
        /// </summary>
        /// <param name="dataStream">The data stream to be sent.</param>
        protected override void SendDataStream(DataStream dataStream)
        {
            if (!IsRunning)
            {
                return;
            }

            foreach (var connection in _connections.Values)
            {
                connection.Send(dataStream);
            }
        }

        #endregion

        #region IServer Members

        /// <summary>
        /// Starts the server.
        /// </summary>
        public bool Start()
        {
            if (IsRunning)
            {
                return true;
            }

            try
            {
                _transport.Start();

                return IsRunning = true;
            }
            catch (Exception exception)
            {
                NetworkLogger.Log(exception, LogLevel.Error);

                return IsRunning = false;
            }
        }

        /// <summary>
        /// Stops the server.
        /// </summary>
        public void Stop()
        {
            if (!IsRunning)
            {
                return;
            }
            
            try
            {
                foreach (var connection in _connections.Values)
                {
                    connection.Close();
                }

                _idPool.Clear();
                _connections.Clear();
                _lastUserId = 0;

                _transport.Stop();

                IsRunning = false;
            }
            catch (Exception exception)
            {
                NetworkLogger.Log(exception, LogLevel.Error);
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

            connection.Close();
        }

        /// <summary>
        /// Sends a message of type <typeparamref name="TMessage"/> to the specified client.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to be sent.</typeparam>
        /// <param name="connectionId">The connection identifier of the client.</param>
        public void SendMessage<TMessage>(int connectionId)
            where TMessage : Message
        {
            TMessage message = GetMessage<TMessage>();
            SendMessage(message, connectionId);
        }

        /// <summary>
        /// Sends a specific message to the specified client.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to be sent.</typeparam>
        /// <param name="message">The message to be sent.</param>
        /// /// <param name="connectionId">The connection identifier of the client.</param>
        public void SendMessage<TMessage>(TMessage message, int connectionId)
            where TMessage : Message
        {
            try
            {
                int messageId = RegisterMessageId<TMessage>();
                DataStream dataStream = message.Serialize(messageId);

                if (dataStream != null && _connections.TryGetValue(connectionId, out var connection))
                {
                    connection.Send(dataStream);
                }
            }
            catch (Exception exception)
            {
                NetworkLogger.Log(exception, LogLevel.Error);
            }

            AllocateMessageRegistry<TMessage>()?.ReleaseMessage(message);
        }

        /// <summary>
        /// Subscribes a message handler for messages of type <typeparamref name="TMessage"/>.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to subscribe to.</typeparam>
        /// <param name="handler">The message handler to be subscribed.</param>
        public void Subscribe<TMessage>(MessageFromHandler<TMessage> handler)
            where TMessage : Message
        {
            AllocateMessageRegistry<TMessage>()?.AddHandler(handler);
        }

        /// <summary>
        /// Unsubscribes a message handler for messages of type <typeparamref name="TMessage"/>.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to unsubscribe from.</typeparam>
        /// <param name="handler">The message handler to be unsubscribed.</param>
        public void Unsubscribe<TMessage>(MessageFromHandler<TMessage> handler)
            where TMessage : Message
        {
            AllocateMessageRegistry<TMessage>()?.RemoveHandler(handler);
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

            _transport.Tick();

            foreach (var connection in _connections)
            {
                connection.Value.Receive(dataStream => OnMessageReceived(connection.Key, dataStream));
            }
        }

        #endregion

        private void OnNewConnection(Connection connection)
        {
            int connectionId = GetConnectionId();
            connection.ConnectionClosed += Handle;
            _connections.Add(connectionId, connection);

            ClientConnected?.Invoke(connectionId);

            void Handle()
            {
                connection.ConnectionClosed -= Handle;

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