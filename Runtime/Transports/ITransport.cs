using JeeLee.Networking.Delegates;

namespace JeeLee.Networking.Transports
{
    public interface ITransport
    {
        MessageReceivedHandler OnMessageReceived { get; set; }

        void Send(byte[] dataBuffer);
        void Tick();
    }
}