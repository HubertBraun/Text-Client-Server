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

        public void Calculate(string[] charbuff)
        {
            double arg1 = Convert.ToDouble(charbuff[6] = Regex.Replace(charbuff[6], "[A-Z]\\S+: ", ""));
            double arg2 = Convert.ToDouble(charbuff[7] = Regex.Replace(charbuff[7], "[A-Z]\\S+: ", ""));

            switch (charbuff[0]) //operacja
            {
                case Statement._OP.Sub:
                    Console.WriteLine("{0} - {1} = {2}", arg1, arg2, arg1 - arg2);
                    break;
                case Statement._OP.Div:
                    Console.WriteLine("{0} / {1} = {2}", arg1, arg2, arg1 / arg2);
                    break;
                case Statement._OP.Mul:
                    Console.WriteLine("{0} * {1} = {2}", arg1, arg2, arg1 * arg2);
                    break;
                case Statement._OP.Exp:
                    //Console.WriteLine("{0} ^ {1} = {2}", arg1, arg2, Math.Pow(arg1 - arg2);
                    break;
                default:
                    throw new ArgumentException("Nierozpoznana operacja");
                    break;
            }
        }
    }
}