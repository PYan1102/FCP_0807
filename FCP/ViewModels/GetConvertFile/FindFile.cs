using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FCP.src.Enum;

namespace FCP.ViewModels.GetConvertFile
{
    public class FindFile : DefaultRules, IFindFile
    {
        private IFindNeedToConvertFile _FindNeedToConvertFile { get; set; }
        public void Init(eFindFileMode mode)
        {
            switch (mode)
            {
                case eFindFileMode.根據檔名開頭:
                    _FindNeedToConvertFile = new FindFileAccordingToStartWith();
                    break;
                case eFindFileMode.根據輸入路徑:
                    _FindNeedToConvertFile = new FindFileAccordingToInputPath();
                    break;
                case eFindFileMode.根據檔案內容:
                    _FindNeedToConvertFile = new FindFileAccordingToFileContent();
                    break;
            }
        }

        public void Reset(CancellationTokenSource cts, List<string> list)
        {
            _FindNeedToConvertFile.Reset(cts, list);
        }

        public void SetDepartmentDictionary()
        {
            _FindNeedToConvertFile.SetDepartmentDictionary(GetDepartmentDictionary());
        }

        public Task<FileInformation> GetFileInfo()
        {
            return _FindNeedToConvertFile.GetFilePathTaskAsync();
        }

        public void SetOPD(string rule)
        {
            OPD = rule;
        }

        public void SetPowder(string rule)
        {
            Powder = rule;
        }

        public void SetUDBatch(string rule)
        {
            UDBatch = rule;
        }

        public void SetUDStat(string rule)
        {
            UDStat = rule;
        }

        public void SetCare(string rule)
        {
            Care = rule;
        }

        public void SetOther(string rule)
        {
            Other = rule;
        }

        public void SetOPDDefault()
        {
            OPDDefault();
        }

        public void SetPowderDefault()
        {
            PowderDefault();
        }

        public void SetUDBatchDefault()
        {
            UDBatchDefault();
        }

        public void SetUDStatDefault()
        {
            UDStatDefault();
        }

        public void SetCareDefault()
        {
            CareDefault();
        }

        public void SetOtherDefault()
        {
            OtherDefault();
        }
    }
}
