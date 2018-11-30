using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace Text_Client_Server
{
    internal class Server : Host
    {
        private History _history = new History();
        private string _op, _arg1, _arg2, _answer;  // do tworzenia wpisu do historii

        public Server(int port) : base(port, IPAddress.Any)
        {
            _Socket.Bind(_IpEndPoint);
        }

        private Int64 CalculateFactorial(double n)
        {
            if (n == 0)
                return 1;
            Int64 result = 1;
            for (int i = 1; i <= n; i++)
            {
                result = checked(result * i);
            }

            return result;
        }

        public bool CheckHistory(string[] encoding, out List<String> list)
        {
            list = new List<string>();
            foreach (var str in encoding)
            {
                switch (Statement.GetKey(str)) //sprawdzenie operacji oraz argumentow
                {
                    case Statement._Keys.PHID:
                        list = CheckHistoryID(Convert.ToInt32(Statement.GetValue(str)));
                        return true;
                        break;
                    case Statement._Keys.PHCID:
                        list = CheckHistoryCID(Convert.ToInt32(Statement.GetValue(str)));
                        return true;
                        break;
                }
            }
            return false;
        }

        private List<String> CheckHistoryID(int id)
        {
            return _history.GetGistoryByID(id);


        }

        private List<String> CheckHistoryCID(int cid)
        {
            return _history.GetGistoryByCID(cid);
        }

        public string Calculate(string[] encoding)
        {
            CID++;
            string OP = "", answer = "";
            double Arg1 = 0, Arg2 = 0;
            foreach (var str in encoding)
            {
                switch (Statement.GetKey(str)) //sprawdzenie operacji oraz argumentow
                {
                    case Statement._Keys.OP:
                        OP = Statement.GetValue(str);
                        break;
                    case Statement._Keys.Arg1:
                        Arg1 = Convert.ToDouble(Statement.GetValue(str));
                        break;
                    case Statement._Keys.Arg2:
                        Arg2 = Convert.ToDouble(Statement.GetValue(str));
                        break;
                }
            }

            try
            {
                Console.WriteLine("".PadLeft(25, '*'));

                switch (OP)
                {
                    case Statement._OP.Div:
                        if (Arg2 == 0)
                        {
                            answer = Statement._ERR.NotAllowed;
                            throw new ArgumentException("Dzielenie przez zero");
                        }

                        answer = (Arg1 / Arg2).ToString();
                        Console.WriteLine("{0} / {1} = {2}", Arg1, Arg1, answer);
                        break;

                    case Statement._OP.Mul:
                        if (double.IsInfinity(Arg1 * Arg2))
                        {
                            answer = Statement._ERR.OverFlow;
                            throw new ArgumentException("Przepelnienie!");
                        }

                        answer = checked(Arg1 * Arg2).ToString();
                        Console.WriteLine("{0} * {1} = {2}", Arg1, Arg1, answer);
                        break;

                    case Statement._OP.Fac:

                        if (Arg1 >= 0)
                        {
                            try
                            {
                                answer = CalculateFactorial(Convert.ToInt32(Arg1)).ToString();
                            }
                            catch (Exception e)
                            {
                                answer = Statement._ERR.OverFlow;
                                throw new ArgumentException("Przepelnienie!");
                            }
                        }
                        else
                        {
                            answer = Statement._ERR.Factorial;
                            throw new ArgumentException("Ujemny argument");
                        }

                        Console.WriteLine("{0}! = {1}", Arg1, answer);
                        break;

                    case Statement._OP.Exp:
                        answer = Math.Pow(Arg1, Arg2).ToString();
                        if (double.IsInfinity(Math.Pow(Arg1, Arg2)))
                        {
                            answer = Statement._ERR.OverFlow;
                            throw new ArgumentException("Przepelnienie!");
                        }

                        Console.WriteLine("{0} ^ {1} = {2}", Arg1, Arg1, answer);
                        break;

                    case Statement._OP.Sub:
                        answer = (Arg1 - Arg2).ToString();
                        Console.WriteLine("{0} - {1} = {2}", Arg1, Arg1, answer);
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Source + " Exception");
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("".PadLeft(25, '*'));
            answer = answer.ToLower(); //w przypadku notacji wykladniczej
            return answer;
        }

        public List<byte[]> CheckIdRequest(string[] encoding)
        {
            List<byte[]> toReturn = new List<byte[]>();
            foreach (var str in encoding)
            {
                switch (Statement.GetKey(str)) // sprawdzenie czy wyslano zapytanie o przydzial ID sesji
                {
                    case Statement._Keys.ST:
                        if (Statement.GetValue(str) == Statement._ST.Request)
                        {
                            ID++;
                            Statement st = new Statement(ID);
                            toReturn = st.CreateBuffer(0);
                        }
                        else
                            throw new ArgumentNullException("Nieprawidlowe zadanie ID");

                        break;
                }
            }

            return toReturn;
        }

        public bool CheckExit(string[] encoding)
        {
            foreach (var str in encoding)
            {
                switch (Statement.GetKey(str)) // sprawdzenie czy klient chce sie rozlaczyc
                {
                    case Statement._Keys.ST:
                        if (Statement.GetValue(str) == Statement._ST.Exit)
                        {
                            return true;
                        }

                        break;
                }
            }

            return false;
        }

        public void AddArgsToHistory(string[] encoding)
        {
            foreach (var str in encoding)
            {
                switch (Statement.GetKey(str)) // sprawdzenie czy klient chce sie rozlaczyc
                {
                    case Statement._Keys.OP:
                        _op = str;
                        break;
                    case Statement._Keys.Arg1:
                        _arg1 = str;
                        break;
                    case Statement._Keys.Arg2:
                        _arg2 = str;
                        break;
                }
            }
        }

        public void AddAnswerToHistory(string[] encoding)
        {
            foreach (var str in encoding)
            {
                switch (Statement.GetKey(str))  // sprawdzenie czy klient chce sie rozlaczyc
                {
                    case Statement._Keys.Arg1:
                        _answer = Statement.GetValue(str);
                        _answer = "Arg3: " + _answer;   // zamiana arg1 na arg3
                        break;
                }
            }
            
            _history.AddNewStatement(ID, CID, _arg1 + _op + _arg2 + _answer);
            _op = _arg1 = _arg2 = _answer = null;


        }
    }
}