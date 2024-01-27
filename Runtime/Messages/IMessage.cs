using JeeLee.Networking.Messages.Streams;

namespace JeeLee.Networking.Messages
{
    /// <summary>
    /// Represents the interface for messages in the network communication system.
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// Gets or sets the data stream associated with the message.
        /// </summary>
        DataStream DataStream { get; set; }

        /// <summary>
        /// Clears the content of the message.
        /// </summary>
        void Clear();
    }
}