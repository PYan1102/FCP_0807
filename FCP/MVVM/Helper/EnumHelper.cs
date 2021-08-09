using System;
using System.Collections.Generic;
using System.Linq;
using FCP.MVVM.Models.Enum;

namespace FCP.MVVM.Helper
{
    static class EnumHelper
    {
        public static List<string> ToList<T>()
        {
            return Enum.GetValues(typeof(T))
                .Cast<T>()
                .Select(x => x.ToString())
                .ToList();
        }
    }
}
