using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCP.MVVM.ViewModels.GetConvertFile
{
    public interface ISpecialRules
    {
        void SetOPD(string rule);
        void SetPowder(string rule);
        void SetUDBatch(string rule);
        void SetUDStat(string rule);
        void SetOther(string rule);
        void SetCare(string rule);
    }
}
