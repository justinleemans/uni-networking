using System;
using JeeLee.UniNetworking.Logging;
using JeeLee.UniNetworking.Messages.Streams;

namespace JeeLee.UniNetworking.Messages
{
    /// <summary>
    /// Represents an abstract base class for messages in the network communication system.
    /// </summary>
    public abstract class Message
    {
        /// <summary>
        /// Clears the content of the message.
        /// </summary>
        internal void Clear()
        {
            OnClear();
        }

        /// <summary>
        /// Serializes the message to a data stream with the specified message ID.
        /// </summary>
        /// <param name="messageId">The ID assigned to the message.</param>
        /// <returns>The serialized message as a data stream.</returns>
        internal DataStream Serialize(int messageId)
        {
            try
            {
                var dataStream = new DataStream();
                
                OnSerialize(dataStream);
                dataStream.Sign(messageId);

                return dataStream;
            }
            catch (Exception exception)
            {
                NetworkLogger.Log(exception, LogLevel.Error);
                
                return null;
            }
        }

        /// <summary>
        /// Deserializes the message from the provided data stream.
        /// </summary>
        /// <param name="dataStream">The data stream containing the serialized message.</param>
        internal void Deserialize(DataStream dataStream)
        {
            try
            {
                OnDeserialize(dataStream);
            }
            catch (Exception exception)
            {
                NetworkLogger.Log(exception, LogLevel.Error);
            }
        }

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