using System.IO;

namespace JeeLee.Networking.Messages
{
    public abstract class Message : IMessage
    {
        public abstract int InternalId { get; }

        #region IMessage Members

        public void Clear()
        {
        }

        #endregion

        public byte[] ReadMessageData()
        {
            using MemoryStream stream = new MemoryStream();
            using BinaryWriter writer = new BinaryWriter(stream);

            InternalSerialize(writer);

            return stream.ToArray();
        }

        public void WriteMessageData(byte[] dataBuffer)
        {
            using MemoryStream stream = new MemoryStream(dataBuffer);
            using BinaryReader reader = new BinaryReader(stream);

            InternalDeserialize(reader);
        }

        protected abstract void Serialize(BinaryWriter writer);
        protected abstract void Deserialize(BinaryReader reader);

        protected virtual void OnClear()
        {
        }

        private void InternalSerialize(BinaryWriter writer)
        {
            writer.Write(InternalId);
            Serialize(writer);
        }

        private void InternalDeserialize(BinaryReader reader)
        {
            Deserialize(reader);
            reader.ReadInt32();
        }
    }
}