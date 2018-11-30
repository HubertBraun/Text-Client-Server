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
        internal int CalcID;
        internal int sessionID;
    }

    internal class History
    {
        private Dictionary<Element, string> _memory;

        public History()
        {
            _memory = new Dictionary<Element, string>(); //poczatkowa wielkosc slownika
        }

        public void AddNewStatement(int ID, int CID, string data)
        {
            Element e = new Element();
            e.CalcID = CID;
            e.sessionID = ID;
            _memory.Add(e, data);
        }


        public List<string> GetGistoryByID(int ID)
        {
            List<string> toReturn = new List<string>();
            List<KeyValuePair<Element, string>> list = _memory.ToList();
            foreach (var p in list)
            {
                if (p.Key.sessionID == ID)
                {
                    toReturn.Add(p.Value);
                }
            }
            return toReturn;

        }

        public List<string> GetGistoryByCID(int CID)
        {
            List<string> toReturn = new List<string>();
            List<KeyValuePair<Element, string>> list = _memory.ToList();
            foreach (var p in list)
            {
                if (p.Key.CalcID == CID)
                    toReturn.Add(p.Value);
            }
            return toReturn;
        }

        public void ReadHistoryByID(int ID)
        {
           List<string> temp = GetGistoryByID(ID);
            foreach (var str in temp)
            {
                Console.WriteLine(str);
            }
        }

        public void ReadHistoryByCID(int CID)
        {
            List<string> temp = GetGistoryByCID(CID);
            foreach (var str in temp)
            {
                Console.WriteLine(str);
            }

        }


    }
}