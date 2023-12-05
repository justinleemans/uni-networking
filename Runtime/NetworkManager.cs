using System;
using JeeLee.Networking.Domain;
using JeeLee.Networking.Transports;
using JeeLee.Networking.Transports.Tcp;
using JeeLee.Signals;

namespace JeeLee.Networking
{
    public class NetworkManager : IDisposable
    {
        private readonly SignalManager _signalManager;
        private readonly ITransport _transport;

        public NetworkManager(SignalManager signalManager, ITransport transport = null)
        {
            _signalManager = signalManager;
            _transport = transport ?? new TcpTransport();

            _signalManager.Subscribe<NetworkSignal>(OnNetworkSignal);
            _transport.OnMessageReceived = OnNetworkMessageReceived;
        }

        public void Dispose()
        {
            _signalManager.Unsubscribe<NetworkSignal>(OnNetworkSignal);
            _transport.OnMessageReceived = null;
        }

        private void OnNetworkSignal(NetworkSignal signal)
        {
            if (signal.IsIncoming)
            {
                return;
            }
        }

        private void OnNetworkMessageReceived(int messageId, byte[] dataBuffer, int length)
        {
        }
    }
}