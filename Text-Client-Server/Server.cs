using System;
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

        public double Calculate(string[] charbuff, out string error)
        {

                double arg1 = Convert.ToDouble(charbuff[6] = Regex.Replace(charbuff[6], "[A-Z]\\S+: ", ""));
                double toReturn = 0;
            error = Statement._ERR.NoError;
            try
            {
                Console.WriteLine("".PadLeft(25, '*'));
                if (charbuff[5] == Statement._FS.Yes)
                {
                    //TODO: if arg1<0 throw exception
                    try
                    {
                        if (arg1 < 0)
                            throw new ArgumentException("Ujemny argument");
                        toReturn = CalculateFactorial(Convert.ToInt32(arg1));
                    }
                    catch (Exception e)
                    {
                        error = Statement._ERR.Factorial;
                        throw e;
                    }
                }
                else
                {
                    double arg2 = Convert.ToDouble(charbuff[7] = Regex.Replace(charbuff[7], "[A-Z]\\S+: ", ""));
                    switch (charbuff[0]) //operacja
                    {
                        case Statement._OP.Sub:
                            toReturn = checked(arg1 - arg2);
                            Console.WriteLine("{0} - {1} = {2}", arg1, arg2, toReturn);
                            break;
                        case Statement._OP.Div:
                            try
                            {
                                if(arg2 == 0)
                                    throw new ArgumentException("Dzielenie przez zero");
                                toReturn = checked(arg1 / arg2);
                                Console.WriteLine("{0} / {1} = {2}", arg1, arg2, toReturn);
                            }
                            catch (Exception e)
                            {
                                error = Statement._ERR.NotAllowed;
                                throw e;
                            }

                            break;
                        case Statement._OP.Mul:
                            try
                            {
                                toReturn = checked(arg1 * arg2);
                                if (double.IsInfinity(toReturn))
                                    throw new ArgumentException("Przepelnienie!");
                                Console.WriteLine("{0} * {1} = {2}", arg1, arg2, toReturn);
                            }
                            catch (Exception e)
                            {
                                error = Statement._ERR.OverFlow;
                                throw e;
                            }
                            break;
                        case Statement._OP.Exp:
                            try
                            {
                                toReturn = Math.Pow(arg1, arg2);
                                if(double.IsInfinity(toReturn))
                                    throw new ArgumentException("Przepelnienie!");
                                Console.WriteLine("{0} ^ {1} = {2}", arg1, arg2, toReturn);
                            }
                            catch (Exception e)
                            {
                                error = Statement._ERR.OverFlow;
                                throw e;
                            }

                            break;
                        default:
                            throw new ArgumentException("Nierozpoznana operacja");
                            break;
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Source + " Exception");
                Console.WriteLine(e.Message);
                toReturn = double.NaN;
            }

            Console.WriteLine("".PadLeft(25, '*'));
            return toReturn;

        }
    }
}