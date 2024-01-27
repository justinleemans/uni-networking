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
        public event Action<int> OnClientConnected;
        public event Action<int> OnClientDisconnected;
        
        private readonly IServerTransport _transport;
        private readonly Queue<int> _idPool = new Queue<int>();
        private readonly Dictionary<int, Connection> _connections = new Dictionary<int, Connection>();

        private int _nextUserId;
        
        /// <summary>
        /// Is this server currently active and running.
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Constructor for this server peer. Start with default tcp protocol.
        /// </summary>
        public Server() : this(new TcpServerTransport())
        {
        }
        
        /// <summary>
        /// Contstructor for this peer. Starts server with given transport.
        /// </summary>
        /// <param name="transport">The transport to run the communications over.</param>
        public Server(IServerTransport transport)
        {
            _transport = transport;
            _transport.OnNewConnection += OnNewConnection;
        }

        /// <summary>
        /// Starts the server over the selected transport.
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
        /// Stops the server if it is currently running.
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

        public void Close(int connectionId)
        {
            if (!_connections.TryGetValue(connectionId, out var connection))
            {
                return;
            }

            connection.Close();
        }

        public void SendMessage<TMessage>(int connectionId)
            where TMessage : Message
        {
            TMessage message = GetMessage<TMessage>();
            SendMessage(connectionId, message);
        }

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

        public void Subscribe<TMessage>(MessageFromHandler<TMessage> handler)
            where TMessage : Message
        {
            AllocateMessageRegistry<TMessage>().AddHandler(handler);
        }

        public void Unsubscribe<TMessage>(MessageFromHandler<TMessage> handler)
            where TMessage : Message
        {
            AllocateMessageRegistry<TMessage>().RemoveHandler(handler);
        }

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