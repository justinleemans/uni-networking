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

        private Connection _connection;

        /// <summary>
        /// Gets a value indicating whether the client is currently connected to a server.
        /// </summary>
        public bool IsConnected { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Client"/> class using the default TCP client transport.
        /// </summary>
        public Client() : this(new TcpClientTransport())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Client"/> class with a specified client transport.
        /// </summary>
        /// <param name="transport">The client transport to use.</param>
        public Client(IClientTransport transport)
        {
            _transport = transport;
        }

        #region Peer Members

        /// <summary>
        /// Called periodically to perform any necessary actions.
        /// </summary>
        public override void Tick()
        {
            if (!IsConnected)
            {
                return;
            }

            _transport.Tick();
            _connection.Receive(dataStream => OnMessageReceived(-1, dataStream));
        }

        /// <summary>
        /// Called before sending a message to perform any specific actions.
        /// </summary>
        /// <typeparam name="TMessage">The type of message being sent.</typeparam>
        /// <param name="message">The message being sent.</param>
        protected override void OnSendMessage<TMessage>(TMessage message)
        {
            if (!IsConnected)
            {
                return;
            }

            _connection.Send(message.DataStream);
        }

        #endregion

        /// <summary>
        /// Connects the client to a server.
        /// </summary>
        public void Connect()
        {
            if (IsConnected)
            {
                Disconnect();
            }

            _connection = _transport.Connect();

            if (_connection != null)
            {
                IsConnected = true;
                _connection.OnConnectionClosed += OnConnectionClosed;
            }

            void OnConnectionClosed()
            {
                _connection.OnConnectionClosed -= OnConnectionClosed;
                _connection = null;
            }
        }

        /// <summary>
        /// Disconnects the client from the current server.
        /// </summary>
        public void Disconnect()
        {
            if (!IsConnected)
            {
                return;
            }
            
            _transport.Disconnect();
            _connection.Close();

            IsConnected = false;
        }
    }
}