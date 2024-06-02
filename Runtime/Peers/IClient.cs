using System;
using JeeLee.UniNetworking.Messages;

namespace JeeLee.UniNetworking.Peers
{
    /// <summary>
    /// Represents the interface for the client peer. Runs the general client code which allows a connection to be made to a server peer.
    /// </summary>
    public interface IClient
    {
        /// <summary>
        /// Event triggered when this client gets disconnected.
        /// </summary>
        event Action ClientDisconnected;

        /// <summary>
        /// Gets a value indicating whether the client is currently connected to a server.
        /// </summary>
        bool IsConnected { get; }
        
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