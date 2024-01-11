using JeeLee.Networking.Messages.Delegates;
using JeeLee.Networking.Messages.Streams;

namespace JeeLee.Networking.Transports
{
    /// <summary>
    /// Interface used to define the generic transport methods.
    /// Generally only peers should communicate with these classes.
    /// </summary>
    public interface ITransport
    {
        /// <summary>
        /// Handler used by the peer to handle incomming messages.
        /// </summary>
        event MessageReceivedHandler OnMessageReceived;

        /// <summary>
        /// Sends a message from this peer.
        /// </summary>
        /// <param name="dataStream">The data stream representation of the message to send.</param>
        void Send(DataStream dataStream);

        /// <summary>
        /// Runs the update loop of the transport.
        /// Use this method to handle all things that should happen on your transport on a regular interval.
        /// </summary>
        void Tick();
    }
}