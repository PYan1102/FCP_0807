using FCP.Models;
using Helper;
using System;
using System.Collections.Generic;

namespace FCP.src.FormatLogic
{
    class FMT_JiAn : FormatCollection
    {
        private List<PrescriptionModel> _opd = new List<PrescriptionModel>();

        public override void ProcessOPD()
        {
            try
            {
                foreach (string s in GetPrescriptionInfoList)
                {
                    EncodingHelper.SetBytes(s.Trim().Replace("?", "  "));
                    string adminCode = EncodingHelper.GetString(204, 20);
                    string medicineCode = EncodingHelper.GetString(134, 20);
                    string medicineName = EncodingHelper.GetString(154, 50);
                    if (FilterRule(adminCode.Substring(1), medicineCode))
                    {
                        continue;
                    }
                    if (WhetherToStopNotHasMultiAdminCode(adminCode.Substring(1)))
                    {
                        return;
                    }
                    _opd.Add(new PrescriptionModel()
                    {
                        PatientName = EncodingHelper.GetString(0, 20),
                        PatientNo = EncodingHelper.GetString(20, 30),
                        LocationName = "門診",
                        DoctorName = EncodingHelper.GetString(100, 26),
                        PerQty = Convert.ToSingle(EncodingHelper.GetString(129, 5)) / GetMultiAdminCodeTimes(adminCode.Substring(1)).Count,
                        SumQty = Convert.ToSingle(EncodingHelper.GetString(129, 5)),
                        MedicineCode = medicineCode,
                        MedicineName = medicineName,
                        AdminCode = adminCode,
                        StartDate = DateTimeHelper.Convert(EncodingHelper.GetString(224, 6), "yyMMdd"),
                        EndDate = DateTimeHelper.Convert(EncodingHelper.GetString(230, 6), "yyMMdd"),
                        Gender = EncodingHelper.GetString(404, 6),
                        RoomNo = EncodingHelper.GetString(410, 20),
                        BedNo = EncodingHelper.GetString(430, 20),
                        HospitalName = EncodingHelper.GetString(451, 30),
                        IsMultiDose = false
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
            string outputDirectory = $@"{OutputDirectory}\{_opd[0].PatientName}_{CurrentSeconds}.txt";
            try
            {
                OP_OnCube.JiAn(_opd, outputDirectory);
                Success();
            }
            catch (Exception ex)
            {
                GenerateOCSFileFail(ex);
            }
        }

        public override void ProcessCare()
        {
            try
            {
                foreach (string s in GetPrescriptionInfoList)
                {
                    EncodingHelper.SetBytes(s.Trim());
                    string adminCode = EncodingHelper.GetString(204, 20);
                    string medicineCode = EncodingHelper.GetString(134, 20);
                    if (FilterRule(adminCode, medicineCode))
                    {
                        continue;
                    }
                    if (WhetherToStopNotHasMultiAdminCode(adminCode))
                    {
                        return;
                    }
                    _opd.Add(new PrescriptionModel()
                    {
                        PatientName = EncodingHelper.GetString(0, 20),
                        PatientNo = EncodingHelper.GetString(20, 30),
                        LocationName = "機構",
                        DoctorName = EncodingHelper.GetString(100, 26),
                        PerQty = Convert.ToSingle(EncodingHelper.GetString(129, 5)),
                        SumQty = Convert.ToSingle(EncodingHelper.GetString(129, 5)) * GetMultiAdminCodeTimes(adminCode).Count,
                        MedicineCode = medicineCode,
                        MedicineName = EncodingHelper.GetString(154, 50),
                        AdminCode = adminCode,
                        StartDate = DateTimeHelper.Convert(EncodingHelper.GetString(224, 6), "yyMMdd"),
                        EndDate = DateTimeHelper.Convert(EncodingHelper.GetString(230, 6), "yyMMdd"),
                        Gender = EncodingHelper.GetString(404, 6),
                        RoomNo = EncodingHelper.GetString(410, 20),
                        BedNo = EncodingHelper.GetString(430, 20),
                        HospitalName = EncodingHelper.GetString(451, 30),
                        IsMultiDose = true
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

        public override void LogicCare()
        {
            string outputDirectory = $@"{OutputDirectory}\{_opd[0].PatientName}_{CurrentSeconds}.txt";
            try
            {
                OP_OnCube.JiAn(_opd, outputDirectory);
                Success();
            }
            catch (Exception ex)
            {
                GenerateOCSFileFail(ex);
            }
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

        public override ReturnsResultModel DepartmentShunt()
        {
            _opd.Clear();
            return base.DepartmentShunt();
        }
    }
}
