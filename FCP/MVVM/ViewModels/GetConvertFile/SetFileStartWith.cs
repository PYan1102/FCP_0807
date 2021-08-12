using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FCP.MVVM.Models.Enum;

namespace FCP.MVVM.ViewModels.GetConvertFile
{
    public class StartWithRules : DefaultRules, ISpecialRules
    {
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

        public void SetOther(string rule)
        {
            Other = rule;
        }

        public void SetCare(string rule)
        {
            Care = rule;
        }
    }
}
