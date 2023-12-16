using System;
using JeeLee.Networking.Exceptions;
using JeeLee.Networking.Messages;
using JeeLee.Networking.Messages.Delegates;

namespace JeeLee.Networking.Transports
{
    public abstract class Connection
    {
        private readonly int _networkIdentifier;

        protected Connection(int networkIdentifier)
        {
            _networkIdentifier = networkIdentifier;
        }

        public void Send<TMessage>(TMessage message)
            where TMessage : Message
        {
            try
            {
                // OnSend(message.InternalId, message.DataStream);
            }
            catch (Exception exception)
            {
                throw new TransportException("Error trying to send message", exception);
            }
        }

        public void Receive(MessageReceivedHandler handler)
        {
            try
            {
                OnReceive(out int messageId, out byte[] dataStream);
                handler(messageId, dataStream);
            }
            catch (Exception exception)
            {
                throw new TransportException("Error trying to recieve message data", exception);
            }
        }

        public void Close()
        {
            OnClose();
        }

        public override bool Equals(object obj)
        {
            if (obj is Connection other)
            {
                return _networkIdentifier.Equals(other._networkIdentifier);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return _networkIdentifier.GetHashCode();
        }

        protected abstract void OnSend(int messageId, byte[] dataStream);
        protected abstract void OnReceive(out int messageId, out byte[] dataStream);
        protected abstract void OnClose();
    }
}