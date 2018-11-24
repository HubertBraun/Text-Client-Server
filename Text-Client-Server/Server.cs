using System.Net;

namespace Text_Client_Server
{
    internal class Server : Host
    {
        public Server(int port) : base(port, IPAddress.Any)
        {
            _Socket.Bind(_IpEndPoint);
        }
    }
}