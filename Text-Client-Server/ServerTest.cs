using System;
using System.Collections.Generic;

namespace Text_Client_Server
{
    internal static class ServerTest
    {
        private static void Main(string[] args)
        {
            Server server = new Server(27015); //  tworzenie gniazda na danym porcie
            Statement st = new Statement();
            double answer = 0;
            string charbuff;
            try
            {
                byte[] buffer = new byte[1024];
                List<byte[]> bufferList = new List<byte[]>();
                Console.WriteLine("Waiting");
                while (true)
                {
                    server.Read(out charbuff);
                    Console.WriteLine("".PadLeft(50, '*'));
                    Console.WriteLine("Odebrane:");
                    st = new Statement(charbuff);
                    Console.WriteLine(st.ReadStatement());
                    st.CreateAnswer(server.Calculate(st.Encoding()));   // obliczanie zadania
                    bufferList = st.CreateBuffer();     // podzielenie komunikatu na czesci
                    st = new Statement(st.GetCharBuffer());
                    Console.WriteLine("Wyslane");
                    Console.WriteLine("".PadLeft(50, '*'));
                    server.Write(bufferList); // wyslanie odpowiedzi
                    Console.WriteLine(st.ReadStatement());

                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Source + " Exception");
                Console.WriteLine(e.Message);
            }

            Console.ReadKey();
        }
    }
}