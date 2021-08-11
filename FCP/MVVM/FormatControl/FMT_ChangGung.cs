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
        ObservableCollection<OPD> opd = new ObservableCollection<OPD>();
        ObservableCollection<Batch> batch = new ObservableCollection<Batch>();
        ObservableCollection<Stat> stat = new ObservableCollection<Stat>();
        private SettingsModel _SettingsModel { get; set; }
        bool _Do;
        string Type;

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
                Type = "門診ONLINE";
                string Content = GetContent;
                List<string> Data = SplitData(Content, 10);
                opd.Clear();
                foreach (string s in Data)
                {
                    string[] Split = s.Split('|');
                    if (_SettingsModel.EN_FilterMedicineCode && !MedicineCodeGiven_L.Contains(Split[4].Substring(4)))
                        continue;
                    opd.Add(new OPD
                    {
                        PrescriptionNo = Split[0].Substring(4),
                        PatientNo = Split[1].Substring(3),
                        PatientName = Split[2].Substring(2),
                        Other = Split[3].Substring(3),  //料位號
                        MedicineCode = Split[4].Substring(4),
                        MedicineName = Split[5].Substring(2),
                        SumQty = Split[6].Substring(2),
                        Mediciner = Split[9].Substring(4)
                    });
                }
                if (opd.Count == 0)
                {
                    ReturnsResult.Shunt(ConvertResult.全數過濾, null);
                    return false;
                }
                _Do = true;
                return true;
            }
            catch (Exception ex)
            {
                Log.Write($"{FullFileName_S} {ex}");
                ReturnsResult.Shunt(ConvertResult.讀取檔案失敗, ex.ToString());
                return false;
            }
        }

        public override bool LogicOPD()
        {
            try
            {
                oncube = new OnputType_OnCube(Log);
                FileNameOutput_S = $@"{OutputPath_S}\{Path.GetFileNameWithoutExtension(FullFileName_S)}_{Time_S}.txt";
                bool yn = oncube.ChangGung_OPD(opd, FileNameOutput_S, Type);
                if (yn)
                    return true;
                else
                {
                    ReturnsResult.Shunt(ConvertResult.產生OCS失敗, null);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.Write($"{FullFileName_S}  {ex}");
                ReturnsResult.Shunt(ConvertResult.處理邏輯失敗, ex.ToString());
                return false;
            }
        }

        public override bool ProcessUDBatch()
        {
            try
            {
                Type = "住院批次";
                string Content = GetContent;
                string[] Data = Content.Split('\n');
                batch.Clear();
                foreach (string s in Data)
                {
                    if (s.Trim() == "")
                        continue;
                    var ecd = Encoding.Default;
                    byte[] A = ecd.GetBytes(s);
                    if (_SettingsModel.EN_FilterMedicineCode && !MedicineCodeGiven_L.Contains(ecd.GetString(A, 131, 7)))
                        continue;
                    DateTime.TryParseExact((Convert.ToInt32(ecd.GetString(A, 37, 8)) + 19110000).ToString(), "yyyyMMdd", null, DateTimeStyles.None, out DateTime StartDate);
                    DateTime.TryParseExact(ecd.GetString(A, 239, 8), "yyyyMMdd", null, DateTimeStyles.None, out DateTime BirthDate);
                    batch.Add(new Batch
                    {
                        PatientNo = ecd.GetString(A, 12, 10).Trim(),
                        BedNo = ecd.GetString(A, 22, 10).Trim(),
                        Date = StartDate,
                        PatientName = ecd.GetString(A, 45, 10).Trim(),
                        Days = ecd.GetString(A, 59, 2).Trim(),
                        PerQty = ecd.GetString(A, 64, 9).Trim(),
                        MedicineName = ecd.GetString(A, 79, 40).Trim(),
                        MaterialNo = ecd.GetString(A, 119, 3).Trim(),
                        FirstDayQty = ecd.GetString(A, 122, 3).Trim(),
                        AdminCode = ecd.GetString(A, 127, 4).Trim(),
                        MedicineCode = ecd.GetString(A, 131, 7).Trim(),
                        ProductName = ecd.GetString(A, 141, 16).Trim(),
                        MedicineShape = ecd.GetString(A, 157, 60).Trim(),
                        NursingStationNo = ecd.GetString(A, 217, 16).Trim(),
                        BirthDate = BirthDate
                    });
                }
                if (batch.Count == 0)
                {
                    ReturnsResult.Shunt(ConvertResult.全數過濾, null);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Write($"{FullFileName_S}  {ex}");
                ReturnsResult.Shunt(ConvertResult.讀取檔案失敗, ex.ToString());
                return false;
            }
        }

        public override bool LogicUDBatch()
        {
            try
            {
                oncube = new OnputType_OnCube(Log);
                FileNameOutput_S = $@"{OutputPath_S}\{"住院批次"}_{Time_S}.txt";
                bool yn = oncube.ChangGung_UD_Batch(batch, FileNameOutput_S, Type);
                if (yn)
                    return true;
                else
                {
                    ReturnsResult.Shunt(ConvertResult.產生OCS失敗, null);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.Write($"{FullFileName_S}  {ex}");
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
                Type = "藥來速";
                string Content = GetContent;
                List<string> Data = SplitData(Content, 9);
                opd.Clear();
                foreach (string s in Data)
                {
                    string[] Split = s.Split('|');
                    if (_SettingsModel.EN_FilterMedicineCode && !MedicineCodeGiven_L.Contains(Split[4].Substring(4)))
                        continue;
                    opd.Add(new OPD
                    {
                        PrescriptionNo = Split[0].Substring(4),
                        PatientNo = Split[1].Substring(3),
                        PatientName = Split[2].Substring(2),
                        Other = Split[3].Substring(3),  //料位號
                        MedicineCode = Split[4].Substring(4),
                        MedicineName = Split[5].Substring(2),
                        SumQty = Split[6].Substring(2)
                    }); ;
                }
                if (opd.Count == 0)
                {
                    ReturnsResult.Shunt(ConvertResult.全數過濾, null);
                    return false;
                }
                _Do = true;
                return true;
            }
            catch (Exception ex)
            {
                Log.Write($"{FullFileName_S} {ex}");
                ReturnsResult.Shunt(ConvertResult.讀取檔案失敗, ex.ToString());
                return false;
            }
        }

        public override bool LogicOther()
        {
            try
            {
                oncube = new OnputType_OnCube(Log);
                FileNameOutput_S = $@"{OutputPath_S}\藥來速_{Time_S}.txt";
                bool yn = oncube.ChangGung_Other(opd, FileNameOutput_S, Type);
                if (yn)
                    return true;
                else
                {
                    ReturnsResult.Shunt(ConvertResult.產生OCS失敗, null);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.Write($"{FullFileName_S}  {ex}");
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

        private List<string> SplitData(string Content, int Count)
        {
            List<string> List = new List<string>();
            StringBuilder sb = new StringBuilder();
            List<string> Split = Content.Split('\n').ToList();
            int order = 0;
            foreach (string s in Split)
            {
                if (s.Trim() == "")
                    continue;
                sb.Append($"{s.Trim()}|");
                order++;
                if (order % Count == 0)
                {
                    List.Add(sb.ToString());
                    sb.Clear();
                    order = 0;
                }
            }
            return List;
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
