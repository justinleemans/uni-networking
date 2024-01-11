using System.Collections.Generic;

namespace JeeLee.Networking.Transports
{
    /// <summary>
    /// Interface that is used for the server specific implementation of the transport.
    /// Generally only the server peer should communicate with this class.
    /// </summary>
    public interface IServerTransport : ITransport
    {
        /// <summary>
        /// List of connections made to this server.
        /// </summary>
        HashSet<Connection> Connections { get; }

        /// <summary>
        /// Should be set to tell the peer if the server is running.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Called when trying to start the server over this transport.
        /// </summary>
        void Start();
        
        /// <summary>
        /// Called when trying to stop the server running over this transport.
        /// </summary>
        void Stop();
    }
}