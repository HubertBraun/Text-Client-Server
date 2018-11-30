using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


/*
 *  Struktura komunikatu:
 *
 *  KOMUNIKAT:
 *  IdSesji: ID     NumerSekwencyjny: NS        Czas: sek:ms:ns    Dane do wyslania     // szablon komunikatu
 *
 *  Mozliwe dane:
 *  Operacja: OP
 *  Status: ST
 *  IdObliczen: CID
 *  Historia: PH    // opcjonalne
 *  Liczba1: ARG1   // opcjonalne
 *  Liczba2: ARG2   // opcjonalne
 *  Liczba3: ARG3   // opcjonalne
 *
 */

namespace Text_Client_Server
{
    internal class Statement
    {
        private string _charbuffer;

        private string
            Time = "",
            Arg1 = "",
            Arg2 = "",
            Arg3 = "",
            OP = "",
            ST = "",
            NS = "",
            ID = "",
            CID = "",
            PHID = "",
            PHCID = "";

        public Statement()
        {
        }

        #region structs

        internal struct _Keys //nazwy kluczy
        {
            internal const string Time = "Czas: ";
            internal const string Arg1 = "Arg1: ";
            internal const string Arg2 = "Arg2: ";
            internal const string Arg3 = "Arg3: ";
            internal const string OP = "Operacja: ";
            internal const string ST = "Status: ";
            internal const string NS = "NumerSekwencyjny: ";
            internal const string ID = "IDSesji: ";
            internal const string CID = "IDObliczen: ";
            internal const string PHID = "HistoriaID: ";
            internal const string PHCID = "HistoriaCID: ";

        }

        internal struct _OP // pole operacji
        {
            internal const string Mul = "mnozenie";
            internal const string Div = "dzielenie";
            internal const string Sub = "odejmowanie";
            internal const string Exp = "potegowanie";
            internal const string Fac = "silnia";
        }

        internal struct _ST // pole statusu
        {
            internal const string Autorized = "autoryzacja";
            internal const string Error = "blad";
            internal const string Request = "ustalanieidsesji";
            internal const string Exit = "konczeniepolaczenia";
            internal const string ID = "nieprawildowyidentyfikator";
            internal const string CID = "nieprawildowyidentyfikatorobliczen";
        }


        internal struct _ERR // pole bledow
        {
            internal const string NotAllowed = "dzielenieprzezzero";
            internal const string OverFlow = "przepelnienie";
            internal const string Factorial = "silniazliczbyujemnej";
            internal const string Other = "nierozpoznanyblad";
        }


        #endregion

        public Statement(string buffer)
        {
            string[] encoding = Encoding(buffer);

            foreach (var str in encoding)
            {
                switch (GetKey(str))
                {
                    case _Keys.ID:
                        ID = str;
                        break;
                    case _Keys.NS:
                        NS = str;
                        break;
                    case _Keys.Time:
                        Time = str;
                        break;
                    case _Keys.OP:
                        OP = str;
                        break;
                    case _Keys.ST:
                        ST = str;
                        break;
                    case _Keys.CID:
                        CID = str;
                        break;
                    case _Keys.PHID:
                        PHID = str;
                        break;
                    case _Keys.PHCID:
                        PHCID = str;
                        break;
                    case _Keys.Arg1:
                        Arg1 = str;
                        break;
                    case _Keys.Arg2:
                        Arg2 = str;
                        break;
                    case _Keys.Arg3:
                        Arg3 = str;
                        break;
                }
            }

            _charbuffer = ID + NS + Time + OP + ST + CID + PHID + PHCID + Arg1 + Arg2 + Arg3;
        }

        public Statement(int id) // zadanie przydzielenia id
        {
            ID = id.ToString();
            ST = _ST.Request;
            Time += GetTime();
        }

