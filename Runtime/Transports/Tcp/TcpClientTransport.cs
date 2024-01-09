using System.Net;
using System.Net.Sockets;
using JeeLee.Networking.Messages.Delegates;
using JeeLee.Networking.Messages.Streams;

namespace JeeLee.Networking.Transports.Tcp
{
    public class TcpClientTransport : IClientTransport
    {
        private Socket _socket;

        public string IpAddress { get; set; } = "127.0.0.1";
        public ushort Port { get; set; } = 7777;

        public MessageReceivedHandler OnMessageReceived { get; set; }
        public Connection Connection { get; private set; }
        public bool IsConnected { get; private set; }

        public bool Connect()
        {
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse(IpAddress), Port);
            _socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            _socket.Connect(remoteEndPoint);

            Connection = new TcpConnection(_socket, remoteEndPoint);

            return IsConnected = _socket.Connected;
        }

        public void Disconnect()
        {
            IsConnected = false;
            _socket.Close();
            Connection = null;
        }

        public void Send(DataStream dataStream)
        {
            Connection.Send(dataStream);
        }

        public void Tick()
        {
            Connection.Receive(OnMessageReceived);
        }
    }
}