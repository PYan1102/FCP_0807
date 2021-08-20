﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FCP.MVVM.Models.Enum;
using FCP.MVVM.Models;

namespace FCP.MVVM.ViewModels.GetConvertFile
{
    class FindFileAccordingToFileContent : CompareFileStartWith, IFindNeedToConvertFile
    {
        private CancellationTokenSource _CTS { get; set; }
        private SettingsModel _SettingsModel { get; set; }
        private List<string> _InputPathList { get; set; }
        public void Reset(CancellationTokenSource cts, List<string> list)
        {
            _CTS = cts;
            _InputPathList = list;
        }
        public Task<FileInformation> GetFilePathTaskAsync()
        {
            throw new NotImplementedException();
        }

        public void SetDepartmentDictionary(Dictionary<Parameter, DepartmentEnum> department)
        {
            throw new NotImplementedException();
        }
    }
}