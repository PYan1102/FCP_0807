using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FCP.MVVM.Models;
using FCP.MVVM.Models.Enum;

namespace FCP.MVVM.ViewModels.GetConvertFile
{
    public interface IFindFile
    {
        void Reset(CancellationTokenSource cts, List<string> list);
        void SetDepartmentDictionary();
        Task<FileInformation> GetFileInfo();
        void SetOPD(string rule);
        void SetPowder(string rule);
        void SetUDBatch(string rule);
        void SetUDStat(string rule);
        void SetCare(string rule);
        void SetOther(string rule);
        void SetOPDDefault();
        void SetPowderDefault();
        void SetUDBatchDefault();
        void SetUDStatDefault();
        void SetCareDefault();
        void SetOtherDefault();
    }

    public class FileInformation
    {
        public string InputPath { get; set; }
        public string FilePath { get; set; }
        public DepartmentEnum Department { get; set; }
    }
}
