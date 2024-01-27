using System;
using System.Collections.Generic;
using System.Reflection;
using JeeLee.Networking.Exceptions;
using JeeLee.Networking.Messages;
using JeeLee.Networking.Messages.Attributes;
using JeeLee.Networking.Messages.Streams;

namespace JeeLee.Networking
{
    /// <summary>
    /// General peer class which includes all shared functionality between peers `Client` and `Server`.
    /// </summary>
    public abstract class Peer
    {
        private readonly Dictionary<int, IMessageRegistry> _messageRegistries = new Dictionary<int, IMessageRegistry>();
        private readonly Dictionary<Type, int> _messageIdMap = new Dictionary<Type, int>();

        /// <summary>
        /// Sends a new instance of this message without setting properties.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to be send.</typeparam>
        public void SendMessage<TMessage>()
            where TMessage : Message
        {
            SendMessage(GetMessage<TMessage>());
        }

        /// <summary>
        /// Sends a message of the given instance. Properties can be set before hand.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to be send.</typeparam>
        /// <param name="message">The message instance to be send.</param>
        public void SendMessage<TMessage>(TMessage message)
            where TMessage : Message
        {
            int messageId = RegisterMessageId<TMessage>();
            message.DataStream.Sign(messageId);
            
            OnSendMessage(message);

            AllocateMessageRegistry<TMessage>().ReleaseMessage(message);
        }

        /// <summary>
        /// Allocates a message instance of the given type.
        /// </summary>
        /// <typeparam name="TMessage">The message type to allocate.</typeparam>
        /// <returns>An instance of the message of the requested type.</returns>
        public TMessage GetMessage<TMessage>()
            where TMessage : Message
        {
            return AllocateMessageRegistry<TMessage>().GetMessage();
        }

        /// <summary>
        /// Subscribe a given method to messages of this type.
        /// </summary>
        /// <typeparam name="TMessage">The message type to subscribe to.</typeparam>
        /// <param name="handler">The method to be called when this message is fired.</param>
        public void Subscribe<TMessage>(MessageHandler<TMessage> handler)
            where TMessage : Message
        {
            AllocateMessageRegistry<TMessage>().AddHandler(handler);
        }

        /// <summary>
        /// Unsubscribe a given method from messages of this type.
        /// </summary>
        /// <typeparam name="TMessage">The message type to unscubscribe from.</typeparam>
        /// <param name="handler">The method which needs to be unsubscribed.</param>
        public void Unsubscribe<TMessage>(MessageHandler<TMessage> handler)
            where TMessage : Message
        {
            AllocateMessageRegistry<TMessage>().RemoveHandler(handler);
        }

        /// <summary>
        /// Runs the update loop of the networking solution. Recommended to run this in `FixedUpdate`.
        /// Makes sure to handle all connections and receiving of incomming messages.
        /// </summary>
        public abstract void Tick();

        protected abstract void OnSendMessage<TMessage>(TMessage message)
            where TMessage : Message;

        protected void OnMessageReceived(int connectionId, DataStream dataStream)
        {
            int messageId = dataStream.ReadInt();

            if (!_messageRegistries.TryGetValue(messageId, out var registry))
            {
                return;
            }

            registry.Handle(connectionId, dataStream);
        }

        protected MessageRegistry<TMessage> AllocateMessageRegistry<TMessage>()
            where TMessage : Message
        {
            int messageId = RegisterMessageId<TMessage>();
            
            if (!_messageRegistries.TryGetValue(messageId, out var registry))
            {
                _messageRegistries.Add(messageId, registry = new MessageRegistry<TMessage>());
            }

            return (MessageRegistry<TMessage>)registry;
        }

        protected int RegisterMessageId<TMessage>()
            where TMessage : Message
        {
            if (!_messageIdMap.TryGetValue(typeof(TMessage), out var messageId))
            {
                MessageAttribute messageAttribute = typeof(TMessage).GetCustomAttribute(typeof(MessageAttribute)) as MessageAttribute;

                if (messageAttribute == null)
                {
                    throw new InvalidBindException();
                }

                _messageIdMap.Add(typeof(TMessage), messageId = messageAttribute.MessageId);
            }

            return messageId;
        }
    }
}