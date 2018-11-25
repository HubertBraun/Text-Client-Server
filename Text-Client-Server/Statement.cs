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
 *  IdentyfikatorSesji: ID     IdentyfikatorObliczen: ID    PoleSilni: FS            Czas: sek:ms:ns    // drugi komunikat
 *  Liczba1: ARG1              Liczba2: ARG2                                         Czas: sek:ms:ns    // trzeci komunikat
 *
 *  Prosba o podanie historii
 *  Operacja: OP               Status: ST                   NumerSekwencyjny: NS     Czas: sek:ms:ns    // pierwszy komunikat
 *  IdentyfikatorSesji: ID     IdentyfikatorObliczen: ID    Historia: HS             Czas: sek:ms:ns    // drugi komunikat
 *  
 *
 * KOMUNIKATY SERWER -> KLIENT
 *
 * Standardowy komunikat
 * Operacja: OP               Status: ST                   NumerSekwencyjny: NS     Czas: sek:ms:ns    // pierwszy komunikat
 * IdentyfikatorSesji: ID     IdentyfikatorObliczen: ID    PoleSilni: FS            Czas: sek:ms:ns    // drugi komunikat
 * Liczba1: ARG1              Blad: B                                               Czas: sek:ms:ns    // trzeci komunikat
 *
 * Wyslanie historii  
 * Standardowy komunikat
 * Operacja: OP               Status: ST                   NumerSekwencyjny: NS     Czas: sek:ms:ns    // pierwszy komunikat
 * IdentyfikatorSesji: ID     IdentyfikatorObliczen: ID    PoleSilni: FS            Czas: sek:ms:ns    // drugi komunikat
 * Liczba1: ARG1              Liczba2: ARG2                Liczba3: ARG3            Czas: sek:ms:ns    // trzeci komunikat
 *
 * Operacja z bledem
 * Operacja: OP               Status: ST                   NumerSekwencyjny: NS     Czas: sek:ms:ns    // pierwszy komunikat
 * IdentyfikatorSesji: ID     IdentyfikatorObliczen: ID    PoleSilni: FS            Czas: sek:ms:ns    // drugi komunikat
 * Liczba1: ARG1              Liczba2: ARG2                Blad: B                  Czas: sek:ms:ns    // trzeci komunikat
 *
 */

namespace Text_Client_Server
{
    internal class Statement
    {
        private string _charbuffer;

        #region structs

        internal struct OP // pole operacji
        {
            internal static readonly string Mul = "Operacja: Mnozenie";
            internal static readonly string Div = "Operacja: Dzielenie";
            internal static readonly string Sub = "Operacja: Odejmowanie";
            internal static readonly string Exp = "Operacja: Potegowanie";
        }

        internal struct ST // pole statusu
        {
            internal static readonly string Autorized = "Status: Autoryzacja";
            internal static readonly string Error = "Status: Blad";
            internal static readonly string ID = "Status: NieprawildowyIdentyfikator";
        }

        internal struct PS // pole silnii
        {
            internal static readonly string yes = "Silnia: Liczyc";
            internal static readonly string no = "Silnia: NieLiczyc";
        }

        internal struct ERR // pole bledow
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

        public void CreateBuffer()
        {
            throw new NotImplementedException("Nie zaimplementowana metoda - ReadStatement");
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