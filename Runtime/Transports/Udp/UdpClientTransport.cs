using System;
using JeeLee.UniNetworking.Payloads;

namespace JeeLee.UniNetworking.Transports.Udp
{
    public class UdpClientTransport : IClientTransport
    {
        public event Action ClientDisconnected;

        public bool IsConnected => throw new NotImplementedException();

        public void Connect()
        {
            throw new NotImplementedException();
        }

        public void Disconnect()
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

        public void Receive(Action<Payload, int> onMessageReceived)
        {
            throw new NotImplementedException();
        }
    }
}