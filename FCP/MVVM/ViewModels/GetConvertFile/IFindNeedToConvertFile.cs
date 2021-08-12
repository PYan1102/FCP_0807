using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using FCP.MVVM.Models.Enum;

namespace FCP.MVVM.ViewModels.GetConvertFile
{
    public interface IFindNeedToConvertFile : ISpecialRules
    {
        void ResetDictionary();
        void Reset(CancellationTokenSource cts, List<string> list);
        Task<FileInformation> GetFilePathTaskAsync();
        void SetOPDDefault();
        void SetPowderDefault();
        void SetUDBatchDefault();
        void SetUDStatDefault();
        void SetOtherDefault();
        void SetCareDefault();
    }

    public class FileInformation
    {
        public string InputPath { get; set; }
        public string FilePath { get; set; }
        public DepartmentEnum Department { get; set; }
    }
}
