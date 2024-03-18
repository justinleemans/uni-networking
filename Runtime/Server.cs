using System;
using System.Collections.Generic;
using JeeLee.UniNetworking.Messages;
using JeeLee.UniNetworking.Transports;
using JeeLee.UniNetworking.Transports.Tcp;

namespace JeeLee.UniNetworking
{
    /// <summary>
    /// Server peer. Runs the general server code which allows clients to connect to it.
    /// </summary>
    public sealed class Server : Peer
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
        /// Called periodically to perform any necessary actions.
        /// </summary>
        public override void Tick()
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

        /// <summary>
        /// Called before sending a message to perform any specific actions.
        /// </summary>
        /// <typeparam name="TMessage">The type of message being sent.</typeparam>
        /// <param name="message">The message being sent.</param>
        protected override void OnSendMessage<TMessage>(TMessage message)
        {
            if (!IsRunning)
            {
                return;
            }

            foreach (var connection in _connections.Values)
            {
                connection.Send(message.DataStream);
            }
        }

        #endregion

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
            catch
            {
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
            catch
            {
            }
        }

        /// <summary>
        /// Closes the connection with the specified connection identifier.
        /// </summary>
        /// <param name="connectionId">The connection identifier to close.</param>
        public void Close(int connectionId)
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
            int messageId = RegisterMessageId<TMessage>();
            message.DataStream.Sign(messageId);

            if (_connections.TryGetValue(connectionId, out var connection))
            {
                connection.Send(message.DataStream);
            }

            AllocateMessageRegistry<TMessage>().ReleaseMessage(message);
        }

        /// <summary>
        /// Subscribes a message handler for messages of type <typeparamref name="TMessage"/>.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to subscribe to.</typeparam>
        /// <param name="handler">The message handler to be subscribed.</param>
        public void Subscribe<TMessage>(MessageFromHandler<TMessage> handler)
            where TMessage : Message
        {
            AllocateMessageRegistry<TMessage>().AddHandler(handler);
        }

        /// <summary>
        /// Unsubscribes a message handler for messages of type <typeparamref name="TMessage"/>.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to unsubscribe from.</typeparam>
        /// <param name="handler">The message handler to be unsubscribed.</param>
        public void Unsubscribe<TMessage>(MessageFromHandler<TMessage> handler)
            where TMessage : Message
        {
            AllocateMessageRegistry<TMessage>().RemoveHandler(handler);
        }

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