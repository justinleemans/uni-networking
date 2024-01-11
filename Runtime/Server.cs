using JeeLee.Networking.Transports;
using JeeLee.Networking.Transports.Tcp;

namespace JeeLee.Networking
{
    /// <summary>
    /// Server peer. Runs the general server code which allows clients to connect to it.
    /// </summary>
    public sealed class Server : Peer
    {
        private readonly IServerTransport _transport;
        
        /// <summary>
        /// Is this server currently active and running.
        /// </summary>
        public bool IsRunning => _transport.IsRunning;

        /// <summary>
        /// Constructor for this server peer. Start with default tcp protocol.
        /// </summary>
        public Server() : this(new TcpServerTransport())
        {
        }
        
        /// <summary>
        /// Contstructor for this peer. Starts server with given transport.
        /// </summary>
        /// <param name="transport">The transport to run the communications over.</param>
        public Server(IServerTransport transport) : base(transport)
        {
            _transport = transport;
        }

        /// <summary>
        /// Starts the server over the selected transport.
        /// </summary>
        public void Start()
        {
            if (_transport.IsRunning)
            {
                _transport.Stop();
            }

            _transport.Start();
        }

        /// <summary>
        /// Stops the server if it is currently running.
        /// </summary>
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