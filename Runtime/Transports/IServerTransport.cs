using System;
using System.Collections.Generic;
using JeeLee.UniNetworking.Payloads;

namespace JeeLee.UniNetworking.Transports
{
    /// <summary>
    /// Represents the interface for server-side network transports in the communication system.
    /// </summary>
    public interface IServerTransport
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
        /// Gets a value indicating whether the server transport is running.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Gets the collection of connection identifiers.
        /// </summary>
        IReadOnlyCollection<int> ConnectionIds { get; }

        /// <summary>
        /// Starts the server transport to listen for incoming connections.
        /// </summary>
        void Start();
        
        /// <summary>
        /// Stops the server transport, preventing new connections.
        /// </summary>
        void Stop();

        /// <summary>
        /// Closes the connection with the specified connection identifier.
        /// </summary>
        /// <param name="connectionId">The connection identifier to close.</param>
        void CloseConnection(int connectionId);

        /// <summary>
        /// Called periodically to perform any necessary actions.
        /// </summary>
        void Tick();

        /// <summary>
        /// Sends a payload to all connected clients.
        /// </summary>
        /// <param name="payload">The payload to send.</param>
        void Send(Payload payload);

        /// <summary>
        /// Sends a payload to a specific client identified by connection ID.
        /// </summary>
        /// <param name="payload">The payload to send.</param>
        /// <param name="connectionId">The connection identifier of the client.</param>
        void Send(Payload payload, int connectionId);

        /// <summary>
        /// Receives payloads and processes them using the specified handler.
        /// </summary>
        /// <param name="onMessageReceived">The handler to process received payloads.</param>
        void Receive(Action<Payload, int> onMessageReceived);
    }
}