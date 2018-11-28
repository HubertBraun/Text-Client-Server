using System;
using System.Text.RegularExpressions;

namespace Text_Client_Server
{
    internal static class ClientTest
    {
        private static string[] ReadUserInput()
        {
            Regex reg = new Regex("(-?[0-9]+,?[0-9]*)\\s*?(\\D)\\s*?(-?[0-9]+,?[0-9]*)");
            string UserInput = Console.ReadLine(); // wczytanie danych do wyslania
            string[] str = new string[3];
            if (UserInput.ToLower() == "exit")
            {
                str[0] = "exit";
                return str;
            }

            if (UserInput.ToLower() == "history")   //TODO: przetestowac
            {
                str[0] = "history";
                return str;
            }

            Match m = reg.Match(UserInput);
            GroupCollection groups = m.Groups;
            if (m.Groups.Count == 4)
            {
                str = new string[3];
                str[0] = m.Groups[1].Value; // pierwsza liczba
                str[1] = m.Groups[2].Value; // operacja matematyczna
                str[2] = m.Groups[3].Value; // druga liczba
            }
            else
            {
                reg = new Regex("(-?[0-9]+,?[0-9]*)\\s*?(\\D)");
                m = reg.Match(UserInput);
                groups = m.Groups;
                str = new string[2];
                str[0] = m.Groups[1].Value; // pierwsza liczba
                str[1] = m.Groups[2].Value; // operacja matematyczna
            }

            return str;
        }

        private static void Main()
        {
            Client client = new Client(27015);
            try
            {
                byte[] buffer = new byte[1024]; 
                int[] BufferLenght;
                Statement st = client.GetIDRequest();       // zadanie przydzielenia IDsesji
                buffer = st.CreateBuffer(out BufferLenght); // wyslanie zadania
                client.Write(buffer, BufferLenght);
                client.Read(ref buffer);
                st = new Statement(BufferUtilites.BufferToString(buffer, buffer.Length));
                string[] temp = st.Encoding();
                client.ID = Convert.ToInt32(Statement.GetValue(temp[3]));
                Console.WriteLine("ID sesji: {0}", client.ID);
                while (true)
                {
                    Console.WriteLine("Proszę wpisać tekst");
                    string[] UserInput = ReadUserInput();
                    st = new Statement(UserInput, client.CID, client.ID, 0);
                    buffer = st.CreateBuffer(out BufferLenght);
                    client.Write(buffer, BufferLenght);
                    buffer = new byte[1024];
                    client.Read(ref buffer);
                    Console.WriteLine("Server: {0}", client.ReadAnswer(buffer));
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