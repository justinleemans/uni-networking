using JeeLee.Networking.Delegates;
using JeeLee.Networking.Messages;

namespace JeeLee.Networking.Transports
{
    public interface ITransport
    {
        MessageReceivedHandler OnMessageReceived { get; set; }

        void Send<TMessage>(TMessage message)
            where TMessage : Message;
            
        void Tick();
    }
}