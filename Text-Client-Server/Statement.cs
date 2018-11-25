using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


/*
 *  Struktura komunikatu:
 *
 *  KOMUNIKATY KLIENT -> SERWER
 *
 *  Standardowy komunikat
 *  Operacja: OP               Status: ST                   NumerSekwencyjny: NS     Czas: sek:ms:ns    // pierwszy komunikat
 *  IdentyfikatorSesji: ID     IdentyfikatorObliczen: CID    PoleSilni: FS           Czas: sek:ms:ns    // drugi komunikat
 *  Liczba1: ARG1              Liczba2: ARG2                                         Czas: sek:ms:ns    // trzeci komunikat
 *
 *  Prosba o podanie historii
 *  Operacja: OP               Status: ST                   NumerSekwencyjny: NS     Czas: sek:ms:ns    // pierwszy komunikat
 *  IdentyfikatorSesji: ID     IdentyfikatorObliczen: CID   Historia: HS             Czas: sek:ms:ns    // drugi komunikat
 *  
 *
 * KOMUNIKATY SERWER -> KLIENT
 *
 * Standardowy komunikat
 * Operacja: OP               Status: ST                   NumerSekwencyjny: NS     Czas: sek:ms:ns    // pierwszy komunikat
 * IdentyfikatorSesji: ID     IdentyfikatorObliczen: CID    PoleSilni: FS           Czas: sek:ms:ns    // drugi komunikat
 * Liczba1: ARG1              Blad: B                                               Czas: sek:ms:ns    // trzeci komunikat
 *
 * Wyslanie historii  
 * Standardowy komunikat
 * Operacja: OP               Status: ST                   NumerSekwencyjny: NS     Czas: sek:ms:ns    // pierwszy komunikat
 * IdentyfikatorSesji: ID     IdentyfikatorObliczen: CID   PoleSilni: FS            Czas: sek:ms:ns    // drugi komunikat
 * Liczba1: ARG1              Liczba2: ARG2                Liczba3: ARG3            Czas: sek:ms:ns    // trzeci komunikat
 *
 * Operacja z bledem
 * Operacja: OP               Status: ST                   NumerSekwencyjny: NS     Czas: sek:ms:ns    // pierwszy komunikat
 * IdentyfikatorSesji: ID     IdentyfikatorObliczen: CID   PoleSilni: FS            Czas: sek:ms:ns    // drugi komunikat
 * Liczba1: ARG1              Liczba2: ARG2                Blad: B                  Czas: sek:ms:ns    // trzeci komunikat
 *
 */

namespace Text_Client_Server
{
    internal class Statement
    {
        private string _charbuffer;

        private string
            Time = "Czas: ",
            Arg1 = "Arg1: ", 
            Arg2 = "Arg2: ", 
            OP = "Operacja: ", 
            ST = "Status: ", 
            NS = "NumerSekwencyjny: ", 
            ID = "IdentyfikatorSesji: ", 
            CID = "IdentyfikatorObliczen: ",
            FS = "FlagaSilni: "; 

        #region structs

        internal struct _OP // pole operacji
        {
            internal static readonly string Mul = "Operacja: Mnozenie";
            internal static readonly string Div = "Operacja: Dzielenie";
            internal static readonly string Sub = "Operacja: Odejmowanie";
            internal static readonly string Exp = "Operacja: Potegowanie";
        }

        internal struct _ST // pole statusu
        {
            internal static readonly string Autorized = "Status: Autoryzacja";
            internal static readonly string Error = "Status: Blad";
            internal static readonly string ID = "Status: NieprawildowyIdentyfikator";
        }

        internal struct _FS // pole silnii
        {
            internal static readonly string Yes = "Silnia: Tak";
            internal static readonly string No = "Silnia: Nie";
        }

        internal struct _ERR // pole bledow
        {
            internal static readonly string NotAllowed = "Status: DzieleniePrzezZero";
            internal static readonly string OverFlow = "Status: Przepelnienie";
            internal static readonly string Factorial = "Status: SilniaZLiczbyUjemnej";
            internal static readonly string Other = "Status: NierozpoznaneDzialanie";
        }

