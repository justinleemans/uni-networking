using System.Net;
using System.Net.Sockets;
using JeeLee.Networking.Delegates;

namespace JeeLee.Networking.Transports.Tcp
{
    public class TcpConnection : Connection
    {
        private readonly Socket _socket;
        private readonly IPEndPoint _endPoint;

        public TcpConnection(Socket socket, IPEndPoint endPoint) : base(endPoint.GetHashCode())
        {
            _socket = socket;
            _endPoint = endPoint;
        }

        protected override void OnSend(int messageId, byte[] dataStream)
        {
            throw new System.NotImplementedException();
        }

        protected override void OnReceive(out int messageId, out byte[] dataStream)
        {
            throw new System.NotImplementedException();
        }

        protected override void OnClose()
        {
            _socket.Close();
        }
    }
}