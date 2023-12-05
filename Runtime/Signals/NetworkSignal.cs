using JeeLee.Signals.Domain;

namespace JeeLee.Networking.Domain
{
    public abstract class NetworkSignal : Signal, INetworkSignal
    {
        public abstract int Id { get; }
        public bool IsIncoming { get; set; }
    }
}