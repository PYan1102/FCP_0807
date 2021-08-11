using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using FCP.MVVM.Models.Enum;
using FCP.MVVM.Models;
using FCP.MVVM.Factory.ViewModels;

namespace FCP.MVVM.ViewModels.GetConvertFile
{
    public class CompareFileStartWith : SetFileStartWith, ICompareFileStartWith
    {
        private Dictionary<Parameter, DepartmentEnum> _DepartmentDictionary { get; set; }
        private MainWindowModel _MainWindowModel { get; set; }

        public CompareFileStartWith()
        {
            _MainWindowModel = MainWindowFacotry.GenerateMainWindowModel();
        }

        public void ResetDeparmnentDictionary()
        {
            _DepartmentDictionary = DepartmentDictionary;
        }

        public DepartmentEnum Compare(string fullFilePath)
        {
            string fileName = Path.GetFileNameWithoutExtension(fullFilePath);
            foreach (var v in _DepartmentDictionary)
            {
                if (v.Key.StartWith == string.Empty)
                    continue;
                if (fileName.StartsWith(v.Key.StartWith))
                    return v.Value;
            }
            return _MainWindowModel.IsOPD ? DepartmentEnum.OPD : DepartmentEnum.UDStat;
        }
    }
}
