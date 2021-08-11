using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.ObjectModel;
using FCP.MVVM.Models.Enum;

namespace FCP
{
    class FMT_E_DA : FormatCollection
    {
        ObservableCollection<Data> EDA_UD = new ObservableCollection<Data>();

        public override bool ProcessOPD()
        {
            throw new NotImplementedException();
        }

        public override bool LogicOPD()
        {
            throw new NotImplementedException();
        }

        public override bool ProcessUDBatch()
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
                    if (SettingsModel.EN_FilterMedicineCode && !MedicineCodeGiven_L.Contains(A[4]))
                        continue;
                    if (JudgePackedMode($"{A[5]}"))
                        continue;
                    if (!CheckCombiCode($"S{A[5]}"))
                    {
                        Log.Write($"{FilePath} 在OnCube中未建置此種包頻率 S{A[5]}");
                        ReturnsResult.Shunt(ConvertResult.沒有種包頻率, A[5]);
                        return false;
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

                if (EDA_UD.Count == 0)
                {
                    ReturnsResult.Shunt(ConvertResult.全數過濾, null);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Write($"{FilePath} {ex}");
                ReturnsResult.Shunt(ConvertResult.讀取檔案失敗, ex.ToString());
                return false;
            }
        }

        public override bool LogicUDBatch()
        {
            try
            {
                bool yn = false;
                string OutputFileName = $@"{OutputPath}\{Path.GetFileNameWithoutExtension(FilePath)}_{CurrentSeconds}.txt";
                OnCube = new OnputType_OnCube(Log);
                yn = OnCube.E_DA_UD(EDA_UD, OutputFileName);
                if (yn)
                    return true;
                else
                {
                    List<string> day = new List<string>();
                    for (int x = 0; x <= EDA_UD.Count - 1; x++)
                    {
                        day.Add(EDA_UD[x].StartDate + "~" + EDA_UD[x].StartDate);
                    }
                    Log.Prescription(FilePath, EDA_UD.Select(x => x.PatientName).ToList(), EDA_UD.Select(x => x.PrescriptionNo).ToList(), EDA_UD.Select(x => x.MedicineCode).ToList(), EDA_UD.Select(x => x.MedicineName).ToList(),
                        EDA_UD.Select(x => x.AdminTime).ToList(), EDA_UD.Select(x => x.PerQty).ToList(), EDA_UD.Select(x => x.SumQty).ToList(), day);
                    ReturnsResult.Shunt(ConvertResult.產生OCS失敗, null);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.Write($"{FilePath} {ex}");
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
