using System;

namespace JeeLee.UniNetworking.Transports
{
    /// <summary>
    /// Represents the interface for server-side network transports in the communication system.
    /// </summary>
    public interface IServerTransport : ITransport
    {
        /// <summary>
        /// Event triggered when a new connection is established.
        /// </summary>
        event Action<Connection> NewConnection;
        
        /// <summary>
        /// Starts the server transport to listen for incoming connections.
        /// </summary>
        void Start();
        
        /// <summary>
        /// Stops the server transport, preventing new connections.
        /// </summary>
        void Stop();
    }
}