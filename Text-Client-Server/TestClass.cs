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
                    Console.ReadLine();
                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}