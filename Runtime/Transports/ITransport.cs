using System.Threading.Tasks;
using JeeLee.Networking.Delegates;

namespace JeeLee.Networking.Transports
{
    public interface ITransport
    {
        MessageReceivedHandler OnClientMessageReceived { get; set; }
        bool IsServerRunning { get; }
        bool IsClientConnected { get; }
        bool IsClientHost { get; }

        void ServerStart();
        void ServerStop();
        void ClientConnect();
        void ClientDisconnect();
        void ClientSend(byte[] dataBuffer, int length);
    }
}