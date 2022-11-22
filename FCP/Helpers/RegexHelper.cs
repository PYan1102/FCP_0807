using System.Text.RegularExpressions;

namespace FCP.Helpers
{
    public static class RegexHelper
    {
        public static string FilterSpecialSymbols(string value)
        {
            string result = Regex.Replace(value, "[][!\"#$%&'()*+,.:;<=>?@\\\\^_`{|}~]", " ");
            Regex.Replace(result, "[＆！＠＃＄％＊？]", "  ");
            return result;
        }
    }
}
