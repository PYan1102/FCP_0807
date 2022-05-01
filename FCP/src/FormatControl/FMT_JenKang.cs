using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using FCP.src.Enum;
using FCP.Models;
using Helper;

namespace FCP.src.FormatControl
{
    class FMT_JenKang : FormatCollection
    {
        private List<JenKang> _OPD = new List<JenKang>();
        private List<JenKang> _UDBatch = new List<JenKang>();

        public override bool ProcessOPD()
        {
            try
            {
                List<string> list = GetContent.Trim().Split('\n').ToList();
                List<string> medicineList = GetMedicineCodeWhenWeightIs10g();
                foreach (string s in list)
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
                        Log.Write($"{FilePath} 在OnCube中未建置種包頻率 {adminCode}");
                        ReturnsResult.Shunt(eConvertResult.缺少種包頻率, adminCode);
                        return false;
                    }
                    DateTime.TryParseExact(EncodingHelper.GetString(41, 8), "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime startDate);
                    _OPD.Add(new JenKang()
                    {
                        DrugNo = EncodingHelper.GetString(0, 10),
                        PatientName = EncodingHelper.GetString(11, 20),
                        PrescriptionNo = EncodingHelper.GetString(31, 10),
                        StartDay = startDate,
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
                        _OPD[_OPD.Count - 1]._IsPowder = EncodingHelper.GetString(236, 1) != "";
                    }
                    catch (Exception) { _OPD[_OPD.Count - 1]._IsPowder = false; }
                }
                if (_OPD.Count == 0)
                {
                    ReturnsResult.Shunt(eConvertResult.全數過濾, null);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Write($"{FilePath} {ex}");
                ReturnsResult.Shunt(eConvertResult.讀取檔案失敗, ex.ToString());
                return false;
            }
        }

        public override bool LogicOPD()
        {
            string outputDirectory = $@"{OutputDirectory}\{Path.GetFileNameWithoutExtension(FilePath)}_{_OPD[0].PatientName}_{CurrentSeconds}.txt";
            try
            {
                OP_OnCube.JenKang_OPD(_OPD, outputDirectory);
                return true;
            }
            catch (Exception ex)
            {
                Log.Write($"{FilePath} {ex}");
                ReturnsResult.Shunt(eConvertResult.產生OCS失敗, ex.ToString());
                return false;
            }
        }

        public override bool ProcessUDBatch()
        {
            try
            {
                List<string> list = GetContent.Trim().Split('\n').ToList();
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
                        Log.Write($"{FilePath} 在OnCube中未建置餐包頻率 {adminCode}");
                        ReturnsResult.Shunt(eConvertResult.缺少餐包頻率, adminCode);
                        return false;
                    }
                    string location = "住院";
                    if (EncodingHelper.Length > 242)
                        location = EncodingHelper.GetString(247, EncodingHelper.Length - 247);
                    int adminCodeTimePerDay = GetMultiAdminCodeTimes(adminCode).Count;
                    Single sumQty = Convert.ToSingle(EncodingHelper.GetString(200, 10));
                    Single perQty = Convert.ToSingle(EncodingHelper.GetString(167, 10));
                    string days = Math.Ceiling(sumQty / perQty / adminCodeTimePerDay).ToString();
                    if (SettingModel.CrossDayAdminCode.Contains(adminCode))
                    {
                        days = EncodingHelper.GetString(197, 3);
                    }
                    _UDBatch.Add(new JenKang()
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
                    var lastUDBatchTemp = _UDBatch[_UDBatch.Count - 1];
                    DateTime startDate = DateTimeHelper.Convert(EncodingHelper.GetString(41, 8), "yyyyMMdd");
                    lastUDBatchTemp.StartDay = startDate.AddDays(location == "住院" ? 2 : 1);  //住院+2天，養護+1天
                    try
                    {
                        _UDBatch[_UDBatch.Count - 1]._IsPowder = EncodingHelper.GetString(236, 1) != "";
                    }
                    catch (Exception) { _UDBatch[_UDBatch.Count - 1]._IsPowder = false; }
                }
                if (_UDBatch.Count == 0)
                {
                    ReturnsResult.Shunt(eConvertResult.缺少餐包頻率, null);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Write($"{FilePath} {ex}");
                ReturnsResult.Shunt(eConvertResult.讀取檔案失敗, ex.ToString());
                return false;
            }
        }

        public override bool LogicUDBatch()
        {
            DateTime minStartDate = DateTime.Now;
            foreach (var v in _UDBatch)
            {
                DateTime dateTemp = v.StartDay;
                if (_UDBatch.IndexOf(v) == 0)
                {
                    minStartDate = dateTemp;
                    continue;
                }
                if (DateTime.Compare(minStartDate, dateTemp) == 1)
                {
                    minStartDate = dateTemp;
                }
            }
            foreach (var v in _UDBatch)
            {
                v.EndDay = minStartDate.AddDays(Convert.ToInt32(v.Days) - 1);
            }
            string outputDirectory = string.Empty;
            if (_UDBatch[0].Location.Contains("住院"))
                outputDirectory = $@"{OutputDirectory}\{Path.GetFileNameWithoutExtension(FilePath)}_{CurrentSeconds}.txt";
            else
                outputDirectory = $@"{OutputDirectory}\{Path.GetFileNameWithoutExtension(FilePath)}_{_UDBatch[0].PatientName}_{CurrentSeconds}.txt";
            try
            {
                OP_OnCube.JenKang_UD(_UDBatch, outputDirectory, minStartDate);
                return true;
            }
            catch (Exception ex)
            {
                Log.Write($"{FilePath} {ex}");
                ReturnsResult.Shunt(eConvertResult.產生OCS失敗, ex.ToString());
                return false;
            }
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

        public override ReturnsResultFormat MethodShunt()
        {
            _OPD.Clear();
            _UDBatch.Clear();
            return base.MethodShunt();
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
        public DateTime StartDay { get; set; }
        public DateTime EndDay { get; set; }
        public bool _IsPowder { get; set; }
        public string BedNo { get; set; }
        public string Location { get; set; }
    }
}
