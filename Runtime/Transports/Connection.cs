using JeeLee.Networking.Delegates;

namespace JeeLee.Networking.Transports
{
    public abstract class Connection
    {
        private readonly int _networkIdentifier;

        protected Connection(int networkIdentifier)
        {
            _networkIdentifier = networkIdentifier;
        }

        public abstract void Send(byte[] dataBuffer);
        public abstract void Receive(MessageReceivedHandler handler);
        public abstract void Close();

        public override bool Equals(object obj)
        {
            if (obj is Connection other)
            {
                return _networkIdentifier.Equals(other._networkIdentifier);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return _networkIdentifier.GetHashCode();
        }
    }
}