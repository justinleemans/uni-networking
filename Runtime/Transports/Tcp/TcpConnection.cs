using System;
using System.Net.Sockets;

namespace JeeLee.UniNetworking.Transports.Tcp
{
    /// <summary>
    /// Represents a TCP connection implementation based on the Connection abstract class.
    /// </summary>
    public class TcpConnection : Connection
    {
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

        #region Connection Members

        /// <summary>
        /// Sends a byte array to the connected peer using the underlying Socket.
        /// </summary>
        /// <param name="dataBuffer">The byte array to be sent.</param>
        protected override void OnSend(byte[] dataBuffer)
        {
            _socket.Send(dataBuffer, SocketFlags.None);
        }

        /// <summary>
        /// Receives a byte array from the connected peer using the underlying Socket.
        /// </summary>
        /// <param name="dataBuffer">The received byte array.</param>
        protected override void OnReceive(out byte[] dataBuffer)
        {
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
            else
            {
                dataBuffer = null;
            }
        }

        /// <summary>
        /// Closes the connection by closing the underlying Socket.
        /// </summary>
        protected override void OnClose()
        {
            try
            {
                _socket.Shutdown(SocketShutdown.Both);
            }
            catch
            {
                _socket.Close();
            }
        }

        #endregion
    }
}