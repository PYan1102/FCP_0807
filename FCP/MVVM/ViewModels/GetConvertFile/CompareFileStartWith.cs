using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using FCP.MVVM.Models.Enum;
using FCP.MVVM.Models;
using FCP.MVVM.Factory.ViewModels;

namespace FCP.MVVM.ViewModels.GetConvertFile
{
    public class CompareFileStartWith
    {
        public Dictionary<Parameter, DepartmentEnum> DepartmentDictionary { get; set; }
        private DepartmentEnum _Department { get; set; }

        public DepartmentEnum GetDepartment()
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
