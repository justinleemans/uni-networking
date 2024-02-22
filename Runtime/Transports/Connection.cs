using System;
using JeeLee.Networking.Exceptions;
using JeeLee.Networking.Messages.Streams;

namespace JeeLee.Networking.Transports
{
    /// <summary>
    /// Represents an abstract base class for network connections in the communication system.
    /// </summary>
    public abstract class Connection
    {
        /// <summary>
        /// Event triggered when the connection is closed.
        /// </summary>
        public event Action ConnectionClosed;

        /// <summary>
        /// Sends a data stream to the connected peer.
        /// </summary>
        /// <param name="dataStream">The data stream to be sent.</param>
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
        /// Receives data from the connected peer and invokes the provided handler.
        /// </summary>
        /// <param name="handler">The handler to process the received data.</param>
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
                throw new TransportException("Error trying to receive message data", exception);
            }
        }

        /// <summary>
        /// Closes the connection.
        /// </summary>
        public void Close()
        {
            OnClose();
            ConnectionClosed?.Invoke();
        }

        /// <summary>
        /// Sends a byte array to the connected peer.
        /// </summary>
        /// <param name="dataBuffer">The byte array to be sent.</param>
        protected abstract void OnSend(byte[] dataBuffer);

        /// <summary>
        /// Receives a byte array from the connected peer.
        /// </summary>
        /// <param name="dataBuffer">The received byte array.</param>
        protected abstract void OnReceive(out byte[] dataBuffer);

        /// <summary>
        /// Closes the connection. Derived classes should implement specific closing logic.
        /// </summary>
        protected abstract void OnClose();
    }
}