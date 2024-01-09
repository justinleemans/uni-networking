namespace JeeLee.Networking.Transports
{
    public interface IClientTransport : ITransport
    {
        Connection Connection { get; }
        bool IsConnected { get; }

        bool Connect();
        
        void Disconnect();
    }
}