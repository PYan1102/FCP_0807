using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using FCP.MVVM.Models.Enum;
using FCP.MVVM.Models;

namespace FCP
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
                    EncodingHelper.SetEncodingBytes(s.Trim());
                    //途徑PO轉為PC
                    string way = EncodingHelper.GetEncodingString(193, 4).Replace("PO", "PC");
                    string adminCode = EncodingHelper.GetEncodingString(183, 10) + way;
                    string medicineCode = EncodingHelper.GetEncodingString(57, 10);
                    if (!medicineList.Contains(medicineCode) || IsFilterAdminCode(adminCode))
                        continue;
                    if (!IsExistsCombiAdminCode(adminCode))
                    {
                        Log.Write($"{FilePath} 在OnCube中未建置種包頻率 {adminCode}");
                        ReturnsResult.Shunt(ConvertResult.沒有種包頻率, adminCode);
                        return false;
                    }
                    DateTime.TryParseExact(EncodingHelper.GetEncodingString(41, 8), "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime startDate);
                    _OPD.Add(new JenKang()
                    {
                        DrugNo = EncodingHelper.GetEncodingString(0, 10),
                        PatientName = EncodingHelper.GetEncodingString(11, 20),
                        PrescriptionNo = EncodingHelper.GetEncodingString(31, 10),
                        StartDay = startDate,
                        MedicineCode = medicineCode,
                        MedicineName = EncodingHelper.GetEncodingString(67, 50),
                        PerQty = Convert.ToSingle(EncodingHelper.GetEncodingString(167, 10)).ToString("0.##"),
                        AdminCode = $"S{adminCode}",
                        Days = EncodingHelper.GetEncodingString(197, 3),
                        SumQty = EncodingHelper.GetEncodingString(200, 10),
                        Location = string.Empty
                    });
                    try
                    {
                        _OPD[_OPD.Count - 1]._IsPowder = EncodingHelper.GetEncodingString(236, 1) != "";
                    }
                    catch (Exception) { _OPD[_OPD.Count - 1]._IsPowder = false; }
                }
                if (_OPD.Count == 0)
                {
                    ReturnsResult.Shunt(ConvertResult.全數過濾, null);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Write($"{FilePath}  {ex}");
                ReturnsResult.Shunt(ConvertResult.讀取檔案失敗, ex.ToString());
                return false;
            }
        }

        public override bool LogicOPD()
        {
            string filePathOutput = $@"{OutputPath}\{Path.GetFileNameWithoutExtension(FilePath)}_{_OPD[0].PatientName}_{CurrentSeconds}.txt";
            try
            {
                OP_OnCube.JenKang_OPD(_OPD, filePathOutput);
                return true;
            }
            catch (Exception ex)
            {
                Log.Write($"{FilePath} {ex}");
                ReturnsResult.Shunt(ConvertResult.產生OCS失敗, ex.ToString());
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
                    EncodingHelper.SetEncodingBytes(s.Trim());
                    //途徑PO轉為PC
                    string way = EncodingHelper.GetEncodingString(193, 4).Replace("PO", "PC");
                    string adminCode = EncodingHelper.GetEncodingString(183, 10) + way;
                    string medicineCode = EncodingHelper.GetEncodingString(57, 10);
                    if (IsExistsMedicineCode(medicineCode) || IsFilterAdminCode(adminCode))
                        continue;
                    if (!IsExistsMultiAdminCode(adminCode))
                    {
                        Log.Write($"{FilePath} 在OnCube中未建置餐包頻率 {adminCode}");
                        ReturnsResult.Shunt(ConvertResult.沒有餐包頻率, adminCode);
                        return false;
                    }
                    string location = "住院";
                    if (EncodingHelper.GetBytesLength > 240)
                        location = EncodingHelper.GetEncodingString(247, EncodingHelper.GetBytesLength - 247);
                    int adminCodeTimePerDay = GetMultiAdminCodeTimes(adminCode).Count;
                    Single sumQty = Convert.ToSingle(EncodingHelper.GetEncodingString(200, 10));
                    Single perQty = Convert.ToSingle(EncodingHelper.GetEncodingString(167, 10));
                    _UDBatch.Add(new JenKang()
                    {
                        BedNo = EncodingHelper.GetEncodingString(0, 10),
                        PatientName = EncodingHelper.GetEncodingString(11, 20),
                        PrescriptionNo = EncodingHelper.GetEncodingString(31, 10),
                        MedicineCode = medicineCode,
                        MedicineName = EncodingHelper.GetEncodingString(67, 50),
                        PerQty = perQty.ToString("0.##"),
                        AdminCode = adminCode,
                        Days = Math.Ceiling(sumQty / perQty / adminCodeTimePerDay).ToString(),
                        SumQty = sumQty.ToString(),
                        Location = location
                    });
                    var lastUDBatchTemp = _UDBatch[_UDBatch.Count - 1];
                    DateTime.TryParseExact(EncodingHelper.GetEncodingString(41, 8), "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime startDate);
                    lastUDBatchTemp.StartDay = startDate.AddDays(location == "住院" ? 2 : 1);  //住院+2天，養護+1天
                    try
                    {
                        _UDBatch[_UDBatch.Count - 1]._IsPowder = EncodingHelper.GetEncodingString(236, 1) != "";
                    }
                    catch (Exception) { _UDBatch[_UDBatch.Count - 1]._IsPowder = false; }
                }
                if (_UDBatch.Count == 0)
                {
                    ReturnsResult.Shunt(ConvertResult.沒有餐包頻率, null);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Write($"{FilePath}  {ex}");
                ReturnsResult.Shunt(ConvertResult.讀取檔案失敗, ex.ToString());
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
            string filePathOutput = string.Empty;
            if (_UDBatch[0].Location.Contains("住院"))
                filePathOutput = $@"{OutputPath}\{Path.GetFileNameWithoutExtension(FilePath)}_{CurrentSeconds}.txt";
            else
                filePathOutput = $@"{OutputPath}\{Path.GetFileNameWithoutExtension(FilePath)}_{_UDBatch[0].PatientName}_{CurrentSeconds}.txt";
            try
            {
                OP_OnCube.JenKang_UD(_UDBatch, filePathOutput, minStartDate);
                return true;
            }
            catch (Exception ex)
            {
                Log.Write($"{FilePath} {ex}");
                ReturnsResult.Shunt(ConvertResult.產生OCS失敗, ex.ToString());
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
