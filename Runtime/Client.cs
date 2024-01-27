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

        public Connection Connection { get; private set; }
        public bool IsConnected { get; private set; }

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
        public Client(IClientTransport transport)
        {
            _transport = transport;
        }

        /// <summary>
        /// Connects the client over the selected transport.
        /// </summary>
        /// <returns></returns>
        public void Connect()
        {
            if (IsConnected)
            {
                Disconnect();
            }

            Connection = _transport.Connect();

            if (Connection != null)
            {
                IsConnected = true;
                Connection.OnConnectionClosed += OnConnectionClosed;
            }

            void OnConnectionClosed()
            {
                Connection.OnConnectionClosed -= OnConnectionClosed;
                Connection = null;
            }
        }

        /// <summary>
        /// Disconnects the client from a server if it is connected to one.
        /// </summary>
        public void Disconnect()
        {
            if (!IsConnected)
            {
                return;
            }
            
            Connection.Close();

            IsConnected = false;
        }

        public override void Tick()
        {
            if (!IsConnected)
            {
                return;
            }

            _transport.Tick();
            Connection.Receive(dataStream => OnMessageReceived(-1, dataStream));
        }

        protected override void OnSendMessage<TMessage>(TMessage message)
        {
            if (!IsConnected)
            {
                return;
            }

            Connection.Send(message.DataStream);
        }
    }
}