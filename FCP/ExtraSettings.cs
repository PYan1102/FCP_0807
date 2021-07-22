using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace FCP
{
    class ExtraSettings
    {
        string ExtraFileName;

        public void GivePath(string ConvertFormatType)
        {
            ExtraFileName = $@"Resources\ExtraSettings\{ConvertFormatType}\ExtraSettings.txt";
        }

        public void Check()
        {
            if (!File.Exists(ExtraFileName))
                Create();
        }

        public void Create()
        {
            using (FileStream fs = File.Create(ExtraFileName))
            {
            }
            using (StreamWriter sw = new StreamWriter(ExtraFileName, false, Encoding.Default))
            {
                sw.WriteLine("[MedicineCodeGiven]");
                sw.WriteLine("");
                sw.WriteLine("");
                sw.WriteLine("[AdminTimeValue]");
                for (int i = 1; i <= 24; i++)
                {
                    if (i == 24)
                        sw.Write(i + "=");
                    else
                        sw.WriteLine(i + "=");
                }
            }
        }

        public string GetExtraSettingsContent()
        {
            using (StreamReader sr = new StreamReader(ExtraFileName))
            {
                return sr.ReadToEnd();
            }
        }

        public List<string> SpreateExtraSettings()
        {
            List<string> ExtraSettingsContent = new List<string>();
            string extrasettingscontent = GetExtraSettingsContent();
            int a = extrasettingscontent.IndexOf("[AdminTimeValue]");
            ExtraSettingsContent.Add(extrasettingscontent.Substring(0, a).Trim());
            ExtraSettingsContent.Add(extrasettingscontent.Substring(a, extrasettingscontent.Length - a).Trim());
            return ExtraSettingsContent;
        }
    }
}
