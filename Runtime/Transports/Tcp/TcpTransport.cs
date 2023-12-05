using System.Threading.Tasks;
using JeeLee.Networking.Delegates;

namespace JeeLee.Networking.Transports.Tcp
{
    public class TcpTransport : ITransport
    {
        #region ITransport Members
        
        public MessageReceivedHandler OnMessageReceived { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public bool IsConnected => throw new System.NotImplementedException();
        public bool IsHost => throw new System.NotImplementedException();

        public Task Start()
        {
            throw new System.NotImplementedException();
        }

        public Task Connect()
        {
            throw new System.NotImplementedException();
        }

        public Task Disconnect()
        {
            throw new System.NotImplementedException();
        }

        public Task Send(byte[] dataBuffer, int length)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}