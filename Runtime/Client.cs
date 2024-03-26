using System;
using JeeLee.UniNetworking.Messages.Streams;
using JeeLee.UniNetworking.Transports;
using JeeLee.UniNetworking.Transports.Tcp;

namespace JeeLee.UniNetworking
{
    /// <summary>
    /// Client peer. Runs the general client code which allows a connection to be made to a server peer.
    /// </summary>
    public sealed class Client : Peer, IClient
    {
        /// <summary>
        /// Event triggered when this client gets disconnected.
        /// </summary>
        public event Action ConnectionClosed;
        
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
        /// Sends a data stream to the peer.
        /// </summary>
        /// <param name="dataStream">The data stream to be sent.</param>
        protected override void SendDataStream(DataStream dataStream)
        {
            if (!IsConnected)
            {
                return;
            }

            _connection.Send(dataStream);
        }

        #endregion

        #region IClient Members

        /// <summary>
        /// Connects the client to a server.
        /// </summary>
        public bool Connect()
        {
            if (IsConnected)
            {
                Disconnect();
            }

            _connection = _transport.Connect();

            if (_connection != null)
            {
                IsConnected = true;
                _connection.ConnectionClosed += Handle;
            }

            return IsConnected;

            void Handle()
            {
                _connection.ConnectionClosed -= Handle;
                _connection = null;

                ConnectionClosed?.Invoke();
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

        /// <summary>
        /// Called periodically to perform any necessary actions.
        /// </summary>
        public void Tick()
        {
            if (!IsConnected)
            {
                return;
            }

            _transport.Tick();
            _connection.Receive(dataStream => OnMessageReceived(-1, dataStream));
        }

        #endregion
    }
}