using JeeLee.Networking.Transports;
using JeeLee.Networking.Transports.Tcp;

namespace JeeLee.Networking
{
    /// <summary>
    /// Client peer. Runs the general client code which allows a connection to be made to a server peer.
    /// </summary>
    public sealed class Client : Peer
    {
        private readonly IClientTransport _transport;
        
        /// <summary>
        /// Is this client currently active and connected.
        /// </summary>
        public bool IsConnected => _transport.IsConnected;

        /// <summary>
        /// Constructor for this client peer. Start with default tcp protocol.
        /// </summary>
        public Client() : this(new TcpClientTransport())
        {
        }
        
        /// <summary>
        /// Contstructor for this peer. Starts client with given transport.
        /// </summary>
        /// <param name="transport">The transport to run the communications over.</param>
        public Client(IClientTransport transport) : base(transport)
        {
            _transport = transport;
        }

        /// <summary>
        /// Connects the client over the selected transport.
        /// </summary>
        /// <returns></returns>
        public bool Connect()
        {
            if (_transport.IsConnected)
            {
                _transport.Disconnect();
            }
            
            return _transport.Connect();
        }

        /// <summary>
        /// Disconnects the client from a server if it is connected to one.
        /// </summary>
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