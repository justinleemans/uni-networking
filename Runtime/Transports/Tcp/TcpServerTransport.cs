using System;
using System.Net;
using System.Net.Sockets;

namespace JeeLee.Networking.Transports.Tcp
{
    public class TcpServerTransport : IServerTransport
    {
        public event Action<Connection> OnNewConnection;
        
        private Socket _socket;

        public ushort Port { get; set; } = 7777;
        public int MaxConnections { get; set; } = 10;

        public void Start()
        {
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.IPv6Any, Port);
            _socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            _socket.Bind(localEndPoint);
            _socket.Listen(MaxConnections);
        }

        public void Stop()
        {
            _socket.Close();
        }

        public void Tick()
        {
            if (_socket.Poll(0, SelectMode.SelectRead))
            {
                OnNewConnection?.Invoke(new TcpConnection(_socket.Accept()));
            }
        }
    }
}