using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Diagnostics;

namespace FCP
{
    class FMT_JVServer:FormatCollection
    {
        string Random;
        List<string> JVServerRandom = new List<string>();
        List<string> OnCubeRandom = new List<string>();

        public override void Load(string inp, string oup, string filename, string time, Settings settings, Log log)
        {
            base.Load(inp, oup, filename, time, settings, log);
        }

        public override string MethodShunt(int? MethodID)
        {
            return Do(OPD_Process(), OPD_Logic());
        }

        private ResultType OPD_Process()
        {
            if (!File.Exists(FullFileName_S))
            {
                log.Write(FullFileName_S + "忽略");
                return ResultType.全數過濾;
            }
            try
            {
                ClearList();
                oncube = new OnputType_OnCube(log);
                string Content = GetContent.Trim();
                //判斷資料裡是否為JVSERVER的字樣
                int JVMIndex = Content.IndexOf("|JVPEND||JVMHEAD|");
                if (JVMIndex == -1)
                {
                    ErrorContent = $"{FullFileName_S}轉檔格式錯誤，輸入格式不是JVServer";
                    return ResultType.失敗;
                }
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
                    if (Settings.EN_FilterMedicineCode && !MedicineCodeGiven_L.Contains(ecd.GetString(CTemp, 1, 15).Trim()))
                        continue;
                    if (JudgePackedMode(AdminCode_S))
                        continue;
                    if (!GOD.Is_Admin_Code_For_Multi_Created(AdminCode_S))
                    {
                        log.Write($"{FullFileName_S} 在OnCube中未建置此餐包頻率 {AdminCode_S}");
                        LoseContent = $"{FullFileName_S} 在OnCube中未建置此餐包頻率 {AdminCode_S} 的頻率";
                        return ResultType.沒有頻率;
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

        private ResultType OPD_Logic()
        {
            if (AdminCode_L.Count == 0)
                return ResultType.全數過濾;
            try
            {
                for (int x = 0; x <= 14; x++)
                    OnCubeRandom.Add("");
                if (!string.IsNullOrEmpty(Settings.ExtraRandom))  //將JVServer的Random放入OnCube的Radnom
                {
                    int head;
                    int middle;
                    string[] randomlist = Settings.ExtraRandom.Split(',');
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
                string FileNameOutputCount = $@"{OutputPath_S}\{PatientName_S.Trim()}-{Path.GetFileNameWithoutExtension(FullFileName_S)}_{Time_S}.txt";
                DateTime.TryParseExact(BirthDate_S, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime _date);  //生日
                string Birthdaynew = _date.ToString("yyyy-MM-dd");
                yn = oncube.JVS(MedicineName_L, MedicineCode_L, AdminCode_L, PerQty_L, SumQty_L, StartDay_L, EndDay_L, FileNameOutputCount, Settings, OnCubeRandom,
                        PatientName_S, PatientNo_S, HospitalName_S, Location_S, PrescriptionNo_S, Birthdaynew, Gender_S, Random);
                if (yn)
                    return ResultType.成功;
                else
                {
                    List<string> day = new List<string>();
                    for (int x = 0; x <= StartDay_L.Count - 1; x++)
                    {
                        day.Add(StartDay_L[x] + "~" + EndDay_L[x]);
                    }
                    log.Prescription(FullFileName_S, PatientName_S, PrescriptionNo_S, MedicineCode_L, MedicineName_L, AdminCode_L, PerQty_L, day);
                    ErrorContent = $"{FullFileName_S} 產生OCS時發生問題";
                    return ResultType.失敗;
                }
            }
            catch (Exception ex)
            {
                log.Write($"{FullFileName_S}  {ex}");
                ErrorContent = $"{FullFileName_S} 處理邏輯時發生問題 {ex}";
                return ResultType.失敗;
            }

        }

        private void ClearList()
        {
            JVServerRandom.Clear();
            OnCubeRandom.Clear();
        }
    }
}
