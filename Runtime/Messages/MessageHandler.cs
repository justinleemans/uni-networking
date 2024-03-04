namespace JeeLee.UniNetworking.Messages
{
    /// <summary>
    /// Represents a delegate for handling messages of type <typeparamref name="TMessage"/>.
    /// </summary>
    /// <typeparam name="TMessage">The type of message to handle.</typeparam>
    /// <param name="message">The message to be handled.</param>
    public delegate void MessageHandler<in TMessage>(TMessage message)
        where TMessage : IMessage;

    /// <summary>
    /// Represents a delegate for handling messages of type <typeparamref name="TMessage"/> from a specific connection.
    /// </summary>
    /// <typeparam name="TMessage">The type of message to handle.</typeparam>
    /// <param name="connectionId">The identifier of the connection from which the message is received.</param>
    /// <param name="message">The message to be handled.</param>
    public delegate void MessageFromHandler<in TMessage>(TMessage message, int connectionId)
        where TMessage : IMessage;
}