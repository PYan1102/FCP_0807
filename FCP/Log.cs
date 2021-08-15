using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FCP
{
    public static class Log
    {
        private static string _LogPath = @"D:\FCP\LOG";
        private static string _CurrentDate { get; set; }

        public static void Write(string message)
        {
            Check();
            string detailTime = DateTime.Now.ToString("yyyy-MM-dd H:mm:ss.ffff");
            File.AppendAllText($@"{_LogPath}\{_CurrentDate}\Error_Log.txt", $"[{detailTime}] >> {message}\n\n");
        }

        private static void Check()
        {
            _CurrentDate = DateTime.Now.ToString("yyyy-MM-dd");
            if (!Directory.Exists($@"{_LogPath}\{_CurrentDate}"))
                Directory.CreateDirectory($@"{_LogPath}\{_CurrentDate}");
        }
    }
}
