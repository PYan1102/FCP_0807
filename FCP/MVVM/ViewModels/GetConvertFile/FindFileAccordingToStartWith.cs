using System;
using System.Threading;
using System.Threading.Tasks;
using FCP.MVVM.Models;
using FCP.MVVM.Factory;
using System.Collections.Generic;
using System.IO;
using FCP.MVVM.Models.Enum;

namespace FCP.MVVM.ViewModels.GetConvertFile
{
    public class FindFileAccordingToStartWith : CompareFileStartWith, IFindNeedToConvertFile
    {
        private CancellationTokenSource _CTS { get; set; }
        private SettingsModel _SettingsModel { get; set; }
        private List<string> _InputPathList { get; set; }

        public FindFileAccordingToStartWith()
        {
            _SettingsModel = SettingsFactory.GenerateSettingsModels();
        }

        public void Reset(CancellationTokenSource cts, List<string> list)
        {
            _CTS = cts;
            _InputPathList = list;
        }

        public void SetDepartmentDictionary(Dictionary<Parameter, DepartmentEnum> department)
        {
            DepartmentDictionary = department;
        }

        public async Task<FileInformation> GetFilePathTaskAsync()
        {
            try
            {
                while (!_CTS.IsCancellationRequested)
                {
                    await Task.Delay(_SettingsModel.Speed);
                    foreach (string path in _InputPathList)
                    {
                        if (path.Trim().Length == 0)
                            continue;
                        int index = _InputPathList.IndexOf(path);
                        foreach (string filePath in Directory.GetFiles(path, _SettingsModel.DeputyFileName))
                        {
                            bool isCompareCompleted = IsFileCompareSuccess(filePath);
                            if (isCompareCompleted)
                            {
                                return new FileInformation() { InputPath = path, FilePath = filePath, Department = GetDepartment() };
                            }
                            continue;
                        }
                    }
                }
                return new FileInformation();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
