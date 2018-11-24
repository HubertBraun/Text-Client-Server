using System;

namespace Text_Client_Server
{
    internal static class ClientTest
    {
        private static void Main()
        {
            Client client = new Client(27015);
            try
            {
                byte[] buffer = new byte[1024];
                while (true)
                {
                    Console.WriteLine("Proszę wpisać tekst");
                    string UserInput = Console.ReadLine();
                    buffer = UserInput.ToBuffer();
                    client.Write(buffer);
                    buffer = new byte[1024];
                    client.Read(ref buffer);
                    Console.WriteLine("Server: {0}", BufferUtilites.ReadBuffer(buffer, client._ReceivedData));
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