using System;
using System.Collections.Generic;

namespace Text_Client_Server
{
    internal static class ServerTest
    {
        private static void Main(string[] args)
        {
            Server server = new Server(27015); //  tworzenie gniazda na danym porcie
            while (true)
            {
                Statement st = new Statement();
                string charbuff;
                List<byte[]> bufferList;
                Console.WriteLine("Waiting");
                server.Read(out charbuff); // oczekiwanie na IDSesji
                st = new Statement(charbuff);
                Console.WriteLine(st.ReadStatement());
                bufferList = server.CheckIdRequest(st.Encoding()); // ustalenie IDsesji
                Console.WriteLine("Przydzielono ID {0}", server.ID);
                server.Write(bufferList); // wyslanie odpowiedzi
                try
                {
                    byte[] buffer = new byte[1024];
                    while (true)
                    {
                        server.Read(out charbuff);
                        Console.WriteLine("".PadLeft(50, '*'));
                        Console.WriteLine("Odebrane:");
                        st = new Statement(charbuff);
                        Console.WriteLine(st.ReadStatement());
                        if (server.CheckExit(st.Encoding()) == true)
                        {
                            Console.WriteLine("Klient sie rozlaczyl");
                            break;
                        }
                            

                        st.CreateAnswer(server.Calculate(st.Encoding())); // obliczanie zadania
                        bufferList = st.CreateBuffer(); // podzielenie komunikatu na czesci
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

            }
            Console.ReadKey();
        }
    }
}