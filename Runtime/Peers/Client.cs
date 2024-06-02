using System;
using JeeLee.UniNetworking.Logging;
using JeeLee.UniNetworking.Messages;
using JeeLee.UniNetworking.Messages.Registries;
using JeeLee.UniNetworking.Payloads;
using JeeLee.UniNetworking.Transports;
using JeeLee.UniNetworking.Transports.Tcp;

namespace JeeLee.UniNetworking.Peers
{
    /// <summary>
    /// Runs the general client code which allows a connection to be made to a server.
    /// </summary>
    public sealed class Client : IClient
    {
        /// <summary>
        /// Event triggered when this client gets disconnected.
        /// </summary>
        public event Action ClientDisconnected
        {
            add => _clientTransport.ClientDisconnected += value;
            remove => _clientTransport.ClientDisconnected -= value;
        }

        private readonly IMessageRegistry _messageRegistry = new MessageRegistry();
        private readonly IClientTransport _clientTransport;

        /// <summary>
        /// Gets a value indicating whether the client is currently connected to a server.
        /// </summary>
        public bool IsConnected => _clientTransport.IsConnected;

        /// <summary>
        /// Initializes a new instance of the <see cref="Client"/> class using the default TCP client transport.
        /// </summary>
        public Client() : this(new TcpClientTransport())
        {
            NetworkLogger.Log("Created new client instance with default transport (Tcp)", LogLevel.Warning);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Client"/> class with a specified client transport.
        /// </summary>
        /// <param name="transport">The client transport to use.</param>
        public Client(IClientTransport clientTransport)
        {
            _clientTransport = clientTransport;
        }

        /// <summary>
        /// Connects the client to a server.
        /// </summary>
        public bool Connect()
        {
            _clientTransport.Connect();

            return IsConnected;
        }

        /// <summary>
        /// Disconnects the client from the current server.
        /// </summary>
        public void Disconnect()
        {
            _clientTransport.Disconnect();
        }
        
        /// <summary>
        /// Called periodically to perform any necessary actions.
        /// </summary>
        public void Tick()
        {
            if (!IsConnected)
            {
                return;
            }
            
            _clientTransport.Tick();
            _clientTransport.Receive(OnMessageReceived);
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
        /// Sends a specific message.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to be sent.</typeparam>
        /// <param name="message">The message to be sent.</param>
        public void SendMessage<TMessage>(TMessage message)
            where TMessage : Message
        {
            try
            {
                short messageId = _messageRegistry.RegisterMessageId<TMessage>();
                Payload payload = message.Serialize(messageId);
            
                if (payload != null)
                {
                    _clientTransport.Send(payload);
                }
            }
            catch (Exception exception)
            {
                NetworkLogger.Log(exception, LogLevel.Error);
            }

            _messageRegistry.AllocateMessageBroker<TMessage>()?.ReleaseMessage(message);
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
        /// Unsubscribes a message handler for the specified message type.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to unsubscribe from.</typeparam>
        /// <param name="handler">The message handler to be unsubscribed.</param>
        public void Unsubscribe<TMessage>(MessageHandler<TMessage> handler)
            where TMessage : Message
        {
            _messageRegistry.AllocateMessageBroker<TMessage>()?.RemoveHandler(handler);
        }
        
        private void OnMessageReceived(Payload payload, int connectionId)
        {
            if (!_messageRegistry.MessageBrokers.TryGetValue(payload.MessageId, out var registry))
            {
                return;
            }

            registry.Handle(connectionId, payload);
        }
    }
}