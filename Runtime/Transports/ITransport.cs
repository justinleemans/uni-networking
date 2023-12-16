using JeeLee.Networking.Messages;
using JeeLee.Networking.Messages.Delegates;

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