using System.Collections.Generic;

namespace JeeLee.UniNetworking.Messages.Registries
{
    public interface IMessageRegistry
    {
        IReadOnlyDictionary<short, IMessageBroker> MessageBrokers { get; }

        MessageBroker<TMessage> AllocateMessageBroker<TMessage>()
            where TMessage : Message;

        short RegisterMessageId<TMessage>()
            where TMessage : Message;
    }
}