using System;
using System.Collections.Generic;
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

        private int CalculateFactorial(int n)
        {
            if (n <= 1) return 1;
            else return checked(n * CalculateFactorial(n - 1));
        }

        public string Calculate(string[] encoding)
        {
            string OP = "", answer = "";
            double Arg1 = 0, Arg2 = 0;
            foreach (var str in encoding)
            {
                switch (Statement.GetKey(str))  //sprawdzenie operacji oraz argumentow
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
                            answer = CalculateFactorial(Convert.ToInt32(Arg1)).ToString();
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
            answer = answer.ToLower();  //w przypadku notacji wykladniczej
            return answer;
        }

        public List<byte[]> CheckIdRequest(string[] encoding)
        {
            List<byte[]> toReturn = new List<byte[]>();
            foreach (var str in encoding)
            {
                switch (Statement.GetKey(str))  // sprawdzenie czy wyslano zapytanie o przydzial ID sesji
                {
                    case Statement._Keys.ST:
                        if (Statement.GetValue(str) == Statement._ST.Request)
                        {
                         ID++;
                         Statement st = new Statement(ID);
                         toReturn = st.CreateBuffer();
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
                switch (Statement.GetKey(str))  // sprawdzenie czy klient chce sie rozlaczyc
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
    }
}