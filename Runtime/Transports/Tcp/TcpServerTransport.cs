using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using JeeLee.Networking.Messages.Delegates;
using JeeLee.Networking.Messages.Streams;

namespace JeeLee.Networking.Transports.Tcp
{
    public class TcpServerTransport : IServerTransport
    {
        public event MessageReceivedHandler OnMessageReceived;
        
        private Socket _socket;

        public ushort Port { get; set; } = 7777;
        public int MaxConnections { get; set; } = 10;

        public HashSet<Connection> Connections { get; private set; }
        public bool IsRunning { get; private set; }

        public void Start()
        {
            Connections = new HashSet<Connection>();
            
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.IPv6Any, Port);
            _socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            _socket.Bind(localEndPoint);
            _socket.Listen(MaxConnections);

            IsRunning = true;
        }

        public void Stop()
        {
            IsRunning = false;
            _socket.Close();

            Connections.Clear();
        }

        public void Send(DataStream dataStream)
        {
            foreach (var connection in Connections)
            {
                connection.Send(dataStream);
            }
        }

        public void Tick()
        {
            if (_socket.Poll(0, SelectMode.SelectRead))
            {
                Socket connectionSocket = _socket.Accept();
                IPEndPoint remoteEndPoint = connectionSocket.RemoteEndPoint as IPEndPoint;
                TcpConnection acceptedConnection = new TcpConnection(connectionSocket, remoteEndPoint);

                if (!Connections.Add(acceptedConnection))
                {
                    acceptedConnection.Close();
                }
            }

            foreach (var connection in Connections)
            {
                connection.Receive(OnMessageReceived);
            }
        }
    }
}