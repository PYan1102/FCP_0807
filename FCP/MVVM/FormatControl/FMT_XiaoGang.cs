using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using FCP.MVVM.Models.Enum;
using Helper;

namespace FCP.MVVM.FormatControl
{
    class FMT_XiaoGang : FormatCollection
    {
        int EffectiveDay = 180;
        List<string> MillingMark = new List<string>();
        List<string> ModifyMark = new List<string>();
        List<string> MedicineBagsNumber = new List<string>();
        List<string> AutoNumber = new List<string>();
        List<string> PatientInfo = new List<string>();
        List<string> NursingSattionNumber = new List<string>();
        List<string> WareHouse = new List<string>();
        string MixingTable;
        string WriteDate;
        string EffectiveDate;
        DateTime Effective = DateTime.Now;

        public override bool ProcessOPD()
        {
            try
            {
                var ecd = Encoding.Default;
                List<string> PatientInfo = new List<string>();
                List<string> list = GetContent.Split('\n').ToList();
                list.ToList().ForEach(x => PatientInfo.Add(x));
                Byte[] Temp = Encoding.Default.GetBytes(PatientInfo[0]);
                int InfoCount = 65;
                for (int r = 0; r <= PatientInfo.Count - 2; r++)
                {
                    Byte[] ATemp = Encoding.Default.GetBytes(PatientInfo[r]);
                    string data = ecd.GetString(ATemp, 0, 127);
                    AdminCode_S = ecd.GetString(ATemp, 127, 8).Trim();  //頻率
                    if (AdminCode_S != "*")
                        if (AdminCode_S.Substring(AdminCode_S.Length - 2, 2) == "PC")
                            AdminCode_S = AdminCode_S.Substring(0, AdminCode_S.Length - 2);
                    string MedicineCodeJudge = ecd.GetString(ATemp, 71, 6).Trim();
                    float totalqua = Convert.ToSingle(ecd.GetString(ATemp, 138, 9));  //總量
                    if (IsFilterMedicineCode(MedicineCodeJudge))
                        continue;
                    if (MedicineCodeJudge == "1UNIFR")
                        continue;
                    if (IsFilterAdminCode(AdminCode_S))
                        continue;
                    if (Convert.ToSingle(GetID2(MedicineCodeJudge)) <= totalqua)  //總量 >= ID2 不輸出
                        continue;
                    if (totalqua % Convert.ToSingle(GetID1(MedicineCodeJudge)) == 0)  //總量 % ID1 = 0 不輸出
                        continue;
                    if (!IsExistsCombiAdminCode($"S{AdminCode_S}"))
                    {
                        Log.Write($"{FilePath} 在OnCube中未建置此種包頻率 S{AdminCode_S}");
                        ReturnsResult.Shunt(ConvertResult.沒有種包頻率, AdminCode_S);
                        return false;
                    }
                    AdminCode_S = "S" + AdminCode_S;
                    PrescriptionNo_S = ecd.GetString(ATemp, 1, 4).Trim();  //領藥序號
                    PatientNo_S = ecd.GetString(ATemp, 5, 8).Trim();  //病歷號
                    if (ecd.GetString(ATemp, 13, 10).Contains("?"))  //病患姓名
                        PatientName_L.Add((Convert.ToChar(InfoCount) + ecd.GetString(ATemp, 13, 10).Replace("?", " ")).Trim());
                    else
                        PatientName_L.Add((Convert.ToChar(InfoCount) + ecd.GetString(ATemp, 13, 10)).Trim());
                    InfoCount += 1;
                    Age_S = ecd.GetString(ATemp, 23, 3).Trim();  //病患年齡
                    Gender_S = ecd.GetString(ATemp, 26, 1);  //病患性別
                    int year = Int32.Parse(ecd.GetString(ATemp, 27, 3));  //年
                    StartDay_L.Add((1911 + year) + ecd.GetString(ATemp, 30, 4));  //調劑日期
                    Class_S = ecd.GetString(ATemp, 34, 20).Trim();  //科別
                    DoctorName_S = ecd.GetString(ATemp, 54, 10).Trim();  //醫師姓名
                    MedicineBagsNumber.Add(ecd.GetString(ATemp, 64, 7).Trim());  //藥袋數
                    MedicineCode_L.Add(ecd.GetString(ATemp, 71, 6).Trim());  //藥品代碼
                    MedicineName_L.Add(ecd.GetString(ATemp, 77, 30).Trim());  //藥品名稱
                    string temp = ecd.GetString(ATemp, 107, 7).Contains("Tab") ? "複方" : ecd.GetString(ATemp, 107, 7);
                    MedicineContent_L.Add(temp.Trim());  //藥品含量
                    Unit_L.Add(ecd.GetString(ATemp, 114, 6).Trim());  //藥品單位
                    PerQty_L.Add(ecd.GetString(ATemp, 120, 7).Trim());  //單次劑量
                    AdminCode_L.Add(AdminCode_S.Trim());  //頻率
                    Days_L.Add(ecd.GetString(ATemp, 135, 3).Trim());  //天數
                    SumQty_L.Add(ecd.GetString(ATemp, 138, 9).Trim());  //總量
                    MillingMark.Add(ecd.GetString(ATemp, 147, 1));  //磨粉註記
                    ModifyMark.Add(ecd.GetString(ATemp, 148, 1));  //修改註記
                    MixingTable = ecd.GetString(ATemp, 149, 2).Trim();  //調劑台
                    try
                    {
                        BirthDate_S = ecd.GetString(ATemp, 151, 8);  //病患生日
                    }
                    catch (Exception b)
                    {
                        b.ToString();
                        BirthDate_S = "        ";
                    }
                    ID1_L.Add(GetID1(ecd.GetString(ATemp, 71, 6).Trim()));  //ID1
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
                Log.Write($"{FilePath} {ex}");
                ReturnsResult.Shunt(ConvertResult.讀取檔案失敗, ex.ToString());
                return false;

            }
        }

        public override bool LogicOPD()
        {
            try
            {
                Effective = DateTime.Now;
                EffectiveDate = Effective.AddDays(EffectiveDay).ToString("yyyy-MM-dd");
                EndDay_L = StartDay_L;
                List<string> FileNameOutputCount = new List<string>();
                FileNameOutputCount.Add($@"{InputPath}\{PrescriptionNo_S}-{PatientName_L[0].Substring(1).Trim()}_{GetFileInfo(FilePath)}.txt");  //檔名
                DateTime.TryParseExact(BirthDate_S, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime _date);  //生日
                string Birthdaynew = string.Format("{0:d}", _date);
                float a = CalculationID1Remainder();
                if (a == 0)
                {
                    ReturnsResult.Shunt(ConvertResult.全數過濾, null);
                    return false;
                }
                bool result = OP_OnCube.XiaoGang_OPD(MedicineName_L, MedicineCode_L, AdminCode_L, PerQty_L, SumQty_L, StartDay_L, FileNameOutputCount, ID1_L,
                        MedicineBagsNumber, MedicineContent_L, MillingMark, ModifyMark, PatientName_L, PatientNo_S, "小港醫院", DoctorName_S, PrescriptionNo_S,
                        Birthdaynew, Class_S, Gender_S, MixingTable, EffectiveDate);
                if (result)
                {
                    ModifyFileName($@"{InputPath}\{PrescriptionNo_S}-{PatientName_L[0].Substring(1).Trim()}_{GetFileInfo(FilePath)}.txt", PrescriptionNo_S, OutputPath);
                    return true;
                }
                else
                {
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

        public override bool ProcessUDBatch()
        {
            try
            {
                ClearList();
                var ecd = Encoding.Default;
                string Identity;
                string WriteTime;
                string FileNameTemp = Path.GetFileNameWithoutExtension(FilePath);
                string[] info;
                info = GetContent.Split('\n');
                info.ToList().ForEach(x => PatientInfo.Add(x));
                Byte[] Temp = Encoding.Default.GetBytes(PatientInfo[0]);
                int InfoCount = 65;
                for (int r = 0; r <= PatientInfo.Count - 2; r++)  //將藥品資料放入List<string>
                {
                    Byte[] ATemp = Encoding.Default.GetBytes(PatientInfo[r]);
                    string MedicineCodeJudge = ecd.GetString(ATemp, 109, 12).Trim();  //藥品代碼
                    float totalqua = Convert.ToSingle(ecd.GetString(ATemp, 131, 11));  //總量
                    string AdminCodeTemp = ecd.GetString(ATemp, 69, 20).Trim();  //頻率 含空白 
                    if (AdminCodeTemp != "*")
                    {
                        if (AdminCodeTemp.Substring(AdminCodeTemp.Length - 2, 2) == "PC")
                            AdminCode_S = AdminCodeTemp.Substring(0, AdminCodeTemp.Length - 2).Trim();
                        else
                            AdminCode_S = AdminCodeTemp;
                    }
                    if (ecd.GetString(ATemp, 13, 4).Trim() == "1")
                        InfoCount = 65;
                    if (IsFilterMedicineCode(MedicineCodeJudge))
                        continue;
                    if (IsFilterAdminCode(AdminCode_S))
                        continue;
                    if (Convert.ToSingle(GetID2(MedicineCodeJudge)) <= totalqua)  //總量>=ID2不輸出
                        continue;
                    if (!IsExistsMultiAdminCode(AdminCode_S))
                    {
                        Log.Write($"{FilePath} 在OnCube中未建置此餐包頻率 {AdminCode_S}");
                        ReturnsResult.Shunt(ConvertResult.沒有餐包頻率, AdminCode_S);
                        return false;
                    }
                    if (!IsExistsCombiAdminCode($"S{AdminCode_S}"))
                    {
                        Log.Write($"{FilePath} 在OnCube中未建置此種包頻率 S{AdminCode_S}");
                        ReturnsResult.Shunt(ConvertResult.沒有種包頻率, AdminCode_S);
                        return false;
                    }
                    WriteDate = $"{ecd.GetString(ATemp, 0, 4)}/{ecd.GetString(ATemp, 4, 2)}/{ecd.GetString(ATemp, 6, 2)}"; //寫入日期
                    AutoNumber.Add(ecd.GetString(ATemp, 8, 5)); //自動編號
                    NumberDetail_L.Add(ecd.GetString(ATemp, 13, 4));  //編號細項
                    IsStat_B = ecd.GetString(ATemp, 17, 1) == "O";  //長期or即時
                    PatientNo_L.Add(ecd.GetString(ATemp, 18, 8).Trim());  //病歷號
                    StartDay_L.Add(ecd.GetString(ATemp, 0, 8));  //入院日期
                    Class_L.Add(ecd.GetString(ATemp, 34, 6).Trim());  //科別
                    NursingSattionNumber.Add(ecd.GetString(ATemp, 40, 6).Trim());  //護理站編號
                    if (ecd.GetString(ATemp, 40, 6).Trim() == "3W" | ecd.GetString(ATemp, 40, 6).Trim() == "OP")
                        RoomNo_L.Add("0000");
                    else
                        RoomNo_L.Add(ecd.GetString(ATemp, 46, 8).Trim());  //病房號碼
                    if (RoomNo_L[RoomNo_L.Count - 1].Trim() == "")
                        RoomNo_L[RoomNo_L.Count - 1] = "0000";
                    BedNo_L.Add(ecd.GetString(ATemp, 54, 2).Trim());  //床號
                    if (ecd.GetString(ATemp, 56, 10).Contains("?"))  //病患姓名
                        PatientName_L.Add(ecd.GetString(ATemp, 56, 10).Replace("?", " ").Trim());
                    else
                        PatientName_L.Add(ecd.GetString(ATemp, 56, 10).Trim());
                    Identity = ecd.GetString(ATemp, 66, 3).Trim();  //身份
                    //頻率放到後面判斷種餐包後處理
                    Way_L.Add(ecd.GetString(ATemp, 89, 20).Trim());  //途徑
                    MedicineCode_L.Add(Convert.ToChar(InfoCount) + ecd.GetString(ATemp, 109, 11).Trim());  //藥品碼
                    InfoCount += 1;
                    DoctorName_S = ecd.GetString(ATemp, 121, 10).Trim();  //醫師姓名
                    //總量、次量放到後面判斷種餐包後處理
                    MedicalUnit_L.Add(ecd.GetString(ATemp, 149, 10).Trim());  //醫囑單位
                    WriteTime = ecd.GetString(ATemp, 159, 4);  //寫入時間
                    WareHouse.Add(ecd.GetString(ATemp, 163, 1));  //庫房
                    string PreTemp = ecd.GetString(ATemp, 167, 17).Trim();
                    if (PreTemp.Contains("_"))
                        PreTemp = PreTemp.Substring(1);
                    PrescriptionNo_L.Add(PreTemp);  //領藥序號
                    DateTime Effective = DateTime.Now;
                    string EffectiveDate = Effective.AddDays(EffectiveDay).ToString("yyyy-MM-dd");
                    try
                    {
                        BirthDate_L.Add(ecd.GetString(ATemp, 184, 10));  //生日
                        MedicineName_L.Add(ecd.GetString(ATemp, 194, 30).Trim());  //藥名
                        string temp = ecd.GetString(ATemp, 224, 7).Contains("Tab") ? "複方" : ecd.GetString(ATemp, 224, 7);
                        MedicineSpecification_L.Add(temp.Trim());  //規格
                        Unit_L.Add(ecd.GetString(ATemp, 231, 6).Trim());  //單位
                    }
                    catch (Exception b)
                    {
                        b.ToString();
                        BirthDate_L.Add("1997-01-01");
                        MedicineName_L.Add("");
                        MedicineSpecification_L.Add("");
                        Unit_L.Add("");
                    }
                    ID1_L.Add(GetID1(ecd.GetString(ATemp, 109, 12).Trim()));
                    DateTime dt = DateTime.Parse(WriteDate);
                    float id1float = Convert.ToSingle(GetID1(ecd.GetString(ATemp, 109, 12).Trim()));
                    float totalquanityfloat = Convert.ToSingle(ecd.GetString(ATemp, 131, 11).Trim());  //總
                    float dosagefloat = Convert.ToSingle(ecd.GetString(ATemp, 142, 7).Trim());  //次量

                    if (ecd.GetString(ATemp, 46, 8).Trim() == "8DH" & SettingsModel.StatOrBatch == "B")
                    {
                        PerQty_L.Add(ecd.GetString(ATemp, 142, 7).Trim());
                        AdminCode_S = "S" + AdminCode_S;
                        SumQty_L.Add(ecd.GetString(ATemp, 131, 11).Trim());  //正常總量
                        EndDay_L.Add(Convert.ToDateTime(WriteDate).ToString("yyyyMMdd"));
                    }
                    else
                    {
                        if (ecd.GetString(ATemp, 142, 7).ToString().IndexOf(".") == -1)  //次量整數時
                        {
                            PerQty_L.Add(ecd.GetString(ATemp, 142, 7).Trim());  //次量
                            SumQty_L.Add(totalquanityfloat.ToString());
                            float f = Convert.ToSingle(Math.Ceiling((totalquanityfloat / dosagefloat) - 1));
                            DateTime Days;
                            if (f < 1)
                                Days = dt.AddDays(0);
                            else
                                Days = dt.AddDays(f);
                            EndDay_L.Add(Days.ToString("yyyyMMdd"));
                            AdminCode_S = "S" + AdminCode_S;
                        }
                        else                                       //次量不是整數時
                        {
                            SumQty_L.Add(ecd.GetString(ATemp, 131, 11).Trim());  //正常總量
                            PerQty_L.Add(ecd.GetString(ATemp, 142, 7).Trim());  //次量
                            EndDay_L.Add(dt.ToString("yyyyMMdd"));
                            AdminCode_S = "S" + AdminCode_S;
                        }
                    }
                    AdminCode_L.Add(AdminCode_S);  //頻率
                }
                string FileName = $"{Path.GetFileNameWithoutExtension(FilePath)}_{CurrentSeconds}.txt";
                if (AdminCode_L.Count == 0)
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
            if (AdminCode_L.Count == 0)
            {
                ReturnsResult.Shunt(ConvertResult.全數過濾, null);
                return false;
            }
                

            try
            {
                bool yn;
                Effective = DateTime.Now;
                EffectiveDate = Effective.AddDays(EffectiveDay).ToString("yyyy-MM-dd");
                List<string> FileNameOutputCount = new List<string>();
                if (IsStat_B)
                    FileNameOutputCount.Add($@"{InputPath}\{PatientName_L[0].Trim()}-{PrescriptionNo_L[0].Trim()}-{RoomNo_L[0]}-{BedNo_L[0]}_{GetFileInfo(FilePath)}.txt");  //檔名
                else
                    FileNameOutputCount.Add($@"{InputPath}\住院長期_{CurrentSeconds}.txt");  //檔名
                //一般方式包藥
                DateTime dt1 = DateTime.Parse(WriteDate);
                yn = OP_OnCube.XiaoGang_UD(MedicineName_L, MedicineCode_L, AdminCode_L, PerQty_L, SumQty_L, dt1.ToString("yyyyMMdd"), EndDay_L, FileNameOutputCount,
                MedicineSpecification_L, Unit_L, ID1_L, PatientName_L, PatientNo_L, DoctorName_S, PrescriptionNo_L, BirthDate_L, RoomNo_L, BedNo_L,
                Class_L, WareHouse, IsStat_B, AutoNumber, NumberDetail_L, NursingSattionNumber, EffectiveDate, MedicalUnit_L);
                if (yn)
                {
                    if (IsStat_B)
                        ModifyFileName($@"{InputPath}\{PatientName_L[0].Trim()}-{PrescriptionNo_L[0].Trim()}-{RoomNo_L[0]}-{BedNo_L[0]}_{GetFileInfo(FilePath)}.txt", PatientName_L[0].Trim(), OutputPath);
                    else
                    {
                        ModifyFileName($@"{InputPath}\住院長期_{CurrentSeconds}.txt", "住院長期", OutputPath);
                    }
                    return true;
                }
                else
                {
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
            try
            {
                var ecd = Encoding.Default;
                string FileNameTemp = Path.GetFileNameWithoutExtension(FilePath);
                string[] info;
                Debug.WriteLine($"POWDER_{FileNameTemp}");
                List<string> PatientInfo = new List<string>();
                info = GetContent.Split('\n');
                info.ToList().ForEach(x => PatientInfo.Add(x));
                Byte[] Temp = Encoding.Default.GetBytes(PatientInfo[0]);

                int InfoCount = 65;
                for (int r = 0; r <= PatientInfo.Count - 2; r++)  //將藥品資料放入ArrayList
                {
                    var ATemp = Encoding.Default.GetBytes(PatientInfo[r]);
                    string AdminCodeTemp = ecd.GetString(ATemp, 137, 8).Trim();  //頻率 含空白
                    if (AdminCodeTemp != "*")
                    {
                        if (AdminCodeTemp.Substring(AdminCodeTemp.Length - 2, 2) == "PC")
                            AdminCode_S = AdminCodeTemp.Substring(0, AdminCodeTemp.Length - 2).Trim();
                        else
                            AdminCode_S = AdminCodeTemp;
                    }
                    if (!IsExistsCombiAdminCode($"S{AdminCode_S}"))
                    {
                        Log.Write($"{FilePath} 在OnCube中未建置此種包頻率 S{AdminCode_S}");
                        ReturnsResult.Shunt(ConvertResult.沒有種包頻率, AdminCode_S);
                        return false;
                    }
                    AdminCode_S = "S" + AdminCode_S;
                    string MedicineCodeJudge = ecd.GetString(ATemp, 81, 6).Trim();
                    float totalqua = Convert.ToSingle(ecd.GetString(ATemp, 148, 9).Trim());  //總量
                    if (IsFilterMedicineCode(MedicineCodeJudge))
                        continue;
                    if (IsFilterAdminCode(AdminCode_S))
                        continue;
                    if (Convert.ToSingle(GetID2(MedicineCodeJudge)) <= Math.Ceiling(totalqua))  //總量 >= ID2 不輸出
                        continue;
                    if (Math.Ceiling(totalqua) % Convert.ToSingle(GetID1(MedicineCodeJudge)) == 0)  //總量 % ID1 = 0 不輸出
                        continue;
                    PrescriptionNo_S = ecd.GetString(ATemp, 1, 4);  //領藥序號
                    PatientNo_S = ecd.GetString(ATemp, 5, 8).Trim();  //病歷號
                    if (ecd.GetString(ATemp, 13, 20).Contains("?"))  //病患姓名
                        PatientName_L.Add(Convert.ToChar(InfoCount) + ecd.GetString(ATemp, 13, 19).Replace("?", " ").Trim());
                    else
                        PatientName_L.Add(Convert.ToChar(InfoCount) + ecd.GetString(ATemp, 13, 19).Trim());
                    InfoCount += 1;
                    Age_S = ecd.GetString(ATemp, 33, 3).Trim(); //病患年齡
                    Gender_S = ecd.GetString(ATemp, 36, 1);  //病患性別
                    int year = Int32.Parse(ecd.GetString(ATemp, 37, 3));
                    StartDay_L.Add((1911 + year).ToString() + ecd.GetString(ATemp, 40, 4));  //調劑日期
                    Class_S = ecd.GetString(ATemp, 44, 20).Trim();  //科別
                    DoctorName_S = ecd.GetString(ATemp, 64, 10).Trim(); //醫師姓名
                    MedicineBagsNumber.Add(ecd.GetString(ATemp, 74, 7).Trim());  //藥袋數
                    MedicineCode_L.Add(ecd.GetString(ATemp, 81, 6).Trim());  //藥品代碼
                    MedicineName_L.Add(ecd.GetString(ATemp, 87, 30).Trim());  //藥品名稱
                    string temp = ecd.GetString(ATemp, 117, 7).Contains("Tab") ? "複方" : ecd.GetString(ATemp, 117, 7);
                    MedicineContent_L.Add(temp.Trim()); //藥品規格量
                    Unit_L.Add(ecd.GetString(ATemp, 124, 6).Trim());  //單位
                    PerQty_L.Add(ecd.GetString(ATemp, 130, 7).Trim());  //劑量
                    AdminCode_L.Add(AdminCode_S);  //頻率
                    Days_L.Add(ecd.GetString(ATemp, 145, 3).Trim());  //天數
                    SumQty_L.Add(ecd.GetString(ATemp, 148, 9).Trim());  //總量
                    try
                    {
                        BirthDate_S = ecd.GetString(ATemp, 157, 8);  //生日
                    }
                    catch (Exception b)
                    {
                        b.ToString();
                        BirthDate_S = "        ";
                    }
                    ID1_L.Add(GetID1(ecd.GetString(ATemp, 81, 6).Trim()));  //ID1
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
                Log.Write($"{FilePath} {ex}");
                ReturnsResult.Shunt(ConvertResult.讀取檔案失敗, ex.ToString());
                return false;
            }
        }

        public override bool LogicPOWDER()
        {
            try
            {
                bool yn;
                EndDay_L = StartDay_L.ToList();
                Effective = DateTime.Now;
                EffectiveDate = Effective.AddDays(EffectiveDay).ToString("yyyy-MM-dd");
                List<string> FileNameOutputCount = new List<string>();
                FileNameOutputCount.Add($@"{InputPath}\{PrescriptionNo_S}-{PatientName_L[0].Substring(1).Trim()}_{GetFileInfo(FilePath)}.txt");  //檔名
                Debug.WriteLine(GetFileInfo(InputPath + "\\" + FilePath));  //檔名
                DateTime.TryParseExact(BirthDate_S, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime _date);  //生日
                string Birthdaynew = string.Format("{0:d}", _date);
                float a = CalculationID1Remainder();
                if (a != 0)
                    yn = OP_OnCube.XiaoGang_POWDER(MedicineName_L, MedicineCode_L, AdminCode_L, PerQty_L, SumQty_L, StartDay_L, FileNameOutputCount, Unit_L, ID1_L,
                        MedicineBagsNumber, MedicineContent_L, PatientName_L, PatientNo_S, "小港醫院", DoctorName_S, PrescriptionNo_S,
                        Birthdaynew, Class_S, Gender_S, EffectiveDate);
                else
                {
                    ReturnsResult.Shunt(ConvertResult.全數過濾, null);
                    return false;
                }
                if (yn)
                {
                    ModifyFileName($@"{InputPath}\{PrescriptionNo_S}-{PatientName_L[0].Substring(1).Trim()}_{GetFileInfo(FilePath)}.txt", PrescriptionNo_S, OutputPath);
                    return true;
                }
                else
                {
                    ReturnsResult.Shunt(ConvertResult.產生OCS失敗, null);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.Write($"{FilePath} {ex}");
                ReturnsResult.Shunt(ConvertResult.處理邏輯失敗, null);
                return false;
            }
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
            MillingMark.Clear();
            ModifyMark.Clear();
            MedicineBagsNumber.Clear();
            AutoNumber.Clear();
            PatientInfo.Clear();
            NursingSattionNumber.Clear();
            WareHouse.Clear();
        }

        private string GetFileInfo(string FilePath)
        {
            DateTime FileTime = Directory.GetLastWriteTime(FilePath);
            if (FileTime.ToString().Contains("下午"))
                return (Convert.ToInt32(Directory.GetLastWriteTime(FilePath).ToString("HH")) + 12).ToString() + FileTime.ToString("-mm");
            else
                return FileTime.ToString("HH-mm");
        }

        private void ModifyFileName(string Name, string Data, string OutputPath)
        {
            string NewName = OutputPath + "\\" + Name.Substring(Name.IndexOf(Data), Name.Length - Name.IndexOf(Data));
            if (File.Exists(NewName))
            {
                int c = 0;
                foreach (string file in Directory.GetFiles(OutputPath, "*.txt"))
                    c += 1;
                NewName = OutputPath + "\\" + Name.Substring(Name.IndexOf(Data), Name.Length - Name.IndexOf(Data) - 4) + "_" + c + ".txt";
            }
            File.Move(Name, NewName);
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
