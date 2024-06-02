using System;

namespace JeeLee.UniNetworking.Peers
{
    /// <summary>
    /// Represents the interface for the client peer. Runs the general client code which allows a connection to be made to a server peer.
    /// </summary>
    public interface IClient : IPeer
    {
        /// <summary>
        /// Event triggered when this client gets disconnected.
        /// </summary>
        event Action ConnectionClosed;

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
    }
}