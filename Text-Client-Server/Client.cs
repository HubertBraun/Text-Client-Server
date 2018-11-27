﻿using System;
using System.Net;

namespace Text_Client_Server
{
    internal class Client : Host
    {
        public Client(int port) : base(port, IPAddress.Parse("127.0.0.1"))
        {
        }

        public string ReadAnswer(byte[] buffer)
        {
            string[] str = Statement.Encoding(BufferUtilites.BufferToString(buffer, buffer.Length));
            return Statement.GetValue(str[6]);
        }
    }
}