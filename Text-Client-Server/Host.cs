using System.Net;
using System.Net.Sockets;

namespace Text_Client_Server
{
    internal class Host
    {
        protected EndPoint _EndPoint;
        protected IPEndPoint _IpEndPoint;
        protected int _port;
        public int _ReceivedData = 0;
        protected Socket _Socket;

        public Host(int port, IPAddress IP)
        {
            _port = port;
            _IpEndPoint = new IPEndPoint(IP, _port);
            _EndPoint = _IpEndPoint;
            _Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }


        public void Write(byte[] buffer)
        {
            _Socket.SendTo(buffer, buffer.Length, SocketFlags.None, _EndPoint);
        }

        public void Read(ref byte[] buffer)
        {
            _ReceivedData = _Socket.ReceiveFrom(buffer, ref _EndPoint);
        }
    }
}