using System;
using System.Collections.Generic;
using JeeLee.Networking.Messages.Streams;

namespace JeeLee.Networking.Messages
{
    public sealed class MessageRegistry<TMessage> : IMessageRegistry
        where TMessage : IMessage
    {
        private readonly Queue<TMessage> _pool = new Queue<TMessage>();

        private readonly HashSet<MessageHandler<TMessage>> _handlers = new HashSet<MessageHandler<TMessage>>();
        private readonly List<MessageHandler<TMessage>> _retroAddHandlers = new List<MessageHandler<TMessage>>();
        private readonly List<MessageHandler<TMessage>> _retroRemoveHandlers = new List<MessageHandler<TMessage>>();

        private readonly HashSet<MessageFromHandler<TMessage>> _fromHandlers = new HashSet<MessageFromHandler<TMessage>>();
        private readonly List<MessageFromHandler<TMessage>> _retroAddFromHandlers = new List<MessageFromHandler<TMessage>>();
        private readonly List<MessageFromHandler<TMessage>> _retroRemoveFromHandlers = new List<MessageFromHandler<TMessage>>();

        private bool _isProcessing;

        public void Handle(int connectionId, DataStream dataStream)
        {
            _isProcessing = true;

            TMessage message = GetMessage();
            message.DataStream = dataStream;

            foreach (var handler in _handlers)
            {
                handler?.Invoke(message);
            }

            foreach (var handler in _fromHandlers)
            {
                handler?.Invoke(connectionId, message);
            }

            _isProcessing = false;

            ProcessHandlerQueues();
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

        public void AddHandler(MessageFromHandler<TMessage> handler)
        {
            if (handler == null)
            {
                throw new NullReferenceException();
            }

            if (!_isProcessing)
            {
                _fromHandlers.Add(handler);
            }
            else
            {
                _retroRemoveFromHandlers.RemoveAll(handle => handle == handler);
                _retroAddFromHandlers.Add(handler);
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

        public void RemoveHandler(MessageFromHandler<TMessage> handler)
        {
            if (handler == null)
            {
                throw new NullReferenceException();
            }

            if (!_isProcessing)
            {
                _fromHandlers.Remove(handler);
            }
            else
            {
                _retroAddFromHandlers.RemoveAll(handle => handle == handler);
                _retroRemoveFromHandlers.Add(handler);
            }
        }

        private void ProcessHandlerQueues()
        {
            foreach (var handler in _retroAddHandlers)
            {
                AddHandler(handler);
            }

            _retroAddHandlers.Clear();

            foreach (var handler in _retroAddFromHandlers)
            {
                AddHandler(handler);
            }

            _retroAddFromHandlers.Clear();

            foreach (var handler in _retroRemoveHandlers)
            {
                RemoveHandler(handler);
            }

            _retroRemoveHandlers.Clear();

            foreach (var handler in _retroRemoveFromHandlers)
            {
                RemoveHandler(handler);
            }

            _retroRemoveFromHandlers.Clear();
        }
    }
}