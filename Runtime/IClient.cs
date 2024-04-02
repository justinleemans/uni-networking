namespace JeeLee.UniNetworking
{
    public interface IClient : IPeer
    {
        /// <summary>
        /// Connects the client to a server.
        /// </summary>
        bool Connect();

        /// <summary>
        /// Disconnects the client from the current server.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Called periodically to perform any necessary actions.
        /// </summary>
        void Tick();
    }
}