namespace JeeLee.Networking.Messages.Delegates
{
    public delegate void MessageHandler<in TMessage>(TMessage message)
        where TMessage : IMessage;
}