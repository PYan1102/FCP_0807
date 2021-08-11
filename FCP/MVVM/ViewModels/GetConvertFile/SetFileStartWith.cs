using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FCP.MVVM.Models.Enum;

namespace FCP.MVVM.ViewModels.GetConvertFile
{
    public class SetFileStartWith
    {
        public Dictionary<Parameter, DepartmentEnum> DepartmentDictionary = new Dictionary<Parameter, DepartmentEnum>();
        private Parameter _OPD { get; set; } = new Parameter();
        private Parameter _Powder { get; set; } = new Parameter();
        private Parameter _UDBatch { get; set; } = new Parameter();
        private Parameter _UDStat { get; set; } = new Parameter();
        private Parameter _Other { get; set; } = new Parameter();
        private Parameter _Care { get; set; } = new Parameter();
        private string _OPDTemp { get; set; }
        private string _PowderTemp { get; set; }
        private string _UDBatchTemp { get; set; }
        private string _UDStatTemp { get; set; }
        private string _OtherTemp { get; set; }
        private string _CareTemp { get; set; }

        public SetFileStartWith()
        {
            DepartmentDictionary.Add(_OPD, DepartmentEnum.OPD);
            DepartmentDictionary.Add(_Powder, DepartmentEnum.POWDER);
            DepartmentDictionary.Add(_UDBatch, DepartmentEnum.UDBatch);
            DepartmentDictionary.Add(_UDStat, DepartmentEnum.UDStat);
            DepartmentDictionary.Add(_Other, DepartmentEnum.Other);
            DepartmentDictionary.Add(_Care, DepartmentEnum.Care);
        }

        protected internal void ResetDictionary()
        {
            _OPD.StartWith = string.Empty;
            _Powder.StartWith = string.Empty;
            _UDBatch.StartWith = string.Empty;
            _UDStat.StartWith = string.Empty;
            _Other.StartWith = string.Empty;
            _Care.StartWith = string.Empty;
        }

        protected internal void SetOPD(string startWith)
        {
            _OPDTemp = startWith;
        }

        protected internal void SetPowder(string startWith)
        {
            _PowderTemp = startWith;
        }

        protected internal void SetUDBatch(string startWith)
        {
            _UDBatchTemp = startWith;
        }

        protected internal void SetUDStat(string startWith)
        {
            _UDStatTemp = startWith;
        }

        protected internal void SetOther(string startWith)
        {
            _OtherTemp = startWith;
        }

        protected internal void SetCare(string startWith)
        {
            _CareTemp = startWith;
        }

        protected internal void SetOPDIntoDictionary()
        {
            _OPD.StartWith = _OPDTemp;
        }

        protected internal void SetPowderIntoDictionary()
        {
            _Powder.StartWith = _PowderTemp;
        }

        protected internal void SetUDBatchIntoDictionary()
        {
            _UDBatch.StartWith = _UDBatchTemp;
        }

        protected internal void SetUDStatIntoDictionary()
        {
            _UDStat.StartWith = _UDStatTemp;
        }

        protected internal void SetOtherIntoDictionary()
        {
            _Other.StartWith = _OtherTemp;
        }

        protected internal void SetCareIntoDictionary()
        {
            _Care.StartWith = _CareTemp;
        }
    }

    public class Parameter
    {
        public string StartWith { get; set; } = string.Empty;
    }
}
