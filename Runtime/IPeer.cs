using JeeLee.UniNetworking.Messages;

namespace JeeLee.UniNetworking
{
    public interface IPeer
    {
        /// <summary>
        /// Sends a message of type <typeparamref name="TMessage"/>.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to be sent.</typeparam>
        void SendMessage<TMessage>() where TMessage : Message;

        /// <summary>
        /// Sends a specific message.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to be sent.</typeparam>
        /// <param name="message">The message to be sent.</param>
        void SendMessage<TMessage>(TMessage message) where TMessage : Message;

        /// <summary>
        /// Gets an instance of the message of type <typeparamref name="TMessage"/>.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to retrieve.</typeparam>
        /// <returns>An instance of the specified message type.</returns>
        TMessage GetMessage<TMessage>() where TMessage : Message;

        /// <summary>
        /// Subscribes a message handler for the specified message type.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to subscribe to.</typeparam>
        /// <param name="handler">The message handler to be subscribed.</param>
        void Subscribe<TMessage>(MessageHandler<TMessage> handler) where TMessage : Message;

        /// <summary>
        /// Unsubscribes a message handler for the specified message type.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to unsubscribe from.</typeparam>
        /// <param name="handler">The message handler to be unsubscribed.</param>
        void Unsubscribe<TMessage>(MessageHandler<TMessage> handler) where TMessage : Message;
    }
}