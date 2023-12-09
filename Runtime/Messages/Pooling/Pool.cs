using System;
using System.Collections.Generic;

namespace JeeLee.Networking.Messages.Pooling
{
    public sealed class Pool<TMessage> : IPool
        where TMessage : IMessage
    {
        private readonly Queue<TMessage> _pool;

        public Pool()
        {
            _pool = new Queue<TMessage>();
        }

        public TMessage Get()
        {
            return _pool.Count > 0 ? _pool.Dequeue() : Activator.CreateInstance<TMessage>();
        }

        public void Release(TMessage message)
        {
            message.Clear();
            _pool.Enqueue(message);
        }
    }
}