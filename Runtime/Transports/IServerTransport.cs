using System.Collections.Generic;

namespace JeeLee.Networking.Transports
{
    public interface IServerTransport : ITransport
    {
        HashSet<Connection> Connections { get; }
        bool IsRunning { get; }

        void Start(ushort port, int maxConnections);
        
        void Stop();
    }
}