using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace FCP
{
    class FMT_E_DA : FormatCollection
    {
        ObservableCollection<Data> EDA_UD = new ObservableCollection<Data>();
        public override void Load(string inp, string oup, string filename, string time, Settings settings, Log log)
        {
            base.Load(inp, oup, filename, time, settings, log);
        }

        public override string MethodShunt(int? MethodID)
        {
            return Do(UD_Process(), UD_Logic());
        }

        private ResultType UD_Process()
        {
            try
            {
                var ecd = Encoding.Default;
                string Content = GetContent;
                bool isExists = false;
                string[] FirstSplit = Content.Split('\n');
                EDA_UD.Clear();
                foreach (string s in FirstSplit)
                {
                    isExists = false;
                    string[] SecondSplit = s.Split('	');
                    List<string> A = new List<string>();
                    SecondSplit.ToList().ForEach(x => A.Add(x.Trim()));
                    if (A.Count <= 1)
                        continue;
                    if (A[5] == "BID+HS")
                        continue;
                    if (Settings.EN_FilterMedicineCode && !MedicineCodeGiven_L.Contains(A[4]))
                        continue;
                    if (JudgePackedMode($"{A[5]}"))
                        continue;
                    if (!CheckCombiCode($"S{A[5]}"))
                    {
                        log.Write($"{FullFileName_S} 在OnCube中未建置此種包頻率 S{A[5]}");
                        LoseContent = $"{FullFileName_S} 在OnCube中未建置此種包頻率 S{A[5]} 的頻率";
                        return ResultType.沒有頻率;
                    }
                    DateTime.TryParseExact($"{(int.Parse(A[12]) + 19110000)}", "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime Start);
                    for (int i = EDA_UD.Count - 1; i >= 0; i--)
                    {
                        if (EDA_UD[i].PatientName == A[1] & EDA_UD[i].MedicineCode == A[4] & EDA_UD[i].AdminTime == $"S{A[5]}")
                        {
                            isExists = true;
                            EDA_UD[i].SumQty = (float.Parse(EDA_UD[i].SumQty) + float.Parse(A[10])).ToString("0.###");
                            break;
                        }
                    }
                    if (!isExists)
                    {
                        EDA_UD.Add(new Data
                        {
                            PatientName = A[1],
                            MedicineCode = A[4],
                            AdminTime = $"S{A[5]}",
                            PerQty = A[9],
                            SumQty = float.Parse(A[10]).ToString("0.###"),
                            StartDate = Start.ToString("yyMMdd"),
                            Days = A[13],
                            BedNo = A[15],
                            MedicineName = A[19],
                            PrescriptionNo = A[20],
                            StartTime = A[23],
                            BirthDate = A[39].Insert(3, "/").Insert(6, "/")
                        });
                    }
                }
                //Debug.WriteLine($@"{EDA_UD[600].PatientName}
                //{EDA_UD[600].MedicineCode}
                //{EDA_UD[600].MedicineName}
                //{EDA_UD[600].AdminTime}
                //{EDA_UD[600].PerQty}
                //{EDA_UD[600].SumQty}
                //{EDA_UD[600].BedNo}
                //{EDA_UD[600].PrescriptionNo}
                //{EDA_UD[600].StartDate}
                //{EDA_UD[600].StartTime}
                //{EDA_UD[600].Days}
                //{EDA_UD[600].BirthDate}");

                if (EDA_UD.Count == 0)
                    return ResultType.全數過濾;
                return ResultType.成功;
            }
            catch (Exception ex)
            {
                log.Write($"{FullFileName_S} {ex}");
                ErrorContent = $"{FullFileName_S} 讀取處方籤時發生問題 {ex}";
                return ResultType.失敗;
            }
        }

        private ResultType UD_Logic()
        {
            if (EDA_UD.Count == 0)
                return ResultType.全數過濾;
            try
            {
                bool yn = false;
                string OutputFileName = $@"{OutputPath_S}\{Path.GetFileNameWithoutExtension(FullFileName_S)}_{Time_S}.txt";
                oncube = new OnputType_OnCube(log);
                yn = oncube.E_DA_UD(EDA_UD, OutputFileName);
                if (yn)
                    return ResultType.成功;
                else
                {
                    List<string> day = new List<string>();
                    for (int x = 0; x <= EDA_UD.Count - 1; x++)
                    {
                        day.Add(EDA_UD[x].StartDate + "~" + EDA_UD[x].StartDate);
                    }
                    log.Prescription(FullFileName_S, EDA_UD.Select(x => x.PatientName).ToList(), EDA_UD.Select(x => x.PrescriptionNo).ToList(), EDA_UD.Select(x => x.MedicineCode).ToList(), EDA_UD.Select(x => x.MedicineName).ToList(),
                        EDA_UD.Select(x => x.AdminTime).ToList(), EDA_UD.Select(x => x.PerQty).ToList(), EDA_UD.Select(x => x.SumQty).ToList(), day);
                    ErrorContent = $"{FullFileName_S} 產生OCS時發生問題";
                    return ResultType.失敗;
                }
            }
            catch (Exception ex)
            {
                log.Write($"{FullFileName_S} {ex}");
                ErrorContent = $"{FullFileName_S} 處理邏輯時發生問題 {ex}";
                return ResultType.失敗;
            }
        }

        public class Data
        {
            public string PrescriptionNo { get; set; }
            public string PatientName { get; set; }
            public string MedicineCode { get; set; }
            public string MedicineName { get; set; }
            public string AdminTime { get; set; }
            public string PerQty { get; set; }
            public string SumQty { get; set; }
            public string StartDate { get; set; }
            public string StartTime { get; set; }
            public string BedNo { get; set; }
            public string BirthDate { get; set; }
            public string Days { get; set; }
        }
    }
}
