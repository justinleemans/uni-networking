using JeeLee.UniNetworking.Payloads;

namespace JeeLee.UniNetworking.Messages.Registries
{
    /// <summary>
    /// Represents the interface for a message broker in the network communication system.
    /// </summary>
    public interface IMessageBroker
    {
        /// <summary>
        /// Handles a received message.
        /// </summary>
        /// <param name="connectionId">The identifier of the connection from which the message is received.</param>
        /// <param name="payload">The payload containing the received message.</param>
        void Handle(int connectionId, Payload payload);
    }
}