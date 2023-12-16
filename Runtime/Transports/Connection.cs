using System;
using JeeLee.Networking.Exceptions;
using JeeLee.Networking.Messages;
using JeeLee.Networking.Messages.Delegates;
using JeeLee.Networking.Messages.Streams;

namespace JeeLee.Networking.Transports
{
    public abstract class Connection
    {
        private readonly int _networkIdentifier;

        protected Connection(int networkIdentifier)
        {
            _networkIdentifier = networkIdentifier;
        }

        public void Send(DataStream dataStream)
        {
            try
            {
                OnSend(dataStream.GetBytes());
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
                OnReceive(out byte[] dataBuffer);

                if (dataBuffer == null || dataBuffer.Length < sizeof(int))
                {
                    return;
                }

                DataStream dataStream = new DataStream(dataBuffer);
                handler(dataStream);
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

        protected abstract void OnSend(byte[] dataBuffer);
        protected abstract void OnReceive(out byte[] dataBuffer);
        protected abstract void OnClose();
    }
}