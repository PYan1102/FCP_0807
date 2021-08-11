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

        public SetFileStartWith()
        {
            DepartmentDictionary.Add(_OPD, DepartmentEnum.OPD);
            DepartmentDictionary.Add(_Powder, DepartmentEnum.POWDER);
            DepartmentDictionary.Add(_UDBatch, DepartmentEnum.UDBatch);
            DepartmentDictionary.Add(_UDStat, DepartmentEnum.UDStat);
            DepartmentDictionary.Add(_Other, DepartmentEnum.Other);
            DepartmentDictionary.Add(_Care, DepartmentEnum.Care);
        }

        protected internal void SetOPD(string startWith)
        {
            _OPD.StartWith = startWith;
        }

        protected internal void SetPowder(string startWith)
        {
            _Powder.StartWith = startWith;
        }

        protected internal void SetUDBatch(string startWith)
        {
            _UDBatch.StartWith = startWith;
        }

        protected internal void SetUDStat(string startWith)
        {
            _UDStat.StartWith = startWith;
        }

        protected internal void SetOther(string startWith)
        {
            _Other.StartWith = startWith;
        }

        protected internal void SetCare(string startWith)
        {
            _Care.StartWith = startWith;
        }

        protected internal Parameter GetOPD()
        {
            if (_OPD.StartWith == string.Empty)
                throw new Exception("OPD 為空值");
            return _OPD;
        }

        protected internal Parameter GetPowder()
        {
            if (_Powder.StartWith == string.Empty)
                throw new Exception("Powder 為空值");
            return _Powder;
        }

        protected internal Parameter GetUDBatch()
        {
            if (_UDBatch.StartWith == string.Empty)
                throw new Exception("UDBatch 為空值");
            return _UDBatch;
        }

        protected internal Parameter GetUDStat()
        {
            if (_UDStat.StartWith == string.Empty)
                throw new Exception("UDStat 為空值");
            return _UDStat;
        }

        protected internal Parameter GetOther()
        {
            if (_Other.StartWith == string.Empty)
                throw new Exception("Other 為空值");
            return _Other;
        }

        protected internal Parameter GetCare()
        {
            if (_Care.StartWith == string.Empty)
                throw new Exception("Care 為空值");
            return _Care;
        }
    }

    public class Parameter
    {
        public string StartWith { get; set; } = string.Empty;
    }
}
