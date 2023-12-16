using System;
using System.Collections.Generic;
using JeeLee.Networking.Messages.Delegates;
using JeeLee.Networking.Messages.Streams;

namespace JeeLee.Networking.Messages
{
    public sealed class MessageRegistry<TMessage> : IMessageRegistry
        where TMessage : IMessage
    {
        private readonly Queue<TMessage> _pool;
        private readonly HashSet<MessageHandler<TMessage>> _handlers;
        private readonly List<MessageHandler<TMessage>> _retroAddHandlers;
        private readonly List<MessageHandler<TMessage>> _retroRemoveHandlers;

        private bool _isProcessing;

        public MessageRegistry()
        {
            _pool = new Queue<TMessage>();
            _handlers = new HashSet<MessageHandler<TMessage>>();
            _retroAddHandlers = new List<MessageHandler<TMessage>>();
            _retroRemoveHandlers = new List<MessageHandler<TMessage>>();
        }

        public TMessage GetMessage()
        {
            return _pool.Count > 0 ? _pool.Dequeue() : Activator.CreateInstance<TMessage>();
        }

        public void ReleaseMessage(TMessage message)
        {
            message.Clear();
            _pool.Enqueue(message);
        }

        public void Handle(DataStream dataStream)
        {
            _isProcessing = true;

            TMessage message = GetMessage();
            message.DataStream = dataStream;

            foreach (var handler in _handlers)
            {
                handler?.Invoke(message);
            }

            _isProcessing = false;

            ProcessHandlerQueues();
        }

        public void AddHandler(MessageHandler<TMessage> handler)
        {
            if (handler == null)
            {
                throw new NullReferenceException();
            }

            if (!_isProcessing)
            {
                _handlers.Add(handler);
            }
            else
            {
                _retroRemoveHandlers.RemoveAll(handle => handle == handler);
                _retroAddHandlers.Add(handler);
            }
        }

        public void RemoveHandler(MessageHandler<TMessage> handler)
        {
            if (handler == null)
            {
                throw new NullReferenceException();
            }

            if (!_isProcessing)
            {
                _handlers.Remove(handler);
            }
            else
            {
                _retroAddHandlers.RemoveAll(handle => handle == handler);
                _retroRemoveHandlers.Add(handler);
            }
        }

        private void ProcessHandlerQueues()
        {
            foreach (var handler in _retroAddHandlers)
            {
                AddHandler(handler);
            }

            _retroAddHandlers.Clear();

            foreach (var handler in _retroRemoveHandlers)
            {
                RemoveHandler(handler);
            }

            _retroRemoveHandlers.Clear();
        }
    }
}