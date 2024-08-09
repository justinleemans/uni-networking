using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using JeeLee.UniNetworking.Payloads;

namespace JeeLee.UniNetworking.Transports.Udp
{
    public class UdpServerTransport : IServerTransport
    {
        private const int BufferSize = 1024 * 8;
        
        public Func<int> ClientConnected { get; set; }
        public Action<int> ClientDisconnected { get; set; }
        public Action<Payload, int> MessageReceived { get; set; }

        private readonly Dictionary<int, EndPoint> _connections = new Dictionary<int, EndPoint>();
        private readonly Dictionary<EndPoint, int> _endPointIdMap = new Dictionary<EndPoint, int>();

        private Socket _socket;
        private EndPoint _remoteEndPoint;

        public ushort Port { get; set; } = 7777;
        public int MaxConnections { get; set; } = 10;

        public void Start()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
            _socket.Bind(new IPEndPoint(IPAddress.Any, Port));

            _remoteEndPoint = new IPEndPoint(IPAddress.Any, Port);
        }

        public void Stop()
        {
            try
            {
                var connectionIds = _connections.Keys.ToArray();
                
                foreach (var connectionId in connectionIds)
                {
                    CloseConnection(connectionId);
                }

                _connections.Clear();
                
                _socket.Shutdown(SocketShutdown.Both);
            }
            finally
            {
                _socket.Close();
            }
        }

        public void CloseConnection(int connectionId)
        {
            if (!UnregisterConnection(connectionId))
            {
                return;
            }

            Send(new Payload(PayloadType.Disconnect), connectionId);
        }

        public void Tick()
        {
            if (_socket.Available > 0)
            {
                byte[] dataBuffer = new byte[BufferSize];
                int receivedBytes = _socket.ReceiveFrom(dataBuffer, ref _remoteEndPoint);
                int connectionId = GetOrRegisterConnection(_remoteEndPoint);

                Payload payload = new Payload(dataBuffer.Skip(sizeof(int)));

                switch (payload.Type)
                {
                    case PayloadType.Message:
                        MessageReceived(payload, connectionId);
                        break;
            
                    case PayloadType.Disconnect:
                        CloseConnection(connectionId);
                        break;
                }
            }
        }

        public void Send(Payload payload)
        {
            foreach (var connectionId in _connections.Keys)
            {
                Send(payload, connectionId);
            }
        }

        public void Send(Payload payload, int connectionId)
        {
            if (!_connections.TryGetValue(connectionId, out var connection))
            {
                return;
            }

            _socket.SendTo(payload.GetBytes(), SocketFlags.None, connection);
        }

        private int GetOrRegisterConnection(EndPoint endPoint)
        {
            if (!_endPointIdMap.TryGetValue(endPoint, out var connectionId))
            {
                connectionId = ClientConnected();

                _connections.Add(connectionId, endPoint);
                _endPointIdMap.Add(endPoint, connectionId);
            }

            return connectionId;
        }

        private bool UnregisterConnection(int connectionId)
        {
            if(!_connections.Remove(connectionId, out var connection))
            {
                return false;
            }

            _endPointIdMap.Remove(connection);
            return true;
        }
    }
}