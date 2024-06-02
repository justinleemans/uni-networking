using System;
using JeeLee.UniNetworking.Logging;
using JeeLee.UniNetworking.Messages;
using JeeLee.UniNetworking.Messages.Registries;
using JeeLee.UniNetworking.Payloads;

namespace JeeLee.UniNetworking.Peers
{
    /// <summary>
    /// The base class for network communication peers.
    /// </summary>
    public abstract class Peer : IPeer
    {
        protected IMessageRegistry MessageRegistry { get; } = new MessageRegistry();

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
                short messageId = MessageRegistry.RegisterMessageId<TMessage>();
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

            MessageRegistry.AllocateMessageBroker<TMessage>()?.ReleaseMessage(message);
        }

        /// <summary>
        /// Gets an instance of the message of type <typeparamref name="TMessage"/>.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to retrieve.</typeparam>
        /// <returns>An instance of the specified message type.</returns>
        public TMessage GetMessage<TMessage>()
            where TMessage : Message
        {
            return MessageRegistry.AllocateMessageBroker<TMessage>()?.GetMessage();
        }

        /// <summary>
        /// Subscribes a message handler for the specified message type.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to subscribe to.</typeparam>
        /// <param name="handler">The message handler to be subscribed.</param>
        public void Subscribe<TMessage>(MessageHandler<TMessage> handler)
            where TMessage : Message
        {
            MessageRegistry.AllocateMessageBroker<TMessage>()?.AddHandler(handler);
        }

        /// <summary>
        /// Unsubscribes a message handler for the specified message type.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to unsubscribe from.</typeparam>
        /// <param name="handler">The message handler to be unsubscribed.</param>
        public void Unsubscribe<TMessage>(MessageHandler<TMessage> handler)
            where TMessage : Message
        {
            MessageRegistry.AllocateMessageBroker<TMessage>()?.RemoveHandler(handler);
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
            if (!MessageRegistry.MessageBrokers.TryGetValue(payload.MessageId, out var registry))
            {
                return;
            }

            registry.Handle(connectionId, payload);
        }
    }
}