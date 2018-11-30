using System;
using System.Collections.Generic;
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

            if (Statement.GetKey(UserInput.ToUpper()) == "HISTORIAID: ")
            {
                str[0] = Statement._Keys.PHID;
                str[1] = Statement.GetValue(UserInput.ToUpper());
                return str;
            }
            else if (Statement.GetKey(UserInput.ToUpper()) == "HISTORIACID: ")
            {
                str[0] = Statement._Keys.PHCID;
                str[1] = Statement.GetValue(UserInput.ToUpper());
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
            Statement st = new Statement(); // zadanie przydzielenia IDsesji
            List<byte[]> bufferList = new List<byte[]>();
            string charbuff;
            try
            {
                bufferList = client.GetIDRequest(); // utworzenie zapytania o przydzial ID
                client.Write(bufferList); //  wyslanie zapytania o przydzial ID
                client.Read(out charbuff);  // odczytanie odpowiedzi
                st = new Statement(charbuff);
                client.ChangeID(st.Encoding()); // deserializacja komunikatu
                Console.WriteLine("Przydzielony numer sesji: {0} ", client.ID);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Source + " Exception");
                Console.WriteLine(e.Message);
            }

            while (true) // petla glowna
            {
                try
                {
                    Console.WriteLine("Proszę wpisać dzialanie matematyczne / historiaID: / historiaCID: ");
                    string[] UserInput = ReadUserInput();
                    if (UserInput[0] == "exit") // zakonczenie polaczenia wyslaniem odpowiedniego komunikatu
                    {
                        st = new Statement(client.ID, "exit");
                        bufferList = st.CreateBuffer(0);
                        client.Write(bufferList); //wyslanie listy  komunikatow
                        break;
                    }

                    st = new Statement(UserInput, client.ID, ref client.CID); //utworzenie nowego komunikatu
                    bufferList = st.CreateBuffer(0);    // podzial komunikatu na czesci
                    client.Write(bufferList); //wyslanie listy  komunikatow

                    client.Read(out charbuff);  // odczytanie komunikatu
                    if (UserInput[0] == Statement._Keys.PHID || UserInput[0] == Statement._Keys.PHCID)  // jesli otrzymano historie
                    {
                        Console.WriteLine(client.ReadHistory(charbuff));    // odczytanie wszytkich wpisow
                    }
                    else
                    {
                        Console.WriteLine("CID: {0} Odpowiedz: {1}",client.CID ,client.ReadAnswer(charbuff));  // odczytanie odpowiedzi
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