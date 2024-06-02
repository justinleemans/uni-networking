using System;
using System.Net.Sockets;
using JeeLee.UniNetworking.Logging;
using JeeLee.UniNetworking.Payloads;

namespace JeeLee.UniNetworking.Transports.Tcp
{
    /// <summary>
    /// Represents a TCP connection implementation.
    /// </summary>
    public class TcpConnection
    {
        /// <summary>
        /// Event triggered when the connection is closed.
        /// </summary>
        public event Action ConnectionClosed;
        
        private readonly Socket _socket;
        private readonly byte[] _sizeBuffer = new byte[sizeof(int)];

        private int _nextMessageSize;

        /// <summary>
        /// Initializes a new instance of the TcpConnection class with the specified Socket.
        /// </summary>
        /// <param name="socket">The underlying Socket for the connection.</param>
        public TcpConnection(Socket socket)
        {
            _socket = socket;
        }

        /// <summary>
        /// Sends a payload to the connected peer.
        /// </summary>
        /// <param name="payload">The payload to be sent.</param>
        public void Send(Payload payload)
        {
            try
            {
                _socket.Send(payload.GetBytes(), SocketFlags.None);
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
                byte[] dataBuffer = null;
                if (_nextMessageSize <= 0 && _socket.Available >= sizeof(int))
                {
                    _socket.Receive(_sizeBuffer, sizeof(int), SocketFlags.None);
                    _nextMessageSize = BitConverter.ToInt32(_sizeBuffer);
                }
            
                if (_nextMessageSize > 0 && _socket.Available >= _nextMessageSize)
                {
                    dataBuffer = new byte[_nextMessageSize];
                    _socket.Receive(dataBuffer, _nextMessageSize, SocketFlags.None);
                    _nextMessageSize = 0;
                }
                
                if (dataBuffer == null || dataBuffer.Length < sizeof(int))
                {
                    return;
                }

                Payload payload = new Payload(dataBuffer);

                handler(payload);
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
            try
            {
                _socket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception exception)
            {
                NetworkLogger.Log(exception, LogLevel.Error);
                _socket.Close();
            }
            finally
            {
                ConnectionClosed?.Invoke();
            }
        }
    }
}