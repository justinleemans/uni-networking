using JeeLee.UniNetworking.Messages.Streams;

namespace JeeLee.UniNetworking.Messages
{
    /// <summary>
    /// Represents an abstract base class for messages in the network communication system.
    /// </summary>
    public abstract class Message : IMessage
    {
        private DataStream _dataStream = new DataStream();

        #region IMessage Members

        /// <summary>
        /// Gets or sets the data stream associated with the message.
        /// </summary>
        public DataStream DataStream
        {
            get
            {
                if (!_dataStream.IsWritten)
                {
                    OnSerialize(_dataStream);
                }

                return _dataStream;
            }
            set
            {
                _dataStream = value;
                OnDeserialize(_dataStream);
            }
        }

        /// <summary>
        /// Clears the content of the message.
        /// </summary>
        public void Clear()
        {
            _dataStream.Reset();
            OnClear();
        }

        #endregion

        /// <summary>
        /// Called when clearing the content of the message.
        /// </summary>
        protected virtual void OnClear()
        {
        }

        /// <summary>
        /// Called when serializing the message to a data stream.
        /// </summary>
        /// <param name="dataStream">The data stream to which the message is serialized.</param>
        protected virtual void OnSerialize(IWriteDataStream dataStream)
        {
        }

        /// <summary>
        /// Called when deserializing the message from a data stream.
        /// </summary>
        /// <param name="dataStream">The data stream from which the message is deserialized.</param>
        protected virtual void OnDeserialize(IReadDataStream dataStream)
        {
        }
    }
}