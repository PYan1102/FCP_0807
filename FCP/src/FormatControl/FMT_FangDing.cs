using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Globalization;
using FCP.src.Enum;
using FCP.Models;
using Helper;

namespace FCP.src.FormatControl
{
    class FMT_FangDing : FormatCollection
    {
        private List<FangDingOPD> _opd = new List<FangDingOPD>();

        public override void ProcessOPD()
        {
            try
            {
                foreach (string s in GetPrescriptionInfoList)
                {
                    List<string> properties = s.Split('|').Where(x => !string.IsNullOrEmpty(x)).ToList();
                    string adminCode = properties[9];
                    string medicineCode = properties[5];
                    if (FilterRule(adminCode, medicineCode))
                    {
                        continue;
                    }
                    if (!IsExistsMultiAdminCode(adminCode))
                    {
                        LostMultiAdminCode(adminCode);
                        return;
                    }
                    int days = Convert.ToInt32(properties[10]);
                    DateTime endDate = DateTimeHelper.Convert(properties[4], "yyyyMMdd").AddDays(days - 1);
                    _opd.Add(new FangDingOPD
                    {
                        PrescriptionNo = properties[2],
                        PatientName = properties[3],
                        StartDate = properties[4].Substring(2),
                        EndDate = endDate.ToString("yyMMdd"),
                        MedicineCode = medicineCode,
                        MedicineName = properties[6],
                        PerQty = properties[7],
                        AdminCode = adminCode,
                        Days = days,
                        SumQty = properties[11],
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
            string outputDirectory = $@"{OutputDirectory}\{_opd[0].PatientName}-{_opd[0].PrescriptionNo}-{SourceFileNameWithoutExtension}_{CurrentSeconds}.txt";
            try
            {
                OP_OnCube.FangDing(_opd, outputDirectory);
                Success();
            }
            catch (Exception ex)
            {
                GenerateOCSFileFail(ex);
            }
        }

        public override void ProcessUDBatch()
        {
            throw new NotImplementedException();
        }

        public override void LogicUDBatch()
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
            _opd.Clear();
            return base.DepartmentShunt();
        }
    }

    internal class FangDingOPD
    {
        public string PrescriptionNo { get; set; }
        public string PatientName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string MedicineCode { get; set; }
        public string MedicineName { get; set; }
        public string PerQty { get; set; }
        public string AdminCode { get; set; }
        public int Days { get; set; }
        public string SumQty { get; set; }
    }
}
