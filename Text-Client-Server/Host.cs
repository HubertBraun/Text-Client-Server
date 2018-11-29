using System;
using System.Collections.Generic;
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
        public int CID = 0; // identyfikator obliczen
        public int ID = -1; // identyfikator sesji

        public Host(int port, IPAddress IP)
        {
            _port = port;
            _IpEndPoint = new IPEndPoint(IP, _port);
            _EndPoint = _IpEndPoint;
            _Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }


        public void Write(List<byte[]> bufferList)
        {
            foreach (var buffer in bufferList)
            {
                string temp = BufferUtilites.BufferToString(buffer,buffer.Length);
                temp += "\n";   // dla zwiekszenia czytelnosci dodaje znak nowej linii
                _Socket.SendTo(temp.ToBuffer(), buffer.Length + 1, SocketFlags.None, _EndPoint);
            }
        }

        public void Read(out string buffer)
        {
            byte[] tempbuff = new byte[1024];
            string charbuff;
            buffer = "";
            int NS = -1; // liczba komunikatow do odebrania
            Regex reg = new Regex("NumerSekwencyjny: ([0-9]*)"); // wyrazenie znajdujace liczbe komunikatow

            while (NS != 1) // numer ostaniego komunikatu
            {
                _ReceivedData = _Socket.ReceiveFrom(tempbuff, ref _EndPoint);
                charbuff = BufferUtilites.BufferToString(tempbuff, _ReceivedData - 1); // ostatni znak to znak nowej linii
                buffer += charbuff; // dodanie do listy
                Match m = reg.Match(charbuff);
                GroupCollection groups = m.Groups;
                if (m.Groups.Count == 2)
                    NS = Convert.ToInt32(m.Groups[1].Value); // przypisanie numeru sekwencyjnego
            }

            _ReceivedData = buffer.Length * 2;
        }
    }
}