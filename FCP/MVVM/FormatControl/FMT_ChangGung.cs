using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.ObjectModel;
using System.Globalization;
using FCP.MVVM.Models;
using FCP.MVVM.Models.Enum;
using FCP.MVVM.Factory;

namespace FCP
{
    class FMT_ChangGung : FormatCollection
    {
        private List<OPD> _OPD = new List<OPD>();
        private List<Batch> _Batch = new List<Batch>();
        private List<Stat> _Stat = new List<Stat>();
        private SettingsModel _SettingsModel { get; set; }
        private bool _Do { get; set; }

        public FMT_ChangGung()
        {
            _SettingsModel = SettingsFactory.GenerateSettingsModels();
        }

        public override ReturnsResultFormat MethodShunt()
        {
            _Do = false;
            return base.MethodShunt();
        }

        public override bool ProcessOPD()
        {
            try
            {
                List<string> list = SplitData(GetContent, 10);
                _OPD.Clear();
                foreach (string s in list)
                {
                    string[] listSplit = s.Split('|');
                    if (IsExistsMedicineCode(listSplit[4].Substring(4)))
                        continue;
                    _OPD.Add(new OPD
                    {
                        PrescriptionNo = listSplit[0].Substring(4),
                        PatientNo = listSplit[1].Substring(3),
                        PatientName = listSplit[2].Substring(2),
                        Other = listSplit[3].Substring(3),  //料位號
                        MedicineCode = listSplit[4].Substring(4),
                        MedicineName = listSplit[5].Substring(2),
                        SumQty = listSplit[6].Substring(2),
                        Mediciner = listSplit[9].Substring(4)
                    });
                }
                if (_OPD.Count == 0)
                {
                    ReturnsResult.Shunt(ConvertResult.全數過濾, null);
                    return false;
                }
                _Do = true;
                return true;
            }
            catch (Exception ex)
            {
                Log.Write($"{FilePath} {ex}");
                ReturnsResult.Shunt(ConvertResult.讀取檔案失敗, ex.ToString());
                return false;
            }
        }

        public override bool LogicOPD()
        {
            try
            {
                string filePathOutput = $@"{OutputPath}\{Path.GetFileNameWithoutExtension(FilePath)}_{CurrentSeconds}.txt";
                bool result = OP_OnCube.ChangGung_OPD(_OPD, filePathOutput, "門診ONLINE");
                if (result)
                    return true;
                ReturnsResult.Shunt(ConvertResult.產生OCS失敗, null);
                return false;
            }
            catch (Exception ex)
            {
                Log.Write($"{FilePath}  {ex}");
                ReturnsResult.Shunt(ConvertResult.處理邏輯失敗, ex.ToString());
                return false;
            }
        }

