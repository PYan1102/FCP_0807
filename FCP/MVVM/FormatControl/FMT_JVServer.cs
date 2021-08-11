using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using FCP.MVVM.Models.Enum;

namespace FCP
{
    class FMT_JVServer : FormatCollection
    {
        string Random;
        List<string> JVServerRandom = new List<string>();
        List<string> OnCubeRandom = new List<string>();

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
                OnCube = new OnputType_OnCube(Log);
                string Content = GetContent.Trim();
                //判斷資料裡是否為JVSERVER的字樣
                int JVMIndex = Content.IndexOf("|JVPEND||JVMHEAD|");
                Byte[] ATemp = Encoding.Default.GetBytes(Content.Substring(9, JVMIndex - 9));  //病患基本資料
                Byte[] BTemp = Encoding.Default.GetBytes(Content.Substring(JVMIndex + 17, Content.Length - 17 - JVMIndex));  //病患藥品資料
                var ecd = Encoding.Default;
                PatientNo_S = ecd.GetString(ATemp, 1, 15).Trim(); //病歷號
                PrescriptionNo_S = ecd.GetString(ATemp, 16, 20).Trim(); //處方籤號碼
                Age_S = ecd.GetString(ATemp, 36, 5).Trim(); //年齡
                ID_S = ecd.GetString(ATemp, 54, 10).Trim(); //身分證字號
                BirthDate_S = ecd.GetString(ATemp, 94, 8).Trim(); //生日
                Random = ecd.GetString(ATemp, 132, 30).Trim(); //額外位置
                PatientName_S = ecd.GetString(ATemp, 177, 20).Trim(); //病患姓名

                if (PatientName_S.Contains("?"))
                    PatientName_S = PatientName_S.Replace("?", " ");
                Gender_S = ecd.GetString(ATemp, 197, 2).Trim();  //性別
                HospitalName_S = ecd.GetString(ATemp, 229, 40).Trim();  //醫院名稱
                Location_S = ecd.GetString(ATemp, 229, 30).Trim(); //醫院位置
                List<string> Count = JVS_Count(ecd.GetString(BTemp, 0, BTemp.Length), 547);  //計算有多少種藥品資料
                for (int r = 0; r <= Count.Count - 1; r++)  //將藥品資料放入List<string>
                {
                    Byte[] CTemp = Encoding.Default.GetBytes(Count[r].ToString());
                    AdminCode_S = ecd.GetString(CTemp, 66, 10).Trim();  //頻率
                    if (SettingsModel.EN_FilterMedicineCode && !MedicineCodeGiven_L.Contains(ecd.GetString(CTemp, 1, 15).Trim()))
                        continue;
                    if (JudgePackedMode(AdminCode_S))
                        continue;
                    if (!GOD.Is_Admin_Code_For_Multi_Created(AdminCode_S))
                    {
                        Log.Write($"{FilePath} 在OnCube中未建置此餐包頻率 {AdminCode_S}");
                        ReturnsResult.Shunt(ConvertResult.沒有餐包頻率, AdminCode_S);
                    }
                    MedicineCode_L.Add(ecd.GetString(CTemp, 1, 15).Trim());  //藥品代碼
                    MedicineName_L.Add(ecd.GetString(CTemp, 16, 50).Trim());  //藥品名稱
                    AdminCode_L.Add(ecd.GetString(CTemp, 66, 10).Trim());  //頻率
                    PerQty_L.Add(ecd.GetString(CTemp, 81, 6).Trim());  //劑量
                    SumQty_L.Add(ecd.GetString(CTemp, 87, 8).Trim()); //總量
                    int RandomPosition = 107;  //Random位置
                    for (int x = 0; x <= 9; x++)  //Random
                    {
                        string randomcache = ecd.GetString(CTemp, RandomPosition, 30).Trim();  //符合OnCube輸出
                        JVServerRandom.Add(randomcache);
                        RandomPosition += 40;
                    }
                    StartDay_L.Add(ecd.GetString(CTemp, 509, 6).Trim());  //開始日期
                    EndDay_L.Add(ecd.GetString(CTemp, 529, 6).Trim()); //結束日期
                }
                if (AdminCode_L.Count == 0)
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
            try
            {
                for (int x = 0; x <= 14; x++)
                    OnCubeRandom.Add("");
                if (!string.IsNullOrEmpty(SettingsModel.ExtraRandom))  //將JVServer的Random放入OnCube的Radnom
                {
                    int head;
                    int middle;
                    string[] randomlist = SettingsModel.ExtraRandom.Split(',');
                    foreach (string s in randomlist)
                    {
                        if (!string.IsNullOrEmpty(s))
                        {
                            head = s.IndexOf(':');
                            middle = s.IndexOf("&");
                            OnCubeRandom[Int32.Parse(s.Substring(middle + 1, s.Length - middle - 1)) - 1] = JVServerRandom[Int32.Parse(s.Substring(head + 1, middle - head - 1))];
                        }
                    }
                }
                bool yn;
                string FileNameOutputCount = $@"{OutputPath}\{PatientName_S.Trim()}-{Path.GetFileNameWithoutExtension(FilePath)}_{CurrentSeconds}.txt";
                DateTime.TryParseExact(BirthDate_S, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime _date);  //生日
                string Birthdaynew = _date.ToString("yyyy-MM-dd");
                yn = OnCube.JVS(MedicineName_L, MedicineCode_L, AdminCode_L, PerQty_L, SumQty_L, StartDay_L, EndDay_L, FileNameOutputCount, OnCubeRandom,
                        PatientName_S, PatientNo_S, HospitalName_S, Location_S, PrescriptionNo_S, Birthdaynew, Gender_S, Random);
                if (yn)
                    return true;
                else
                {
                    List<string> day = new List<string>();
                    for (int x = 0; x <= StartDay_L.Count - 1; x++)
                    {
                        day.Add(StartDay_L[x] + "~" + EndDay_L[x]);
                    }
                    Log.Prescription(FilePath, PatientName_S, PrescriptionNo_S, MedicineCode_L, MedicineName_L, AdminCode_L, PerQty_L, day);
                    ReturnsResult.Shunt(ConvertResult.產生OCS失敗, null);
                    return false;
                }
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
        private void ClearList()
        {
            JVServerRandom.Clear();
            OnCubeRandom.Clear();
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
