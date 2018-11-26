using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Text_Client_Server
{
    internal class Element
    {
        public string CalcID;
        public string sessionID;
    }

    internal class History
    {
        private Dictionary<Element, string> _memory;

        public History()
        {
            _memory = new Dictionary<Element, string>(100);
        }

        public void AddNewStatement(string id, string idCalc, string data)
        {
            Element e = new Element();
            e.CalcID = idCalc;
            e.sessionID = id;
            _memory[e] = data;
        }

        public void DisplayMemoryByID(string id)
        {
            foreach (var c in _memory)
            {
                if (c.Key.sessionID == id)
                {
                    Console.WriteLine("Id obliczeń: " + c.Key.sessionID);
                    Console.WriteLine("Dane: ");
                    Console.WriteLine(c.Value);
                }
            }
        }

        public void DisplayMemoryByCalc(string idCalc)
        {
            foreach (var c in _memory)
            {
                if (c.Key.CalcID == idCalc)
                {
                    Console.WriteLine("Id obliczeń: " + c.Key.CalcID);
                    Console.WriteLine("Dane: ");
                    Console.WriteLine(c.Value);
                }
            }
        }
    }
}