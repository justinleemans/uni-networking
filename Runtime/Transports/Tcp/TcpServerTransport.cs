using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using JeeLee.Networking.Delegates;
using JeeLee.Networking.Messages;

namespace JeeLee.Networking.Transports.Tcp
{
    public class TcpServerTransport : IServerTransport
    {
        private Socket _socket;

        public MessageReceivedHandler OnMessageReceived { get; set; }
        public HashSet<Connection> Connections { get; private set; }
        public bool IsRunning { get; private set; }

        public void Start(ushort port, int maxConnections)
        {
            Connections = new HashSet<Connection>();
            
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.IPv6Any, port);
            _socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            _socket.Bind(localEndPoint);
            _socket.Listen(maxConnections);

            IsRunning = true;
        }

        public void Stop()
        {
            IsRunning = false;
            _socket.Close();

            Connections.Clear();
        }

        public void Send(Message message)
        {
            foreach (var connection in Connections)
            {
                connection.Send(message);
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