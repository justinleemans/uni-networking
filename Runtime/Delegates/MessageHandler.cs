using JeeLee.Networking.Messages;

namespace JeeLee.Networking.Delegates
{
    public delegate void MessageHandler<in TMessage>(TMessage message)
        where TMessage : IMessage;
}