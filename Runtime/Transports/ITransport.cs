namespace JeeLee.UniNetworking.Transports
{
    /// <summary>
    /// Represents the interface for network transports in the communication system.
    /// </summary>
    public interface ITransport
    {
        /// <summary>
        /// Performs any necessary actions during each tick of the network transport.
        /// </summary>
        void Tick();
    }
}