        public Statement(int id, string str) // zadanie przydzielenia id
        {
            if (str == "exit")
            {
                ID = id.ToString();
                ST = _ST.Exit;
                Time += GetTime();
            }
        }
        public Statement(string[] Arguments, int id, ref int cid) // argumenty, numer sekwencyjny, id sesji, id obliczen
        {
            if(Arguments[0] == _Keys.PHID)
            {
                Console.WriteLine("Arg1 {0}", Arguments[1]);
                PHID += Arguments[1];   //ID
                OP = "";
            }
            else if (Arguments[0] == _Keys.PHCID)
            {
                Console.WriteLine("Arg1 {0}", Arguments[1]);
                PHCID += Arguments[1];   //CID
                OP = "";
            }
            else
            {
                Arg1 += Arguments[0]; // pierwsza liczba
                switch (Arguments[1]) // rozpoznawanie operacji matematycznej
                {
                    case "*":
                        cid++;
                        CID += cid.ToString(); // przypisanie identyfikatora obliczen
                        Arg2 += Arguments[2];
                        OP = _OP.Mul;
                        break;

                    case "/":
                        cid++;
                        CID += cid.ToString(); // przypisanie identyfikatora obliczen
                        Arg2 += Arguments[2];
                        OP = _OP.Div;
                        break;

                    case "-":
                        cid++;
                        CID += cid.ToString(); // przypisanie identyfikatora obliczen
                        Arg2 += Arguments[2];
                        OP = _OP.Sub;
                        break;

                    case "^":
                        cid++;
                        CID += cid.ToString(); // przypisanie identyfikatora obliczen
                        Arg2 += Arguments[2];
                        OP = _OP.Exp;

                        break;
                    case "!":
                        cid++;
                        CID += cid.ToString(); // przypisanie identyfikatora obliczen
                        OP = _OP.Fac; // w przypadku silni operacja nie ma znaczenia
                        break;

                    default:
                        throw new ArgumentException("Nierozpoznana operacja matematyczna");
                        break;
                }

            }

            ID += id.ToString(); // przypisanie identyfikatora sesji
            ST = _ST.Autorized; // do testow
            Time += GetTime();
        }

        public static string GetTime()
        {
            return DateTime.UtcNow.ToString();
        }

        public void CreateAnswer(string arg1)
        {
            Arg1 = Arg2 = Arg3 = "";
            Arg1 = arg1;
            ID = GetValue(ID);
            NS = GetValue(NS);
            Time = GetValue(Time);
            OP = GetValue(OP);
            double temp;
            if (double.TryParse(arg1, out temp))
                ST = GetValue(ST);  // autoryzacja
            else
                ST = _ST.Error; // blad
            CID = GetValue(CID);
        }

        public List<byte[]> CreateHistoryAnswer(List<string> list)
        {
            Time = GetTime();
            Arg1 = ""; // zresetowanie argumentow
            Arg2 = "";
            Arg3 = "";
            OP = "";
            ST = _ST.Autorized; //TODO: do testow
            NS = "";

            Regex reg = new Regex("([A-Z]\\S+): (((?![A-Z]).)*)");
            int ns = 1; // numer ostatniego komunikatu
            List<byte[]> toReturn = new List<byte[]>(); //lista komunikatow
            foreach (var match in list)
            {
                MatchCollection matches = reg.Matches(match);
                for (int i = 0; i < matches.Count; i++) // wpisanie wyrazen do lancuchow znakow
                {
                    ns++; // obliczenie numeru sekwencyjnego
                    switch (GetKey(matches[i].Value)) // sprawdzenie czy klient chce sie rozlaczyc
                    {
                        case _Keys.OP:  // przy kazdej operacji dodaje status, na pozniejszym etapie
                            ns++;
                            break;
                    }
                }
            }

            foreach (var match in list)
            {
                MatchCollection matches = reg.Matches(match);
                string[] keys = new string[matches.Count];

                for (int i = 0; i < matches.Count; i++) // wpisanie wyrazen do lancuchow znakow
                {
                    switch (GetKey(matches[i].Value)) // sprawdzenie czy klient chce sie rozlaczyc
                    {
                        case _Keys.OP:
                            OP = GetValue(matches[i].Value);
                            if (GetValue(matches[i].Value) == _OP.Fac)  //  w przypadku silni
                                Arg2 = "";
                            break;
                        case _Keys.Arg1:
                            Arg1 = GetValue(matches[i].Value);
                            break;
                        case _Keys.Arg2:
                            Arg2 = GetValue(matches[i].Value);
                            break;
                        case _Keys.Arg3:
                            Arg3 = GetValue(matches[i].Value);
                            toReturn.AddRange(CreateBuffer(ns - toReturn.Count));
                            PHCID = "";
                            PHID = "";
                            break;
                    }
                }
            }

            return toReturn;
        }


        private void BufferCopy(Array src, Array dst, int dstOffset)
        {
            Buffer.BlockCopy(src, 0, dst, dstOffset, src.Length);
        }


        private byte[] CreateHeader()
        {
            int lenght = 0;
            byte[] header = new byte[(_Keys.ID + ID).Length + (_Keys.NS + NS).Length + (_Keys.Time + Time).Length];
            BufferCopy((_Keys.ID + ID).ToBuffer(), header, lenght);
            lenght += (_Keys.ID + ID).ToBuffer().Length;
            BufferCopy((_Keys.NS + NS).ToBuffer(), header, lenght);
            lenght += (_Keys.NS + NS).ToBuffer().Length;
            BufferCopy((_Keys.Time + Time).ToBuffer(), header, lenght);
            lenght += (_Keys.Time + Time).ToBuffer().Length;
            return header;
        }

