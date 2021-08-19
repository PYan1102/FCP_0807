using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using FCP.MVVM.Models.Enum;
using FCP.MVVM.Models;
using FCP.MVVM.Helper;

namespace FCP.MVVM.FormatControl
{
    class FMT_JVServer : FormatCollection
    {
        private string _Random { get; set; }
        private JVServerOPDBasic _Basic { get; set; }
        private List<JVServerOPD> _OPD = new List<JVServerOPD>();
        private List<string> _JVServerRandom = new List<string>();
        private List<string> _OnCubeRandom = new List<string>();

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
                ClearList();
                string content = GetContent.Trim();
                int jvmPosition = content.IndexOf("|JVPEND||JVMHEAD|");
                EncodingHelper.SetBytes(content.Substring(9, jvmPosition - 9));
                var ecd = Encoding.Default;
                _Basic.PatientNo = EncodingHelper.GetString(1, 15);
                _Basic.PrescriptionNo = EncodingHelper.GetString(16, 20);
                _Basic.Age = EncodingHelper.GetString(36, 5);
                _Basic.ID = EncodingHelper.GetString(54, 10);
                _Basic.BirthDate = EncodingHelper.GetString(94, 8);
                _Random = EncodingHelper.GetString(132, 30);
                _Basic.PatientName = EncodingHelper.GetString(177, 20);
                if (_Basic.PatientName.Contains("?"))
                    _Basic.PatientName = _Basic.PatientName.Replace("?", " ");
                _Basic.Gender = EncodingHelper.GetString(197, 2);
                _Basic.HospitalName = EncodingHelper.GetString(229, 40);
                _Basic.LocationName = EncodingHelper.GetString(229, 30);

                EncodingHelper.SetBytes(content.Substring(jvmPosition + 17, content.Length - 17 - jvmPosition));
                List<string> list = SeparateString(EncodingHelper.GetString(0, EncodingHelper.Length), 547);  //計算有多少種藥品資料
                foreach (string s in list)
                {
                    EncodingHelper.SetBytes(s);
                    string adminCode = EncodingHelper.GetString(66, 10);
                    string medicineCode = EncodingHelper.GetString(1, 15);
                    if (IsFilterMedicineCode(medicineCode) || IsFilterAdminCode(adminCode))
                        continue;
                    if (!IsExistsMultiAdminCode(adminCode))
                    {
                        Log.Write($"{FilePath} 在OnCube中未建置此餐包頻率 {adminCode}");
                        ReturnsResult.Shunt(ConvertResult.沒有餐包頻率, adminCode);
                        return false;
                    }
                    _OPD.Add(new JVServerOPD()
                    {
                        MedicineCode = medicineCode,
                        MedicineName = EncodingHelper.GetString(16, 50),
                        AdminCode = adminCode,
                        PerQty = EncodingHelper.GetString(81, 6),
                        SumQty = EncodingHelper.GetString(87, 8),
                        StartDay = EncodingHelper.GetString(509, 6),
                        EndDay = EncodingHelper.GetString(529, 6)
                    });
                    int randomPosition = 107;  //Random位置
                    for (int x = 0; x <= 9; x++)  //Random
                    {
                        string randomTemp = EncodingHelper.GetString(randomPosition, 30);  //符合OnCube輸出
                        _JVServerRandom.Add(randomTemp);
                        randomPosition += 40;
                    }
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
            for (int x = 0; x <= 14; x++)
                _OnCubeRandom.Add("");
            if (!string.IsNullOrEmpty(SettingsModel.ExtraRandom))  //將JVServer的Random放入OnCube的Radnom
            {
                int head;
                int middle;
                string[] randomList = SettingsModel.ExtraRandom.Split(',');
                foreach (string s in randomList)
                {
                    if (!string.IsNullOrEmpty(s))
                    {
                        head = s.IndexOf(':');
                        middle = s.IndexOf("&");
                        _OnCubeRandom[Int32.Parse(s.Substring(middle + 1, s.Length - middle - 1)) - 1] = _JVServerRandom[Int32.Parse(s.Substring(head + 1, middle - head - 1))];
                    }
                }
            }
            string filePathOutput = $@"{OutputPath}\{_Basic.PatientName}-{Path.GetFileNameWithoutExtension(FilePath)}_{CurrentSeconds}.txt";
            DateTime.TryParseExact(_Basic.BirthDate, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime date);  //生日
            _Basic.BirthDate = date.ToString("yyyy-MM-dd");
            try
            {
                OP_OnCube.JVServer(_OPD, _Basic, _OnCubeRandom, _Random, filePathOutput);
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
        private void ClearList()
        {
            _JVServerRandom.Clear();
            _OnCubeRandom.Clear();
        }

        public override ReturnsResultFormat MethodShunt()
        {
            _Basic = null;
            _Basic = new JVServerOPDBasic();
            _OPD.Clear();
            return base.MethodShunt();
        }
    }

    internal class JVServerOPDBasic
    {
        public string PatientName { get; set; }
        public string PatientNo { get; set; }
        public string PrescriptionNo { get; set; }
        public string Age { get; set; }
        public string ID { get; set; }
        public string Gender { get; set; }
        public string BirthDate { get; set; }
        public string LocationName { get; set; }
        public string HospitalName { get; set; }
    }

    internal class JVServerOPD
    {
        public string MedicineCode { get; set; }
        public string MedicineName { get; set; }
        public string PerQty { get; set; }
        public string AdminCode { get; set; }
        public string Days { get; set; }
        public string SumQty { get; set; }
        public string BedNo { get; set; }
        public string StartDay { get; set; }
        public string EndDay { get; set; }
    }
}
