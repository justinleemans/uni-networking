using System;
using System.Collections.Generic;
using JeeLee.UniNetworking.Payloads;

namespace JeeLee.UniNetworking.Transports.Udp
{
    public class UdpServerTransport : IServerTransport
    {
        public event Action<int> ClientConnected;
        public event Action<int> ClientDisconnected;

        public bool IsRunning => throw new NotImplementedException();

        public IReadOnlyCollection<int> ConnectionIds => throw new NotImplementedException();

        public ushort Port { get; set; } = 7777;
        
        public int MaxConnections { get; set; } = 10;

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void CloseConnection(int connectionId)
        {
            throw new NotImplementedException();
        }

        public void Tick()
        {
            throw new NotImplementedException();
        }

        public void Send(Payload payload)
        {
            throw new NotImplementedException();
        }

        public void Send(Payload payload, int connectionId)
        {
            throw new NotImplementedException();
        }

        public void Receive(Action<Payload, int> onMessageReceived)
        {
            throw new NotImplementedException();
        }
    }
}