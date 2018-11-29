using System;
using System.Collections.Generic;
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

        public List<byte[]> GetIDRequest()
        {
            Statement st = new Statement(ID);
            List<byte[]> toReturn = st.CreateBuffer();
            return toReturn;
        }

        public void ChangeID(string[] encoding)
        {
            List<byte[]> toReturn = new List<byte[]>();
            foreach (var str in encoding)
            {
                switch (Statement.GetKey(str))  // sprawdzenie czy wyslano zapytanie o przydzial ID sesji
                {
                    case Statement._Keys.ID:
                        ID = Convert.ToInt32(Statement.GetValue(str));
                        break;
                    default:
                        break;
                }
            }
        }
    }
}