using JeeLee.Networking.Transports;

namespace JeeLee.Networking
{
    public class NetworkManager : INetworkManager
    {
        private readonly ITransport _transport;

        public NetworkManager(ITransport transport = null)
        {
            _transport = transport ?? new TcpTransport();
        }
    }
}