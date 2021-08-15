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
                    if (IsExistsMedicineCode(v.MedicineCode))
                    {
                        Remove.Add(MS_OPD.IndexOf(v));
                        continue;
                    }
                    if (IsFilterAdminCode(v.AdminCode))
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
                    if (!IsExistsMultiAdminCode(v.AdminCode))
                    {
                        newCount = 0;
                        Log.Write($"{FilePath} 在OnCube中未建置此餐包頻率 {v.AdminCode}");
                        ReturnsResult.Shunt(ConvertResult.沒有餐包頻率, AdminCode_S);
                        return false;
                    }
                }
                for (int x = Remove.Count - 1; x >= 0; x--)
                {
                    MS_OPD.RemoveAt(Remove[x]);
                }
                if (MS_OPD.Count == 0)
                {
                    ReturnsResult.Shunt(ConvertResult.全數過濾, null);
                    return false;
                }
                    
                return true;
            }
            catch (Exception ex)
            {
                Log.Write($"{FilePath}  {ex}");
                newCount = 0;
                ReturnsResult.Shunt(ConvertResult.讀取檔案失敗, ex.ToString());
                return false;

            }
        }

        public override bool LogicOPD()
        {      
            try
            {
                bool yn;
                string FileName = Path.GetFileNameWithoutExtension(FilePath);
                int Count = MS_OPD.Count;
                FileNameOutput_S = $@"{OutputPath}\{FileName}_{CurrentSeconds}.txt";
                yn = OP_OnCube.MinSheng_OPD(FileNameOutput_S, MS_OPD, Type);
                Debug.WriteLine($"總量 {MS_OPD.Count}  耗時 {sw.ElapsedMilliseconds}");
                if (yn)
                    return true;
                else
                {
                    newCount = 0;
                    ReturnsResult.Shunt(ConvertResult.產生OCS失敗, null);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.Write($"{FilePath}  {ex}");
                newCount = 0;
                ReturnsResult.Shunt(ConvertResult.處理邏輯失敗, ex.ToString());
                return false;
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
                    if (IsExistsMedicineCode(v.MedicineCode))
                    {
                        Remove.Add(MS_UD.IndexOf(v));
                        continue;
                    }
                    if (IsFilterAdminCode(v.AdminCode))
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
                    if (!IsExistsMultiAdminCode(v.AdminCode))
                    {
                        newCount = 0;
                        Log.Write($"{FilePath} 在OnCube中未建置此餐包頻率 {v.AdminCode}");
                        ReturnsResult.Shunt(ConvertResult.沒有餐包頻率, AdminCode_S);
                        return false;
                    }
                }
                for (int x = Remove.Count - 1; x >= 0; x--)
                {
                    MS_UD.RemoveAt(Remove[x]);
                }
                if (MS_UD.Count == 0)
                {
                    newCount = 0;
                    ReturnsResult.Shunt(ConvertResult.全數過濾, null);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Write($"{FilePath}  {ex}");
                newCount = 0;
                ReturnsResult.Shunt(ConvertResult.讀取檔案失敗, ex.ToString());
                return false;
            }
        }

        public override bool LogicUDBatch()
        {
            try
            {
                bool yn;
                string FileName = Path.GetFileNameWithoutExtension(FilePath);
                int Count = MS_UD.Count;
                int Times = 0;
                int CurrentTimes = 0;
                List<string> AdminTimeList = new List<string>();
                FileNameOutput_S = $@"{OutputPath}\{FileName}_{CurrentSeconds}.txt";
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
                    AdminTimeList = GetMultiAdminCodeTimes(MS_UD[i].AdminCode);
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
                yn = OP_OnCube.MinSheng_UD(DataDic, FileNameOutput_S, MS_UD, Type);
                //Debug.WriteLine($"總量 {MS_UD.Count}  耗時 {sw.ElapsedMilliseconds}");
                if (yn)
                    return true;
                else
                {
                    newCount = 0;
                    ReturnsResult.Shunt(ConvertResult.產生OCS失敗, null);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.Write($"{FilePath}  {ex}");
                newCount = 0;
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

        private void OLEDB_UD()
        {
            try
            {
                MS_UD = OLEDB.GetMingSheng_UD(InputPath, FilePath, Index);
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
                MS_OPD = OLEDB.GetMingSheng_OPD(InputPath, FilePath, Index);
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