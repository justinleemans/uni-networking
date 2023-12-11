using System;
using System.Collections.Generic;
using JeeLee.Networking.Delegates;
using JeeLee.Networking.Exceptions;
using JeeLee.Networking.Messages;
using JeeLee.Networking.Messages.Pooling;
using JeeLee.Networking.Messages.Subscriptions;
using JeeLee.Networking.Transports;

namespace JeeLee.Networking
{
    public abstract class Peer : IDisposable
    {
        private readonly ITransport _transport;

        private readonly Dictionary<Type, IPool> _messagePools;
        private readonly Dictionary<Type, ISubscription> _messageSubscriptions;
        private readonly Dictionary<int, Type> _messageIdMap;

        protected Peer(ITransport transport)
        {
            _transport = transport;

            _messagePools = new Dictionary<Type, IPool>();
            _messageSubscriptions = new Dictionary<Type, ISubscription>();
            _messageIdMap = new Dictionary<int, Type>();

            _transport.OnMessageReceived = OnMessageReceived;
        }

        public void Dispose()
        {
            _messagePools.Clear();
            _messageSubscriptions.Clear();
            _messageIdMap.Clear();
            
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
            
            AllocateMessagePool<TMessage>().Release(message);
        }

        public TMessage GetMessage<TMessage>()
            where TMessage : Message
        {
            return AllocateMessagePool<TMessage>().Get();
        }

        public void Subscribe<TMessage>(int messageId, MessageHandler<TMessage> handler)
            where TMessage : Message
        {
            if (!TryBindMessageId<TMessage>(messageId))
            {
                throw new InvalidBindException();
            }

            AllocateMessageSubscription<TMessage>().AddHandler(handler);
        }

        public void Unsubscribe<TMessage>(int messageId, MessageHandler<TMessage> handler)
            where TMessage : Message
        {
            if (!TryBindMessageId<TMessage>(messageId))
            {
                throw new InvalidBindException();
            }

            AllocateMessageSubscription<TMessage>().RemoveHandler(handler);
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
            if (!_messageIdMap.TryGetValue(messageId, out var messageType))
            {
                return;
            }

            if (!_messageSubscriptions.TryGetValue(messageType, out var subscription))
            {
                return;
            }

            throw new NotImplementedException();
        }

        private Pool<TMessage> AllocateMessagePool<TMessage>()
            where TMessage : Message
        {
            if (!_messagePools.TryGetValue(typeof(TMessage), out var pool))
            {
                _messagePools.Add(typeof(TMessage), pool = new Pool<TMessage>());
            }

            return (Pool<TMessage>)pool;
        }

        private Subscription<TMessage> AllocateMessageSubscription<TMessage>()
            where TMessage : Message
        {
            if (!_messageSubscriptions.TryGetValue(typeof(TMessage), out var subscription))
            {
                _messageSubscriptions.Add(typeof(TMessage), subscription = new Subscription<TMessage>());
            }

            return (Subscription<TMessage>)subscription;
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