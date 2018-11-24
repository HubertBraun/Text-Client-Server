using System.Net;

namespace Text_Client_Server
{
    internal class Client : Host
    {
        public Client(int port) : base(port, IPAddress.Parse("127.0.0.1"))
        {
        }
    }
}