using System;

namespace Text_Client_Server
{
    internal static class ServerTest
    {
        private static void Main(string[] args)
        {
            Server server = new Server(27015); //  tworzenie gniazda na danym porcie
            Statement st = new Statement();
            try
            {
                byte[] buffer = new byte[1024];
                int[] BufferLenght;
                Console.WriteLine("Waiting");

                while (true)
                {
                    server.Read(ref buffer);
                    st = new Statement(BufferUtilites.BufferToString(buffer, server._ReceivedData));
                    buffer = st.CreateBuffer(out BufferLenght);
                    server.Calculate(st.Encoding());
                    //Console.WriteLine("Client: {0}", BufferUtilites.BufferToString(buffer, server._ReceivedData));
                    server.Write(buffer, BufferLenght); // wyslanie echa
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