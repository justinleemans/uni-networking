using System;
using System.Collections.Generic;
using System.Reflection;
using JeeLee.Networking.Exceptions;
using JeeLee.Networking.Messages;
using JeeLee.Networking.Messages.Attributes;
using JeeLee.Networking.Messages.Delegates;
using JeeLee.Networking.Messages.Streams;
using JeeLee.Networking.Transports;

namespace JeeLee.Networking
{
    public abstract class Peer : IDisposable
    {
        private readonly ITransport _transport;

        private readonly Dictionary<int, IMessageRegistry> _messageRegistries;
        private readonly Dictionary<Type, int> _messageIdMap;

        protected Peer(ITransport transport)
        {
            _transport = transport;

            _messageRegistries = new Dictionary<int, IMessageRegistry>();
            _messageIdMap = new Dictionary<Type, int>();

            _transport.OnMessageReceived = OnMessageReceived;
        }

        public void Dispose()
        {
            _transport.OnMessageReceived = null;
        }

        public void SendMessage<TMessage>()
            where TMessage : Message
        {
            SendMessage(GetMessage<TMessage>());
        }

        public void SendMessage<TMessage>(TMessage message)
            where TMessage : Message
        {
            var isServerRunning = _transport is IServerTransport server && server.IsRunning;
            var isClientConnected = _transport is IClientTransport client && client.IsConnected;
            
            if(isServerRunning || isClientConnected)
            {
                int messageId = RegisterMessageId<TMessage>();
                DataStream dataStream = message.DataStream;
                dataStream.Sign(messageId);

                _transport.Send(dataStream);
            }
            
            AllocateMessageRegistry<TMessage>().ReleaseMessage(message);
        }

        public TMessage GetMessage<TMessage>()
            where TMessage : Message
        {
            return AllocateMessageRegistry<TMessage>().GetMessage();
        }

        public void Subscribe<TMessage>(MessageHandler<TMessage> handler)
            where TMessage : Message
        {
            AllocateMessageRegistry<TMessage>().AddHandler(handler);
        }

        public void Unsubscribe<TMessage>(MessageHandler<TMessage> handler)
            where TMessage : Message
        {
            AllocateMessageRegistry<TMessage>().RemoveHandler(handler);
        }

        public void Tick()
        {
            var isServerRunning = _transport is IServerTransport server && server.IsRunning;
            var isClientConnected = _transport is IClientTransport client && client.IsConnected;
            
            if(!isServerRunning && !isClientConnected)
            {
                return;
            }

            _transport.Tick();
        }

        private void OnMessageReceived(DataStream dataStream)
        {
            int messageId = dataStream.ReadInt();

            if (!_messageRegistries.TryGetValue(messageId, out var registry))
            {
                return;
            }

            registry.Handle(dataStream);
        }

        private MessageRegistry<TMessage> AllocateMessageRegistry<TMessage>()
            where TMessage : Message
        {
            int messageId = RegisterMessageId<TMessage>();
            
            if (!_messageRegistries.TryGetValue(messageId, out var registry))
            {
                _messageRegistries.Add(messageId, registry = new MessageRegistry<TMessage>());
            }

            return (MessageRegistry<TMessage>)registry;
        }

        private int RegisterMessageId<TMessage>()
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