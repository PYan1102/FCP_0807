using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using FCP.src.Enum;
using FCP.Models;
using Helper;

namespace FCP.src.FormatControl
{
    class FMT_JVServer : FormatCollection
    {
        private string _random { get; set; }
        private JVServerOPDBasic _basic = new JVServerOPDBasic();
        private List<JVServerOPD> _opd = new List<JVServerOPD>();
        private List<string> _jvsRandom = new List<string>();
        private List<string> _oncubeRandom = new List<string>();

        public override void ProcessOPD()
        {
            try
            {
                string content = GetFileContent.Trim();
                int jvmPosition = content.IndexOf("|JVPEND||JVMHEAD|");
                EncodingHelper.SetBytes(content.Substring(9, jvmPosition - 9));
                _basic.PatientNo = EncodingHelper.GetString(1, 15);
                _basic.PrescriptionNo = EncodingHelper.GetString(16, 20);
                _basic.Age = EncodingHelper.GetString(36, 5);
                _basic.ID = EncodingHelper.GetString(54, 10);
                _basic.BirthDate = DateTimeHelper.Convert(EncodingHelper.GetString(94, 8), "yyyyMMdd").ToString("yyyy-MM-dd");
                _random = EncodingHelper.GetString(132, 30);
                _basic.PatientName = EncodingHelper.GetString(177, 20);
                if (_basic.PatientName.Contains("?"))
                    _basic.PatientName = _basic.PatientName.Replace("?", " ");
                _basic.Gender = EncodingHelper.GetString(197, 2);
                //有重疊風險
                _basic.Class = EncodingHelper.GetString(197, 30);
                _basic.HospitalName = EncodingHelper.GetString(229, 40);
                _basic.LocationName = EncodingHelper.GetString(229, 30);
                _basic.Mark = EncodingHelper.GetString(339, 20);

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
                        LostMultiAdminCode(adminCode);
                        return;
                    }
                    _opd.Add(new JVServerOPD()
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
                        _jvsRandom.Add(randomTemp);
                        randomPosition += 40;
                    }
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
            for (int x = 0; x <= 14; x++)
                _oncubeRandom.Add("");
            if (ExtraRandom.Count != 0)  //將JVServer的Random放入OnCube的Radnom
            {
                foreach (var random in ExtraRandom)
                {
                    _oncubeRandom[Convert.ToInt32(random.OnCube)] = _jvsRandom[Convert.ToInt32(random.JVServer)];
                }
            }
            Dictionary<string, List<JVServerOPD>> dictionary = null;
            var splitEachMeal = Properties.Settings.Default.IsSplitEachMeal;
            if (splitEachMeal)
            {
                try
                {
                    dictionary = SplitEachMeal();
                }
                catch (Exception ex)
                {
                    ReturnsResult.Shunt(eConvertResult.處理邏輯失敗, ex);
                }
            }
            try
            {
                if (splitEachMeal)
                {
                    string outputDirectoryWithoutSeconds = $@"{OutputDirectory}\{_basic.PatientName}-{SourceFileNameWithoutExtension}_";
                    OP_OnCube.JVServer_SplitEachMeal(dictionary, _basic, _oncubeRandom, _random, outputDirectoryWithoutSeconds, CurrentSeconds);
                }
                else
                {
                    string outputDirectory = $@"{OutputDirectory}\{_basic.PatientName}-{SourceFileNameWithoutExtension}_{CurrentSeconds}.txt";
                    OP_OnCube.JVServer(_opd, _basic, _oncubeRandom, _random, outputDirectory);
                }
                Success();
            }
            catch (Exception ex)
            {
                GenerateOCSFileFail(ex);
            }
        }

        private Dictionary<string, List<JVServerOPD>> SplitEachMeal()
        {
            Dictionary<string, List<JVServerOPD>> dic = new Dictionary<string, List<JVServerOPD>>();
            for (int x = 0; x <= 23; x++)
            {
                dic.Add(x.ToString().PadLeft(2, '0'), new List<JVServerOPD>());
            }
            for (int i = 0; i <= _opd.Count - 1; i++)
            {
                string adminCode = _opd[i].AdminCode;
                List<string> adminCodeTimeList = GetMultiAdminCodeTimes(adminCode);
                foreach (var v in adminCodeTimeList)
                {
                    string hour = v.Substring(0, 2);
                    dic[hour].Add(_opd[i]);
                }
            }
            return dic;
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
        private void ClearList()
        {
            _jvsRandom.Clear();
            _oncubeRandom.Clear();
        }

        public override ReturnsResultModel DepartmentShunt()
        {
            _basic = null;
            _basic = new JVServerOPDBasic();
            _opd.Clear();
            ClearList();
            return base.DepartmentShunt();
        }
    }

    internal class JVServerOPDBasic
    {
        public string PatientName { get; set; }
        public string PatientNo { get; set; }
        public string PrescriptionNo { get; set; }
        public string Class { get; set; }
        public string Mark { get; set; }
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
