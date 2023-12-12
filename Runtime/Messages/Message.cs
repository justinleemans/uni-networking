using System.IO;

namespace JeeLee.Networking.Messages
{
    public abstract class Message : IMessage
    {
        public abstract int InternalId { get; }

        public byte[] DataStream { get; private set; }

        public void Clear()
        {
            OnClear();
        }

        protected virtual void OnClear()
        {
        }
    }
}