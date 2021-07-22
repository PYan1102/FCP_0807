using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Diagnostics;
using System.Globalization;

namespace FCP
{
    class FMT_YiSheng:FormatCollection
    {
        public override string MethodShunt(int? MethodID)
        {
            return Do(OPD_Process(), OPD_Logic());
        }

        public override void Load(string inp, string oup, string filename, string time, Settings settings, Log log)
        {
            base.Load(inp, oup, filename, time, settings, log);
        }

        private ResultType OPD_Process()
        {
            var ecd = Encoding.Default;
            try
            {
                oncube = new OnputType_OnCube(log);
                List<string> patientinfo = new List<string>();
                string[] info = Content_S.Split('\n');
                info.ToList().ForEach(x => patientinfo.Add(x));
                Byte[] Temp = Encoding.Default.GetBytes(patientinfo[0]);
                for (int r = 0; r <= patientinfo.Count - 1; r++)  //將藥品資料放入List<string>
                {
                    var ATemp = Encoding.Default.GetBytes(patientinfo[r]);
                    string[] AdminTimeTempList = ecd.GetString(ATemp, 137, 10).Split(' ');
                    if (AdminTimeTempList[0].Contains("/"))
                    {
                        int Location = AdminTimeTempList[0].IndexOf("/");
                        AdminCode_S = AdminTimeTempList[0].Substring(0, Location) + AdminTimeTempList[0].Substring(Location + 1, AdminTimeTempList[0].Length - Location - 1).Trim();
                    }
                    else
                        AdminCode_S = AdminTimeTempList[0].Trim();
                    if (Settings.EN_FilterMedicineCode && !MedicineCodeGiven_L.Contains(ecd.GetString(ATemp, 63, 10).Trim()))
                        continue;
                    if (JudgePackedMode(AdminCode_S))
                        continue;
                    if (!GOD.Is_Admin_Code_For_Multi_Created(AdminCode_S))
                    {
                        log.Write($"{FullFileName_S} 在OnCube中未建置此餐包頻率 {AdminCode_S}");
                        LoseContent = $"{FullFileName_S} 在OnCube中未建置此餐包頻率 {AdminCode_S} 的頻率";
                        return ResultType.沒有頻率;
                    }
                    string DateTemp = (Convert.ToInt32(ecd.GetString(ATemp, 56, 7)) + 19110000).ToString();
                    DateTime.TryParseExact(DateTemp, "yyyyMMdd", null, DateTimeStyles.None, out DateTime _StartDate);
                    PatientName_S = ecd.GetString(ATemp, 6, 20).Trim();  //病患姓名
                    if (PatientName_S.Contains("?"))
                        PatientName_S = PatientName_S.Replace("?", " ");
                    PatientNo_S = ecd.GetString(ATemp, 26, 10).Trim();  //病歷號
                    ID_S = ecd.GetString(ATemp, 36, 10);  //身分證
                    Age_S = Convert.ToInt32(ecd.GetString(ATemp, 46, 10).Substring(0, ecd.GetString(ATemp, 46, 10).IndexOf("."))).ToString();
                    StartDay_L.Add(_StartDate.ToString("yyMMdd"));  //開始日期
                    MedicineCode_L.Add(ecd.GetString(ATemp, 63, 10).Trim());  //藥品代碼
                    MedicineName_L.Add(ecd.GetString(ATemp, 73, 54).Trim());  //藥品名
                    PerQty_L.Add(ecd.GetString(ATemp, 127, 10).Trim());  //劑量
                    AdminCode_L.Add(AdminCode_S);  //頻率
                    Days_L.Add(ecd.GetString(ATemp, 147, 3).Trim());  //天數
                    SumQty_L.Add(ecd.GetString(ATemp, 150, 10).Trim());  //總量
                    DateTime dt2 = DateTime.Parse(_StartDate.ToString("yyyy/MM/dd"));
                    EndDay_L.Add(dt2.AddDays(Int32.Parse(ecd.GetString(ATemp, 147, 3)) - 1).ToString("yyMMdd"));
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
        }  //抓資料及輸出

        private ResultType OPD_Logic()
        {
            if (AdminCode_L.Count == 0)
                return ResultType.全數過濾;
            try
            {
                bool yn = false;
                string FileName = $"{Path.GetFileNameWithoutExtension(FullFileName_S)}_{Time_S}.txt";
                string FileNameOutputCount = $@"{OutputPath_S}\{PatientName_S.Trim()}-{FileName}_{Time_S}.txt";
                string Birthdaynew = "1999-01-01";  //生日
                yn = oncube.YiSheng(MedicineName_L, MedicineCode_L, AdminCode_L, PerQty_L, SumQty_L, StartDay_L, EndDay_L, FileNameOutputCount, Settings,
                    PatientName_S, Birthdaynew, PatientNo_S);
                if (yn)
                    return ResultType.成功;
                else
                {
                    List<string> day = new List<string>();
                    for (int x = 0; x <= StartDay_L.Count - 1; x++)
                        day.Add(StartDay_L[x] + "~" + EndDay_L[x]);
                    log.Prescription(FullFileName_S, PatientName_S, PatientNo_S, MedicineCode_L, MedicineName_L, AdminCode_L, PerQty_L, day);
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
