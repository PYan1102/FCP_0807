using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.IO;
using FCP.src.Enum;
using FCP.Models;
using Helper;

namespace FCP.src.FormatControl
{
    class FMT_ChangGung_POWDER : FormatCollection
    {
        private List<ChangGungPowder> _Powder = new List<ChangGungPowder>();
        private List<string> _NeedFilterMedicineCode = new List<string>() { "P2A090M", "P2A091M", "P2A092M", "P2A093M", "P2A094M", "P2A095M", "P2A096M", "P2A097M", "P2A099M", "P2A101M", "P2A102M" };


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
                List<string> list = SplitData(GetContent);
                _Powder.Clear();
                foreach (string s in list)
                {
                    string[] split = s.Split('|');
                    if (!_NeedFilterMedicineCode.Contains(split[4].Substring(4).Trim()))
                        continue;
                    _Powder.Add(new ChangGungPowder
                    {
                        PrescriptionNo = split[0].Substring(4).Trim(),
                        PatientNo = split[1].Substring(3).Trim(),
                        PatientName = split[2].Substring(2).Trim(),
                        MedicineCode = split[4].Substring(4).Trim(),
                        MedicineName = split[5].Substring(2).Trim(),
                        SumQty = split[6].Substring(2).Trim(),
                        Mediciner = split[9].Substring(4).Trim()
                    }); ;
                }
                if (_Powder.Count == 0)
                {
                    ReturnsResult.Shunt(eConvertResult.全數過濾, null);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Write($"{FilePath} {ex}");
                ReturnsResult.Shunt(eConvertResult.讀取檔案失敗, ex.ToString());
                return false;
            }
        }

        public override bool LogicPOWDER()
        {
            string outputDirectory = $@"{OutputPath}\{Path.GetFileNameWithoutExtension(FilePath)}_{CurrentSeconds}.txt";
            try
            {
                OP_JVServer.ChangGung_POWDER(_Powder, outputDirectory);
                return true;
            }
            catch (Exception ex)
            {
                Log.Write($"{FilePath} {ex}");
                ReturnsResult.Shunt(eConvertResult.產生OCS失敗, ex.ToString());
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

        public override ReturnsResultFormat MethodShunt()
        {
            _Powder.Clear();
            return base.MethodShunt();
        }
    }

    internal class ChangGungPowder
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
