using FCP.Models;
using Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FCP.src.FormatLogic
{
    class FMT_E_DA : FormatCollection
    {
        private List<PrescriptionModel> _data = new List<PrescriptionModel>();

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
                    float sumQty = Convert.ToSingle(list[10]);
                    if (list.Count <= 1 || adminCode == "BID+HS")
                        continue;
                    if (FilterRule(adminCode, medicineCode))
                    {
                        continue;
                    }
                    if (WhetherToStopNotHasCombiAdminCode(adminCode))
                    {
                        return;
                    }
                    DateTime startDate = DateTimeHelper.Convert($"{Convert.ToInt32(list[12]) + 19110000}", "yyyyMMdd");
                    DateTime birthDate = DateTimeHelper.Convert((Convert.ToInt32(list[39]) + 19110000).ToString(), "yyyyMMdd");
                    for (int i = _data.Count - 1; i >= 0; i--)
                    {
                        if (_data[i].PatientName == list[1] & _data[i].MedicineCode == medicineCode & _data[i].AdminCode == $"S{adminCode}")
                        {
                            exists = true;
                            _data[i].SumQty = _data[i].SumQty + sumQty;
                            break;
                        }
                    }
                    if (!exists)
                    {
                        _data.Add(new PrescriptionModel()
                        {
                            PatientName = list[1],
                            MedicineCode = medicineCode,
                            AdminCode = $"S{adminCode}",
                            PerQty = Convert.ToSingle(list[9]),
                            SumQty = sumQty,
                            StartDate = startDate,
                            Days = Convert.ToInt32(list[13]),
                            BedNo = list[15],
                            MedicineName = list[19],
                            PrescriptionNo = list[20],
                            HospitalName = "義大醫院",
                            LocationName = "住院",
                            BirthDate = birthDate.ToString("yyyy-MM-dd")
                        });
                    }
                }
                if (_data.Count == 0)
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
                OP_OnCube.E_DA_UD(_data, outputDirectory);
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
            _data.Clear();
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
