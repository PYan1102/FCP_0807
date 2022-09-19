using System;
using System.Collections.Generic;
using FCP.src.Enum;
using FCP.Models;
using Helper;

namespace FCP.src.FormatLogic
{
    class FMT_ChuangSheng : FormatCollection
    {
        List<PrescriptionModel> _data = new List<PrescriptionModel>();

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
                    if (WhetherToStopNotHasMultiAdminCode(adminCode))
                    {
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
                    int days = Convert.ToInt32(EncodingHelper.GetString(82, 3));
                    _data.Add(new PrescriptionModel()
                    {
                        StartDate = startDate,
                        EndDate = startDate.AddDays(days - 1),
                        Class = EncodingHelper.GetString(30, 12),
                        PatientName = EncodingHelper.GetString(42, 10).Replace("?", " "),
                        MedicineCode = medicineCode,
                        PerQty = Convert.ToSingle(EncodingHelper.GetString(62, 6)),
                        AdminCode = adminCode,
                        Days = days,
                        SumQty = Convert.ToSingle(EncodingHelper.GetString(85, 10)),
                        MedicineName = medicineName,
                        HospitalName = hospitalName
                    }); ;
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

        public override void LogicOPD()
        {
            string outputDirectory = $@"{OutputDirectory}\{_data[0].PatientName}-{SourceFileNameWithoutExtension}_{CurrentSeconds}.txt";
            try
            {
                OP_OnCube.ChuangSheng(_data, outputDirectory);
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
            _data.Clear();
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
