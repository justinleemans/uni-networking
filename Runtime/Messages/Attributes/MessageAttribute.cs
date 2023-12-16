using System;

namespace JeeLee.Networking.Messages.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MessageAttribute : Attribute
    {
        public int MessageId { get; private set; }

        public MessageAttribute(int messageId)
        {
            MessageId = messageId;
        }
    }
}