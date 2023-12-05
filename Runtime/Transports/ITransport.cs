using System.Threading.Tasks;
using JeeLee.Networking.Delegates;

namespace JeeLee.Networking.Transports
{
    public interface ITransport
    {
        MessageReceivedHandler OnMessageReceived { get; set; }
        bool IsConnected { get; }
        bool IsHost { get; }

        Task Start();
        Task Connect();
        Task Disconnect();
        Task Send(byte[] dataBuffer, int length);
    }
}