        public override bool ProcessUDBatch()
        {
            try
            {
                string[] listSplit = GetContent.Split('\n');
                _Batch.Clear();
                foreach (string s in listSplit)
                {
                    if (s.Trim().Length == 0)
                        continue;
                    var ecd = Encoding.Default;
                    byte[] temp = ecd.GetBytes(s);
                    if (IsExistsMedicineCode(ecd.GetString(temp, 131, 7)))
                        continue;
                    DateTime.TryParseExact((Convert.ToInt32(ecd.GetString(temp, 37, 8)) + 19110000).ToString(), "yyyyMMdd", null, DateTimeStyles.None, out DateTime startDate);
                    DateTime.TryParseExact(ecd.GetString(temp, 239, 8), "yyyyMMdd", null, DateTimeStyles.None, out DateTime birthDate);
                    _Batch.Add(new Batch
                    {
                        PatientNo = ecd.GetString(temp, 12, 10).Trim(),
                        BedNo = ecd.GetString(temp, 22, 10).Trim(),
                        Date = startDate,
                        PatientName = ecd.GetString(temp, 45, 10).Trim(),
                        Days = ecd.GetString(temp, 59, 2).Trim(),
                        PerQty = ecd.GetString(temp, 64, 9).Trim(),
                        MedicineName = ecd.GetString(temp, 79, 40).Trim(),
                        MaterialNo = ecd.GetString(temp, 119, 3).Trim(),
                        FirstDayQty = ecd.GetString(temp, 122, 3).Trim(),
                        AdminCode = ecd.GetString(temp, 127, 4).Trim(),
                        MedicineCode = ecd.GetString(temp, 131, 7).Trim(),
                        ProductName = ecd.GetString(temp, 141, 16).Trim(),
                        MedicineShape = ecd.GetString(temp, 157, 60).Trim(),
                        NursingStationNo = ecd.GetString(temp, 217, 16).Trim(),
                        BirthDate = birthDate
                    });
                }
                if (_Batch.Count == 0)
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

        public override bool LogicUDBatch()
        {
            try
            {
                string filePathOutput = $@"{OutputPath}\住院批次_{CurrentSeconds}.txt";
                bool result = OP_OnCube.ChangGung_UD_Batch(_Batch, filePathOutput, "住院批次");
                if (result)
                    return true;
                ReturnsResult.Shunt(ConvertResult.產生OCS失敗, null);
                return false;
            }
            catch (Exception ex)
            {
                Log.Write($"{FilePath}  {ex}");
                ReturnsResult.Shunt(ConvertResult.處理邏輯失敗, ex.ToString());
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
            try
            {
                List<string> list = SplitData(GetContent, 9);
                _OPD.Clear();
                foreach (string s in list)
                {
                    string[] listSplit = s.Split('|');
                    if (IsExistsMedicineCode(listSplit[4].Substring(4)))
                        continue;
                    _OPD.Add(new OPD
                    {
                        PrescriptionNo = listSplit[0].Substring(4),
                        PatientNo = listSplit[1].Substring(3),
                        PatientName = listSplit[2].Substring(2),
                        Other = listSplit[3].Substring(3),  //料位號
                        MedicineCode = listSplit[4].Substring(4),
                        MedicineName = listSplit[5].Substring(2),
                        SumQty = listSplit[6].Substring(2)
                    }); ;
                }
                if (_OPD.Count == 0)
                {
                    ReturnsResult.Shunt(ConvertResult.全數過濾, null);
                    return false;
                }
                _Do = true;
                return true;
            }
            catch (Exception ex)
            {
                Log.Write($"{FilePath} {ex}");
                ReturnsResult.Shunt(ConvertResult.讀取檔案失敗, ex.ToString());
                return false;
            }
        }

        public override bool LogicOther()
        {
            try
            {
                string filePathOutput = $@"{OutputPath}\藥來速_{CurrentSeconds}.txt";
                bool result = OP_OnCube.ChangGung_Other(_OPD, filePathOutput, "藥來速");
                if (result)
                    return true;
                ReturnsResult.Shunt(ConvertResult.產生OCS失敗, null);
                return false;
            }
            catch (Exception ex)
            {
                Log.Write($"{FilePath}  {ex}");
                ReturnsResult.Shunt(ConvertResult.處理邏輯失敗, ex.ToString());
                return false;
            }
        }

        public class OPD
        {
            public string PrescriptionNo { get; set; }
            public string PatientNo { get; set; }
            public string PatientName { get; set; }
            public string Other { get; set; }
            public string MedicineCode { get; set; }
            public string MedicineName { get; set; }
            public string SumQty { get; set; }
            public string Mediciner { get; set; }
        }

        public class Batch
        {
            public string PatientNo { get; set; }
            public string BedNo { get; set; }
            public DateTime Date { get; set; }
            public string PatientName { get; set; }
            public string Days { get; set; }
            public string PerQty { get; set; }
            public string MedicineName { get; set; }
            public string MaterialNo { get; set; }
            public string FirstDayQty { get; set; }
            public string AdminCode { get; set; }
            public string MedicineCode { get; set; }
            public string ProductName { get; set; }
            public string MedicineShape { get; set; }
            public string NursingStationNo { get; set; }
            public DateTime BirthDate { get; set; }
        }

        public class Stat
        {

        }

        private List<string> SplitData(string content, int count)
        {
            List<string> list = new List<string>();
            StringBuilder stringBuilder = new StringBuilder();
            List<string> contentSplit = content.Split('\n').ToList();
            int order = 0;
            foreach (string s in contentSplit)
            {
                if (s.Trim() == "")
                    continue;
                stringBuilder.Append($"{s.Trim()}|");
                order++;
                if (order % count == 0)
                {
                    list.Add(stringBuilder.ToString());
                    stringBuilder.Clear();
                    order = 0;
                }
            }
            return list;
        }

        public override bool ProcessCare()
        {
            throw new NotImplementedException();
        }

        public override bool LogicCare()
        {
            throw new NotImplementedException();
        }
    }
}
