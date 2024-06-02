using System;
using System.Collections.Generic;
using System.Reflection;
using JeeLee.UniNetworking.Logging;
using JeeLee.UniNetworking.Messages.Attributes;

namespace JeeLee.UniNetworking.Messages.Registries
{
    public class MessageRegistry : IMessageRegistry
    {
        private readonly Dictionary<short, IMessageBroker> _messageBrokers = new Dictionary<short, IMessageBroker>();
        private readonly Dictionary<Type, short> _messageIdMap = new Dictionary<Type, short>();

        public IReadOnlyDictionary<short, IMessageBroker> MessageBrokers => _messageBrokers;

        public MessageBroker<TMessage> AllocateMessageBroker<TMessage>()
            where TMessage : Message
        {
            try
            {
                short messageId = RegisterMessageId<TMessage>();
            
                if (!_messageBrokers.TryGetValue(messageId, out var registry))
                {
                    _messageBrokers.Add(messageId, registry = new MessageBroker<TMessage>());
                }

                return (MessageBroker<TMessage>)registry;
            }
            catch (Exception exception)
            {
                NetworkLogger.Log(exception, LogLevel.Error);
                
                return null;
            }
        }

        public short RegisterMessageId<TMessage>()
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