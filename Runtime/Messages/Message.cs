namespace JeeLee.Networking.Messages
{
    public abstract class Message : IMessage
    {
        public byte[] DataStream { get; private set; }

        public void Clear()
        {
            OnClear();
        }

        protected virtual void OnClear()
        {
        }
    }
}