using System;
using System.Net;
using System.Net.Sockets;

namespace JeeLee.Networking.Transports.Tcp
{
    public class TcpConnection : Connection
    {
        private readonly Socket _socket;
        private readonly byte[] _sizeBuffer = new byte[sizeof(int)];

        private int _nextMessageSize;

        public TcpConnection(Socket socket, IPEndPoint endPoint) : base(endPoint.GetHashCode())
        {
            _socket = socket;
        }

        protected override void OnSend(byte[] dataBuffer)
        {
            _socket.Send(dataBuffer, SocketFlags.None);
        }

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

        protected override void OnClose()
        {
            _socket.Close();
        }
    }
}