using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Globalization;
using System.Collections.ObjectModel;
using FCP.MVVM.Models.Enum;
using FCP.MVVM.Models;

namespace FCP
{
    class FMT_MinSheng : FormatCollection
    {
        OLEDB OLEDB = new OLEDB();
        Stopwatch sw = new Stopwatch();
        Dictionary<string, decimal> TotalQuantityDic = new Dictionary<string, decimal>();
        ObservableCollection<OLEDB.MinSheng_UD> MS_UD;
        ObservableCollection<OLEDB.MinSheng_OPD> MS_OPD;
        List<string> FilterUnit = new List<string>() { "TUBE", "SUPP", "PCE", "BOT", "BTL", "FLEXPEN", "個", "IU", "適量", "CC" };
        int OLEDBIndex = 1;
        int OLEDBCount = 0;
        string Type;

        public override ReturnsResultFormat MethodShunt()
        {
            newCount = 0;
            return base.MethodShunt();
        }

        public int Index
        {
            get { return OLEDBIndex; }
            set { OLEDBIndex = value; }
        }

        public int newCount
        {
            get { return OLEDBCount; }
            set { OLEDBCount = value; }
        }

        public override bool ProcessOPD()
        {
            try
            {
                OLEDB_OPD();
                List<int> Remove = new List<int>();
                foreach (var v in MS_OPD)
                {
                    if (SettingsModel.EN_FilterMedicineCode && !MedicineCodeGiven_L.Contains(v.MedicineCode))
                    {
                        Remove.Add(MS_OPD.IndexOf(v));
                        continue;
                    }
                    if (JudgePackedMode(v.AdminCode))
                    {
                        Remove.Add(MS_OPD.IndexOf(v));
                        continue;
                    }
                    if (FilterUnit.Contains(v.Unit))
                    {
                        Remove.Add(MS_OPD.IndexOf(v));
                        continue;
                    }
                    //if (!int.TryParse(float.Parse(v.PerQty).ToString("0.##"), out int i))  //劑量為小數點不包
                    //{
                    //    Remove.Add(MS_OPD.IndexOf(v));
                    //    continue;
                    //}
                    if (!CheckMultiCode(v.AdminCode))
                    {
                        newCount = 0;
                        Log.Write($"{FullFileName_S} 在OnCube中未建置此餐包頻率 {v.AdminCode}");
                        FailMessage = $"{FullFileName_S} 在OnCube中未建置此餐包頻率 {v.AdminCode} 的頻率";
                        return ConvertResult.沒有頻率;
                    }
                }
                for (int x = Remove.Count - 1; x >= 0; x--)
                {
                    MS_OPD.RemoveAt(Remove[x]);
                }
                if (MS_OPD.Count == 0)
                    return ConvertResult.全數過濾;
                return ConvertResult.成功;
            }
            catch (Exception ex)
            {
                Log.Write($"{FullFileName_S}  {ex}");
                newCount = 0;
                ErrorContent = $"{FullFileName_S} 讀取處方籤時發生問題 {ex}";
                return ConvertResult.失敗;

            }
        }

        public override bool LogicOPD()
        {
            if (MS_OPD.Count == 0)
                return ConvertResult.全數過濾;
            try
            {
                oncube = new OnputType_OnCube(Log);
                bool yn;
                string FileName = Path.GetFileNameWithoutExtension(FullFileName_S);
                int Count = MS_OPD.Count;
                FileNameOutput_S = $@"{OutputPath_S}\{FileName}_{Time_S}.txt";
                yn = oncube.MinSheng_OPD(FileNameOutput_S, MS_OPD, Type);
                Debug.WriteLine($"總量 {MS_OPD.Count}  耗時 {sw.ElapsedMilliseconds}");
                if (yn)
                    return ConvertResult.成功;
                else
                {
                    Log.Prescription(FullFileName_S, MS_OPD.ToList().Select(x => x.PatientName).ToList(), MS_OPD.ToList().Select(x => x.PrescriptionNo).ToList(), MS_OPD.ToList().Select(x => x.MedicineCode).ToList(), MS_OPD.ToList().Select(x => x.MedicineName).ToList(),
                        MS_OPD.ToList().Select(x => x.AdminCode).ToList(), MS_OPD.ToList().Select(x => x.PerQty).ToList(), MS_OPD.ToList().Select(x => x.SumQty).ToList(), MS_OPD.ToList().Select(x => x.StartDay).ToList());
                    newCount = 0;
                    ErrorContent = $"{FullFileName_S} 產生OCS時發生問題";
                    return ConvertResult.失敗;
                }
            }
            catch (Exception ex)
            {
                Log.Write($"{FullFileName_S}  {ex}");
                newCount = 0;
                ErrorContent = $"{FullFileName_S} 處理邏輯時發生問題 {ex}";
                return ConvertResult.失敗;
            }
        }

