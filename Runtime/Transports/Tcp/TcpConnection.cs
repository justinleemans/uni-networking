using System.Net;
using System.Net.Sockets;

namespace JeeLee.Networking.Transports.Tcp
{
    public class TcpConnection : Connection
    {
        private readonly Socket _socket;

        public TcpConnection(Socket socket, IPEndPoint endPoint) : base(endPoint.GetHashCode())
        {
            _socket = socket;
        }

        protected override void OnSend(byte[] dataBuffer)
        {
            throw new System.NotImplementedException();
        }

        protected override void OnReceive(out byte[] dataBuffer)
        {
            throw new System.NotImplementedException();
        }

        protected override void OnClose()
        {
            _socket.Close();
        }
    }
}