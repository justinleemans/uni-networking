using JeeLee.Networking.Messages.Delegates;
using JeeLee.Networking.Messages.Streams;

namespace JeeLee.Networking.Transports
{
    public interface ITransport
    {
        MessageReceivedHandler OnMessageReceived { get; set; }

        void Send(DataStream dataStream);
        void Tick();
    }
}