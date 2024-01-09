using JeeLee.Networking.Transports;
using JeeLee.Networking.Transports.Tcp;

namespace JeeLee.Networking
{
    public sealed class Client : Peer
    {
        private readonly IClientTransport _transport;
        
        public bool IsConnected => _transport.IsConnected;

        public Client() : this(new TcpClientTransport())
        {
        }
        
        public Client(IClientTransport transport) : base(transport)
        {
            _transport = transport;
        }

        public bool Connect()
        {
            if (_transport.IsConnected)
            {
                _transport.Disconnect();
            }
            
            return _transport.Connect();
        }

        public void Disconnect()
        {
            if (!_transport.IsConnected)
            {
                return;
            }
            
            _transport.Disconnect();
        }
    }
}