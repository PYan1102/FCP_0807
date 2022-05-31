using FCP.src.Enum;
using System.Collections.Generic;
using System.Windows.Media;

namespace FCP.src.Dictionary
{
    internal class dColor
    {
        private static Dictionary<eColor, SolidColorBrush> _solidColorBrushDict = null;
        private static string _yellow = "#F4D03F";
        private static string _darkYellow = "#E1DB60";  //Warning
        private static string _blue = "#1D6FB1";
        private static string _darkBlue = "#11446D";
        private static string _royalBlue = "#0064D5";   //Information
        private static string _white = "#FFFFFF";
        private static string _red = "#FF5255";         //Error

        public static SolidColorBrush GetSolidColorBrush(eColor color)
        {
            if (_solidColorBrushDict == null)
            {
                InitSolidColorBrushDict();
            }
            try
            {
                _solidColorBrushDict.TryGetValue(color, out SolidColorBrush value);
                return value;
            }
            catch
            {
                throw;
            }
        }

        private static void InitSolidColorBrushDict()
        {
            _solidColorBrushDict = new Dictionary<eColor, SolidColorBrush>();
            _solidColorBrushDict.Add(eColor.Yellow, new SolidColorBrush((Color)ColorConverter.ConvertFromString(_yellow)));
            _solidColorBrushDict.Add(eColor.DarkYellow, new SolidColorBrush((Color)ColorConverter.ConvertFromString(_darkYellow)));
            _solidColorBrushDict.Add(eColor.Blue, new SolidColorBrush((Color)ColorConverter.ConvertFromString(_blue)));
            _solidColorBrushDict.Add(eColor.DarkBlue, new SolidColorBrush((Color)ColorConverter.ConvertFromString(_darkBlue)));
            _solidColorBrushDict.Add(eColor.RoyalBlue, new SolidColorBrush((Color)ColorConverter.ConvertFromString(_royalBlue)));
            _solidColorBrushDict.Add(eColor.White, new SolidColorBrush((Color)ColorConverter.ConvertFromString(_white)));
            _solidColorBrushDict.Add(eColor.Red, new SolidColorBrush((Color)ColorConverter.ConvertFromString(_red)));
        }
    }
}
