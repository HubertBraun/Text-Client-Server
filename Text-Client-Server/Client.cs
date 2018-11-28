using System;
using System.Net;

namespace Text_Client_Server
{
    internal class Client : Host
    {
        public Client(int port) : base(port, IPAddress.Parse("127.0.0.1"))
        {
        }

        public string ReadAnswer(string charbuff)
        {
            string[] encoding = Statement.Encoding(charbuff);
            foreach (var str in encoding)
            {
                switch (Statement.GetKey(str))
                {
                    case Statement._Keys.Arg1:
                        return Statement.GetValue(str);
                }
            }

            return "Brak odpowiedzi";
        }

        public Statement GetIDRequest()
        {
            string[] arguments = new string[3];
            arguments[0] = arguments[2] = "0";
            arguments[1] = "-";
            Statement temp = new Statement(arguments, -1, -1);
            return temp;
        }
    }
}