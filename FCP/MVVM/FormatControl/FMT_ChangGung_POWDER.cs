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
                {
                    ReturnsResult.Shunt(ConvertResult.全數過濾, null);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Write($"{FilePath} {ex}");
                ReturnsResult.Shunt(ConvertResult.讀取檔案失敗, ex.ToString());
                return false;
            }
        }

        public override bool LogicPOWDER()
        {
            try
            {
                bool yn = false;
                FileNameOutput_S = $@"{OutputPath}\{Path.GetFileNameWithoutExtension(FilePath)}_{CurrentSeconds}.txt";
                yn = OP_JVServer.ChangGung_POWDER(pow, FileNameOutput_S);
                if (yn)
                    return true;
                else
                {
                    ReturnsResult.Shunt(ConvertResult.產生OCS失敗, null);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.Write($"{FilePath}  {ex}");
                ReturnsResult.Shunt(ConvertResult.處理邏輯失敗, ex.ToString());
                return false;
            }
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

        public override bool ProcessCare()
        {
            throw new NotImplementedException();
        }

        public override bool LogicCare()
        {
            throw new NotImplementedException();
        }
    }
}
