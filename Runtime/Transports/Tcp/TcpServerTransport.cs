using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using JeeLee.Networking.Delegates;

namespace JeeLee.Networking.Transports.Tcp
{
    public class TcpServerTransport : IServerTransport
    {
        private Socket _socket;

        public MessageReceivedHandler OnMessageReceived { get; set; }
        public HashSet<Connection> Connections { get; private set; }
        public bool IsRunning { get; private set; }

        public void Start()
        {
            Connections = new HashSet<Connection>();
            StartListening();
        }

        public void Stop()
        {
            StopListening();
            Connections.Clear();
        }

        public void Send(byte[] dataBuffer)
        {
            if (!IsRunning)
            {
                return;
            }

            foreach (var connection in Connections)
            {
                connection.Send(dataBuffer);
            }
        }

        public void Tick()
        {
            if (!IsRunning)
            {
                return;
            }

            if (_socket.Poll(0, SelectMode.SelectRead))
            {
                Socket connectionSocket = _socket.Accept();
                IPEndPoint remoteEndPoint = connectionSocket.RemoteEndPoint as IPEndPoint;
                TcpConnection acceptedConnection = new TcpConnection(connectionSocket, remoteEndPoint);

                if (!Connections.Add(acceptedConnection))
                {
                    connectionSocket.Close();
                }
            }

            foreach (var connection in Connections)
            {
                connection.Receive(OnMessageReceived);
            }
        }

        private void StartListening()
        {
            if (IsRunning)
            {
                StopListening();
            }

            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 7777);
            _socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            _socket.Bind(localEndPoint);
            _socket.Listen(2);

            IsRunning = true;
        }

        private void StopListening()
        {
            if (!IsRunning)
            {
                return;
            }

            IsRunning = false;
            _socket.Close();
        }
    }
}