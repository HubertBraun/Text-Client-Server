using System.Text;
using System.Text.RegularExpressions;

namespace Text_Client_Server
{
    internal static class BufferUtilites
    {
        public static string BufferToString(byte[] buffer, int ReceivedData) // zamiana buffora na string
        {
            return Encoding.ASCII.GetString(buffer, 0, ReceivedData);
        }

        public static byte[] ToBuffer(this string str) // zamiana lancucha znakow na buffor
        {
            return Encoding.ASCII.GetBytes(str);
        }
    }
}