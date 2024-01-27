using System;
using JeeLee.Networking.Exceptions;
using JeeLee.Networking.Messages.Streams;

namespace JeeLee.Networking.Transports
{
    /// <summary>
    /// Base class for all connection specific logic.
    /// Transports should include their own implementation of a connection.
    /// </summary>
    public abstract class Connection
    {
        /// <summary>
        /// The event that is raised when this connection gets closed.
        /// </summary>
        public event Action OnConnectionClosed;

        /// <summary>
        /// Sends a message to this connection.
        /// </summary>
        /// <param name="dataStream">The data stream representation of the message to send.</param>
        /// <exception cref="TransportException">Captures all exceptions that might be thrown during send.</exception>
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

        /// <summary>
        /// Attempts to receive a new message from this connection.
        /// </summary>
        /// <param name="handler">Callback when a new message is received.</param>
        /// <exception cref="TransportException">Captures all exceptions that might be thrown during receive.</exception>
        public void Receive(Action<DataStream> handler)
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

        /// <summary>
        /// Closes this connection and disconnects it.
        /// </summary>
        public void Close()
        {
            OnClose();
            OnConnectionClosed?.Invoke();
        }

        /// <summary>
        /// Called when send is called. Used for transport specific implementation of network communication.
        /// </summary>
        /// <param name="dataBuffer">Byte array of the converted message.</param>
        protected abstract void OnSend(byte[] dataBuffer);

        /// <summary>
        /// Called when receive is calle. Used for transport specific implementation of network communication.
        /// </summary>
        /// <param name="dataBuffer">Should return byte array which is received over the network. Should contain the message id as int prefixed.</param>
        protected abstract void OnReceive(out byte[] dataBuffer);

        /// <summary>
        /// Called when close is called. Used to implement closing of this connection for this specific transport implementation.
        /// </summary>
        protected abstract void OnClose();
    }
}