using System;

namespace Text_Client_Server
{
    internal static class ServerTest
    {
        private static void Main(string[] args)
        {
            Server server = new Server(27015); //  tworzenie gniazda na danym porcie
            Statement st = new Statement();
            double answer = 0;
            try
            {
                byte[] buffer = new byte[1024];
                int[] BufferLenght = new int[4];
                Console.WriteLine("Waiting");
                string error;
                while (true)
                {
                    server.Read(ref buffer);
                    st = new Statement(BufferUtilites.BufferToString(buffer, server._ReceivedData));
                    Console.WriteLine("".PadLeft(50, '*'));
                    Console.WriteLine("Odebrane:");
                    Console.WriteLine(st.ReadStatement());  // wyswietlenie komunikatu
                    if (server.CheckIdRequest(st.Encoding()))   // sprawdzenie czy nie trzeba skonfigurowac klienta
                    {
                        server.ID++;    // zwiekszenie id o 1
                        buffer = st.CreateBuffer(out BufferLenght, answer, Statement._ERR.NoError, server.ID);  // przyznanie id
                    }
                    else
                    {
                        answer = server.Calculate(st.Encoding(), out error);    // obliczenie zadania
                        buffer = st.CreateBuffer(out BufferLenght, answer, error, server.ID);   // utworzenie odpowiedzi
                        Console.WriteLine(BufferUtilites.BufferToString(buffer, buffer.Length));

                    }

                    Console.WriteLine("Wyslane");
                    st = new Statement(BufferUtilites.BufferToString(buffer, buffer.Length));
                    Console.WriteLine(st.ReadStatement());
                    Console.WriteLine("".PadLeft(50, '*'));
                    server.Write(buffer, BufferLenght); // wyslanie odpowiedzi
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