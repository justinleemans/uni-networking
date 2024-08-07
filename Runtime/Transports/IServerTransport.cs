using System;
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
        Func<int> ClientConnected { get; set; }

        /// <summary>
        /// Event triggered when a client disconnects from the server.
        /// </summary>
        Action<int> ClientDisconnected { get; set; }

        /// <summary>
        /// Event triggered when the server receives a message from a client.
        /// </summary>
        Action<Payload, int> MessageReceived { get; set; }

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
    }
}