using FCP.src.Enum;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FCP.ViewModels.GetConvertFile
{
    interface IFindNeedToConvertFile
    {
        void Reset(CancellationTokenSource cts, List<string> list);
        void SetDepartmentDictionary(Dictionary<Parameter, eConvertLocation> department);
        Task<FileInformation> GetFilePathTaskAsync();
    }
}
