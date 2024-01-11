using JeeLee.Networking.Messages.Streams;

namespace JeeLee.Networking.Messages
{
    /// <summary>
    /// Abstract message class used to define messages.
    /// </summary>
    public abstract class Message : IMessage
    {
        private DataStream _dataStream = new DataStream();

        /// <summary>
        /// The data stream associated with this message. Used to translate the message over the network.
        /// </summary>
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

        /// <summary>
        /// Used internally by the message registry to reset properties.
        /// </summary>
        public void Clear()
        {
            _dataStream.Reset();
            OnClear();
        }

        /// <summary>
        /// Method used to define what needs to happen to a message when it gets reset for future use.
        /// </summary>
        protected virtual void OnClear()
        {
        }

        /// <summary>
        /// Called when message gets serialized in preperation to be send over the network.
        /// Make sure the order of variables read and set is the same as in `OnDeserialize()`.
        /// </summary>
        /// <param name="dataStream">Provides a set of methods used to write data to the data stream.</param>
        protected virtual void OnSerialize(IWriteDataStream dataStream)
        {
        }

        /// <summary>
        /// Called when a message gets deserialized after being received.
        /// Make sure the order of variables read and set is the same as in `OnSerialize()`.
        /// </summary>
        /// <param name="dataStream">Provides a set of methods used to read data from the data stream.</param>
        protected virtual void OnDeserialize(IReadDataStream dataStream)
        {
        }
    }
}