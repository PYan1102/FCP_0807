using System;
using System.Text;

namespace FCP.MVVM.Helper
{
    static class EncodingHelper
    {
        public static int GetBytesLength { get => _Byte.Length; }
        private static byte[] _Byte { get; set; }
        public static void SetEncodingBytes(string str)
        {
            _Byte = Encoding.Default.GetBytes(str);
        }
        public static string GetEncodingString(int start, int length)
        {
            return Encoding.Default.GetString(_Byte, start, length).Trim();
        }
    }
}
