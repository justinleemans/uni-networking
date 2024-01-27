namespace JeeLee.Networking.Messages
{
    public delegate void MessageHandler<in TMessage>(TMessage message)
        where TMessage : IMessage;

    public delegate void MessageFromHandler<in TMessage>(int connectionId, TMessage message)
        where TMessage : IMessage;
}