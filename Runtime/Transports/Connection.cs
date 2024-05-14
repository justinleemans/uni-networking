using System;
using JeeLee.UniNetworking.Logging;
using JeeLee.UniNetworking.Messages.Payloads;

namespace JeeLee.UniNetworking.Transports
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
        /// Sends a payload to the connected peer.
        /// </summary>
        /// <param name="payload">The payload to be sent.</param>
        public void Send(Payload payload)
        {
            try
            {
                OnSend(payload.GetBytes());
            }
            catch (Exception exception)
            {
                NetworkLogger.Log(exception, LogLevel.Error);
                Close();
            }
        }

        /// <summary>
        /// Receives data from the connected peer and invokes the provided handler.
        /// </summary>
        /// <param name="handler">The handler to process the received data.</param>
        public void Receive(Action<Payload> handler)
        {
            try
            {
                OnReceive(out byte[] dataBuffer);

                if (dataBuffer == null || dataBuffer.Length < sizeof(int))
                {
                    return;
                }

                Payload payload = new Payload(dataBuffer);

                switch (payload.Type)
                {
                    case PayloadType.Message:
                        handler(payload);
                        break;

                    case PayloadType.Disconnect:
                        Close();
                        break;
                }
            }
            catch (Exception exception)
            {
                NetworkLogger.Log(exception, LogLevel.Error);
                Close();
            }
        }

        /// <summary>
        /// Closes the connection.
        /// </summary>
        public void Close()
        {
            Send(new Payload(PayloadType.Disconnect));
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