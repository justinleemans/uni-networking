using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using JeeLee.UniNetworking.Payloads;

namespace JeeLee.UniNetworking.Transports.Udp
{
    public class UdpClientTransport : IClientTransport
    {
        private const int BufferSize = 1024 * 8;
        
        public Action ClientDisconnected { get; set; }
        public Action<Payload, int> MessageReceived { get; set; }

        private Socket _socket;

        public string IpAddress { get; set; } = "127.0.0.1";
        public ushort Port { get; set; } = 7777;

        public void Connect()
        {
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse(IpAddress), Port);
            
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
            _socket.Connect(remoteEndPoint);
        }

        public void Disconnect()
        {
            Send(new Payload(PayloadType.Disconnect));
            Close();
        }

        public void Tick()
        {
            if (_socket.Available > 0)
            {
                byte[] dataBuffer = new byte[BufferSize];
                int receivedBytes = _socket.Receive(dataBuffer, SocketFlags.None);

                Payload payload = new Payload(dataBuffer.Skip(sizeof(int)));

                switch (payload.Type)
                {
                    case PayloadType.Message:
                        MessageReceived(payload, -1);
                        break;
                
                    case PayloadType.Disconnect:
                        Close();
                        break;
                }
            }
        }

        public void Send(Payload payload)
        {
            _socket.Send(payload.GetBytes(), SocketFlags.None);
        }

        private void Close()
        {
            ClientDisconnected();
            
            try
            {
                _socket.Shutdown(SocketShutdown.Both);
            }
            finally
            {
                _socket.Close();
            }
        }
    }
}