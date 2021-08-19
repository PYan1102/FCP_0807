using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using FCP.MVVM.Models.Enum;
using FCP.MVVM.Models;
using FCP.MVVM.Helper;

namespace FCP.MVVM.FormatControl
{
    class FMT_ChuangSheng : FormatCollection
    {
        List<ChuangShengOPD> _OPD = new List<ChuangShengOPD>();

        public override bool ProcessOPD()
        {
            try
            {
                List<string> list = GetContent.Split('\n').ToList();
                foreach (string s in list)
                {
                    if (s.Trim().Length == 0)
                        continue;
                    EncodingHelper.SetBytes(s.TrimEnd());
                    string adminCode = EncodingHelper.GetString(74, 8);
                    string medicineCode = EncodingHelper.GetString(52, 10);
                    if (IsFilterMedicineCode(medicineCode))
                        continue;
                    if (IsFilterAdminCode(adminCode))
                        continue;
                    if (!IsExistsMultiAdminCode(adminCode))
                    {
                        Log.Write($"{FilePath} 在OnCube中未建置此餐包頻率 {adminCode}");
                        ReturnsResult.Shunt(ConvertResult.沒有餐包頻率, adminCode);
                        return false;
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
                    _OPD.Add(new ChuangShengOPD
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
                ReturnsResult.Shunt(ConvertResult.讀取檔案失敗, null);
                return false;
            }
        }

        public override bool LogicOPD()
        {
            string filePathOutput = $@"{OutputPath}\{_OPD[0].PatientName}-{Path.GetFileNameWithoutExtension(FilePath)}_{CurrentSeconds}.txt";
            try
            {
                OP_OnCube.ChuangSheng(_OPD, filePathOutput);
                return true;
            }
            catch (Exception ex)
            {
                Log.Write($"{FilePath}  {ex}");
                ReturnsResult.Shunt(ConvertResult.產生OCS失敗, ex.ToString());
                return false;
            }
        }

        public override bool ProcessUDBatch()
        {
            throw new NotImplementedException();
        }

        public override bool LogicUDBatch()
        {
            throw new NotImplementedException();
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
            return base.MethodShunt();
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
