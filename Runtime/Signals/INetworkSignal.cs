using JeeLee.Signals.Domain;

namespace JeeLee.Networking.Domain
{
    public interface INetworkSignal : ISignal
    {
        int Id { get; }
        bool IsIncoming { get; }
    }
}