using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using FCP.MVVM.Models.Enum;

namespace FCP
{
    class FMT_YiSheng : FormatCollection
    {
        public override bool ProcessOPD()
        {
            var ecd = Encoding.Default;
            try
            {
                oncube = new OnputType_OnCube(Log);
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
                    if (SettingsModel.EN_FilterMedicineCode && !MedicineCodeGiven_L.Contains(ecd.GetString(ATemp, 63, 10).Trim()))
                        continue;
                    if (JudgePackedMode(AdminCode_S))
                        continue;
                    if (!GOD.Is_Admin_Code_For_Multi_Created(AdminCode_S))
                    {
                        Log.Write($"{FullFileName_S} 在OnCube中未建置此餐包頻率 {AdminCode_S}");
                        ReturnsResult.Shunt(ConvertResult.沒有餐包頻率, AdminCode_S);
                        return false;
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
                string FileName = $"{Path.GetFileNameWithoutExtension(FullFileName_S)}_{Time_S}.txt";
                string FileNameOutputCount = $@"{OutputPath_S}\{PatientName_S.Trim()}-{FileName}_{Time_S}.txt";
                string Birthdaynew = "1999-01-01";  //生日
                bool result = oncube.YiSheng(MedicineName_L, MedicineCode_L, AdminCode_L, PerQty_L, SumQty_L, StartDay_L, EndDay_L, FileNameOutputCount,
                    PatientName_S, Birthdaynew, PatientNo_S);
                if (result)
                {
                    return true;
                }
                else
                {
                    List<string> day = new List<string>();
                    for (int x = 0; x <= StartDay_L.Count - 1; x++)
                        day.Add(StartDay_L[x] + "~" + EndDay_L[x]);
                    Log.Prescription(FullFileName_S, PatientName_S, PatientNo_S, MedicineCode_L, MedicineName_L, AdminCode_L, PerQty_L, day);
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
