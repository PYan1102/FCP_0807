using FCP.MVVM.Models.Enum;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FCP.MVVM.ViewModels.GetConvertFile
{
    interface IFindNeedToConvertFile
    {
        void Reset(CancellationTokenSource cts, List<string> list);
        void SetDepartmentDictionary(Dictionary<Parameter, DepartmentEnum> department);
        Task<FileInformation> GetFilePathTaskAsync();
    }
}
