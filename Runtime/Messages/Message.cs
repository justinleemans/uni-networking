using JeeLee.Networking.Messages.Streams;

namespace JeeLee.Networking.Messages
{
    public abstract class Message : IMessage
    {
        private DataStream _dataStream = new DataStream();

        public DataStream DataStream
        {
            get
            {
                OnSerialize(_dataStream);
                return _dataStream;
            }
            set
            {
                _dataStream = value;
                OnDeserialize(_dataStream);
            }
        }

        public void Clear()
        {
            _dataStream.Reset();
            OnClear();
        }

        protected virtual void OnClear()
        {
        }

        protected virtual void OnSerialize(IWriteDataStream dataStream)
        {
        }

        protected virtual void OnDeserialize(IReadDataStream dataStream)
        {
        }
    }
}