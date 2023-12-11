using System.Net;
using System.Net.Sockets;
using JeeLee.Networking.Delegates;
using JeeLee.Networking.Messages;

namespace JeeLee.Networking.Transports.Tcp
{
    public class TcpClientTransport : IClientTransport
    {
        private Socket _socket;

        public MessageReceivedHandler OnMessageReceived { get; set; }
        public Connection Connection { get; private set; }
        public bool IsConnected { get; private set; }

        public bool Connect(string remoteAddress, ushort port)
        {
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse(remoteAddress), port);
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

        public void Send(Message message)
        {
            Connection.Send(message);
        }

        public void Tick()
        {
            Connection.Receive(OnMessageReceived);
        }
    }
}