        #endregion

        public Statement()
        {
            throw new NotImplementedException("Nie zaimplementowana metoda - Statement");
        }

        public Statement(string buffer)
        {
            _charbuffer = buffer;
        }


        public Statement(string[] Arguments, int ns, int id, int cid)     // argumenty, numer sekwencyjny, identyfikator sesji, identyfikator obliczen
        {
            Arg1 += Arguments[0];    // pierwsza liczba
            FS += _FS.No;    // ustawienie flagi silni na nie

            switch (Arguments[1])   // rozpoznawanie operacji matematycznej
            {
                case "*":
                    Arg2 += Arguments[2];
                    OP = _OP.Mul;
                    break;

                case "/":
                    Arg2 += Arguments[2];
                    OP = _OP.Div;
                    break;

                case "-":
                    Arg2 += Arguments[2];
                    OP = _OP.Sub;
                    break;

                case "^":
                    Arg2 += Arguments[2];
                    OP = _OP.Exp;

                    break;
                case "!":
                    OP = _OP.Mul;   // w przypadku silni operacja nie ma znaczenia
                    FS = _FS.Yes;   // ustawienie flagi silni na tak
                    break;

                default:
                    throw new ArgumentException("Nierozpoznana operacja matematyczna");
                    break;


            }


            NS += ns.ToString();    // przypisanie numeru sekwencyjnego
            ID += id.ToString();    // przypisanie identyfikatora sesji
            CID += cid.ToString();  // przypisanie identyfikatora obliczen
            ST = _ST.Autorized;     // do testow
            Time += "0:0:0";        // do testow

        }

        private void BufferCopy(Array src, Array dst, int dstOffset)
        {
            Buffer.BlockCopy(src, 0, dst, dstOffset, src.Length);
        }

        public byte[] CreateBuffer()  // operacja, status, numer sekwencyjny, identyfikator sesji, identyfikator obliczen
        {
            string[] temp = new string[9];
            temp[0] = OP;
            temp[1] = ST;
            temp[2] = NS;
            temp[3] = ID;
            temp[4] = CID;
            temp[5] = FS;
            temp[6] = Arg1;
            temp[7] = Arg2;
            temp[8] = Time;
            for (int i = 0; i < temp.Length; i++)
            {
                _charbuffer += temp[i];
            }


            byte[] toReturn = new byte[(OP.Length + ST.Length + NS.Length + ID.Length
                                       + CID.Length + FS.Length + Arg1.Length + Arg2.Length + Time.Length)*2];

            int lenght = 0;

            BufferCopy(OP.ToBuffer(), toReturn, lenght);
            lenght += OP.ToBuffer().Length;

            BufferCopy(ST.ToBuffer(), toReturn, lenght);
            lenght += ST.ToBuffer().Length;

            BufferCopy(NS.ToBuffer(), toReturn, lenght);
            lenght += NS.ToBuffer().Length;
            BufferCopy(ID.ToBuffer(), toReturn, lenght);
            lenght += ID.ToBuffer().Length;
            BufferCopy(CID.ToBuffer(), toReturn, lenght);
            lenght += CID.ToBuffer().Length;
            BufferCopy(FS.ToBuffer(), toReturn, lenght);
            lenght += FS.ToBuffer().Length;
            BufferCopy(Arg1.ToBuffer(), toReturn, lenght);
            lenght += Arg1.ToBuffer().Length;
            BufferCopy(Arg2.ToBuffer(), toReturn, lenght);
            lenght += Arg2.ToBuffer().Length;
            BufferCopy(Time.ToBuffer(), toReturn, lenght);
            return toReturn;
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


        public string[] Encoding()
        {
            Regex reg = new Regex("([A-Z]\\S+): ([a-z]*[0-@]*)");
            MatchCollection matches = reg.Matches(_charbuffer);
            string[] str = new string[matches.Count];

            for (int i = 0; i < matches.Count; i++) // wpisanie wyrazen do lancuchow znakow
                str[i] = matches[i].Value;

            return str;
        }
    }
}