using JeeLee.UniNetworking.Messages;

namespace JeeLee.UniNetworking
{
    public interface IServer : IPeer
    {
        bool Start();
        void Stop();
        void CloseConnection(int connectionId);
        void SendMessage<TMessage>(int connectionId) where TMessage : Message;
        void SendMessage<TMessage>(TMessage message, int connectionId) where TMessage : Message;
        void Subscribe<TMessage>(MessageFromHandler<TMessage> handler) where TMessage : Message;
        void Unsubscribe<TMessage>(MessageFromHandler<TMessage> handler) where TMessage : Message;
        void Tick();
    }
}