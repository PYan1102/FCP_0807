using System;
using System.Collections.Generic;
using System.Linq;
using FCP.src.Enum;
using FCP.Models;
using Helper;

namespace FCP.src.FormatControl
{
    class FMT_JenKang : FormatCollection
    {
        private List<JenKang> _opd = new List<JenKang>();
        private List<JenKang> _batch = new List<JenKang>();

        public override void ProcessOPD()
        {
            try
            {
                List<string> medicineList = GetMedicineCodeWhenWeightIs10g();
                foreach (string s in GetPrescriptionInfoList)
                {
                    EncodingHelper.SetBytes(s.Trim());
                    //途徑PO轉為PC
                    string way = EncodingHelper.GetString(193, 4).Replace("PO", "PC");
                    string adminCode = EncodingHelper.GetString(183, 10) + way;
                    string medicineCode = EncodingHelper.GetString(57, 10);
                    medicineCode = medicineCode == "EXFO160/51" ? "EXFO160／51" : medicineCode;
                    if (!medicineList.Contains(medicineCode) || IsFilterAdminCode(adminCode))
                        continue;
                    if (!IsExistsCombiAdminCode(adminCode))
                    {
                        LostCombiAdminCode(adminCode);
                        return;
                    }
                    DateTime startDate = DateTimeHelper.Convert(EncodingHelper.GetString(41, 8), "yyyyMMdd");
                    _opd.Add(new JenKang()
                    {
                        DrugNo = EncodingHelper.GetString(0, 10),
                        PatientName = EncodingHelper.GetString(11, 20),
                        PrescriptionNo = EncodingHelper.GetString(31, 10),
                        StartDate = startDate,
                        MedicineCode = medicineCode,
                        MedicineName = EncodingHelper.GetString(67, 50),
                        PerQty = Convert.ToSingle(EncodingHelper.GetString(167, 10)).ToString("0.##"),
                        AdminCode = $"S{adminCode}",
                        Days = EncodingHelper.GetString(197, 3),
                        SumQty = Convert.ToSingle(EncodingHelper.GetString(200, 10)).ToString("0.##"),
                        Location = string.Empty
                    });
                    try
                    {
                        _opd[_opd.Count - 1].IsPowder = EncodingHelper.GetString(236, 1) != "";
                    }
                    catch (Exception) { _opd[_opd.Count - 1].IsPowder = false; }
                }
                if (_opd.Count == 0)
                {
                    Pass();
                    return;
                }
                Success();
            }
            catch (Exception ex)
            {
                ReadFileFail(ex);
            }
        }

        public override void LogicOPD()
        {
            string outputDirectory = $@"{OutputDirectory}\{SourceFileNameWithoutExtension}-{_opd[0].PatientName}_{CurrentSeconds}.txt";
            try
            {
                OP_OnCube.JenKang_OPD(_opd, outputDirectory);
                Success();
            }
            catch (Exception ex)
            {
                GenerateOCSFileFail(ex);
            }
        }

        public override void ProcessUDBatch()
        {
            try
            {
                List<string> list = GetFileContent.Trim().Split('\n').ToList();
                foreach (string s in list)
                {
                    EncodingHelper.SetBytes(s.Trim());
                    //途徑PO轉為PC
                    string way = EncodingHelper.GetString(193, 4).Replace("PO", "PC");
                    string adminCode = EncodingHelper.GetString(183, 10) + way;
                    string medicineCode = EncodingHelper.GetString(57, 10);
                    medicineCode = medicineCode == "EXFO160/51" ? "EXFO160／51" : medicineCode;
                    if (IsFilterMedicineCode(medicineCode) || IsFilterAdminCode(adminCode))
                        continue;
                    if (!IsExistsMultiAdminCode(adminCode))
                    {
                        LostMultiAdminCode(adminCode);
                        return;
                    }
                    string location = "住院";
                    if (EncodingHelper.Length > 242)
                        location = EncodingHelper.GetString(247, EncodingHelper.Length - 247);
                    int adminCodeTimePerDay = GetMultiAdminCodeTimes(adminCode).Count;
                    float sumQty = Convert.ToSingle(EncodingHelper.GetString(200, 10));
                    float perQty = Convert.ToSingle(EncodingHelper.GetString(167, 10));
                    string days = Math.Ceiling(sumQty / perQty / adminCodeTimePerDay).ToString();
                    if (CrossDayAdminCode.Contains(adminCode))
                    {
                        days = EncodingHelper.GetString(197, 3);
                    }
                    _batch.Add(new JenKang()
                    {
                        BedNo = EncodingHelper.GetString(0, 10),
                        PatientName = EncodingHelper.GetString(11, 20),
                        PrescriptionNo = EncodingHelper.GetString(31, 10),
                        MedicineCode = medicineCode,
                        MedicineName = EncodingHelper.GetString(67, 50),
                        PerQty = perQty.ToString("0.##"),
                        AdminCode = adminCode,
                        Days = days,
                        SumQty = sumQty.ToString("0.##"),
                        Location = location
                    });
                    var lastUDBatchTemp = _batch[_batch.Count - 1];
                    DateTime startDate = DateTimeHelper.Convert(EncodingHelper.GetString(41, 8), "yyyyMMdd");
                    lastUDBatchTemp.StartDate = startDate.AddDays(location == "住院" ? 2 : 1);  //住院+2天，養護+1天
                    try
                    {
                        _batch[_batch.Count - 1].IsPowder = EncodingHelper.GetString(236, 1) != "";
                    }
                    catch (Exception) { _batch[_batch.Count - 1].IsPowder = false; }
                }
                if (_batch.Count == 0)
                {
                    Pass();
                    return;
                }
                Success();
            }
            catch (Exception ex)
            {
                ReadFileFail(ex);
            }
        }

        public override void LogicUDBatch()
        {
            DateTime minStartDate = DateTime.Now;
            foreach (var v in _batch)
            {
                DateTime dateTemp = v.StartDate;
                if (_batch.IndexOf(v) == 0)
                {
                    minStartDate = dateTemp;
                    continue;
                }
                if (DateTime.Compare(minStartDate, dateTemp) == 1)
                {
                    minStartDate = dateTemp;
                }
            }
            foreach (var v in _batch)
            {
                v.EndDate = minStartDate.AddDays(Convert.ToInt32(v.Days) - 1);
            }
            string outputDirectory = string.Empty;
            if (_batch[0].Location.Contains("住院"))
                outputDirectory = $@"{OutputDirectory}\{SourceFileNameWithoutExtension}_{CurrentSeconds}.txt";
            else
                outputDirectory = $@"{OutputDirectory}\{SourceFileNameWithoutExtension}-{_batch[0].PatientName}_{CurrentSeconds}.txt";
            try
            {
                OP_OnCube.JenKang_UD(_batch, outputDirectory, minStartDate);
                Success();
            }
            catch (Exception ex)
            {
                GenerateOCSFileFail(ex);
            }
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
            _batch.Clear();
            return base.DepartmentShunt();
        }
    }
    internal class JenKang
    {
        public string PrescriptionNo { get; set; }
        public string PatientName { get; set; }
        public string DrugNo { get; set; }
        public string MedicineCode { get; set; }
        public string MedicineName { get; set; }
        public string PerQty { get; set; }
        public string AdminCode { get; set; }
        public string Days { get; set; }
        public string SumQty { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsPowder { get; set; }
        public string BedNo { get; set; }
        public string Location { get; set; }
    }
}
