using System;
using System.Collections.Generic;
using System.Linq;
using FCP.src.Enum;
using FCP.Models;
using Helper;

namespace FCP.src.FormatControl
{
    class FMT_E_DA : FormatCollection
    {
        private List<EDAUDBatch> _batch = new List<EDAUDBatch>();

        public override bool ProcessUDBatch()
        {
            try
            {
                bool exists = false;
                foreach (string s in GetPrescriptionInfoList)
                {
                    exists = false;
                    List<string> data = s.Split('	').Select(x => x.Trim()).ToList();
                    if (data.Count <= 1 || data[5] == "BID+HS")
                        continue;
                    if (IsFilterMedicineCode(data[4]) || IsFilterAdminCode($"{data[5]}"))
                        continue;
                    if (!IsExistsCombiAdminCode($"{data[5]}"))
                    {
                        ReturnsResult.Shunt(eConvertResult.缺少種包頻率, data[5]);
                        return false;
                    }
                    DateTime startDate = DateTimeHelper.Convert($"{Convert.ToInt32(data[12]) + 19110000}", "yyyyMMdd");
                    for (int i = _batch.Count - 1; i >= 0; i--)
                    {
                        if (_batch[i].PatientName == data[1] & _batch[i].MedicineCode == data[4] & _batch[i].AdminTime == $"S{data[5]}")
                        {
                            exists = true;
                            _batch[i].SumQty = (float.Parse(_batch[i].SumQty) + float.Parse(data[10])).ToString("0.###");
                            break;
                        }
                    }
                    if (!exists)
                    {
                        _batch.Add(new EDAUDBatch
                        {
                            PatientName = data[1],
                            MedicineCode = data[4],
                            AdminTime = $"S{data[5]}",
                            PerQty = data[9],
                            SumQty = float.Parse(data[10]).ToString("0.###"),
                            StartDate = startDate.ToString("yyMMdd"),
                            Days = data[13],
                            BedNo = data[15],
                            MedicineName = data[19],
                            PrescriptionNo = data[20],
                            StartTime = data[23],
                            BirthDate = data[39].Insert(3, "/").Insert(6, "/")
                        });
                    }
                }
                if (_batch.Count == 0)
                {
                    ReturnsResult.Shunt(eConvertResult.全數過濾);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                ReturnsResult.Shunt(eConvertResult.讀取檔案失敗, ex);
                return false;
            }
        }

        public override bool LogicUDBatch()
        {
            string outputDirectory = $@"{OutputDirectory}\{SourceFileNameWithoutExtension}_{CurrentSeconds}.txt";
            try
            {
                OP_OnCube.E_DA_UD(_batch, outputDirectory);
                return true;
            }
            catch (Exception ex)
            {
                ReturnsResult.Shunt(eConvertResult.產生OCS失敗, ex);
                return false;
            }
        }

        public override bool ProcessOPD()
        {
            throw new NotImplementedException();
        }

        public override bool LogicOPD()
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

        public override bool ProcessCare()
        {
            throw new NotImplementedException();
        }

        public override bool LogicCare()
        {
            throw new NotImplementedException();
        }

        public override ReturnsResultModel MethodShunt()
        {
            _batch.Clear();
            return base.MethodShunt();
        }
    }
    internal class EDAUDBatch
    {
        public string PrescriptionNo { get; set; }
        public string PatientName { get; set; }
        public string MedicineCode { get; set; }
        public string MedicineName { get; set; }
        public string AdminTime { get; set; }
        public string PerQty { get; set; }
        public string SumQty { get; set; }
        public string StartDate { get; set; }
        public string StartTime { get; set; }
        public string BedNo { get; set; }
        public string BirthDate { get; set; }
        public string Days { get; set; }
    }
}
