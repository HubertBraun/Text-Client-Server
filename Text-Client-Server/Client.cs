using System;
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

            string temp = Statement.GetValue(str[6]);
                if (Double.TryParse(temp, out double d))
                    return temp;
            else
                return Statement.GetValue(str[7]);
        }

        public Statement GetIDRequest()
        {
            string[] arguments = new string[3];
            arguments[0] = arguments[2] = "0";
            arguments[1] = "-";
            Statement temp = new Statement(arguments, 0, -1, -1);
            return temp;
        }
    }
}