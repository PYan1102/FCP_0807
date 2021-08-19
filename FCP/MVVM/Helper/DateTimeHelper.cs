using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCP.MVVM.Helper
{
    static class DateTimeHelper
    {
        public static DateTime Convert(string dateTime, string format)
        {
            DateTime.TryParseExact(dateTime, format, null, System.Globalization.DateTimeStyles.None, out DateTime date);
            return date;
        }
    }
}
