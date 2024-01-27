using JeeLee.Networking.Messages.Streams;

namespace JeeLee.Networking.Messages
{
    public interface IMessageRegistry
    {
        void Handle(int connectionId, DataStream dataStream);
    }
}