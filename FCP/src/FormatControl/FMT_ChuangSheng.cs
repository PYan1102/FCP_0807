using System;
using System.Collections.Generic;
using FCP.src.Enum;
using FCP.Models;
using Helper;

namespace FCP.src.FormatControl
{
    class FMT_ChuangSheng : FormatCollection
    {
        List<ChuangShengOPD> _opd = new List<ChuangShengOPD>();

        public override void ProcessOPD()
        {
            try
            {
                foreach (string s in GetPrescriptionInfoList)
                {
                    EncodingHelper.SetBytes(s.TrimEnd());
                    string adminCode = EncodingHelper.GetString(74, 8);
                    string medicineCode = EncodingHelper.GetString(52, 10);
                    if(FilterRule(adminCode,medicineCode))
                    {
                        continue;
                    }
                    if (!IsExistsMultiAdminCode(adminCode))
                    {
                        LostMultiAdminCode(adminCode);
                        return;
                    }
                    string medicineName;
                    string hospitalName;
                    int totalLength = EncodingHelper.Length;
                    if (totalLength - 95 <= 50)
                    {
                        medicineName = EncodingHelper.GetString(95, totalLength - 95);
                        hospitalName = "";
                    }
                    else
                    {
                        medicineName = EncodingHelper.GetString(95, 50);
                        hospitalName = EncodingHelper.GetString(145, 20);
                    }
                    DateTime startDate = DateTime.Parse(EncodingHelper.GetString(20, 10));
                    string days = EncodingHelper.GetString(82, 3);
                    _opd.Add(new ChuangShengOPD
                    {
                        StartDate = startDate.ToString("yyMMdd"),
                        EndDate = startDate.AddDays(Convert.ToInt32(days) - 1).ToString("yyMMdd"),
                        Class = EncodingHelper.GetString(30, 12),
                        PatientName = EncodingHelper.GetString(42, 10).Replace("?", " "),
                        MedicineCode = medicineCode,
                        PerQty = EncodingHelper.GetString(62, 6),
                        Times = EncodingHelper.GetString(68, 6),
                        AdminCode = adminCode,
                        Days = days,
                        SumQty = EncodingHelper.GetString(85, 10),
                        MedicineName = medicineName,
                        HospitalName = hospitalName
                    }); ;
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
                OP_OnCube.ChuangSheng(_opd, outputDirectory);
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

    internal class ChuangShengOPD
    {
        public string PatientName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string MedicineCode { get; set; }
        public string MedicineName { get; set; }
        public string PerQty { get; set; }
        public string AdminCode { get; set; }
        public string Days { get; set; }
        public string SumQty { get; set; }
        public string Class { get; set; }
        public string Times { get; set; }
        public string HospitalName { get; set; }
    }
}
