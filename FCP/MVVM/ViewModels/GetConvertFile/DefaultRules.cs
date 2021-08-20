﻿using System;
using System.Collections.Generic;
using FCP.MVVM.Models.Enum;

namespace FCP.MVVM.ViewModels.GetConvertFile
{
    public class DefaultRules
    {
        public string OPD { set => _OPD.Rule = value; }
        public string Powder { set => _Powder.Rule = value; }
        public string UDBatch { set => _UDBatch.Rule = value; }
        public string UDStat { set => _UDStat.Rule = value; }
        public string Care { set => _Care.Rule = value; }
        public string Other { set => _Other.Rule = value; }
        private Dictionary<Parameter, DepartmentEnum> _DepartmentDictionary = new Dictionary<Parameter, DepartmentEnum>();
        private Parameter _OPD { get; set; } = new Parameter();
        private Parameter _Powder { get; set; } = new Parameter();
        private Parameter _UDBatch { get; set; } = new Parameter();
        private Parameter _UDStat { get; set; } = new Parameter();
        private Parameter _Other { get; set; } = new Parameter();
        private Parameter _Care { get; set; } = new Parameter();

        public DefaultRules()
        {
            _DepartmentDictionary.Add(_OPD, DepartmentEnum.OPD);
            _DepartmentDictionary.Add(_Powder, DepartmentEnum.POWDER);
            _DepartmentDictionary.Add(_UDBatch, DepartmentEnum.UDBatch);
            _DepartmentDictionary.Add(_UDStat, DepartmentEnum.UDStat);
            _DepartmentDictionary.Add(_Care, DepartmentEnum.Care);
            _DepartmentDictionary.Add(_Other, DepartmentEnum.Other);
        }

        protected internal Dictionary<Parameter, DepartmentEnum> GetDepartmentDictionary()
        {
            return _DepartmentDictionary;
        }

        protected internal void ResetRuleToEmpty()
        {
            _OPD.Rule = string.Empty;
            _Powder.Rule = string.Empty;
            _UDBatch.Rule = string.Empty;
            _UDStat.Rule = string.Empty;
            _Care.Rule = string.Empty;
            _Other.Rule = string.Empty;
        }

        protected internal void OPDDefault()
        {
            _OPD.Rule = nameof(DefaultEnum.Default);
        }

        protected internal void PowderDefault()
        {
            _Powder.Rule = nameof(DefaultEnum.Default);
        }

        protected internal void UDBatchDefault()
        {
            _UDBatch.Rule = nameof(DefaultEnum.Default);
        }

        protected internal void UDStatDefault()
        {
            _UDStat.Rule = nameof(DefaultEnum.Default);
        }

        protected internal void CareDefault()
        {
            _Care.Rule = nameof(DefaultEnum.Default);
        }

        protected internal void OtherDefault()
        {
            _Other.Rule = nameof(DefaultEnum.Default);
        }
    }

    public class Parameter
    {
        public string Rule { get; set; } = string.Empty;
    }

    public enum DefaultEnum
    { Default }
}