using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using FCP.MVVM.Models.Enum;

namespace FCP
{
    class FMT_HongYen : FormatCollection
    {
        string Random;
        bool isTwo = false;
        bool isSpace = false;
        int Position = 0;
        bool _IsSkip = false;

        public override bool ProcessOPD()
        {
            try
            {
                _IsSkip = false;
                string Content = GetContent;
                //判斷資料裡是否為JVSERVER的字樣
                int LabelPosition = Content.IndexOf("|JVPEND||JVMHEAD|");
                Byte[] Patient = Encoding.Default.GetBytes(Content.Substring(9, LabelPosition - 9));  //病患基本資料
                Byte[] MedicineData = Encoding.Default.GetBytes(Content.Substring(LabelPosition + 17, Content.Length - 17 - LabelPosition));  //病患藥品資料
                var ecd = Encoding.Default;
                string FileName = Path.GetFileNameWithoutExtension(FullFileName_S);
                PatientName_S = ecd.GetString(Patient, 177, 20).Trim(); //病患姓名
                PatientName_S = PatientName_S.Replace('?', ' ');
                //病患基本資料
                PatientNo_S = ecd.GetString(Patient, 1, 15).Trim(); //病歷號
                PrescriptionNo_S = ecd.GetString(Patient, 16, 20).Trim(); //處方籤號碼
                Age_S = ecd.GetString(Patient, 36, 5).Trim(); //年齡
                ID_S = ecd.GetString(Patient, 54, 10).Trim(); //身分證字號
                BirthDate_S = ecd.GetString(Patient, 94, 8).Trim(); //生日
                Random = ecd.GetString(Patient, 132, 30).Trim(); //額外位置
                Gender_S = ecd.GetString(Patient, 197, 2).Trim();  //性別
                HospitalName_S = ecd.GetString(Patient, 229, 40).Trim();  //醫院名稱
                Location_S = ecd.GetString(Patient, 229, 30).Trim(); //醫院位置
                DoctorName_S = ecd.GetString(Patient, 269, 20).Trim(); //醫生姓名

                //計算有多少種藥品資料
                List<string> MedicineCount = JVS_Count(ecd.GetString(MedicineData, 0, MedicineData.Length), 547);
                isTwo = false;
                isSpace = false;
                Position = 0;
                int x = 0;
                for (int r = 0; r <= MedicineCount.Count - 1; r++)
                {
                    Byte[] Medicine = Encoding.Default.GetBytes(MedicineCount[r].ToString());
                    AdminCode_S = ecd.GetString(Medicine, 66, 10).Trim();  //頻率
                    if (ecd.GetString(Medicine, 16, 50).Trim() == "磨粉.")
                    {
                        _IsSkip = true;
                        ReturnsResult.Shunt(ConvertResult.全數過濾, null);
                        return false;
                    }
                    if (ecd.GetString(Medicine, 1, 15).Trim() == "")
                    {
                        isSpace = true;
                        continue;
                    }
                    if (SettingsModel.EN_FilterMedicineCode && !MedicineCodeGiven_L.Contains(ecd.GetString(Medicine, 1, 15).Trim()))
                        continue;
                    if (JudgePackedMode(AdminCode_S) & ecd.GetString(Medicine, 1, 15).Trim() != "")
                        continue;
                    if (isSpace & Position == 0 & !isTwo)
                    {
                        isTwo = true;
                        Position = x;
                    }
                    x++;
                    MedicineCode_L.Add(ecd.GetString(Medicine, 1, 15).Trim());  //藥品代碼
                    MedicineName_L.Add(ecd.GetString(Medicine, 16, 50).Trim());  //藥品名稱
                    AdminCode_L.Add(ecd.GetString(Medicine, 66, 10).Trim());  //頻率
                    Days_L.Add(ecd.GetString(Medicine, 76, 3).Trim());
                    PerQty_L.Add(ecd.GetString(Medicine, 81, 6).Trim());  //劑量
                    SumQty_L.Add(ecd.GetString(Medicine, 87, 8).Trim()); //總量
                    StartDay_L.Add(ecd.GetString(Medicine, 509, 6).Trim());  //開始日期
                    EndDay_L.Add(ecd.GetString(Medicine, 529, 6).Trim()); //結束日期
                    //string ChangeDays = ecd.GetString(Medicine, 106, 20).Trim();
                    //if (ChangeDays != "1" & ChangeDays != "0" & Int32.TryParse(ChangeDays, out int i))
                    //{
                    //    DateTime.TryParseExact(ecd.GetString(Medicine, 509, 6), "yyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime A);
                    //    A = A.AddDays(Convert.ToInt32(ecd.GetString(Medicine, 106, 20).Trim()) - 1);
                    //    EndDay_L[x - 1] = A.ToString("yyMMdd");
                    //}
                }
                if (_IsSkip || AdminCode_L.Count == 0)
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

        public override bool LogicOPD()
        {
            try
            {
                bool yn;
                string FileName = Path.GetFileNameWithoutExtension(FullFileName_S);
                List<int> order = new List<int>();
                List<string> FileNameOutputCount = new List<string>();
                List<string> OnCubeRandom = new List<string>();
                List<int> UP = new List<int>();
                List<int> DOWN = new List<int>();

                if (Position == 0 & !isTwo)
                {
                    for (int x = 0; x <= AdminCode_L.Count - 1; x++)
                    {
                        UP.Add(x);
                    }
                }
                else if (Position != 0)
                {
                    for (int x = 0; x <= AdminCode_L.Count - 1; x++)
                    {
                        if (x < Position)
                        {
                            UP.Add(x);
                        }
                        else
                            DOWN.Add(x);
                    }
                }
                else if (Position == 0 & isTwo)
                {
                    for (int x = 0; x <= AdminCode_L.Count - 1; x++)
                        DOWN.Add(x);
                }
                List<string> adminCodeTemp = new List<string>();
                List<string> filterAdminCode = new List<string>();
                List<string> filterDays = new List<string>();
                foreach (int i in UP)
                {
                    adminCodeTemp.Add(AdminCode_L[i]);
                }
                var v = from s in adminCodeTemp
                        group s by s into g
                        select new
                        {
                            g.Key,
                            Num = g.Count()
                        };
                foreach (var vv in v)
                {
                    if (vv.Num >= 2)
                        filterAdminCode.Add(vv.Key);
                }
                List<string> filterDay = new List<string>();
                foreach (int i in UP)
                {
                    if (!filterAdminCode.Contains(AdminCode_L[i]))
                        continue;
                    if (filterDay.Contains($"{Days_L[i]}*"))
                        continue;
                    else if (filterDay.Contains(Days_L[i]))
                    {
                        filterDay[filterDay.IndexOf(Days_L[i])] += "*";
                    }
                    else
                        filterDay.Add(Days_L[i]);
                }
                List<string> filterDaylist = filterDay.Where(x => !x.Contains("*")).Select(x => x).ToList();
                if (filterAdminCode.Count == 0 & DOWN.Count <= 1)
                {
                    ReturnsResult.Shunt(ConvertResult.全數過濾, null);
                    return false;
                }
                    FileNameOutputCount.Add($@"{OutputPath_S}\UP-{PatientName_S}-{FileName}_{Time_S}.txt");
                FileNameOutputCount.Add($@"{OutputPath_S}\DOWN-{PatientName_S}-{FileName}_{Time_S}.txt");
                DateTime.TryParseExact(BirthDate_S, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime _date);  //生日
                string Birthdate = _date.ToString("yyyy-MM-dd");
                oncube = new OnputType_OnCube(Log);
                yn = oncube.HongYen(UP, DOWN, filterDaylist, filterAdminCode, MedicineName_L, MedicineCode_L, AdminCode_L, PerQty_L, SumQty_L, StartDay_L, EndDay_L, FileNameOutputCount, OnCubeRandom, Days_L,
                    PatientName_S, PatientNo_S, HospitalName_S, Location_S, DoctorName_S, PrescriptionNo_S, Birthdate, Gender_S, Random, isTwo, Position);
                if (yn)
                    return true;
                else
                {
                    List<string> day = new List<string>();
                    for (int x = 0; x <= StartDay_L.Count - 1; x++)
                    {
                        day.Add(StartDay_L[x] + "~" + EndDay_L[x]);
                    }
                    Log.Prescription(FullFileName_S, PatientName_S, PrescriptionNo_S, MedicineCode_L, MedicineName_L, AdminCode_L, PerQty_L, day);
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
    }
}
