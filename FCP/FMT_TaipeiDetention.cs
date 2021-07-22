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
    class FMT_TaipeiDetention : FormatCollection
    {
        string Random;
        List<string> PutBackAdminCode = new List<string>() { "Q4H", "Q6H", "Q8H", "Q12H", "QDPRN", "QIDPRN", "PRN", "BIDPRN", "TIDPRN", "HSPRN" };

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
                List<string> Count = JVS_Count(ecd.GetString(BTemp, 0, BTemp.Length), 106);  //計算有多少種藥品資料
                for (int r = 0; r <= Count.Count - 1; r++)  //將藥品資料放入List<string>
                {
                    Byte[] CTemp = Encoding.Default.GetBytes(Count[r].ToString());
                    string[] AdminCodeTemp = ecd.GetString(CTemp, 66, 10).Trim().Split(' ');
                    AdminCode_S = "";
                    AdminCodeTemp.ToList().ForEach(x => AdminCode_S += x);
                    AdminCode_S = AdminCode_S.ToUpper();
                    if (NeedFilterMedicineCode(ecd.GetString(CTemp, 1, 15).Trim()))
                        continue;
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
                    AdminCode_L.Add(AdminCode_S);  //頻率
                    Days_L.Add(ecd.GetString(CTemp, 76, 3).Trim());
                    PerQty_L.Add(ecd.GetString(CTemp, 81, 6).Trim());  //劑量
                    SumQty_L.Add(ecd.GetString(CTemp, 87, 8).Trim()); //總量
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
                bool yn;
                string FileNameOutputCount = $@"{OutputPath_S}\{PatientName_S}-{PatientNo_S}-{Random}_{Time_S}.txt";
                DateTime.TryParseExact(BirthDate_S, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime _date);  //生日
                string Birthdaynew = _date.ToString("yyyy-MM-dd");
                yn = oncube.TaipeiDentention(MedicineName_L, MedicineCode_L, AdminCode_L, Days_L, PerQty_L, SumQty_L, FileNameOutputCount, Settings,
                        PatientName_S, PatientNo_S, HospitalName_S, Location_S, PrescriptionNo_S, Birthdaynew, Gender_S, Random, PutBackAdminCode);
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
    }
}
