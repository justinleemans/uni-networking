using System.Net;
using System.Net.Sockets;

namespace JeeLee.Networking.Transports.Tcp
{
    public class TcpClientTransport : IClientTransport
    {
        private Socket _socket;

        public string IpAddress { get; set; } = "127.0.0.1";
        public ushort Port { get; set; } = 7777;

        public Connection Connect()
        {
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse(IpAddress), Port);
            _socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            _socket.Connect(remoteEndPoint);

            return new TcpConnection(_socket);
        }

        public void Disconnect()
        {
        }

        public void Tick()
        {
        }
    }
}