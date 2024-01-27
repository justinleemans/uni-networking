using JeeLee.Networking.Messages.Streams;

namespace JeeLee.Networking.Messages
{
    /// <summary>
    /// Represents the interface for a message registry in the network communication system.
    /// </summary>
    public interface IMessageRegistry
    {
        /// <summary>
        /// Handles a received message.
        /// </summary>
        /// <param name="connectionId">The identifier of the connection from which the message is received.</param>
        /// <param name="dataStream">The data stream containing the received message.</param>
        void Handle(int connectionId, DataStream dataStream);
    }
}