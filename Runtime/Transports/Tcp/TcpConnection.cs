using System.Net;
using System.Net.Sockets;
using JeeLee.Networking.Delegates;

namespace JeeLee.Networking.Transports.Tcp
{
    public class TcpConnection : Connection
    {
        private readonly Socket _socket;
        private IPEndPoint _endPoint;

        public TcpConnection(Socket socket, IPEndPoint endPoint) : base(endPoint.GetHashCode())
        {
            _socket = socket;
            _endPoint = endPoint;
        }

        public override void Send(byte[] dataBuffer)
        {
            throw new System.NotImplementedException();
        }

        public override void Receive(MessageReceivedHandler handler)
        {
            throw new System.NotImplementedException();
        }
    }
}