using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using FCP.MVVM.Models.Enum;

namespace FCP
{
    class FMT_ChuangSheng : FormatCollection
    {
        OnputType_OnCube on = null;

        public override bool ProcessOPD()
        {
            try
            {
                var ecd = Encoding.Default;
                on = new OnputType_OnCube(Log);
                List<string> patientInfo = new List<string>();
                BirthDate_S = "19970101";
                string fileContent = GetContent;
                fileContent.Split('\n').ToList().ForEach(x => patientInfo.Add(x.TrimEnd()));
                for (int r = 0; r <= patientInfo.Count - 2; r++)  //將藥品資料放入List<string>
                {
                    var temp = Encoding.Default.GetBytes(patientInfo[r]);
                    AdminCode_S = ecd.GetString(temp, 74, 8).Trim();
                    if (SettingsModel.EN_FilterMedicineCode && !MedicineCodeGiven_L.Contains(ecd.GetString(temp, 52, 10).Trim()))
                        continue;
                    if (JudgePackedMode(AdminCode_S))
                        continue;
                    if (!GOD.Is_Admin_Code_For_Multi_Created(AdminCode_S))
                    {
                        Log.Write($"{FilePath} 在OnCube中未建置此餐包頻率 {AdminCode_S}");
                        ReturnsResult.Shunt(ConvertResult.沒有餐包頻率, null);
                        return false;
                    }
                    DateTime startDate = DateTime.Parse(ecd.GetString(temp, 20, 10));
                    StartDay_L.Add(startDate.ToString("yyMMdd"));  //調劑日期
                    Class_S = ecd.GetString(temp, 30, 12).Trim();  //科別
                    PatientName_S = ecd.GetString(temp, 42, 10).Trim();  //病患姓名
                    if (PatientName_S.Contains("?"))
                        PatientName_S = PatientName_S.Replace("?", " ");
                    MedicineCode_L.Add(ecd.GetString(temp, 52, 10).Trim());  //藥品代碼
                    PerQty_L.Add(ecd.GetString(temp, 62, 6).Trim());  //劑量
                    Times_L.Add(ecd.GetString(temp, 68, 6).Trim());  //每日幾次
                    AdminCode_L.Add(ecd.GetString(temp, 74, 8).Trim());  //頻率
                    Days_L.Add(ecd.GetString(temp, 82, 3).Trim());  //天數
                    SumQty_L.Add(ecd.GetString(temp, 85, 10).Trim());  //總量
                    int totalLength = temp.Length;
                    if (totalLength - 95 <= 50)
                    {
                        MedicineName_L.Add(ecd.GetString(temp, 95, totalLength - 95).Trim());
                        HospitalName_S = "";
                    }
                    else
                    {
                        MedicineName_L.Add(ecd.GetString(temp, 95, 50).Trim());
                        HospitalName_S = ecd.GetString(temp, 145, 20).Trim();
                    }
                    EndDay_L.Add(startDate.AddDays(Convert.ToInt32(Days_L[Days_L.Count - 1]) - 1).ToString("yyMMdd"));
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
                ReturnsResult.Shunt(ConvertResult.讀取檔案失敗, null);
                return false;
            }
        }

        public override bool LogicOPD()
        {
            try
            {
                string fileNameOutput = $@"{OutputPath}\{PatientName_S}-{Path.GetFileNameWithoutExtension(FilePath)}_{CurrentSeconds}.txt";
                DateTime.TryParseExact(BirthDate_S, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime _date);  //生日
                string newBirthDate = _date.ToString("yyyy-MM-dd");
                bool yn = on.ChuangSheng(MedicineName_L, MedicineCode_L, AdminCode_L, PerQty_L, SumQty_L, StartDay_L, EndDay_L, fileNameOutput,
                        PatientName_S, newBirthDate, HospitalName_S, Class_S);
                if (yn)
                    return true;
                else
                {
                    List<string> day = new List<string>();
                    for (int x = 0; x <= StartDay_L.Count - 1; x++)
                        day.Add(StartDay_L[x] + "~" + EndDay_L[x]);
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
