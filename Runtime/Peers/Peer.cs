using System;
using System.Collections.Generic;
using System.Reflection;
using JeeLee.UniNetworking.Logging;
using JeeLee.UniNetworking.Messages;
using JeeLee.UniNetworking.Messages.Attributes;
using JeeLee.UniNetworking.Payloads;

namespace JeeLee.UniNetworking.Peers
{
    /// <summary>
    /// The base class for network communication peers.
    /// </summary>
    public abstract class Peer : IPeer
    {
        private readonly Dictionary<short, IMessageRegistry> _messageRegistries = new Dictionary<short, IMessageRegistry>();
        private readonly Dictionary<Type, short> _messageIdMap = new Dictionary<Type, short>();

        #region IPeer Members

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
                short messageId = RegisterMessageId<TMessage>();
                Payload payload = message.Serialize(messageId);
            
                if (payload != null)
                {
                    SendPayload(payload);
                }
            }
            catch (Exception exception)
            {
                NetworkLogger.Log(exception, LogLevel.Error);
            }

            AllocateMessageRegistry<TMessage>()?.ReleaseMessage(message);
        }

        /// <summary>
        /// Gets an instance of the message of type <typeparamref name="TMessage"/>.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to retrieve.</typeparam>
        /// <returns>An instance of the specified message type.</returns>
        public TMessage GetMessage<TMessage>()
            where TMessage : Message
        {
            return AllocateMessageRegistry<TMessage>()?.GetMessage();
        }

        /// <summary>
        /// Subscribes a message handler for the specified message type.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to subscribe to.</typeparam>
        /// <param name="handler">The message handler to be subscribed.</param>
        public void Subscribe<TMessage>(MessageHandler<TMessage> handler)
            where TMessage : Message
        {
            AllocateMessageRegistry<TMessage>()?.AddHandler(handler);
        }

        /// <summary>
        /// Unsubscribes a message handler for the specified message type.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to unsubscribe from.</typeparam>
        /// <param name="handler">The message handler to be unsubscribed.</param>
        public void Unsubscribe<TMessage>(MessageHandler<TMessage> handler)
            where TMessage : Message
        {
            AllocateMessageRegistry<TMessage>()?.RemoveHandler(handler);
        }

        #endregion

        /// <summary>
        /// Sends a payload to the peer.
        /// </summary>
        /// <param name="payload">The payload to be sent.</param>
        protected abstract void SendPayload(Payload payload);

        /// <summary>
        /// Called when a message is received, allowing the peer to handle the incoming message.
        /// </summary>
        /// <param name="connectionId">The connection identifier.</param>
        /// <param name="payload">The payload containing the received message.</param>
        protected void OnMessageReceived(int connectionId, Payload payload)
        {
            if (!_messageRegistries.TryGetValue(payload.MessageId, out var registry))
            {
                return;
            }

            registry.Handle(connectionId, payload);
        }

        /// <summary>
        /// Allocates or retrieves the message registry for a specific message type.
        /// </summary>
        /// <typeparam name="TMessage">The type of message for which to allocate or retrieve the registry.</typeparam>
        /// <returns>The message registry for the specified message type.</returns>
        protected MessageRegistry<TMessage> AllocateMessageRegistry<TMessage>()
            where TMessage : Message
        {
            try
            {
                short messageId = RegisterMessageId<TMessage>();
            
                if (!_messageRegistries.TryGetValue(messageId, out var registry))
                {
                    _messageRegistries.Add(messageId, registry = new MessageRegistry<TMessage>());
                }

                return (MessageRegistry<TMessage>)registry;
            }
            catch (Exception exception)
            {
                NetworkLogger.Log(exception, LogLevel.Error);
                
                return null;
            }
        }

        /// <summary>
        /// Registers and retrieves the message identifier for a specific message type.
        /// </summary>
        /// <typeparam name="TMessage">The type of message for which to register and retrieve the identifier.</typeparam>
        /// <returns>The message identifier for the specified message type.</returns>
        protected short RegisterMessageId<TMessage>()
            where TMessage : Message
        {
            if (!_messageIdMap.TryGetValue(typeof(TMessage), out var messageId))
            {
                MessageAttribute messageAttribute = typeof(TMessage).GetCustomAttribute(typeof(MessageAttribute)) as MessageAttribute;

                if (messageAttribute == null)
                {
                    throw new Exception("Could not bind message, message id not found");
                }

                _messageIdMap.Add(typeof(TMessage), messageId = messageAttribute.MessageId);
            }

            return messageId;
        }
    }
}