using System;
using System.Collections.Generic;
using JeeLee.Networking.Delegates;
using JeeLee.Networking.Exceptions;
using JeeLee.Networking.Messages;
using JeeLee.Networking.Transports;

namespace JeeLee.Networking
{
    public abstract class Peer : IDisposable
    {
        private readonly ITransport _transport;

        private readonly Dictionary<Type, IMessageRegistry> _messageRegistries;
        private readonly Dictionary<int, Type> _messageIdMap;

        protected Peer(ITransport transport)
        {
            _transport = transport;

            _messageRegistries = new Dictionary<Type, IMessageRegistry>();
            _messageIdMap = new Dictionary<int, Type>();

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
                _transport.Send(message);
            }
            
            AllocateMessageRegistry<TMessage>().ReleaseMessage(message);
        }

        public TMessage GetMessage<TMessage>()
            where TMessage : Message
        {
            return AllocateMessageRegistry<TMessage>().GetMessage();
        }

        public void Subscribe<TMessage>(int messageId, MessageHandler<TMessage> handler)
            where TMessage : Message
        {
            if (!TryBindMessageId<TMessage>(messageId))
            {
                throw new InvalidBindException();
            }

            AllocateMessageRegistry<TMessage>().AddHandler(handler);
        }

        public void Unsubscribe<TMessage>(int messageId, MessageHandler<TMessage> handler)
            where TMessage : Message
        {
            if (!TryBindMessageId<TMessage>(messageId))
            {
                throw new InvalidBindException();
            }

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

        private void OnMessageReceived(int messageId, byte[] dataStream)
        {
            throw new NotImplementedException();
        }

        private MessageRegistry<TMessage> AllocateMessageRegistry<TMessage>()
            where TMessage : Message
        {
            if (!_messageRegistries.TryGetValue(typeof(TMessage), out var registry))
            {
                _messageRegistries.Add(typeof(TMessage), registry = new MessageRegistry<TMessage>());
            }

            return (MessageRegistry<TMessage>)registry;
        }

        private bool TryBindMessageId<TMessage>(int messageId)
            where TMessage : Message
        {
            if (!_messageIdMap.TryGetValue(messageId, out var messageType))
            {
                _messageIdMap.Add(messageId, messageType = typeof(TMessage));
            }

            return typeof(TMessage) == messageType;
        }
    }
}