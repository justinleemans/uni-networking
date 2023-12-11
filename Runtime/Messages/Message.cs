using System.IO;

namespace JeeLee.Networking.Messages
{
    public abstract class Message : IMessage
    {
        public abstract int InternalId { get; }

        #region IMessage Members

        public void Clear()
        {
            OnClear();
        }

        #endregion

        public byte[] ReadMessageData()
        {
            using MemoryStream stream = new MemoryStream();
            using BinaryWriter writer = new BinaryWriter(stream);

            Serialize(writer);

            return stream.ToArray();
        }

        public void WriteMessageData(byte[] dataStream)
        {
            using MemoryStream stream = new MemoryStream(dataStream);
            using BinaryReader reader = new BinaryReader(stream);

            Deserialize(reader);
        }

        protected abstract void Serialize(BinaryWriter writer);
        protected abstract void Deserialize(BinaryReader reader);

        protected virtual void OnClear()
        {
        }
    }
}