using FCP.Models;
using FCP.src.Enum;
using Helper;
using System;
using System.Collections.Generic;

namespace FCP.src.FormatLogic
{
    class FMT_LittleBear : FormatCollection
    {
        private List<LittleBearOPD> _opd = new List<LittleBearOPD>();

        public override void ProcessOPD()
        {
            try
            {
                foreach (var s in GetPrescriptionInfoList)
                {
                    string[] data = s.Trim().Split(',');
                    string medicineCode = RemoveStringDoubleQuotes(data[2]);
                    string adminCode = RemoveStringDoubleQuotes(data[6]);
                    if (FilterRule(adminCode, medicineCode))
                    {
                        continue;
                    }
                    if (WhetherToStopNotHasMultiAdminCode(adminCode))
                    {
                        return;
                    }
                    DateTime startDate = DateTimeHelper.Convert((Convert.ToInt32(RemoveStringDoubleQuotes(data[0])) + 19110000).ToString(), "yyyyMMdd");
                    int days = Convert.ToInt32(RemoveStringDoubleQuotes(data[7]));
                    _opd.Add(new LittleBearOPD()
                    {
                        PatientName = RemoveStringDoubleQuotes(data[13]),
                        PrescriptionNo = RemoveStringDoubleQuotes(data[1]),
                        MedicineCode = medicineCode,
                        MedicineName = RemoveStringDoubleQuotes(data[3]),
                        PerQty = Convert.ToSingle(RemoveStringDoubleQuotes(data[4])),
                        AdminCode = adminCode,
                        Days = days,
                        SumQty = Convert.ToSingle(RemoveStringDoubleQuotes(data[9])),
                        StartDay = startDate,
                        EndDay = startDate.AddDays(days - 1)
                    });
                }
                if (_opd.Count == 0)
                {
                    Pass();
                }
            }
            catch (Exception ex)
            {
                ReadFileFail(ex);
            }
        }

        public override void LogicOPD()
        {
            string outputDirectory = $@"{OutputDirectory}\{_opd[0].PatientName}-{SourceFileNameWithoutExtension}_{CurrentSeconds}.txt";
            try
            {
                OP_OnCube.LittleBear(_opd, outputDirectory);
                Success();
            }
            catch (Exception ex)
            {
                GenerateOCSFileFail(ex);
            }
        }

        public override void LogicCare()
        {
            throw new NotImplementedException();
        }

        public override void LogicOther()
        {
            throw new NotImplementedException();
        }

        public override void LogicPowder()
        {
            throw new NotImplementedException();
        }

        public override void LogicUDBatch()
        {
            throw new NotImplementedException();
        }

        public override void LogicUDStat()
        {
            throw new NotImplementedException();
        }

        public override void ProcessCare()
        {
            throw new NotImplementedException();
        }

        public override void ProcessOther()
        {
            throw new NotImplementedException();
        }

        public override void ProcessPowder()
        {
            throw new NotImplementedException();
        }

        public override void ProcessUDBatch()
        {
            throw new NotImplementedException();
        }

        public override void ProcessUDStat()
        {
            throw new NotImplementedException();
        }

        private string RemoveStringDoubleQuotes(string data)
        {
            return data.Replace("\"", "");
        }

        public override ReturnsResultModel DepartmentShunt()
        {
            _opd.Clear();
            return base.DepartmentShunt();
        }
    }

    internal class LittleBearOPD
    {
        public string PatientName { get; set; }
        public string PrescriptionNo { get; set; }
        public string MedicineCode { get; set; }
        public string MedicineName { get; set; }
        public float PerQty { get; set; }
        public int Days { get; set; }
        public float SumQty { get; set; }
        public string AdminCode { get; set; }
        public DateTime StartDay { get; set; }
        public DateTime EndDay { get; set; }
    }
}
