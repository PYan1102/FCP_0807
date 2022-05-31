using System;
using System.Collections.Generic;
using FCP.src.Enum;
using FCP.Models;
using Helper;

namespace FCP.src.FormatControl
{
    class FMT_YiSheng : FormatCollection
    {
        private List<YiShengOPD> _opd = new List<YiShengOPD>();

        public override void ProcessOPD()
        {
            try
            {
                foreach (string s in GetPrescriptionInfoList)  //將藥品資料放入List<string>
                {
                    EncodingHelper.SetBytes(s);
                    string adminCode = EncodingHelper.GetString(137, 10).Replace("/", "");
                    adminCode = adminCode.Split(' ')[0];
                    string medicineCode = EncodingHelper.GetString(63, 10);
                    if (FilterRule(adminCode, medicineCode))
                    {
                        continue;
                    }
                    if (!IsExistsMultiAdminCode(adminCode))
                    {
                        LostMultiAdminCode(adminCode);
                        return;
                    }
                    string dateTemp = (Convert.ToInt32(EncodingHelper.GetString(56, 7)) + 19110000).ToString();
                    DateTime startDate = DateTimeHelper.Convert(dateTemp, "yyyyMMdd");
                    int days = Convert.ToInt32(EncodingHelper.GetString(147, 3));
                    _opd.Add(new YiShengOPD()
                    {
                        PatientName = EncodingHelper.GetString(6, 20),
                        PatientNo = EncodingHelper.GetString(26, 10),
                        ID = EncodingHelper.GetString(36, 10),
                        Age = EncodingHelper.GetString(46, 10).Substring(0, EncodingHelper.GetString(46, 10).IndexOf(".")),
                        StartDate = startDate.ToString("yyMMdd"),
                        EndDate = startDate.AddDays(days - 1).ToString("yyMMdd"),
                        MedicineCode = medicineCode,
                        MedicineName = EncodingHelper.GetString(73, 54),
                        PerQty = Convert.ToSingle(EncodingHelper.GetString(127, 10)).ToString("0.###"),
                        AdminCode = adminCode,
                        Days = days.ToString(),
                        SumQty = EncodingHelper.GetString(150, 10),
                    });
                    float sumQty = Convert.ToSingle(_opd[_opd.Count - 1].SumQty);
                    if (adminCode == "prn")
                    {
                        _opd[_opd.Count - 1].PerQty = Math.Ceiling(sumQty / days).ToString("0.###");
                    }
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
            string outputDirectory = $@"{OutputDirectory}\{_opd[0].PatientName}-{SourceFileName}_{CurrentSeconds}.txt";
            try
            {
                OP_OnCube.YiSheng(_opd, outputDirectory);
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
    internal class YiShengOPD
    {
        public string PatientNo { get; set; }
        public string PatientName { get; set; }
        public string Age { get; set; }
        public string ID { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string MedicineCode { get; set; }
        public string MedicineName { get; set; }
        public string PerQty { get; set; }
        public string AdminCode { get; set; }
        public string Days { get; set; }
        public string SumQty { get; set; }
    }
}
