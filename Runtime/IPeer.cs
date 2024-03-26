using JeeLee.UniNetworking.Messages;

namespace JeeLee.UniNetworking
{
    public interface IPeer
    {
        void SendMessage<TMessage>() where TMessage : Message;
        void SendMessage<TMessage>(TMessage message) where TMessage : Message;
        TMessage GetMessage<TMessage>() where TMessage : Message;
        void Subscribe<TMessage>(MessageHandler<TMessage> handler) where TMessage : Message;
        void Unsubscribe<TMessage>(MessageHandler<TMessage> handler) where TMessage : Message;
    }
}