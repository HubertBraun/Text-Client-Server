using System;
using System.Net;
using System.Text.RegularExpressions;

namespace Text_Client_Server
{
    internal class Server : Host
    {
        public Server(int port) : base(port, IPAddress.Any)
        {
            _Socket.Bind(_IpEndPoint);
        }
        public new void Read(ref byte[] buffer)     // metoda przeslonieta
        {
            int lK = -1;    // liczba komunikatow do odebrania
            Regex reg = new Regex("LiczbaKomunikatow: ([0-9])");   // wyrazenie znajdujace liczbe komunikatow
            string charbuff = null;
            for (int i = 0; i != lK; i++)
            {
                _ReceivedData = _Socket.ReceiveFrom(buffer, ref _EndPoint);
                charbuff += BufferUtilites.BufferToString(buffer, _ReceivedData);
                Match m = reg.Match(charbuff);
                GroupCollection groups = m.Groups;
                if (m.Groups.Count == 2)
                {
                    lK = Convert.ToInt32(m.Groups[1].Value);    // przypisanie liczby komunikatow
                }

            }

            buffer = charbuff.ToBuffer();
            _ReceivedData = buffer.Length;
        }
    }
}