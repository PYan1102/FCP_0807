using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FCP.MVVM.Models.Enum;

namespace FCP.MVVM.ViewModels.GetConvertFile
{
    class FindFileAccordingToFileContent : CompareFileStartWith, IFindNeedToConvertFile
    {
        public Task<FileInformation> GetFilePathTaskAsync()
        {
            throw new NotImplementedException();
        }

        public void Reset(CancellationTokenSource cts, List<string> list)
        {
            throw new NotImplementedException();
        }

        public void SetCare(string rule)
        {
            throw new NotImplementedException();
        }

        public void SetCareDefault()
        {
            throw new NotImplementedException();
        }

        public void SetOPD(string rule)
        {
            throw new NotImplementedException();
        }

        public void SetOPDDefault()
        {
            throw new NotImplementedException();
        }

        public void SetOther(string rule)
        {
            throw new NotImplementedException();
        }

        public void SetOtherDefault()
        {
            throw new NotImplementedException();
        }

        public void SetPowder(string rule)
        {
            throw new NotImplementedException();
        }

        public void SetPowderDefault()
        {
            throw new NotImplementedException();
        }

        public void SetUDBatch(string rule)
        {
            throw new NotImplementedException();
        }

        public void SetUDBatchDefault()
        {
            throw new NotImplementedException();
        }

        public void SetUDStat(string rule)
        {
            throw new NotImplementedException();
        }

        public void SetUDStatDefault()
        {
            throw new NotImplementedException();
        }

        void IFindNeedToConvertFile.ResetDictionary()
        {
            throw new NotImplementedException();
        }
    }
}
