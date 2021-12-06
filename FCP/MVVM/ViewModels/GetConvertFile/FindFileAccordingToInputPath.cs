using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FCP.src.Enum;
using FCP.MVVM.Models;
using System.IO;
using FCP.src.Factory.Models;

namespace FCP.MVVM.ViewModels.GetConvertFile
{
    public class FindFileAccordingToInputPath : CompareFileStartWith, IFindNeedToConvertFile
    {
        private CancellationTokenSource _CTS { get; set; }
        private SettingsModel _SettingsModel { get; set; }
        private List<string> _InputPathList { get; set; }

        public FindFileAccordingToInputPath()
        {
            _SettingsModel = SettingsFactory.GenerateSettingsModel();
        }

        public void Reset(CancellationTokenSource cts, List<string> list)
        {
            _CTS = cts;
            _InputPathList = list;
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
                        foreach (string filePath in Directory.GetFiles(path, $"*.{_SettingsModel.FileExtensionName}"))
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

        public void SetDepartmentDictionary(Dictionary<Parameter, eConvertLocation> department)
        {
            throw new NotImplementedException();
        }
    }
}