        private int UniteData(string data, ref byte[] buffer)
        {
            byte[] header = CreateHeader();
            if (GetValue(data) != "")
            {
                buffer = new byte[header.Length + data.Length];
                BufferCopy(header, buffer, 0);
                BufferCopy(data.ToBuffer(), buffer, header.Length);
                NS = (Convert.ToUInt32(NS) - 1).ToString(); //zmniejszenie numeru sekwencyjnego
                return data.ToBuffer().Length;
            }
            else return 0;
        }

        public List<byte[]> CreateBuffer(int ns)
        {
            if (ns != 0)    // ustalone przez parametr
            {
                NS = ns.ToString();
            }
            else if (PHID != "" || PHCID !=  "")    // zapytanie o historie
            {
                NS = "2";
            }
            else if (GetValue(ST) == _ST.Request || GetValue(ST) == _ST.Exit)    // ustalanie IDsesji lub konczenie polaczenia
            {
                NS = "1";
            }
            else if (GetValue(OP) == _OP.Fac || Arg2 == "")  //silnia
            {
                NS = "4";
            }
            else    // zwykle dzialanie
            {
                NS = "5";
            }

            _charbuffer = "";
            byte[] header = CreateHeader();
            List<byte[]> bufferlist = new List<byte[]>();
            byte[] tempbuff = new byte[1];
            List<int> lenghts = new List<int>();

            int lenght = UniteData(_Keys.OP + OP, ref tempbuff);
            if (lenght != 0)
            {
                lenghts.Add(lenght);
                bufferlist.Add(tempbuff);
            }

            lenght = UniteData(_Keys.ST + ST, ref tempbuff);
            if (lenght != 0)
            {
                lenghts.Add(lenght);
                bufferlist.Add(tempbuff);

            }
            _charbuffer += _Keys.ID + ID;
            _charbuffer += _Keys.ST + ST;
            _charbuffer += _Keys.OP + OP;
            _charbuffer += _Keys.Time + Time;
            lenght = UniteData(_Keys.CID + CID, ref tempbuff);
            if (lenght != 0)
            {
                lenghts.Add(lenght);
                bufferlist.Add(tempbuff);
                _charbuffer += _Keys.CID + CID;

            }

            lenght = UniteData(_Keys.PHID + PHID, ref tempbuff);
            if (lenght != 0)
            {
                lenghts.Add(lenght);
                bufferlist.Add(tempbuff);
                _charbuffer += _Keys.PHID + PHID;

            }

            lenght = UniteData(_Keys.PHCID + PHCID, ref tempbuff);
            if (lenght != 0)
            {
                lenghts.Add(lenght);
                bufferlist.Add(tempbuff);
                _charbuffer += _Keys.PHCID + PHCID;

            }

            lenght = UniteData(_Keys.Arg1 + Arg1, ref tempbuff);
            if (lenght != 0)
            {
                lenghts.Add(lenght);
                bufferlist.Add(tempbuff);
                _charbuffer += _Keys.Arg1 + Arg1;

            }

            lenght = UniteData(_Keys.Arg2 + Arg2, ref tempbuff);
            if (lenght != 0)
            {
                lenghts.Add(lenght);
                bufferlist.Add(tempbuff);
                _charbuffer += _Keys.Arg2 + Arg2;

            }

            lenght = UniteData(_Keys.Arg3 + Arg3, ref tempbuff);
            if (lenght != 0)
            {
                lenghts.Add(lenght);
                bufferlist.Add(tempbuff);
                _charbuffer += _Keys.Arg3 + Arg3;

            }

            return bufferlist; // zwraca bufor oraz dlugosc poszczegolnych sektorow
        }

        public string GetCharBuffer()
        {
            return _charbuffer;
        }
        public string ReadStatement()
        {
            string[] temp = Encoding();
            StringBuilder StrBuilder = new StringBuilder();
            foreach (var str in temp)
            {
                StrBuilder.Append(str + "\n");
            }

            return StrBuilder.ToString();
        }

        public static string GetValue(string str)
        {
            return Regex.Replace(str, "[A-Z]\\S+: ", "");
        }

        public static string GetKey(string str)
        {
            Regex reg = new Regex("([A-Z]\\S+): ");
            Match matches = reg.Match(str);
            return matches.Value;
        }

        public string[] Encoding()
        {
            return Encoding(_charbuffer);
        }

        public static string[] Encoding(string charbuffer)
        {

            Regex reg = new Regex("([A-Z]\\S+): (((?![A-Z]).)*)");
            MatchCollection matches = reg.Matches(charbuffer);
            string[] str = new string[matches.Count];
            for (int i = 0; i < matches.Count; i++) // wpisanie wyrazen do lancuchow znakow
            {
                str[i] = matches[i].Value;
            }

            return str;
        }
    }
}