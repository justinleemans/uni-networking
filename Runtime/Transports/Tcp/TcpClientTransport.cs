using System.Net;
using System.Net.Sockets;
using JeeLee.Networking.Delegates;

namespace JeeLee.Networking.Transports.Tcp
{
    public class TcpClientTransport : IClientTransport
    {
        private Socket _socket;

        public MessageReceivedHandler OnMessageReceived { get; set; }
        public Connection Connection { get; private set; }
        public bool IsConnected { get; private set; }

        public bool Connect()
        {
            if (IsConnected)
            {
                Disconnect();
            }

            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 7777);
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

        public void Send(byte[] dataBuffer)
        {
            if (!IsConnected)
            {
                return;
            }

            Connection.Send(dataBuffer);
        }

        public void Tick()
        {
            if (!IsConnected)
            {
                return;
            }

            Connection.Receive(OnMessageReceived);
        }
    }
}