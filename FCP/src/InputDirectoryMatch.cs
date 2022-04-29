using FCP.Models;
using FCP.src.Enum;
using FCP.src.Factory.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCP.src
{
    internal class InputDirectoryMatch
    {
        private static SettingModel _settingModel => SettingFactory.GenerateSettingModel();

        public static Dictionary<eDepartment, string> MatchOPD(bool opd1, bool opd2, bool opd3, bool opd4)
        {
            Dictionary<eDepartment, string> dictionary = new Dictionary<eDepartment, string>();
            if (opd1)
                dictionary.Add(eDepartment.OPD, _settingModel.InputDirectory1);
            if (opd2)
                dictionary.Add(eDepartment.POWDER, _settingModel.InputDirectory2);
            if (opd3)
                dictionary.Add(eDepartment.Care, _settingModel.InputDirectory3);
            if (opd4)
                dictionary.Add(eDepartment.Other, _settingModel.InputDirectory4);
            return dictionary;
        }

        public static Dictionary<eDepartment, string> MatchUD()
        {
            Dictionary<eDepartment, string> dictionary = new Dictionary<eDepartment, string>();
            if (_settingModel.UseStatOrBatch)
            {
                if (_settingModel.StatOrBatch == eDepartment.Stat)
                {
                    dictionary.Add(eDepartment.Stat, _settingModel.InputDirectory4);
                }
                else if (_settingModel.StatOrBatch == eDepartment.Batch)
                {
                    dictionary.Add(eDepartment.Batch, _settingModel.InputDirectory4);
                }
            }
            else
            {
                dictionary.Add(eDepartment.UD, _settingModel.InputDirectory4);
            }
            return dictionary;
        }
    }
}
