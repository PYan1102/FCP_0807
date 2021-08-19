using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using FCP.MVVM.Models.Enum;
using FCP.MVVM.Models;
using FCP.MVVM.Helper;

namespace FCP.MVVM.FormatControl
{
    class FMT_TaipeiDetention : FormatCollection
    {
        private TaipeiDetentionOPDBasic _Basic { get; set; }
        private List<TaipeiDetentionOPD> _OPD = new List<TaipeiDetentionOPD>();

        public override bool ProcessOPD()
        {
            if (!File.Exists(FilePath))
            {
                Log.Write(FilePath + "忽略");
                ReturnsResult.Shunt(ConvertResult.全數過濾, null);
                return false;
            }
            try
            {
                string content = GetContent.Trim();
                int jvmPosition = content.IndexOf("|JVPEND||JVMHEAD|");
                EncodingHelper.SetBytes(content.Substring(9, jvmPosition - 9));
                var ecd = Encoding.Default;
                _Basic.PatientNo = EncodingHelper.GetString(1, 15);
                _Basic.PrescriptionNo = EncodingHelper.GetString(16, 20);
                _Basic.Age = EncodingHelper.GetString(36, 5);
                _Basic.ID = EncodingHelper.GetString(54, 10);
                _Basic.BirthDate = EncodingHelper.GetString(94, 8);
                _Basic.Class = EncodingHelper.GetString(132, 30);
                _Basic.PatientName = EncodingHelper.GetString(177, 20);
                if (_Basic.PatientName.Contains("?"))
                    _Basic.PatientName = _Basic.PatientName.Replace("?", " ");
                _Basic.Gender = EncodingHelper.GetString(197, 2);
                _Basic.HospitalName = EncodingHelper.GetString(229, 40);
                _Basic.LocationName = EncodingHelper.GetString(229, 30);

                EncodingHelper.SetBytes(content.Substring(jvmPosition + 17, content.Length - 17 - jvmPosition));
                List<string> list = SeparateString(EncodingHelper.GetString(0, EncodingHelper.Length), 106);  //計算有多少種藥品資料
                foreach (string s in list)  //將藥品資料放入List<string>
                {
                    EncodingHelper.SetBytes(s);
                    string adminCode = EncodingHelper.GetString(66, 10).Trim().Replace(" ", "").ToUpper();
                    string medicineCode = EncodingHelper.GetString(1, 15);
                    if (NeedFilterMedicineCode(medicineCode) || IsFilterMedicineCode(medicineCode) || IsFilterAdminCode(adminCode))
                        continue;
                    if (!IsExistsMultiAdminCode(adminCode))
                    {
                        Log.Write($"{FilePath} 在OnCube中未建置此餐包頻率 {adminCode}");
                        ReturnsResult.Shunt(ConvertResult.沒有餐包頻率, adminCode);
                        return false;
                    }
                    _OPD.Add(new TaipeiDetentionOPD()
                    {
                        MedicineCode = medicineCode,
                        MedicineName = EncodingHelper.GetString(16, 50),
                        AdminCode = adminCode,
                        Days = EncodingHelper.GetString(76, 3),
                        PerQty = EncodingHelper.GetString(81, 6),
                        SumQty = EncodingHelper.GetString(87, 8)
                    });
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
            string filePathOutput = $@"{OutputPath}\{_Basic.PatientName}-{_Basic.PatientNo}-{_Basic.Class}_{CurrentSeconds}.txt";
            DateTime.TryParseExact(_Basic.BirthDate, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime date);  //生日
            _Basic.BirthDate = date.ToString("yyyy-MM-dd");
            List<string> PutBackAdminCode = new List<string>() { "Q4H", "Q6H", "Q8H", "Q12H", "QDPRN", "QIDPRN", "PRN", "BIDPRN", "TIDPRN", "HSPRN" };
            try
            {
                OP_OnCube.TaipeiDentention(_OPD, _Basic, filePathOutput, PutBackAdminCode);
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
            _Basic = null;
            _Basic = new TaipeiDetentionOPDBasic();
            _OPD.Clear();
            return base.MethodShunt();
        }
    }
    internal class TaipeiDetentionOPDBasic
    {
        public string PatientName { get; set; }
        public string PatientNo { get; set; }
        public string PrescriptionNo { get; set; }
        public string Class { get; set; }
        public string Age { get; set; }
        public string ID { get; set; }
        public string Gender { get; set; }
        public string BirthDate { get; set; }
        public string LocationName { get; set; }
        public string HospitalName { get; set; }
    }

    internal class TaipeiDetentionOPD
    {
        public string MedicineCode { get; set; }
        public string MedicineName { get; set; }
        public string PerQty { get; set; }
        public string AdminCode { get; set; }
        public string Days { get; set; }
        public string SumQty { get; set; }
        public string BedNo { get; set; }
    }
}
