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
 *  Operacja: OP         Status: ST             NumerSekwencyjny: NS     Czas: sek:ms:ns    // pierwszy komunikat
 *  IdSesji: ID          IdObliczen: CID        FlagaSilni: FS           Czas: sek:ms:ns    // drugi komunikat
 *  Liczba1: ARG1        Liczba2: ARG2          LiczbaKomunikatow: LK    Czas: sek:ms:ns    // trzeci komunikat
 *
 *  Prosba o podanie historii
 *  Operacja: OP         Status: ST             NumerSekwencyjny: NS     Czas: sek:ms:ns    // pierwszy komunikat
 *  IdSesji: ID          IdObliczen: CID        Historia: HS             Czas: sek:ms:ns    // drugi komunikat
 *  
 *
 * KOMUNIKATY SERWER -> KLIENT
 *
 * Standardowy komunikat
 * Operacja: OP          Status: ST             NumerSekwencyjny: NS     Czas: sek:ms:ns    // pierwszy komunikat
 * IdSesji: ID           IdObliczen: CID        FlagaSilni: FS           Czas: sek:ms:ns    // drugi komunikat
 * Liczba1: ARG1         Blad: B                                         Czas: sek:ms:ns    // trzeci komunikat
 *
 * Wyslanie historii  
 * Standardowy komunikat
 * Operacja: OP          Status: ST             NumerSekwencyjny: NS     Czas: sek:ms:ns    // pierwszy komunikat
 * IdSesji: ID           IdObliczen: CID        FlagaSilni: FS           Czas: sek:ms:ns    // drugi komunikat
 * Liczba1: ARG1         Liczba2: ARG2          Liczba3: ARG3            Czas: sek:ms:ns    // trzeci komunikat
 *
 * Operacja z bledem
 * Operacja: OP          Status: ST             NumerSekwencyjny: NS     Czas: sek:ms:ns    // pierwszy komunikat
 * IdSesji: ID           IdObliczen: CID        FlagaSilni: FS           Czas: sek:ms:ns    // drugi komunikat
 * Liczba1: ARG1         Liczba2: ARG2          Blad: B                  Czas: sek:ms:ns    // trzeci komunikat
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
            ID = "IDSesji: ",
            CID = "IDObliczen: ",
            FS = "FlagaSilni: ",
            LK = "LiczbaKomunikatow: "; // przyjmuje wartosci od 1-9

        #region structs

        internal struct _OP // pole operacji
        {
            internal const string Mul = "Operacja: mnozenie";
            internal const string Div = "Operacja: dzielenie";
            internal const string Sub = "Operacja: odejmowanie";
            internal const string Exp = "Operacja: potegowanie";
        }

        internal struct _ST // pole statusu
        {
            internal const string Autorized = "Status: autoryzacja";
            internal const string Error = "Status: blad";
            internal const string ID = "Status: nieprawildowyidentyfikator";
        }

        internal struct _FS // pole silnii
        {
            internal const string Yes = "Silnia: tak";
            internal const string No = "Silnia: nie";
        }

        internal struct _ERR // pole bledow
        {
            internal const string NotAllowed = "Status: dzielenieprzezzero";
            internal const string OverFlow = "Status: przepelnienie";
            internal const string Factorial = "Status: silniazliczbyujemnej";
            internal const string Other = "Status: nierozpoznanedzialanie";
        }

        #endregion

        public Statement()
        {
        }

        public Statement(string buffer)
        {
            _charbuffer = buffer;
            string[] temp = Encoding();
            for (int i = 0; i < temp.Length; i++)
            {
                temp[i] = Regex.Replace(temp[i], "[A-Z]\\S+: ", "");
            }

            OP += temp[0]; // operacja
            ST += temp[1]; // status
            NS += temp[2]; // numer sekwencyjny
            ID += temp[3]; // id sesji
            CID += temp[4]; // id obliczen
            FS += temp[5]; // flaga silni
            Arg1 += temp[6]; // argument1
            Arg2 += temp[7]; // argument2
            Time += temp[8]; // czas
        }


        public Statement(string[] Arguments, int ns, int id,
            int cid) // argumenty, numer sekwencyjny, id sesji, id obliczen
        {
            Arg1 += Arguments[0]; // pierwsza liczba
            FS = _FS.No; // ustawienie flagi silni na nie

            switch (Arguments[1]) // rozpoznawanie operacji matematycznej
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
                    OP = _OP.Mul; // w przypadku silni operacja nie ma znaczenia
                    FS = _FS.Yes; // ustawienie flagi silni na tak
                    break;

                default:
                    throw new ArgumentException("Nierozpoznana operacja matematyczna");
                    break;
            }


            NS += ns.ToString(); // przypisanie numeru sekwencyjnego
            ID += id.ToString(); // przypisanie identyfikatora sesji
            CID += cid.ToString(); // przypisanie identyfikatora obliczen
            ST = _ST.Autorized; // do testow
            Time += "0:0:0"; // do testow
        }

        private void BufferCopy(Array src, Array dst, int dstOffset)
        {
            Buffer.BlockCopy(src, 0, dst, dstOffset, src.Length);
        }

        public byte[] CreateBuffer(out int[] BufferLenght)
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


            byte[] Buffer = new byte[(OP.Length + ST.Length + NS.Length + ID.Length
                                      + CID.Length + FS.Length + Arg1.Length
                                      + Arg2.Length + Time.Length + LK.Length + 1) * 2];

            BufferLenght = new int[3];
            int lenght = 0;
            // pierwszy blok danych
            BufferCopy(OP.ToBuffer(), Buffer, lenght);
            lenght += OP.ToBuffer().Length;
            BufferCopy(ST.ToBuffer(), Buffer, lenght);
            lenght += ST.ToBuffer().Length;
            BufferCopy(NS.ToBuffer(), Buffer, lenght);
            lenght += NS.ToBuffer().Length;
            BufferCopy(ID.ToBuffer(), Buffer, lenght);
            lenght += ID.ToBuffer().Length;
            BufferLenght[0] = lenght;
            // drugi blok danych
            BufferCopy(CID.ToBuffer(), Buffer, lenght);
            lenght += CID.ToBuffer().Length;
            BufferCopy(FS.ToBuffer(), Buffer, lenght);
            lenght += FS.ToBuffer().Length;
            BufferCopy(Arg1.ToBuffer(), Buffer, lenght);
            lenght += Arg1.ToBuffer().Length;
            BufferCopy(Arg2.ToBuffer(), Buffer, lenght);
            lenght += Arg2.ToBuffer().Length;
            BufferLenght[1] = lenght - BufferLenght[0];
            // trzeci blok danych
            BufferCopy(Time.ToBuffer(), Buffer, lenght);
            lenght += Time.ToBuffer().Length;
            BufferCopy((LK + "3").ToBuffer(), Buffer, lenght);
            lenght += (LK + "3").ToBuffer().Length;
            BufferLenght[2] = lenght - BufferLenght[0] - BufferLenght[1];
            return Buffer; // zwraca bufor oraz dlugosc poszczegolnych sektorow
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
            Regex reg = new Regex("([A-Z]\\S+): ([a-z]*-?[0-@]*,?[0-@]*)");
            MatchCollection matches = reg.Matches(_charbuffer);
            string[] str = new string[matches.Count];
            for (int i = 0; i < matches.Count; i++) // wpisanie wyrazen do lancuchow znakow
            {
                str[i] = matches[i].Value;

            }

            return str;
        }
    }
}