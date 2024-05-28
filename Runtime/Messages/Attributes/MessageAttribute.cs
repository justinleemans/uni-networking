using System;

namespace JeeLee.UniNetworking.Messages.Attributes
{
    /// <summary>
    /// Attribute used to assign a unique Id to a message. Allows messages to be identified over the network.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class MessageAttribute : Attribute
    {
        /// <summary>
        /// Gets the message id assigned to this message.
        /// </summary>
        public short MessageId { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageAttribute"/> class with the specified message id.
        /// </summary>
        /// <param name="messageId">The unique identifier assigned to this message.</param>
        public MessageAttribute(short messageId)
        {
            MessageId = messageId;
        }
    }
}