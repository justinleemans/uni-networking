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
        Action ClientDisconnected { get; set; }

        /// <summary>
        /// Event triggered when the client receives a message from the server.
        /// </summary>
        Action<Payload, int> MessageReceived { get; set; }
        
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
    }
}