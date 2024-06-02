using System;
using JeeLee.UniNetworking.Messages;

namespace JeeLee.UniNetworking.Peers
{
    /// <summary>
    /// Represents the interface for the server peer. Runs the general server code which allows clients to connect to it.
    /// </summary>
    public interface IServer : IPeer
    {
        /// <summary>
        /// Event triggered when a client connects to the server.
        /// </summary>
        event Action<int> ClientConnected;

        /// <summary>
        /// Event triggered when a client disconnects from the server.
        /// </summary>
        event Action<int> ClientDisconnected;

        /// <summary>
        /// Gets a value indicating whether the server is running.
        /// </summary>
        bool IsRunning { get; }
        
        /// <summary>
        /// Starts the server.
        /// </summary>
        bool Start();

        /// <summary>
        /// Stops the server.
        /// </summary>
        void Stop();

        /// <summary>
        /// Closes the connection with the specified connection identifier.
        /// </summary>
        /// <param name="connectionId">The connection identifier to close.</param>
        void CloseConnection(int connectionId);

        /// <summary>
        /// Sends a message of type <typeparamref name="TMessage"/> to the specified client.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to be sent.</typeparam>
        /// <param name="connectionId">The connection identifier of the client.</param>
        void SendMessage<TMessage>(int connectionId) where TMessage : Message;

        /// <summary>
        /// Sends a specific message to the specified client.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to be sent.</typeparam>
        /// <param name="message">The message to be sent.</param>
        /// /// <param name="connectionId">The connection identifier of the client.</param>
        void SendMessage<TMessage>(TMessage message, int connectionId) where TMessage : Message;

        /// <summary>
        /// Subscribes a message handler for messages of type <typeparamref name="TMessage"/>.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to subscribe to.</typeparam>
        /// <param name="handler">The message handler to be subscribed.</param>
        void Subscribe<TMessage>(MessageFromHandler<TMessage> handler) where TMessage : Message;

        /// <summary>
        /// Unsubscribes a message handler for messages of type <typeparamref name="TMessage"/>.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to unsubscribe from.</typeparam>
        /// <param name="handler">The message handler to be unsubscribed.</param>
        void Unsubscribe<TMessage>(MessageFromHandler<TMessage> handler) where TMessage : Message;

        /// <summary>
        /// Called periodically to perform any necessary actions.
        /// </summary>
        void Tick();
    }
}