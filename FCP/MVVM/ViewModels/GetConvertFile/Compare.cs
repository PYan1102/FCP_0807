using FCP.MVVM.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCP.MVVM.ViewModels.GetConvertFile
{
    abstract public class Compare : DefaultRules
    {
        protected internal Dictionary<Parameter, DepartmentEnum> DepartmentDictionary { get; set; }

        protected internal virtual void ResetDepartmentDictionary()
        {
            DepartmentDictionary = GetDepartmentDictionary();
        }

        public abstract bool IsFileCompareSuccess(string fullFilePath);
        public abstract DepartmentEnum GetDepartment();
    }
}
