using System;
using JeeLee.UniNetworking.Logging;
using JeeLee.UniNetworking.Messages.Payloads;

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
        /// Serializes the message to a payload with the specified message ID.
        /// </summary>
        /// <param name="messageId">The ID assigned to the message.</param>
        /// <returns>The serialized message as a payload.</returns>
        internal Payload Serialize(int messageId)
        {
            try
            {
                var payload = new Payload(messageId);
                
                OnSerialize(payload);

                return payload;
            }
            catch (Exception exception)
            {
                NetworkLogger.Log(exception, LogLevel.Error);
                
                return null;
            }
        }

        /// <summary>
        /// Deserializes the message from the provided payload.
        /// </summary>
        /// <param name="payload">The payload containing the serialized message.</param>
        internal void Deserialize(Payload payload)
        {
            try
            {
                OnDeserialize(payload);
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
        /// Called when serializing the message to a payload.
        /// </summary>
        /// <param name="payload">The payload to which the message is serialized.</param>
        protected virtual void OnSerialize(IWriteablePayload payload)
        {
        }

        /// <summary>
        /// Called when deserializing the message from a payload.
        /// </summary>
        /// <param name="payload">The payload from which the message is deserialized.</param>
        protected virtual void OnDeserialize(IReadablePayload payload)
        {
        }
    }
}