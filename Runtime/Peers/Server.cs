using System;
using System.Collections.Generic;
using JeeLee.UniNetworking.Logging;
using JeeLee.UniNetworking.Messages;
using JeeLee.UniNetworking.Messages.Registries;
using JeeLee.UniNetworking.Payloads;
using JeeLee.UniNetworking.Transports;
using JeeLee.UniNetworking.Transports.Tcp;

namespace JeeLee.UniNetworking.Peers
{
    /// <summary>
    /// Runs the general server code which allows clients to connect to it.
    /// </summary>
    public sealed class Server : IServer
    {
        /// <summary>
        /// Event triggered when a client connects to the server.
        /// </summary>
        public event Action<int> ClientConnected;
        
        /// <summary>
        /// Event triggered when a client disconnects from the server.
        /// </summary>
        public event Action<int> ClientDisconnected;

        private readonly IMessageRegistry _messageRegistry = new MessageRegistry();
        private readonly IServerTransport _serverTransport;
        private readonly Queue<int> _idPool = new Queue<int>();

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
            NetworkLogger.Log("Created new server instance with default transport (Tcp)", LogLevel.Warning);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Server"/> class with a specified server transport.
        /// </summary>
        /// <param name="transport">The server transport to use.</param>
        public Server(IServerTransport serverTransport)
        {
            _serverTransport = serverTransport;

            _serverTransport.ClientConnected = OnClientConnected;
            _serverTransport.ClientDisconnected = OnClientDisconnected;
            _serverTransport.MessageReceived = OnMessageReceived;
        }

        /// <summary>
        /// Starts the server.
        /// </summary>
        public bool Start()
        {
            if (IsRunning)
            {
                NetworkLogger.Log("Server is already running", LogLevel.Warning);
                return IsRunning = true;
            }

            try
            {
                _serverTransport.Start();
                NetworkLogger.Log("Server started");
                
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
                _serverTransport.Stop();
                NetworkLogger.Log("Server stopped");

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
            if (!IsRunning)
            {
                return;
            }
            
            _serverTransport.CloseConnection(connectionId);
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
            
            _serverTransport.Tick();
        }

        /// <summary>
        /// Sends a message of type <typeparamref name="TMessage"/>.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to be sent.</typeparam>
        public void SendMessage<TMessage>()
            where TMessage : Message
        {
            SendMessage(GetMessage<TMessage>());
        }
        
        /// <summary>
        /// Sends a message of type <typeparamref name="TMessage"/> to the specified client.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to be sent.</typeparam>
        /// <param name="connectionId">The connection identifier of the client.</param>
        public void SendMessage<TMessage>(int connectionId)
            where TMessage : Message
        {
            SendMessage(GetMessage<TMessage>(), connectionId);
        }

        /// <summary>
        /// Sends a specific message.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to be sent.</typeparam>
        /// <param name="message">The message to be sent.</param>
        public void SendMessage<TMessage>(TMessage message)
            where TMessage : Message
        {
            try
            {
                if (!IsRunning)
                {
                    return;
                }
                
                short messageId = _messageRegistry.RegisterMessageId<TMessage>();
                Payload payload = message.Serialize(messageId);
            
                if (payload != null)
                {
                    _serverTransport.Send(payload);
                }
            }
            catch (Exception exception)
            {
                NetworkLogger.Log(exception, LogLevel.Error);
            }
            finally
            {
                _messageRegistry.AllocateMessageBroker<TMessage>()?.ReleaseMessage(message);
            }
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
                if (!IsRunning)
                {
                    return;
                }
                
                short messageId = _messageRegistry.RegisterMessageId<TMessage>();
                Payload payload = message.Serialize(messageId);

                if (payload != null)
                {
                    _serverTransport.Send(payload, connectionId);
                }
            }
            catch (Exception exception)
            {
                NetworkLogger.Log(exception, LogLevel.Error);
            }
            finally
            {
                _messageRegistry.AllocateMessageBroker<TMessage>()?.ReleaseMessage(message);
            }
        }

        /// <summary>
        /// Gets an instance of the message of type <typeparamref name="TMessage"/>.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to retrieve.</typeparam>
        /// <returns>An instance of the specified message type.</returns>
        public TMessage GetMessage<TMessage>()
            where TMessage : Message
        {
            return _messageRegistry.AllocateMessageBroker<TMessage>()?.GetMessage();
        }

        /// <summary>
        /// Subscribes a message handler for the specified message type.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to subscribe to.</typeparam>
        /// <param name="handler">The message handler to be subscribed.</param>
        public void Subscribe<TMessage>(MessageHandler<TMessage> handler)
            where TMessage : Message
        {
            _messageRegistry.AllocateMessageBroker<TMessage>()?.AddHandler(handler);
        }
        
        /// <summary>
        /// Subscribes a message handler for messages of type <typeparamref name="TMessage"/>.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to subscribe to.</typeparam>
        /// <param name="handler">The message handler to be subscribed.</param>
        public void Subscribe<TMessage>(MessageFromHandler<TMessage> handler)
            where TMessage : Message
        {
            _messageRegistry.AllocateMessageBroker<TMessage>()?.AddHandler(handler);
        }

        /// <summary>
        /// Unsubscribes a message handler for the specified message type.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to unsubscribe from.</typeparam>
        /// <param name="handler">The message handler to be unsubscribed.</param>
        public void Unsubscribe<TMessage>(MessageHandler<TMessage> handler)
            where TMessage : Message
        {
            _messageRegistry.AllocateMessageBroker<TMessage>()?.RemoveHandler(handler);
        }
        
        /// <summary>
        /// Unsubscribes a message handler for messages of type <typeparamref name="TMessage"/>.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to unsubscribe from.</typeparam>
        /// <param name="handler">The message handler to be unsubscribed.</param>
        public void Unsubscribe<TMessage>(MessageFromHandler<TMessage> handler)
            where TMessage : Message
        {
            _messageRegistry.AllocateMessageBroker<TMessage>()?.RemoveHandler(handler);
        }

        private int OnClientConnected()
        {
            int connectionId = GetConnectionId();
            ClientConnected?.Invoke(connectionId);

            NetworkLogger.Log($"New client connected to server with id {connectionId}");

            return connectionId;
        }

        private void OnClientDisconnected(int connectionId)
        {
            _idPool.Enqueue(connectionId);
            ClientDisconnected?.Invoke(connectionId);

            NetworkLogger.Log($"Client with id {connectionId} disconnected");
        }

        private void OnMessageReceived(Payload payload, int connectionId)
        {
            if (!_messageRegistry.MessageBrokers.TryGetValue(payload.MessageId, out var registry))
            {
                return;
            }

            registry.Handle(connectionId, payload);
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