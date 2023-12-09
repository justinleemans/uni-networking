using JeeLee.Networking.Transports;
using JeeLee.Networking.Transports.Tcp;

namespace JeeLee.Networking
{
    public sealed class Server : Peer
    {
        private readonly IServerTransport _transport;
        
        public Server() : this(new TcpServerTransport())
        {
        }
        
        public Server(IServerTransport transport) : base(transport)
        {
            _transport = transport;
        }

        public void Start()
        {
            _transport.Start();
        }

        public void Stop()
        {
            _transport.Stop();
        }
    }
}