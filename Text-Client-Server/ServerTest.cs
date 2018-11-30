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
                List<string> historyList;
                Console.WriteLine("Waiting");
                server.Read(out charbuff); // oczekiwanie na IDSesji
                st = new Statement(charbuff);
                Console.WriteLine(st.ReadStatement());  // wyswietlenie zadania
                bufferList = server.CheckIdRequest(st.Encoding()); // ustalenie id sesji
                Console.WriteLine("Przydzielono ID {0}", server.ID);    // wyswietlenie id sesji
                server.Write(bufferList); // wyslanie odpowiedzi

                try
                {
                    byte[] buffer = new byte[1024];
                    while (true)
                    {
                        server.Read(out charbuff);  // pobranie zadania
                        Console.WriteLine("".PadLeft(50, '*'));
                        Console.WriteLine("Odebrane:");
                        st = new Statement(charbuff);
                        Console.WriteLine(st.ReadStatement());  // wyswietlenie komunikatu

                        if (server.CheckHistory(st.Encoding(), out historyList))    // sprawdzenie czy zadanie dotyczy historii
                        {
                            Console.WriteLine("ID: {0}, CID {1}", server.ID, server.CID);
                            foreach (var s in historyList)  // wyswietlenie wpisow
                            {
                                Console.WriteLine(s);
                            }

                            server.Write(st.CreateHistoryAnswer(historyList));  
                            continue;
                        }

                        if (server.CheckExit(st.Encoding()))    // sprawdzenie czy klient zakonczyl polaczenie
                        {
                            Console.WriteLine("Klient sie rozlaczyl");
                            server.ClearHistory();
                            server.CID = -1; // zresetowanie id obliczen
                            break;
                        }

                        server.AddArgsToHistory(st.Encoding()); //dodanie argumantow i operacji do historii
                        st.CreateAnswer(server.Calculate(st.Encoding())); // obliczanie zadania
                        bufferList = st.CreateBuffer(0); // podzielenie komunikatu na czesci
                        st = new Statement(st.GetCharBuffer());

                        server.AddAnswerToHistory(st.Encoding()); // dodanie odpowiedzi do historii
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