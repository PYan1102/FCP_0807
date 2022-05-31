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

        public override void ProcessUDBatch()
        {
            try
            {
                bool exists = false;
                foreach (string s in GetPrescriptionInfoList)
                {
                    exists = false;
                    List<string> list = s.Split('	').Select(x => x.Trim()).ToList();
                    string medicineCode = list[4];
                    string adminCode = list[5];
                    float sumQty = float.Parse(list[10]);
                    if (list.Count <= 1 || adminCode == "BID+HS")
                        continue;
                    if (FilterRule(adminCode, medicineCode))
                    {
                        continue;
                    }
                    if (!IsExistsCombiAdminCode(adminCode))
                    {
                        LostCombiAdminCode(adminCode);
                        return;
                    }
                    DateTime startDate = DateTimeHelper.Convert($"{Convert.ToInt32(list[12]) + 19110000}", "yyyyMMdd");
                    for (int i = _batch.Count - 1; i >= 0; i--)
                    {
                        if (_batch[i].PatientName == list[1] & _batch[i].MedicineCode == medicineCode & _batch[i].AdminCode == $"S{adminCode}")
                        {
                            exists = true;
                            _batch[i].SumQty = (float.Parse(_batch[i].SumQty) + sumQty).ToString("0.###");
                            break;
                        }
                    }
                    if (!exists)
                    {
                        _batch.Add(new EDAUDBatch()
                        {
                            PatientName = list[1],
                            MedicineCode = medicineCode,
                            AdminCode = $"S{adminCode}",
                            PerQty = list[9],
                            SumQty = sumQty.ToString("0.###"),
                            StartDate = startDate.ToString("yyMMdd"),
                            Days = list[13],
                            BedNo = list[15],
                            MedicineName = list[19],
                            PrescriptionNo = list[20],
                            StartTime = list[23],
                            BirthDate = list[39].Insert(3, "/").Insert(6, "/")
                        });
                    }
                }
                if (_batch.Count == 0)
                {
                    Pass();
                }
            }
            catch (Exception ex)
            {
                ReadFileFail(ex);
            }
        }

        public override void LogicUDBatch()
        {
            string outputDirectory = $@"{OutputDirectory}\{SourceFileNameWithoutExtension}_{CurrentSeconds}.txt";
            try
            {
                OP_OnCube.E_DA_UD(_batch, outputDirectory);
                Success();
            }
            catch (Exception ex)
            {
                GenerateOCSFileFail(ex);
            }
        }

        public override void ProcessOPD()
        {
            throw new NotImplementedException();
        }

        public override void LogicOPD()
        {
            throw new NotImplementedException();
        }

        public override void ProcessUDStat()
        {
            throw new NotImplementedException();
        }

        public override void LogicUDStat()
        {
            throw new NotImplementedException();
        }

        public override void ProcessPowder()
        {
            throw new NotImplementedException();
        }

        public override void LogicPowder()
        {
            throw new NotImplementedException();
        }

        public override void ProcessOther()
        {
            throw new NotImplementedException();
        }

        public override void LogicOther()
        {
            throw new NotImplementedException();
        }

        public override void ProcessCare()
        {
            throw new NotImplementedException();
        }

        public override void LogicCare()
        {
            throw new NotImplementedException();
        }

        public override ReturnsResultModel DepartmentShunt()
        {
            _batch.Clear();
            return base.DepartmentShunt();
        }
    }
    internal class EDAUDBatch
    {
        public string PrescriptionNo { get; set; }
        public string PatientName { get; set; }
        public string MedicineCode { get; set; }
        public string MedicineName { get; set; }
        public string AdminCode { get; set; }
        public string PerQty { get; set; }
        public string SumQty { get; set; }
        public string StartDate { get; set; }
        public string StartTime { get; set; }
        public string BedNo { get; set; }
        public string BirthDate { get; set; }
        public string Days { get; set; }
    }
}
