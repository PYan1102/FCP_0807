using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.IO;
using FCP.MVVM.Models.Enum;

namespace FCP
{
    class FMT_ChangGung_POWDER : FormatCollection
    {
        ObservableCollection<POWDER> pow = new ObservableCollection<POWDER>();
        List<string> FilterMedicineCode = new List<string>() { "P2A090M", "P2A091M", "P2A092M", "P2A093M", "P2A094M", "P2A095M", "P2A096M", "P2A097M", "P2A099M", "P2A101M", "P2A102M" };

        private ConvertResult POWDER_Process()
        {
            try
            {
                string Content = GetContent;
                List<string> Data = SplitData(Content);
                pow.Clear();
                foreach (string s in Data)
                {
                    string[] Split = s.Split('|');
                    if (!FilterMedicineCode.Contains(Split[4].Substring(4).Trim()))
                        continue;
                    pow.Add(new POWDER
                    {
                        PrescriptionNo = Split[0].Substring(4).Trim(),
                        PatientNo = Split[1].Substring(3).Trim(),
                        PatientName = Split[2].Substring(2).Trim(),
                        MedicineCode = Split[4].Substring(4).Trim(),
                        MedicineName = Split[5].Substring(2).Trim(),
                        SumQty = Split[6].Substring(2).Trim(),
                        Mediciner = Split[9].Substring(4).Trim()
                    }); ;
                }
                if (pow.Count == 0)
                    return ConvertResult.全數過濾;
                return ConvertResult.成功;
            }
            catch (Exception ex)
            {
                Log.Write($"{FullFileName_S} {ex}");
                ErrorContent = $"{FullFileName_S} 讀取處方籤時發生問題 {ex}";
                return ConvertResult.失敗;
            }
        }

        private ConvertResult POWDER_Logic()
        {
            try
            {
                if (pow.Count == 0)
                    return ConvertResult.全數過濾;
                bool yn = false;
                FileNameOutput_S = $@"{OutputPath_S}\{Path.GetFileNameWithoutExtension(FullFileName_S)}_{Time_S}.txt";
                jvserver = new OnputType_JVServer(Log);
                yn = jvserver.ChangGung_POWDER(pow, FileNameOutput_S);
                if (yn)
                    return ConvertResult.成功;
                else
                {
                    ErrorContent = $"{FullFileName_S} 產生OCS時發生問題";
                    return ConvertResult.失敗;
                }
            }
            catch (Exception ex)
            {
                Log.Write($"{FullFileName_S}  {ex}");
                ErrorContent = $"{FullFileName_S} 判斷邏輯時發生問題 {ex}";
                return ConvertResult.失敗;
            }
        }

        private List<string> SplitData(string Content)
        {
            List<string> List = new List<string>();
            StringBuilder sb = new StringBuilder();
            List<string> Split = Content.Split('\n').ToList();
            int order = 0;
            foreach (string s in Split)
            {
                if (s.Trim() == "")
                    continue;
                sb.Append($"{s.Trim()}|");
                order++;
                if (order % 10 == 0)
                {
                    List.Add(sb.ToString());
                    sb.Clear();
                    order = 0;
                }
            }
            return List;
        }

        public override bool ProcessOPD()
        {
            throw new NotImplementedException();
        }

        public override bool LogicOPD()
        {
            throw new NotImplementedException();
        }

        public override bool ProcessUDBatch()
        {
            throw new NotImplementedException();
        }

        public override bool LogicUDBatch()
        {
            throw new NotImplementedException();
        }

        public override bool ProcessUDStat()
        {
            throw new NotImplementedException();
        }

        public override bool LogicUDStat()
        {
            throw new NotImplementedException();
        }

        public override bool ProcessPOWDER()
        {
            throw new NotImplementedException();
        }

        public override bool LogicPOWDER()
        {
            throw new NotImplementedException();
        }

        public override bool ProcessOther()
        {
            throw new NotImplementedException();
        }

        public override bool LogicOther()
        {
            throw new NotImplementedException();
        }

        public class POWDER
        {
            public string PrescriptionNo { get; set; }
            public string PatientNo { get; set; }
            public string PatientName { get; set; }
            public string MedicineCode { get; set; }
            public string MedicineName { get; set; }
            public string SumQty { get; set; }
            public string Mediciner { get; set; }
        }

    }
}
