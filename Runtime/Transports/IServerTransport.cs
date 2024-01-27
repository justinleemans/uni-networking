using System;

namespace JeeLee.Networking.Transports
{
    /// <summary>
    /// Interface that is used for the server specific implementation of the transport.
    /// Generally only the server peer should communicate with this class.
    /// </summary>
    public interface IServerTransport : ITransport
    {
        event Action<Connection> OnNewConnection;
        
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