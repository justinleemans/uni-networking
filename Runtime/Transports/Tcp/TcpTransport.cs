using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using JeeLee.Networking.Delegates;

namespace JeeLee.Networking.Transports.Tcp
{
    public class TcpTransport : ITransport
    {
        #region ITransport Members

        public MessageReceivedHandler OnClientMessageReceived { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public bool IsServerRunning => throw new System.NotImplementedException();
        public bool IsClientConnected => throw new System.NotImplementedException();
        public bool IsClientHost => throw new System.NotImplementedException();

        public void ServerStart()
        {
            throw new System.NotImplementedException();
        }

        public void ServerStop()
        {
            throw new System.NotImplementedException();
        }

        public void ClientConnect()
        {
            throw new System.NotImplementedException();
        }

        public void ClientDisconnect()
        {
            throw new System.NotImplementedException();
        }

        public void ClientSend(byte[] dataBuffer, int length)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}