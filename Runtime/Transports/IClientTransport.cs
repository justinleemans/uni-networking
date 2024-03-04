namespace JeeLee.UniNetworking.Transports
{
    /// <summary>
    /// Represents the interface for client-side network transports in the communication system.
    /// </summary>
    public interface IClientTransport : ITransport
    {
        /// <summary>
        /// Establishes a connection to a remote server.
        /// </summary>
        /// <returns>The connection object representing the established connection.</returns>
        Connection Connect();

        /// <summary>
        /// Disconnects from the current server, if connected.
        /// </summary>
        void Disconnect();
    }
}