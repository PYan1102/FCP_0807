using System;
using System.Collections.Generic;
using FCP.MVVM.Models.Enum;

namespace FCP.MVVM.ViewModels.GetConvertFile
{
    interface ICompareFileStartWith
    {
        void ResetDeparmnentDictionary();

        DepartmentEnum Compare(string fileName);
    }
}
