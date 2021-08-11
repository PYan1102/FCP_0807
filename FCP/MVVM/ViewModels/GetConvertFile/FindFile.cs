using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using FCP.MVVM.Models;
using FCP.MVVM.Factory;
using System.Collections.Generic;
using System.IO;
using FCP.MVVM.ViewModels.GetConvertFile;
using System.Linq;
using FCP.MVVM.Models.Enum;

namespace FCP.MVVM.ViewModels.GetConvertFile
{
    public class FindFile : CompareFileStartWith
    {
        private CancellationTokenSource _CTS { get; set; }
        private SettingsModel _SettingsModel { get; set; }
        private List<string> _InputPathList { get; set; }

        public FindFile()
        {
            _SettingsModel = SettingsFactory.GenerateSettingsModels();
        }

        public void Reset(CancellationTokenSource cts, List<string> list)
        {
            _CTS = cts;
            _InputPathList = list;
            ResetDeparmnentDictionary();
        }

        public string GetFileName()
        {
            if (_CTS == null)
                return string.Empty;
            try
            {
                while (!_CTS.IsCancellationRequested)
                {
                    Thread.Sleep(_SettingsModel.Speed);
                    foreach (string path in _InputPathList)
                    {
                        if (path.Trim().Length == 0)
                            continue;
                        int index = _InputPathList.IndexOf(path);
                        foreach (string filePath in Directory.GetFiles(path, _SettingsModel.DeputyFileName))
                        {
                            DepartmentEnum department = Compare(filePath);
                            Console.WriteLine(department);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return "";
        }
    }
}
