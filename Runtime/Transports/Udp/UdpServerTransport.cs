using System;
using JeeLee.UniNetworking.Payloads;

namespace JeeLee.UniNetworking.Transports.Udp
{
    public class UdpServerTransport : IServerTransport
    {
        public Func<int> ClientConnected { get; set; }
        public Action<int> ClientDisconnected { get; set; }

        public bool IsRunning => throw new NotImplementedException();

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