using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using FCP.src.Enum;
using FCP.Models;
using FCP.src.Factory.ViewModel;

namespace FCP.ViewModels.GetConvertFile
{
    public class CompareFileStartWith
    {
        public Dictionary<Parameter, eConvertLocation> DepartmentDictionary { get; set; }
        private eConvertLocation _Department { get; set; }

        public eConvertLocation GetDepartment()
        {
            return _Department;
        }

        public bool IsFileCompareSuccess(string fullFilePath)
        {
            string fileName = Path.GetFileNameWithoutExtension(fullFilePath);
            foreach (var v in DepartmentDictionary)
            {
                if (v.Key.Rule == nameof(DefaultEnum.Default))
                {
                    _Department = v.Value;
                    return true;
                }
                if (v.Key.Rule == string.Empty)
                    continue;
                if (fileName.StartsWith(v.Key.Rule))
                {
                    _Department = v.Value;
                    return true;
                }
            }
            return false;
        }
    }
}
