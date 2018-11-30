using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Text_Client_Server
{
    internal class Client : Host
    {
        public Client(int port) : base(port, IPAddress.Parse("127.0.0.1"))
        {
        }

        public string ReadAnswer(string charbuff)   // wyswietlenie odpowiedzi
        {
            string[] encoding = Statement.Encoding(charbuff);
            foreach (var str in encoding)
            {
                switch (Statement.GetKey(str))
                {
                    case Statement._Keys.Arg1:  // odpowiedz znajduje sie w arg1
                        return Statement.GetValue(str);
                }
            }

            return "Brak odpowiedzi";
        }

        private string EncodeOperation(string OP)   // zamiana nazw operacji na ich symbole
        {
            switch (OP)
            {
                case Statement._OP.Div:
                    return "/";
                    break;
                case Statement._OP.Exp:
                    return "^";
                case Statement._OP.Fac:
                    return "!";
                case Statement._OP.Mul:
                    return "*";
                case Statement._OP.Sub:
                    return "-";
            }

            return "Nierozpoznana operacja!";
        }


        public string ReadHistory(string charbuff)  // odczytanie historii
        {
            string[] encoding = Statement.Encoding(charbuff);
            StringBuilder toReturn = new StringBuilder();
            foreach (var str in encoding)
            {
                switch (Statement.GetKey(str))
                {
                    case Statement._Keys.Arg1:
                        _arg1 = Statement.GetValue(str);
                        break;
                    case Statement._Keys.Arg2:
                        _arg2 = Statement.GetValue(str);
                        break;
                    case Statement._Keys.Arg3:  // arg3 znajduje sie na koncu wpisu w historii
                        _answer = Statement.GetValue(str);
                        toReturn.Append(_arg1);
                        toReturn.Append(EncodeOperation(_op));
                        toReturn.Append(_arg2);
                        toReturn.Append("=");
                        toReturn.Append(_answer);
                        toReturn.Append("\n");
                        _arg1 = _arg2 = _answer = _op = ""; // czyszczenie argumentow
                        break;
                    case Statement._Keys.OP:
                        _op = Statement.GetValue(str);
                        break;
                    case Statement._Keys.ST:
                        if (Statement.GetValue(str) == Statement._ST.Error)
                        {
                            return Statement._ERR.History;
                        }

                        break;
                }
            }

            return toReturn.ToString();
        }

        public List<byte[]> GetIDRequest()
        {
            Statement st = new Statement(ID);
            List<byte[]> toReturn = st.CreateBuffer(0);
            return toReturn;
        }

        public void ChangeID(string[] encoding) // zmiana id sesjo
        {
            List<byte[]> toReturn = new List<byte[]>();
            foreach (var str in encoding)
            {
                switch (Statement.GetKey(str)) // sprawdzenie czy wyslano zapytanie o przydzial ID sesji
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