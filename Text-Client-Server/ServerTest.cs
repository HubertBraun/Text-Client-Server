using System;

namespace Text_Client_Server
{
    internal static class ServerTest
    {
        private static void Main(string[] args)
        {
            Server server = new Server(27015); //  tworzenie gniazda na danym porcie
            try
            {
                byte[] buffer = new byte[1024];
                Console.WriteLine("Waiting");

                while (true)
                {
                    buffer = new byte[1024];
                    server.Read(ref buffer);
                    Console.WriteLine("Client: {0}", BufferUtilites.ReadBuffer(buffer, server._ReceivedData));
                    Array.Resize(ref buffer, server._ReceivedData);
                    server.Write(buffer); // wyslanie echa
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