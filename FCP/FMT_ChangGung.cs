using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Globalization;

namespace FCP
{
    class FMT_ChangGung : FormatCollection
    {
        ObservableCollection<OPD> opd = new ObservableCollection<OPD>();
        ObservableCollection<Batch> batch = new ObservableCollection<Batch>();
        ObservableCollection<Stat> stat = new ObservableCollection<Stat>();
        bool _Do;
        string Type;
        public override void Load(string inp, string oup, string filename, string time, Settings settings, Log log)
        {
            base.Load(inp, oup, filename, time, settings, log);
        }

        public override string MethodShunt(int? MethodID)
        {
            _Do = false;
            switch (MethodID)
            {
                case 0:
                    return Do(OPD_Process(), OPD_Logic());
                case 1:
                    return Do(Other_Process(), Other_Logic());
                case 2:
                    if (Settings.StatOrBatch == "S")
                        return Do(UD_Stat_Process(), UD_Stat_Logic());
                    else
                        return Do(UD_Batch_Process(), UD_Batch_Logic());
                default:
                    return $"{FullFileName_S} {MethodID} {Settings.StatOrBatch}";
            }
        }

        private ResultType OPD_Process()
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
                    if (Settings.EN_FilterMedicineCode && !MedicineCodeGiven_L.Contains(Split[4].Substring(4)))
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
                    return ResultType.全數過濾;
                _Do = true;
                return ResultType.成功;
            }
            catch (Exception ex)
            {
                log.Write($"{FullFileName_S} {ex}");
                ErrorContent = $"{FullFileName_S} 讀取處方籤時發生問題 {ex}";
                return ResultType.失敗;
            }
        }


        private ResultType OPD_Logic()
        {
            try
            {
                if (opd.Count == 0)
                    return ResultType.全數過濾;
                oncube = new OnputType_OnCube(log);
                FileNameOutput_S = $@"{OutputPath_S}\{Path.GetFileNameWithoutExtension(FullFileName_S)}_{Time_S}.txt";
                bool yn = oncube.ChangGung_OPD(opd, FileNameOutput_S, Type);
                if (yn)
                    return ResultType.成功;
                else
                {
                    ErrorContent = $"{FullFileName_S} 產生OCS時發生問題";
                    return ResultType.失敗;
                }
            }
            catch (Exception ex)
            {
                log.Write($"{FullFileName_S}  {ex}");
                ErrorContent = $"{FullFileName_S} 判斷邏輯時發生問題 {ex}";
                return ResultType.失敗;
            }
        }

        private ResultType Other_Process()
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
                    if (Settings.EN_FilterMedicineCode && !MedicineCodeGiven_L.Contains(Split[4].Substring(4)))
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
                    return ResultType.全數過濾;
                _Do = true;
                return ResultType.成功;
            }
            catch (Exception ex)
            {
                log.Write($"{FullFileName_S} {ex}");
                ErrorContent = $"{FullFileName_S} 讀取處方籤時發生問題 {ex}";
                return ResultType.失敗;
            }
        }

        private ResultType Other_Logic()
        {
            try
            {
                if (opd.Count == 0)
                    return ResultType.全數過濾;
                oncube = new OnputType_OnCube(log);
                FileNameOutput_S = $@"{OutputPath_S}\藥來速_{Time_S}.txt";
                bool yn = oncube.ChangGung_Other(opd, FileNameOutput_S, Type);
                if (yn)
                    return ResultType.成功;
                else
                {
                    ErrorContent = $"{FullFileName_S} 產生OCS時發生問題";
                    return ResultType.失敗;
                }
            }
            catch (Exception ex)
            {
                log.Write($"{FullFileName_S}  {ex}");
                ErrorContent = $"{FullFileName_S} 判斷邏輯時發生問題 {ex}";
                return ResultType.失敗;
            }
        }

        private ResultType UD_Stat_Process()
        {
            return ResultType.全數過濾;
        }

        private ResultType UD_Stat_Logic()
        {
            oncube = new OnputType_OnCube(log);

            return ResultType.全數過濾;
        }

        private ResultType UD_Batch_Process()
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
                    if (Settings.EN_FilterMedicineCode && !MedicineCodeGiven_L.Contains(ecd.GetString(A, 131, 7)))
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
                    return ResultType.全數過濾;
                return ResultType.成功;
            }
            catch (Exception ex)
            {
                log.Write($"{FullFileName_S}  {ex}");
                ErrorContent = $"{FullFileName_S} 讀取處方籤時發生問題 {ex}";
                return ResultType.失敗;
            }
        }

        private ResultType UD_Batch_Logic()
        {
           try
            {
                if (batch.Count == 0)
                    return ResultType.全數過濾;
                oncube = new OnputType_OnCube(log);
                FileNameOutput_S = $@"{OutputPath_S}\{"住院批次"}_{Time_S}.txt";
                bool yn = oncube.ChangGung_UD_Batch(batch, FileNameOutput_S, Type);
                if (yn)
                    return ResultType.成功;
                else
                {
                    ErrorContent = $"{FullFileName_S} 產生OCS時發生問題";
                    return ResultType.失敗;
                }
            }
            catch (Exception ex)
            {
                log.Write($"{FullFileName_S}  {ex}");
                ErrorContent = $"{FullFileName_S} 判斷邏輯時發生問題 {ex}";
                return ResultType.失敗;
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
    }
}
