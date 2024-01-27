using System;
using System.Collections.Generic;
using JeeLee.Networking.Messages;
using JeeLee.Networking.Transports;
using JeeLee.Networking.Transports.Tcp;

namespace JeeLee.Networking
{
    /// <summary>
    /// Server peer. Runs the general server code which allows clients to connect to it.
    /// </summary>
    public sealed class Server : Peer
    {
        /// <summary>
        /// Event triggered when a client connects to the server.
        /// </summary>
        public event Action<int> OnClientConnected;

        /// <summary>
        /// Event triggered when a client disconnects from the server.
        /// </summary>
        public event Action<int> OnClientDisconnected;
        
        private readonly IServerTransport _transport;
        private readonly Queue<int> _idPool = new Queue<int>();
        private readonly Dictionary<int, Connection> _connections = new Dictionary<int, Connection>();

        private int _nextUserId;
        
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
            _transport.OnNewConnection += OnNewConnection;
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
        public void Start()
        {
            if (IsRunning)
            {
                return;
            }

            _transport.Start();
            IsRunning = true;
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
            
            _transport.Stop();
            _connections.Clear();

            IsRunning = false;
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
            SendMessage(connectionId, message);
        }

        /// <summary>
        /// Sends a specific message to the specified client.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to be sent.</typeparam>
        /// <param name="connectionId">The connection identifier of the client.</param>
        /// <param name="message">The message to be sent.</param>
        public void SendMessage<TMessage>(int connectionId, TMessage message)
            where TMessage : Message
        {
            int messageId = RegisterMessageId<TMessage>();
            message.DataStream.Sign(messageId);

            if (!_connections.TryGetValue(connectionId, out var connection))
            {
                return;
            }

            connection.Send(message.DataStream);

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
            connection.OnConnectionClosed += OnConnectionClosed;
            _connections.Add(connectionId, connection);

            OnClientConnected?.Invoke(connectionId);

            void OnConnectionClosed()
            {
                connection.OnConnectionClosed -= OnConnectionClosed;

                if (_connections.Remove(connectionId))
                {
                    _idPool.Enqueue(connectionId);
                }

                OnClientDisconnected?.Invoke(connectionId);
            }
        }

        private int GetConnectionId()
        {
            if (_idPool.Count > 0)
            {
                return _idPool.Dequeue();
            }

            return ++_nextUserId;
        }
    }
}