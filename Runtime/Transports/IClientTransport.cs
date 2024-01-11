namespace JeeLee.Networking.Transports
{
    /// <summary>
    /// Interface that is used for the client specific implementation of the transport.
    /// Generally only the client peer should communicate with this class.
    /// </summary>
    public interface IClientTransport : ITransport
    {
        /// <summary>
        /// The connection of this client. Set when connected to a server.
        /// </summary>
        Connection Connection { get; }

        /// <summary>
        /// Should be set to tell the peer if this client is connected to a server.
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Called when trying to connect this client to a server.
        /// </summary>
        /// <returns>If the connection was successful.</returns>
        bool Connect();
        
        /// <summary>
        /// Called when trying to disconnect this client from a server it is connected to.
        /// </summary>
        void Disconnect();
    }
}