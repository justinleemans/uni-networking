using System;

namespace JeeLee.Networking.Messages.Attributes
{
    /// <summary>
    /// Attribute used to assign a unique Id to a message. Allows messages to be identified over the network.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class MessageAttribute : Attribute
    {
        /// <summary>
        /// The message id assigned to this message.
        /// </summary>
        public int MessageId { get; private set; }

        /// <summary>
        /// Constructor for the message attribute. Sets the message id for this message.
        /// </summary>
        /// <param name="messageId">The message id to use.</param>
        public MessageAttribute(int messageId)
        {
            MessageId = messageId;
        }
    }
}