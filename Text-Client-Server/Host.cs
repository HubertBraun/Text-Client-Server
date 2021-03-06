﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace Text_Client_Server
{
    internal class Host
    {
        protected EndPoint _EndPoint;   // punkt docelowy
        protected IPEndPoint _IpEndPoint;   // ip punktu docelowego
        protected string _op, _arg1, _arg2, _answer; // do wpisywania i odczytywania historii
        protected int _port;    // port
        public int _ReceivedData = 0;   // dlugosc otrzymanych danych
        protected Socket _Socket;   // gniazdo
        public int CID = -1; // identyfikator obliczen
        public int ID = -1; // identyfikator sesji

        public Host(int port, IPAddress IP)
        {
            _port = port;
            _IpEndPoint = new IPEndPoint(IP, _port);
            _EndPoint = _IpEndPoint;
            _Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }


        public void Write(List<byte[]> bufferList)  // wysylanie wiadomosci 
        {
            foreach (var buffer in bufferList)
            {
                _Socket.SendTo(buffer, buffer.Length, SocketFlags.None, _EndPoint);
            }
        }

        public void Read(out string buffer) // odbieranie wiadomosci
        {
            byte[] tempbuff = new byte[1024];
            string charbuff;
            buffer = "";    // resetowanie buffera
            int NS = -1; // liczba komunikatow do odebrania
            Regex reg = new Regex(Statement._Keys.NS + "([0-9]*)"); // wyrazenie znajdujace liczbe komunikatow

            while (NS != 1) // numer ostaniego komunikatu
            {
                _ReceivedData = _Socket.ReceiveFrom(tempbuff, ref _EndPoint);
                charbuff = BufferUtilites.BufferToString(tempbuff,
                    _ReceivedData ); // ostatni znak to znak nowej linii
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