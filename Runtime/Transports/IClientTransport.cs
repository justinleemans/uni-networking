using System;
using JeeLee.UniNetworking.Payloads;

namespace JeeLee.UniNetworking.Transports
{
    /// <summary>
    /// Represents the interface for client-side network transports in the communication system.
    /// </summary>
    public interface IClientTransport
    {
        /// <summary>
        /// Event triggered when the client disconnects from the server.
        /// </summary>
        event Action ClientDisconnected;

        /// <summary>
        /// Gets a value indicating whether the client is currently connected to a server.
        /// </summary>
        bool IsConnected { get; }
        
        /// <summary>
        /// Establishes a connection to a remote server.
        /// </summary>
        void Connect();

        /// <summary>
        /// Disconnects from the current server, if connected.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Called periodically to perform any necessary actions.
        /// </summary>
        void Tick();

        /// <summary>
        /// Sends a payload to the connected server.
        /// </summary>
        /// <param name="payload">The payload to send.</param>
        void Send(Payload payload);

        /// <summary>
        /// Receives payloads from the server and processes them using the specified handler.
        /// </summary>
        /// <param name="onMessageReceived">The handler to process received payloads.</param>
        void Receive(Action<Payload, int> onMessageReceived);
    }
}