﻿using System;
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


        public List<string> DisplayMemoryByID(string id)
        {
            List<string> toReturn = new List<string>();
            foreach (var c in _memory)
            {
                if (c.Key.sessionID == id)
                {
                    toReturn.Add(c.Value);
                }
            }
            return toReturn;

        }

        public List<string> DisplayMemoryByCalc(string idCalc)
        {
            List<string> toReturn = new List<string>();
            foreach (var c in _memory)
            {
                if (c.Key.CalcID == idCalc)
                {
                    toReturn.Add(c.Value);
                }
            }

            return toReturn;
        }
    }
}