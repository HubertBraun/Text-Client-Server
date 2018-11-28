using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


/*
 *  Struktura komunikatu:
 *
 *  KOMUNIKAT KLIENT -> SERWER
 *
 *  Operacja: OP            Status: ST             NumerSekwencyjny: NS      IdSesji: ID         // pierwszy komunikat
 *  IdObliczen: CID         FlagaSilni: FS         Liczba1: ARG1             Liczba2: ARG2       // drugi komunikat
 *  LiczbaKomunikatow: LK   Historia: PS           Czas: sek:ms:ns                               // trzeci komunikat
 *
 * KOMUNIKAT SERWER -> KLIENT
 *
 *  Operacja: OP            Status: ST             NumerSekwencyjny: NS      IdSesji: ID         // pierwszy komunikat
 *  IdObliczen: CID         FlagaSilni: FS         Liczba1: ARG1             Liczba2/Blad: ARG2  // drugi komunikat
 *  LiczbaKomunikatow: LK   Historia: PS           ARG3: ARG3                Czas: sek:ms:ns     // trzeci komunikat
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
            PH = "Historia: ",
            LK = "LiczbaKomunikatow: "; // przyjmuje wartosci od 1-9

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
            internal const string FS = "FlagaSilni: ";
            internal const string ERR = "Blad: ";
            internal const string PH = "Historia: ";
            internal const string LK = "LiczbaKomunikatow: "; // przyjmuje wartosci od 1-9

        }

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
            internal const string NoError = "Blad: brakbledu";
            internal const string NotAllowed = "Blad: dzielenieprzezzero";
            internal const string OverFlow = "Blad: przepelnienie";
            internal const string Factorial = "Blad: silniazliczbyujemnej";
            internal const string Other = "Blad: nierozpoznanyblad";
        }

        internal struct _PH // pole historii
        {
            internal const string No = "Historia: pomin";
            internal const string ID = "Historia: przezidsesji";
            internal const string CID = "Historia: przezidobliczen";
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
                temp[i] = GetValue(temp[i]);
            }

            OP += temp[0]; // operacja
            ST += temp[1]; // status
            NS += temp[2]; // numer sekwencyjny
            ID += temp[3]; // id sesji
            CID += temp[4]; // id obliczen
            FS += temp[5]; // flaga silni
            Arg1 += temp[6]; // argument1
            Arg2 += temp[7]; // argument2
            //History += temp[7]; // pole historii  //TODO
            Time += temp[8]; // czas
        }


        public Statement(string[] Arguments, int ns, int id,int cid) // argumenty, numer sekwencyjny, id sesji, id obliczen
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

        private void CreateCharbuff()
        {
            string[] str = new string[9];
            str[0] = OP;
            str[1] = ST;
            str[2] = NS;
            str[3] = ID;
            str[4] = CID;
            str[5] = FS;
            str[6] = Arg1;
            str[7] = Arg2;
            str[8] = Time;
            for (int i = 0; i < str.Length; i++)
            {
                _charbuffer += str[i];
            }
        }


        public byte[] CreateBuffer(out int[] BufferLenght, double answer, string err, int id)
        {
            NS = GetValue(NS);
            NS = _Keys.NS + Convert.ToInt32(NS + 1);    // zwiekszenie numeru sekwencyjnego o 1
            Arg1 = _Keys.Arg1 + answer.ToString().ToLower();    // zamiana w przypadku notacji wykladniczej
            Time = _Keys.Time + "0:0:0";
            Arg2 = err;
            ID = _Keys.ID + id;
            CreateCharbuff();

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

        public byte[] CreateBuffer(out int[] BufferLenght)
        {
            CreateCharbuff();

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

        public static string GetValue(string str)
        {
            return Regex.Replace(str, "[A-Z]\\S+: ", "");
        }

        public string[] Encoding()
        {
            return Encoding(_charbuffer);
        }
        public static string[] Encoding(string charbuffer)
        {
            Regex reg = new Regex("([A-Z]\\S+): ([a-z]*-?[0-@]*[,]?[0-@]*[e]?[-|+]?[0-@]*)");
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