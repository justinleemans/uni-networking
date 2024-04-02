using System;
using System.Collections.Generic;
using JeeLee.UniNetworking.Logging;
using JeeLee.UniNetworking.Messages.Streams;

namespace JeeLee.UniNetworking.Messages
{
    /// <summary>
    /// Represents a registry for messages of type <typeparamref name="TMessage"/> in the network communication system.
    /// </summary>
    /// <typeparam name="TMessage">The type of messages handled by the registry.</typeparam>
    public sealed class MessageRegistry<TMessage> : IMessageRegistry
        where TMessage : Message
    {
        private readonly Queue<TMessage> _pool = new Queue<TMessage>();

        private readonly HashSet<MessageHandler<TMessage>> _handlers = new HashSet<MessageHandler<TMessage>>();
        private readonly List<MessageHandler<TMessage>> _retroAddHandlers = new List<MessageHandler<TMessage>>();
        private readonly List<MessageHandler<TMessage>> _retroRemoveHandlers = new List<MessageHandler<TMessage>>();

        private readonly HashSet<MessageFromHandler<TMessage>> _fromHandlers = new HashSet<MessageFromHandler<TMessage>>();
        private readonly List<MessageFromHandler<TMessage>> _retroAddFromHandlers = new List<MessageFromHandler<TMessage>>();
        private readonly List<MessageFromHandler<TMessage>> _retroRemoveFromHandlers = new List<MessageFromHandler<TMessage>>();

        private bool _isProcessing;

        #region IMessageRegistry Members

        /// <summary>
        /// Handles a received message.
        /// </summary>
        /// <param name="connectionId">The identifier of the connection from which the message is received.</param>
        /// <param name="dataStream">The data stream containing the received message.</param>
        public void Handle(int connectionId, DataStream dataStream)
        {
            _isProcessing = true;

            TMessage message = GetMessage();
            message.Deserialize(dataStream);

            NetworkLogger.Log($"{message} received");

            foreach (var handler in _handlers)
            {
                handler?.Invoke(message);
            }

            foreach (var handler in _fromHandlers)
            {
                handler?.Invoke(message, connectionId);
            }

            _isProcessing = false;

            ProcessHandlerQueues();
        }

        #endregion

        /// <summary>
        /// Gets a message instance from the message pool or creates a new instance if the pool is empty.
        /// </summary>
        /// <returns>An instance of the message type.</returns>
        public TMessage GetMessage()
        {
            return _pool.Count > 0 ? _pool.Dequeue() : Activator.CreateInstance<TMessage>();
        }

        /// <summary>
        /// Releases a message instance back to the message pool.
        /// </summary>
        /// <param name="message">The message to be released.</param>
        public void ReleaseMessage(TMessage message)
        {
            message.Clear();
            _pool.Enqueue(message);
        }

        /// <summary>
        /// Adds a message handler to the registry.
        /// </summary>
        /// <param name="handler">The message handler to be added.</param>
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

        /// <summary>
        /// Adds a connection-specific message handler to the registry.
        /// </summary>
        /// <param name="handler">The connection-specific message handler to be added.</param>
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

        /// <summary>
        /// Removes a message handler from the registry.
        /// </summary>
        /// <param name="handler">The message handler to be removed.</param>
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

        /// <summary>
        /// Removes a connection-specific message handler from the registry.
        /// </summary>
        /// <param name="handler">The connection-specific message handler to be removed.</param>
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