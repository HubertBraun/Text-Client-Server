using System;
using System.Net;
using System.Text.RegularExpressions;

namespace Text_Client_Server
{
    internal class Server : Host
    {
        public Server(int port) : base(port, IPAddress.Parse("192.168.43.209"))
        {
            _Socket.Bind(_IpEndPoint);
            CID = 1;
            ID = 0;
        }

        private int CalculateFactorial(int n)
        {
            if (n <= 1) return 1;
            else return checked(n * CalculateFactorial(n - 1));
        }

        public double Calculate(string[] charbuff, out string error)
        {
            double arg1 = Convert.ToDouble(charbuff[6] = Statement.GetValue(charbuff[6])); // pierwszy argument
            double toReturn = 0;
            error = Statement._ERR.NoError;
            try
            {
                Console.WriteLine("".PadLeft(25, '*'));
                if (charbuff[5] == Statement._FS.Yes) // silnia
                {
                    try
                    {
                        if (arg1 < 0)
                            throw new ArgumentException("Ujemny argument");
                        toReturn = CalculateFactorial(Convert.ToInt32(arg1));
                    }
                    catch (Exception e)
                    {
                        error = Statement._ERR.Factorial; // ustawienie kodu bledu
                        throw e;
                    }
                }
                else
                {
                    double arg2 = Convert.ToDouble(charbuff[7] = Regex.Replace(charbuff[7], "[A-Z]\\S+: ", ""));
                    switch (charbuff[0]) //operacja
                    {
                        case Statement._OP.Sub: // odejmowanie
                            toReturn = checked(arg1 - arg2);
                            Console.WriteLine("{0} - {1} = {2}", arg1, arg2, toReturn);
                            break;
                        case Statement._OP.Div: // dzielenie
                            try
                            {
                                if (arg2 == 0)
                                    throw new ArgumentException("Dzielenie przez zero");
                                toReturn = checked(arg1 / arg2);
                                Console.WriteLine("{0} / {1} = {2}", arg1, arg2, toReturn);
                            }
                            catch (Exception e)
                            {
                                error = Statement._ERR.NotAllowed; // ustawienie kodu bledu
                                throw e;
                            }

                            break;
                        case Statement._OP.Mul: // mnozenie
                            try
                            {
                                toReturn = checked(arg1 * arg2);
                                if (double.IsInfinity(toReturn))
                                    throw new ArgumentException("Przepelnienie!");
                                Console.WriteLine("{0} * {1} = {2}", arg1, arg2, toReturn);
                            }
                            catch (Exception e)
                            {
                                error = Statement._ERR.OverFlow; // ustawienie kodu bledu
                                throw e;
                            }

                            break;
                        case Statement._OP.Exp: // potegowanie
                            try
                            {
                                toReturn = Math.Pow(arg1, arg2);
                                if (double.IsInfinity(toReturn))
                                    throw new ArgumentException("Przepelnienie!");
                                Console.WriteLine("{0} ^ {1} = {2}", arg1, arg2, toReturn);
                            }
                            catch (Exception e)
                            {
                                error = Statement._ERR.OverFlow; // ustawienie kodu bledu
                                throw e;
                            }

                            break;
                        default:
                            throw new ArgumentException("Nierozpoznana operacja");
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Source + " Exception");
                Console.WriteLine(e.Message);
                toReturn = double.NaN; // przypisanie wyniku obliczen
            }

            Console.WriteLine("".PadLeft(25, '*'));
            return toReturn;
        }

        public bool CheckIdRequest(string[] charbuff)
        {
            if (Statement.GetValue(charbuff[3]) == "-1")    // umowny znak na brak id sesji
            {
                Console.WriteLine("Nowy klient!");
                return true;
            }
            else
                return false;
        }
    }
}