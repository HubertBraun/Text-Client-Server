using System;
namespace Text_Client_Server
{
    internal class TestClass
    {
        private static void Main(string[] args)
        {
            while (true)
            {
                try
                {
                    string j = "zycie jest super";
                    string k = "kocham mame";
                    string l = "lubie cie slodziaku";
                    History h = new History();

                    h.AddNewStatement("10","Dodawanie",j);
                    h.AddNewStatement("11", "Mnozenie", k);
                    h.AddNewStatement("11", "Odejmowanie", l);
                    h.AddNewStatement("11", "Odejmowanie", k);

                    h.DisplayMemoryByID("11");
                   

                    Console.ReadLine();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}