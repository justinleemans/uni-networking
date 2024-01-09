using JeeLee.Networking.Transports;
using JeeLee.Networking.Transports.Tcp;

namespace JeeLee.Networking
{
    public sealed class Server : Peer
    {
        private readonly IServerTransport _transport;
        
        public bool IsRunning => _transport.IsRunning;

        public Server() : this(new TcpServerTransport())
        {
        }
        
        public Server(IServerTransport transport) : base(transport)
        {
            _transport = transport;
        }

        public void Start()
        {
            if (_transport.IsRunning)
            {
                _transport.Stop();
            }

            _transport.Start();
        }

        public void Stop()
        {
            if (!_transport.IsRunning)
            {
                return;
            }
            
            _transport.Stop();
        }
    }
}