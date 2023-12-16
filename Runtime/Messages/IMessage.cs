using JeeLee.Networking.Messages.Streams;

namespace JeeLee.Networking.Messages
{
    public interface IMessage
    {
        DataStream DataStream { get; set; }
        void Clear();
    }
}