using JeeLee.Networking.Messages.Delegates;
using JeeLee.Networking.Messages.Streams;

namespace JeeLee.Networking.Transports
{
    public interface ITransport
    {
        event MessageReceivedHandler OnMessageReceived;

        void Send(DataStream dataStream);
        void Tick();
    }
}