        public override bool ProcessUDBatch()
        {
            try
            {
                OLEDB_UD();
                ClearList();
                List<int> Remove = new List<int>();
                foreach (var v in MS_UD)
                {
                    if (SettingsModel.EN_FilterMedicineCode && !MedicineCodeGiven_L.Contains(v.MedicineCode))
                    {
                        Remove.Add(MS_UD.IndexOf(v));
                        continue;
                    }
                    if (JudgePackedMode(v.AdminCode))
                    {
                        Remove.Add(MS_UD.IndexOf(v));
                        continue;
                    }
                    if (v.Unit == "BAG")
                    {
                        Remove.Add(MS_UD.IndexOf(v));
                        continue;
                    }
                    //if (!int.TryParse(float.Parse(v.Dosage).ToString("0.##"), out int i))  //劑量為小數點不包
                    //{
                    //    Remove.Add(MS_UD.IndexOf(v));
                    //    continue;
                    //}
                    //Debug.WriteLine($"{v.PrescriptionNo}|{v.MedicineName}{v.StartDay}{v.BeginTime}");
                    if (TotalQuantityDic.Count == 0 || !TotalQuantityDic.ContainsKey($"{v.PrescriptionNo}|{v.MedicineName}{v.StartDay}{v.BeginTime}"))
                    {
                        //Debug.WriteLine("New");
                        TotalQuantityDic[$"{v.PrescriptionNo}|{v.MedicineName}{v.StartDay}{v.BeginTime}"] = decimal.Parse(v.PerQty);
                    }
                    else
                    {
                        //Debug.WriteLine("Old");
                        TotalQuantityDic[$"{v.PrescriptionNo}|{v.MedicineName}{v.StartDay}{v.BeginTime}"] += decimal.Parse(v.PerQty);
                        Remove.Add(MS_UD.IndexOf(v));
                        continue;
                    }
                    if (!GOD.Is_Admin_Code_For_Multi_Created(v.AdminCode))
                    {
                        newCount = 0;
                        Log.Write($"{FullFileName_S} 在OnCube中未建置此餐包頻率 {v.AdminCode}");
                        FailMessage = $"{FullFileName_S} 在OnCube中未建置此餐包頻率 {v.AdminCode} 的頻率";
                        return ConvertResult.沒有頻率;
                    }
                }
                for (int x = Remove.Count - 1; x >= 0; x--)
                {
                    MS_UD.RemoveAt(Remove[x]);
                }
                if (MS_UD.Count == 0)
                {
                    newCount = 0;
                    return ConvertResult.全數過濾;
                }
                return ConvertResult.成功;
            }
            catch (Exception ex)
            {
                Log.Write($"{FullFileName_S}  {ex}");
                newCount = 0;
                ErrorContent = $"{FullFileName_S} 讀取處方籤時發生問題 {ex}";
                return ConvertResult.失敗;
            }
        }

