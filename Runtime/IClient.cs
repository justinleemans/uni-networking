namespace JeeLee.UniNetworking
{
    public interface IClient : IPeer
    {
        bool Connect();
        void Disconnect();
        void Tick();
    }
}