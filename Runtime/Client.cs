using JeeLee.Networking.Transports;
using JeeLee.Networking.Transports.Tcp;

namespace JeeLee.Networking
{
    public sealed class Client : Peer
    {
        private readonly IClientTransport _transport;
        
        public Client() : this(new TcpClientTransport())
        {
        }
        
        public Client(IClientTransport transport) : base(transport)
        {
            _transport = transport;
        }

        public bool Connect()
        {
            return _transport.Connect();
        }

        public void Disconnect()
        {
            _transport.Disconnect();
        }
    }
}