        public override bool LogicUDBatch()
        {
            if (MS_UD.Count == 0)
            {
                newCount = 0;
                return ConvertResult.全數過濾;
            }
            try
            {
                //Debug.WriteLine(TotalQuantityDic.First());
                //Debug.WriteLine($"{MS_UD[0].PrescriptionNo}|{MS_UD[0].MedicineName}{MS_UD[0].StartDay}{MS_UD[0].BeginTime}");
                oncube = new OnputType_OnCube(Log);
                bool yn;
                string FileName = Path.GetFileNameWithoutExtension(FullFileName_S);
                int Count = MS_UD.Count;
                int Times = 0;
                int CurrentTimes = 0;
                List<string> AdminTimeList = new List<string>();
                FileNameOutput_S = $@"{OutputPath_S}\{FileName}_{Time_S}.txt";
                for (int i = 0; i <= Count - 1; i++)
                {
                    if (SettingsModel.CrossDayAdminCode.Contains(MS_UD[i].AdminCode))
                    {
                        DataDic[$"{i}_{MS_UD[i].StartDay}"] = new List<string>() { "Combi" };
                        continue;
                    }
                    CurrentTimes = 0;
                    if (TotalQuantityDic.TryGetValue($"{MS_UD[i].PrescriptionNo}|{MS_UD[i].MedicineName}{MS_UD[i].StartDay}{MS_UD[i].BeginTime}", out decimal TotalQuantity))
                        Times = (int)(TotalQuantity / Convert.ToDecimal(MS_UD[i].PerQty));
                    AdminTimeList = GOD.Get_Admin_Code_For_Multi(MS_UD[i].AdminCode);
                    DateTime.TryParseExact($"{MS_UD[i].StartDay} {MS_UD[i].BeginTime}", "yyMMdd HHmm", null, DateTimeStyles.None, out DateTime StartTime);
                    DateTime DateTemp = StartTime;
                    //Debug.WriteLine($"{MedicineName_L[i]} {AdminTime_L[i]}");
                    //Debug.WriteLine($"日期 {DateTemp}");
                    while (CurrentTimes < Times)
                    {
                        DataDic[$"{i}_{DateTemp:yyMMdd}"] = new List<string>();
                        foreach (string s in AdminTimeList)
                        {
                            if (CurrentTimes == Times)
                                break;
                            DateTime.TryParseExact($"{MS_UD[i].StartDay} {s}", "yyMMdd HH:mm", null, DateTimeStyles.None, out DateTime AdminTime);
                            //Debug.WriteLine($"時間點 {s}");
                            if (DateTime.Compare(StartTime, AdminTime) <= 0 & DateTemp == StartTime)
                            {
                                //Debug.WriteLine($"符合 {s}");
                                DataDic[$"{i}_{DateTemp:yyMMdd}"].Add(s.Substring(0, 2));
                                CurrentTimes++;
                                continue;
                            }
                            if (StartTime.CompareTo(DateTemp) == -1)
                            {
                                //Debug.WriteLine($"符合 {s}");
                                DataDic[$"{i}_{DateTemp:yyMMdd}"].Add(s.Substring(0, 2));
                                CurrentTimes++;
                                continue;
                            }
                        }
                        DateTemp = Convert.ToDateTime($"{DateTemp:yyyy/MM/dd} 00:00:00");
                        DateTemp = DateTemp.AddDays(1);
                        //Debug.WriteLine($"日期 {DateTemp}");
                    }
                }
                //MS_UD.ToList().ForEach(x => Debug.WriteLine(x.PrescriptionNo));
                yn = oncube.MinSheng_UD(DataDic, FileNameOutput_S, MS_UD, Type);
                //Debug.WriteLine($"總量 {MS_UD.Count}  耗時 {sw.ElapsedMilliseconds}");
                if (yn)
                    return ConvertResult.成功;
                else
                {
                    List<string> day = new List<string>();
                    StartDay_L.ForEach(x => day.Add(x));
                    Log.Prescription(FullFileName_S, PatientName_L, PrescriptionNo_L, MedicineCode_L, MedicineName_L, AdminCode_L, PerQty_L, SumQty_L, day);
                    newCount = 0;
                    ErrorContent = $"{FullFileName_S} 產生OCS時發生問題";
                    return ConvertResult.失敗;
                }
            }
            catch (Exception ex)
            {
                Log.Write($"{FullFileName_S}  {ex}");
                newCount = 0;
                ErrorContent = $"{FullFileName_S} 處理邏輯時發生問題 {ex}";
                return ConvertResult.失敗;
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

        private void OLEDB_UD()
        {
            try
            {
                MS_UD = OLEDB.GetMingSheng_UD(InputPath_S, FullFileName_S, Index);
                newCount = MS_UD.Count > 0 ? MS_UD[MS_UD.Count - 1].RecNo : 0;
                //newCount = 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"民生OLEDB_UD讀取發生錯誤");
                Log.Write($"{ex}");
            }
        }

        private void OLEDB_OPD()
        {
            try
            {
                MS_OPD = OLEDB.GetMingSheng_OPD(InputPath_S, FullFileName_S, Index);
                newCount = MS_OPD.Count > 0 ? MS_OPD[MS_OPD.Count - 1].RecNo + 1 : 0;
                //Debug.WriteLine($"The number of new count is {newCount}");
                //newCount = 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"民生OLEDB_OPD讀取發生錯誤");
                Log.Write($"{ex}");
            }
        }

        private void ClearList()
        {
            DataDic.Clear();
            TotalQuantityDic.Clear();
        }
    }
}