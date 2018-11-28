using System;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace Text_Client_Server
{
    internal class Host
    {
        protected EndPoint _EndPoint;
        protected IPEndPoint _IpEndPoint;
        protected int _port;
        public int _ReceivedData = 0;
        protected Socket _Socket;
        public int ID; // identyfikator sesji
        public int CID; // identyfikator obliczen

        public Host(int port, IPAddress IP)
        {
            _port = port;
            _IpEndPoint = new IPEndPoint(IP, _port);
            _EndPoint = _IpEndPoint;
            _Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }


        public void Write(byte[] buffer, int[] BufferLenght)
        {
            int Lenghts = 0;
            for (int i = 0; i < BufferLenght.Length; i++)
            {
                _Socket.SendTo(buffer, Lenghts, BufferLenght[i], SocketFlags.None, _EndPoint);
                Lenghts += BufferLenght[i];
            }
        }

        public void Read(ref byte[] buffer)
        {
            int lK = -1; // liczba komunikatow do odebrania
            Regex reg = new Regex("LiczbaKomunikatow: ([0-9])"); // wyrazenie znajdujace liczbe komunikatow
            string charbuff = null;
            for (int i = 0; i != lK; i++)
            {
                _ReceivedData = _Socket.ReceiveFrom(buffer, ref _EndPoint);
                charbuff += BufferUtilites.BufferToString(buffer, _ReceivedData);
                Match m = reg.Match(charbuff);
                GroupCollection groups = m.Groups;
                if (m.Groups.Count == 2)
                {
                    lK = Convert.ToInt32(m.Groups[1].Value); // przypisanie liczby komunikatow
                }
            }

            buffer = charbuff.ToBuffer();
            _ReceivedData = buffer.Length;
        }
    }
}