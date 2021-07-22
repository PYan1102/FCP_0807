using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Diagnostics;
using System.Globalization;

namespace FCP
{
    class FMT_KuangTien:FormatCollection
    {
        string HospitalInformation = "";
        string PatientInformation = "";
        string MedicineInformation = "";
        string GetMedicineNumber = "";
        string WriteDate = "";
        string EffectiveDate = "";
        List<string> TreatmentDate = new List<string>();
        List<string> PrescriptionCutTime = new List<string>();
        List<string> CalculationDays = new List<string>();
        List<string> SpecialCode = new List<string>();
        List<string> TimesPerDay = new List<string>();
        Dictionary<string, List<string>> Dic = new Dictionary<string, List<string>>();
        Dictionary<string, string> JudgeMedicineGivenDic = new Dictionary<string, string>();
        Dictionary<int, string> HospitalInformationDic = new Dictionary<int, string>();
        Dictionary<int, string> PatientInformationDic = new Dictionary<int, string>();
        Dictionary<int, string> MedicineInformationDic = new Dictionary<int, string>();
        Dictionary<int, string> BarcodeDic = new Dictionary<int, string>();
        SqlConnection con;
        SqlCommand com;

        public override string MethodShunt(int? MethodID)
        {
            switch(MethodID)
            {
                case 0:
                    return Do(OPD_Process(), OPD_Logic());
                case 1:
                    return Do(POWDER_Process(), POWDER_Logic());
                case 2:
                    if (Settings.StatOrBatch == "S")
                        return Do(UD_Stat_Process(), UD_Stat_Logic());
                    else
                        return Do(UD_Batch_Process(), UD_Batch_Logic());
                default:
                    return $"{FullFileName_S} {MethodID} {Settings.StatOrBatch}";
            }
        }

        public override void Load(string inp, string oup, string filename, string time, Settings settings, Log log)
        {
            base.Load(inp, oup, filename, time, settings, log);
        }
        
        private ResultType OPD_Process()
        {
            try
            {
                ClearList();
                var ecd = Encoding.Default;
                string FileContent = DeleteSpace(GetContent);
                string FileNameTemp = Path.GetFileNameWithoutExtension(FullFileName_S);
                string[] FileContentSplit = FileContent.Split('\n');
                byte[] Temp = ecd.GetBytes(FileContentSplit[0]);
                WriteDate = ecd.GetString(Temp, 9, 9).Trim();
                GetMedicineNumber = ecd.GetString(Temp, 54, 8).Trim();
                Age_S = ecd.GetString(Temp, 67, 5).Trim();
                DoctorName_S = ecd.GetString(Temp, 77, 11).Trim();
                Temp = ecd.GetBytes(FileContentSplit[1]);
                PatientNo_S = ecd.GetString(Temp, 9, 11).Trim();
                PatientName_S = ecd.GetString(Temp, 25, 21).Trim();
                Gender_S = ecd.GetString(Temp, 47, 2).Trim();
                Class_S = ecd.GetString(Temp, 67, Temp.Length - 67).Trim();
                for (int x = 4; x <= FileContentSplit.ToList().Count - 3; x++)
                {
                    byte[] ATemp = ecd.GetBytes(FileContentSplit[x]);
                    AdminCode_S = ecd.GetString(ATemp, 57, 11).Trim() + ecd.GetString(ATemp, 68, 9).Trim();
                    if (JudgePackedMode(AdminCode_S))
                        continue;
                    if (Settings.EN_FilterMedicineCode && !MedicineCodeGiven_L.Contains(ecd.GetString(ATemp, 2, 10).Trim()))
                        continue;
                    if (Convert.ToInt32(ecd.GetString(ATemp, 82, 8).Trim()) >= GetID2(ecd.GetString(ATemp, 2, 10).Trim()) & GetID2(ecd.GetString(ATemp, 2, 10).Trim()) >= 1)  //總量 >= ID2
                        continue;
                    if (GetID1(ecd.GetString(ATemp, 2, 10).Trim()) >= 1)  //ID1 >= 1
                    {
                        if (Convert.ToInt32(ecd.GetString(ATemp, 82, 8).Trim()) % GetID1(ecd.GetString(ATemp, 2, 10).Trim()) == 0)  //總量可以被ID1整除
                            continue;
                    }
                    if (!GOD.Is_Admin_Code_For_Multi_Created(ecd.GetString(ATemp, 57, 11).Trim() + ecd.GetString(ATemp, 68, 9).Trim()))
                    {
                        LoseContent = $"{FullFileName_S} OnCube中未建置此餐包頻率 {AdminCode_S}";
                        return ResultType.沒有頻率;
                    }
                    MedicineCode_L.Add(ecd.GetString(ATemp, 2, 10).Trim());
                    MedicineName_L.Add(ecd.GetString(ATemp, 12, 31).Trim());
                    AdminCode_L.Add(AdminCode_S);
                    PerQty_L.Add(ecd.GetString(ATemp, 43, 8).Trim());
                    Days_L.Add(ecd.GetString(ATemp, 77, 5).Trim());
                    SumQty_L.Add(ecd.GetString(ATemp, 82, 8).Trim());
                    TimesPerDay.Add($"{GOD.Get_Admin_Code_For_Multi(AdminCode_S).Count}");
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

        public ResultType OPD_Logic()
        {
            if (AdminCode_L.Count == 0)
                return ResultType.全數過濾;
            try
            {
                bool yn;
                oncube = new OnputType_OnCube(log);
                string Year = (Convert.ToInt32(DateTime.Now.ToString("yyyy")) - 1911).ToString();
                FileNameOutput_S = $@"{OutputPath_S}\{Year}{DateTime.Now:MMdd}_{GetMedicineNumber.Trim()}_{PatientName_S.Trim()}_{Time_S}.txt";
                yn = oncube.KuangTien_OPD(MedicineCode_L, MedicineName_L, AdminCode_L, Days_L, PerQty_L, TimesPerDay, SumQty_L, PatientName_S, DoctorName_S, GetMedicineNumber, PatientNo_S, Age_S, Gender_S, Class_S, WriteDate, FileNameOutput_S);
                if (yn)
                    return ResultType.成功;
                else
                {
                    List<string> DaysList = new List<string>();
                    for (int x = 0; x <= StartDay_L.Count - 1; x++)
                    {
                        DaysList.Add(StartDay_L[x] + "~" + EndDay_L[x]);
                    }
                    log.Prescription(FullFileName_S, PatientName_S, "", MedicineCode_L, MedicineName_L, AdminCode_L, PerQty_L, DaysList);
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

        public ResultType POWDER_Process()
        {
            try
            {
                ClearList();
                var ecd = Encoding.Default;
                string GrindTable;
                List<string> TimesPerDay = new List<string>();
                string FileNameTemp = Path.GetFileNameWithoutExtension(FullFileName_S);
                string[] FileContentSplit = DeleteSpace(GetContent).Split('\n');
                byte[] Temp = ecd.GetBytes(FileContentSplit[0]);
                WriteDate = ecd.GetString(Temp, 9, 9).Trim();
                GetMedicineNumber = ecd.GetString(Temp, 54, 8).Trim();
                Age_S = ecd.GetString(Temp, 67, 5).Trim();
                DoctorName_S = ecd.GetString(Temp, 77, 11).Trim();
                Temp = ecd.GetBytes(FileContentSplit[1]);
                PatientNo_S = ecd.GetString(Temp, 9, 11).Trim();
                PatientName_S = ecd.GetString(Temp, 25, 21).Trim();
                Gender_S = ecd.GetString(Temp, 47, 2).Trim();
                Class_S = ecd.GetString(Temp, 67, Temp.Length - 67).Trim();
                for (int x = 4; x <= FileContentSplit.ToList().Count - 3; x++)
                {
                    byte[] ATemp = ecd.GetBytes(FileContentSplit[x]);
                    AdminCode_S = ecd.GetString(ATemp, 57, 11).Trim() + ecd.GetString(ATemp, 68, 9).Trim();
                    GrindTable = ecd.GetString(ATemp, 100, 8).Trim();
                    if (GrindTable.Equals(""))
                        continue;
                    if (JudgePackedMode(AdminCode_S))
                        continue;
                    if (!GOD.Is_Admin_Code_For_Multi_Created(AdminCode_S))
                    {
                        log.Write($"{FullFileName_S} 在OnCube中未建置此餐包頻率 {AdminCode_S}");
                        LoseContent = $"{FullFileName_S} OnCube中未建置此餐包頻率 {AdminCode_S}";
                        return ResultType.沒有頻率;
                    }
                    if (!Dic.TryGetValue(GrindTable, out List<string> a))
                        Dic.Add(GrindTable, new List<string>());
                    Dic[GrindTable].Add(ecd.GetString(ATemp, 2, 10).Trim());  //MedicineCode
                    Dic[GrindTable].Add(ecd.GetString(ATemp, 12, 31).Trim());  //MedicineName
                    Dic[GrindTable].Add(ecd.GetString(ATemp, 43, 8).Trim());  //PerQty
                    Dic[GrindTable].Add(ecd.GetString(ATemp, 57, 11).Trim() + ecd.GetString(ATemp, 68, 9).Trim());  //AdminTime
                    Dic[GrindTable].Add(ecd.GetString(ATemp, 77, 5).Trim());  //Days
                    Dic[GrindTable].Add(ecd.GetString(ATemp, 82, 8).Trim());  //SumQty
                    Dic[GrindTable].Add(ecd.GetString(ATemp, 100, 8).Trim());  //GrindTanle
                    Dic[GrindTable].Add($"{GOD.Get_Admin_Code_For_Multi(AdminCode_S).Count}");  //TimesPerDay
                    EffectiveDate = DateTime.Now.AddDays(Convert.ToInt32(ecd.GetString(ATemp, 77, 5).Trim())).ToString("yyyy/MM/dd");
                }
                if (Dic.Count == 0)
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

        public ResultType POWDER_Logic()
        {
            if (Dic.Count == 0)
                return ResultType.全數過濾;
            try
            {
                bool yn;
                jvserver = new OnputType_JVServer(log);
                string Year = (Convert.ToInt32(DateTime.Now.ToString("yyyy")) - 1911).ToString();
                FileNameOutput_S = $@"{OutputPath_S}\{Year}{DateTime.Now:MMdd}_{GetMedicineNumber.Trim()}_{PatientName_S}_{Time_S}.txt";
                List<string> DicDistinct = new List<string>();
                foreach(var v in Dic)
                {
                    if (!DicDistinct.Contains(v.Key))
                        DicDistinct.Add(v.Key);
                }
                yn = jvserver.KuangTien_磨粉(Dic, DicDistinct, PatientName_S, DoctorName_S, GetMedicineNumber, PatientNo_S, Age_S, Gender_S, Class_S, WriteDate, FileNameOutput_S, EffectiveDate);
                if (yn)
                    return ResultType.成功;
                else
                {
                    List<string> DaysList = new List<string>();
                    for (int x = 0; x <= StartDay_L.Count - 1; x++)
                    {
                        DaysList.Add(StartDay_L[x] + "~" + EndDay_L[x]);
                    }
                    log.Prescription(FullFileName_S, PatientName_S, "", MedicineCode_L, MedicineName_L, AdminCode_L, PerQty_L, DaysList);
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

        private ResultType UD_Batch_Process()
        {
            try
            {
                ClearList();
                var ecd = Encoding.Default;
                bool _JudgeMedicinePosition = false;
                oncube = new OnputType_OnCube(log);
                string[] FileContentSplit= DeleteSpace(GetContent).Split('\n');
                int Hos = -1;
                Byte[] Temp = Encoding.Default.GetBytes(FileContentSplit[0]);
                for (int r = 0; r <= FileContentSplit.Length - 1; r++)  //將藥品資料放入List<string>
                {
                    if (FileContentSplit[r].Contains("-----"))
                        continue;
                    if (FileContentSplit[r].Trim().Equals(""))
                        continue;
                    if (FileContentSplit[r].Contains("診療日"))
                    {
                        Hos += 1;
                        Byte[] ATemp = Encoding.Default.GetBytes(FileContentSplit[r]);
                        string StartDay_L = (Convert.ToInt32(ecd.GetString(ATemp, 8, 3) + ecd.GetString(ATemp, 12, 2) + ecd.GetString(ATemp, 15, 2)) + 19110000).ToString();
                        HospitalInformation = $"{StartDay_L};{ecd.GetString(ATemp, 27, 11).Trim()}";
                        HospitalInformationDic[Hos] = HospitalInformation;
                        HospitalInformation = "";
                    }
                    else if (FileContentSplit[r].Contains("床號"))
                    {
                        Byte[] ATemp = Encoding.Default.GetBytes(FileContentSplit[r]);
                        PatientInformation = $"{ecd.GetString(ATemp, 6, 9).Trim()};{ecd.GetString(ATemp, 22, 11).Trim()};{ecd.GetString(ATemp, 38, 11).Trim()};{ecd.GetString(ATemp, 70, 10).Trim()}";
                        PatientInformationDic[Hos] = PatientInformation;
                        PatientInformation = "";
                        //Debug.WriteLine(ecd.GetString(ATemp, 38, 11).Trim());
                    }
                    else if (FileContentSplit[r].Contains("醫令碼") | (FileContentSplit[r].Contains("醫囑碼")))
                    {
                        _JudgeMedicinePosition = true;
                        continue;
                    }
                    else if (FileContentSplit[r].Contains("C!"))
                        _JudgeMedicinePosition = false;
                    if (_JudgeMedicinePosition)
                    {
                        Byte[] ATemp = Encoding.Default.GetBytes(FileContentSplit[r]);
                        MedicineInformation = $"{ecd.GetString(ATemp, 5, 10).Trim()};{ecd.GetString(ATemp, 15, 31).Trim()};{ecd.GetString(ATemp, 52, 6).Trim()};" +
                            $"{ecd.GetString(ATemp, 58, 11).Trim()};{ecd.GetString(ATemp, 69, 5).Trim()};{ecd.GetString(ATemp, 74, 9)};{ecd.GetString(ATemp, 83, 8).Trim()};{ecd.GetString(ATemp, 91, 7).Trim()};" +
                            $"{ecd.GetString(ATemp, 99, 4)};{ecd.GetString(ATemp, 104, ATemp.Length - 104)};{ecd.GetString(ATemp, 112, 7)}、";
                        if (MedicineInformationDic.ContainsKey(Hos))
                            MedicineInformationDic[Hos] = MedicineInformationDic[Hos] + MedicineInformation;
                        else
                            MedicineInformationDic.Add(Hos, MedicineInformation);
                        MedicineInformation = "";
                    }
                }
                StringBuilder sb = new StringBuilder();
                List<string> Data = new List<string>();
                foreach (var v in HospitalInformationDic)
                {
                    string[] MedicineSplit = MedicineInformationDic[v.Key].Split('、');
                    foreach (string s in MedicineSplit)
                    {
                        if (!string.IsNullOrEmpty(s))
                        {
                            sb.Append(HospitalInformationDic[v.Key] + ";");
                            sb.Append(PatientInformationDic[v.Key] + ";");
                            sb.Append(s);
                        }
                        Data.Add(sb.ToString());
                        sb.Clear();
                    }
                }
                int number = 0;
                for (int r = 0; r <= Data.Count - 1; r++)
                {
                    if (string.IsNullOrEmpty(Data[r]))
                        continue;
                    string[] DataSplit = Data[r].Split(';');
                    AdminCode_S = DataSplit[9] + DataSplit[11].Trim();
                    if (Settings.EN_FilterMedicineCode && !MedicineCodeGiven_L.Contains(DataSplit[6]))
                        continue;
                    if (JudgePackedMode(AdminCode_S))
                        continue;
                    if (AdminCode_S.Contains("TTS"))
                        AdminCode_S = "STTS";
                    if (!GOD.Is_Admin_Code_For_Multi_Created($"{AdminCode_S}"))
                    {
                        log.Write($"{FullFileName_S} 在OnCube中未建置此餐包頻率 {AdminCode_S}");
                        continue;
                    }
                    if (!GOD.Is_Admin_Code_For_Combi_Created($"S{AdminCode_S}"))
                    {
                        log.Write($"{FullFileName_S} 在OnCube中未建置此種包頻率 S{AdminCode_S}");
                        continue;
                    }
                    CalculationDays.Add($"{GOD.Get_Admin_Code_For_Multi(AdminCode_S).Count}");
                    DateTime.TryParseExact(DataSplit[0], "yyyyMMdd", null, DateTimeStyles.None, out DateTime _treatmentdate);
                    TreatmentDate.Add(_treatmentdate.ToString("yyyy-MM-dd"));
                    StayDay_L.Add(DataSplit[1].Trim());
                    //BirthDate.Add((1911 + (109 - Int32.Parse(DataSplit[2]))).ToString() + "-" + DataSplit[3] + "-" + DataSplit[4]);
                    BedNo_L.Add(DataSplit[2].Trim());
                    PrescriptionNo_L.Add(DataSplit[3].Trim());
                    PatientName_L.Add(DataSplit[4].Trim());
                    Class_L.Add(DataSplit[5].Trim());
                    MedicineCode_L.Add(DataSplit[6].Trim());
                    MedicineName_L.Add(DataSplit[7].Trim());
                    PerQty_L.Add(DataSplit[8].Trim());
                    AdminCode_L.Add(AdminCode_S);
                    Days_L.Add(DataSplit[10].Trim());
                    SumQty_L.Add(DataSplit[12].Trim());
                    PrescriptionCutTime.Add(DataSplit[14].Trim());
                    SpecialCode.Add(DataSplit[16].Trim());
                    DateTime.TryParseExact((Int32.Parse(DataSplit[13]) + 19110000).ToString(), "yyyyMMdd", null, DateTimeStyles.None, out DateTime _StartDay_L);
                    StartDay_L.Add(_StartDay_L.ToString("yyMMdd"));
                    if (!string.IsNullOrEmpty(DataSplit[15]) & int.TryParse(DataSplit[15], out int i))
                    {
                        DateTime.TryParseExact((Int32.Parse(DataSplit[15]) + 19110000).ToString(), "yyyyMMdd", null, DateTimeStyles.None, out DateTime _EndDay_L);
                        EndDay_L.Add(_EndDay_L.ToString("yyMMdd"));
                    }
                    else
                        EndDay_L.Add("");
                    if (!BarcodeDic.ContainsKey(Int32.Parse(DataSplit[3])))
                        BarcodeDic.Add(Int32.Parse(DataSplit[3]), $"{DataSplit[3]},{DataSplit[2]},{DateTime.Now:yyyy-MM-dd},*{DataSplit[6]}#{DataSplit[9] + DataSplit[11].Trim()}#{Math.Ceiling(Convert.ToSingle(DataSplit[12]))}");
                    else
                        BarcodeDic[Int32.Parse(DataSplit[3])] += $"*{DataSplit[6]}#{DataSplit[9] + DataSplit[11].Trim()}#{Math.Ceiling(Convert.ToSingle(DataSplit[12]))}";
                    number += 1;
                }
                if (AdminCode_L.Count == 0)
                    return ResultType.全數過濾;
                return ResultType.成功;
            }
            catch (Exception ex)
            {
                log.Write($"{FullFileName_S} {ex}");
                ErrorContent = $"{FullFileName_S} 讀取處方籤時發生問題 {ex}";
                return ResultType.失敗;
            }
        }

        public ResultType UD_Batch_Logic()
        {
            if (AdminCode_L.Count == 0)
                return ResultType.全數過濾;
            try
            {
                Dictionary<string, List<string>> DataDic = new Dictionary<string, List<string>>();
                List<bool> DoseType = new List<bool>();  //False > MultiDose，True > CombiDose
                List<bool> CrossAdminTimeType = new List<bool>();  //False > No，True > Yes
                List<int> TimesList = new List<int>();
                List<int> AdminTimeTimePointList = new List<int>();
                DateTime DateTemp = new DateTime();
                string JudgeTimer = "";
                int TimesCount = 0;
                string TimeTemp;
                string QODTEMP;
                List<string> CurrentDate = new List<string>();
                List<string> QODDescription = new List<string>();
                for (int x = 0; x <= AdminCode_L.Count - 1; x++)
                {
                    TimesCount = 0;
                    QODTEMP = "";
                    foreach (string s in GOD.Get_Admin_Code_For_Multi(AdminCode_L[x]))
                    {
                        AdminTimeTimePointList.Add(Convert.ToInt32(s.Substring(0, 2)));
                    }
                    float PerQty_LTemp;
                    if (PerQty_L[x].Contains("."))
                        PerQty_LTemp = Convert.ToSingle($"0{PerQty_L[x]}");
                    else
                        PerQty_LTemp = Convert.ToSingle(PerQty_L[x]);
                    int Times = Convert.ToInt32(Convert.ToSingle(SumQty_L[x]) / PerQty_LTemp);
                    DateTime.TryParseExact(TreatmentDate[x], "yyyy-MM-dd", null, DateTimeStyles.None, out DateTime TMD);
                    DateTime.TryParseExact(StartDay_L[x], "yyMMdd", null, DateTimeStyles.None, out DateTime SD);

                    //日間照護
                    if (BedNo_L[x].Contains("111DAY"))
                    {
                        if (!PerQty_LTemp.ToString().Contains("."))
                            QODTEMP = $"每次劑量 : {PerQty_LTemp}   ";
                        else
                            QODTEMP = $"每次劑量 : {PerQty_LTemp} 非整數";
                        DoseType.Add(true);
                        CrossAdminTimeType.Add(false);
                        CurrentDate.Add("");
                        EndDay_L[x] = SD.ToString("yyMMdd");
                        DataDic.Add($"{x}_{EndDay_L[x]}", new List<string>() { "" });
                        AdminCode_L[x] = $"S{AdminCode_L[x]}";
                        QODDescription.Add(QODTEMP);
                        PerQty_L[x] = SumQty_L[x];
                        con = new SqlConnection(SQLInfo_S);
                        con.Open();
                        //沙鹿
                        using (com = new SqlCommand(@"update PrintFormItem set DeletedYN=1 where RawID in (120180,120195)", con))
                        {
                            com.ExecuteNonQuery();
                        }

                        //大甲
                        //using (com = new SqlCommand(@"update PrintFormItem set DeletedYN=1 where RawID in (120156,120172)", con))
                        //{
                        //    com.ExecuteNonQuery();
                        //}
                        con.Close();
                        continue;
                    }
                    //種包
                    if (Properties.Settings.Default.DoseType == "Combi" || !int.TryParse(PerQty_L[x], out int i) | AdminCode_L[x] == "STTS" | MedicineCode_L[x] == "23521" | BedNo_L[x].Contains("111DAY"))  //劑量為非整數
                    {
                        DoseType.Add(true);
                        CrossAdminTimeType.Add(false);
                        CurrentDate.Add("");
                        EndDay_L[x] = SD.ToString("yyMMdd");
                        DataDic.Add($"{x}_{EndDay_L[x]}", new List<string>() { "" });
                        if (AdminCode_L[x] == "QODAC" | AdminCode_L[x] == "QODPC")
                        {
                            for (int d = 0; d <= Times - 1; d++)
                            {
                                DateTime FullDate = TMD.AddDays(Convert.ToInt32(Days_L[x]));
                                if (d == 0)
                                {
                                    if (Properties.Settings.Default.DoseType == "Multi" || !PerQty_LTemp.ToString().Contains("."))
                                        QODTEMP = $"{SD:MM/dd} 劑量 : {PerQty_LTemp}   ";
                                    else
                                        QODTEMP = $"{SD:MM/dd} 劑量 : {PerQty_LTemp} 非整數";
                                }
                                else
                                {
                                    if (SD.AddDays(2) <= FullDate)
                                    {
                                        SD = SD.AddDays(2);
                                        if (Properties.Settings.Default.DoseType == "Multi" || !PerQty_LTemp.ToString().Contains("."))
                                            QODTEMP += $"{SD:MM/dd} 劑量 : {PerQty_LTemp}   ";
                                        else
                                            QODTEMP += $"{SD:MM/dd} 劑量 : {PerQty_LTemp} 非整數";
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (Properties.Settings.Default.DoseType == "Multi" || !PerQty_LTemp.ToString().Contains("."))
                                QODTEMP = $"每次劑量 : {PerQty_LTemp}   ";
                            else
                                QODTEMP = $"每次劑量 : {PerQty_LTemp} 非整數";
                        }
                        AdminCode_L[x] = $"S{AdminCode_L[x]}";
                        QODDescription.Add(QODTEMP);
                        PerQty_L[x] = SumQty_L[x];
                        continue;
                    }

                    //跨天數頻率
                    if (Settings.CrossDayAdminCode.Contains(AdminCode_L[x]))
                    {
                        DoseType.Add(false);
                        CrossAdminTimeType.Add(true);
                        //Debug.WriteLine(SD.ToString("yyyy/MM/dd"));
                        CurrentDate.Add("服用日 " + SD.ToString("yyyy/MM/dd"));
                        DateTime.TryParseExact(TreatmentDate[x], "yyyy-MM-dd", null, DateTimeStyles.None, out DateTime dt);
                        //Debug.WriteLine(dt);
                        EndDay_L[x] = dt.AddDays(Convert.ToInt32(Days_L[x])).ToString("yyMMdd");
                        if (DateTime.Compare(SD, dt.AddDays(Convert.ToInt32(Days_L[x]))) == 1)
                            EndDay_L[x] = SD.ToString("yyMMdd");
                        DataDic.Add($"{x}_{EndDay_L[x]}", new List<string>() { "" });
                        QODDescription.Add(QODTEMP);
                        continue;
                    }

                    JudgeTimer = PrescriptionCutTime[x].Insert(2, ":");
                    DoseType.Add(false);
                    CrossAdminTimeType.Add(false);
                    CurrentDate.Add("服用日 " + SD.ToString("yyyy/MM/dd"));
                    QODDescription.Add(QODTEMP);
                    DateTemp = SD;
                    foreach (int y in AdminTimeTimePointList)
                    {
                        if (TimesCount == Times)
                            break;
                        if (y.ToString().Length == 1)
                            TimeTemp = "0" + y;
                        else
                            TimeTemp = y.ToString();
                        if (DateTime.Compare(Convert.ToDateTime($"{TimeTemp}:00"), Convert.ToDateTime(JudgeTimer)) >= 0)
                        {
                            if (DataDic.ContainsKey($"{x}_{DateTemp:yyMMdd}"))
                                DataDic[$"{x}_{DateTemp:yyMMdd}"].Add(y.ToString());
                            else
                                DataDic.Add($"{x}_{DateTemp:yyMMdd}", new List<string>() { TimeTemp });
                            TimesCount += 1;
                        }
                    }
                    if (TimesCount == Times)
                    {
                        EndDay_L[x] = DateTemp.ToString("yyMMdd");
                        continue;
                    }
                    if (TimesCount == 0)
                    {
                        DateTime dt = DateTemp;
                        CurrentDate[x] = $"服用日 {dt.AddDays(1).ToString("yyyy/MM/dd")}";
                    }
                    DateTemp = DateTemp.AddDays(1);
                        
                    if (TimesCount < Times)
                    {
                    Keep:
                        foreach (int y in AdminTimeTimePointList)
                        {
                            if (TimesCount == Times)
                                break;
                            if (y.ToString().Length == 1)
                                TimeTemp = "0" + y;
                            else
                                TimeTemp = y.ToString();
                            if (DataDic.ContainsKey($"{x}_{DateTemp:yyMMdd}"))
                                DataDic[$"{x}_{DateTemp:yyMMdd}"].Add(TimeTemp.ToString());
                            else
                                DataDic.Add($"{x}_{DateTemp:yyMMdd}", new List<string>() { TimeTemp });
                            TimesCount++;
                        }
                        if (TimesCount == Times)
                        {
                            //Debug.WriteLine(DateTemp);
                            EndDay_L[x] = DateTemp.ToString("yyMMdd");
                            continue;
                        }
                        DateTemp = DateTemp.AddDays(1);
                        goto Keep;
                    }
                }
                DateTime.TryParseExact(TreatmentDate[0], "yyyy-MM-dd", null, DateTimeStyles.None, out DateTime FirstDate);
                bool yn;FileNameOutput_S = $@"{OutputPath_S}\{Path.GetFileNameWithoutExtension(FullFileName_S)}_{Time_S}.txt";
                yn = oncube.KuangTien_UD(MedicineName_L, MedicineCode_L, AdminCode_L, PerQty_L, SumQty_L, Settings, StartDay_L, EndDay_L,
                    PatientName_L, PrescriptionNo_L, BedNo_L, BarcodeDic, FileNameOutput_S, Class_L, StayDay_L, DataDic, DoseType, CrossAdminTimeType, FirstDate.ToString("yyMMdd"), QODDescription,
                    CurrentDate, "住院", SpecialCode);
                if (yn)
                    return ResultType.成功;
                else
                {
                    List<string> DaysList = new List<string>();
                    for (int x = 0; x <= StartDay_L.Count - 1; x++)
                    {
                        DaysList.Add(StartDay_L[x] + "~" + EndDay_L[x]);
                    }
                    log.Prescription(FullFileName_S, PatientName_L[0], "", MedicineCode_L, MedicineName_L, AdminCode_L, PerQty_L, DaysList);
                    ErrorContent = $"{FullFileName_S} 產生OCS時發生問題";
                    return ResultType.失敗;
                }
            }
            catch (Exception ex)
            {
                log.Write($"{FullFileName_S} {ex}");
                ErrorContent = $"{FullFileName_S} 處理邏輯時發生問題 {ex}";
                return ResultType.失敗;
            }
        }

        private ResultType UD_Stat_Process()
        {
            try
            {
                ClearList();
                bool _JudgeMedicinePosition = false;
                var ecd = Encoding.Default;
                oncube = new OnputType_OnCube(log);
                string[] FileContentSplit = DeleteSpace(GetContent).Split('\n');
                int Hos = -1;
                Byte[] Temp = Encoding.Default.GetBytes(FileContentSplit[0]);
                for (int r = 0; r <= FileContentSplit.Length - 1; r++)  //將藥品資料放入List<string>
                {
                    if (FileContentSplit[r].Contains("-----"))
                        continue;
                    if (FileContentSplit[r].Trim().Equals(""))
                        continue;
                    if (FileContentSplit[r].Contains("診療日"))
                    {
                        Hos += 1;
                        Byte[] ATemp = Encoding.Default.GetBytes(FileContentSplit[r]);
                        string StartDay_L = (Convert.ToInt32(ecd.GetString(ATemp, 8, 3) + ecd.GetString(ATemp, 12, 2) + ecd.GetString(ATemp, 15, 2)) + 19110000).ToString();
                        HospitalInformation = $"{StartDay_L};{ecd.GetString(ATemp, 27, 10).Trim()}";
                        HospitalInformationDic[Hos] = HospitalInformation;
                        HospitalInformation = "";
                    }
                    else if (FileContentSplit[r].Contains("床號"))
                    {
                        Byte[] ATemp = Encoding.Default.GetBytes(FileContentSplit[r]);
                        PatientInformation = $"{ecd.GetString(ATemp, 6, 9).Trim()};{ecd.GetString(ATemp, 22, 11).Trim()};{ecd.GetString(ATemp, 38, 11).Trim()};{ecd.GetString(ATemp, 70, 10).Trim()}";
                        PatientInformationDic[Hos] = PatientInformation;
                        PatientInformation = "";
                    }
                    else if (FileContentSplit[r].Contains("醫囑代碼"))
                    {
                        _JudgeMedicinePosition = true;
                        continue;
                    }
                    else if (FileContentSplit[r].Contains("C!"))
                        _JudgeMedicinePosition = false;
                    if (_JudgeMedicinePosition)
                    {
                        Byte[] ATemp = Encoding.Default.GetBytes(FileContentSplit[r]);
                        MedicineInformation = $"{ecd.GetString(ATemp, 5, 10).Trim()};{ecd.GetString(ATemp, 15, 21).Trim()};{ecd.GetString(ATemp, 42, 6).Trim()};" +
                            $"{ecd.GetString(ATemp, 48, 5).Trim()};{ecd.GetString(ATemp, 53, 5).Trim()};{ecd.GetString(ATemp, 58, 5)};{ecd.GetString(ATemp, 63, 8).Trim()};{ecd.GetString(ATemp, 71, 7).Trim()};" +
                            $"{ecd.GetString(ATemp, 79, 4)};{ecd.GetString(ATemp, 83, ATemp.Length - 83)};{ecd.GetString(ATemp, 92, 7)}、";
                        if (MedicineInformationDic.ContainsKey(Hos))
                            MedicineInformationDic[Hos] = MedicineInformationDic[Hos] + MedicineInformation;
                        else
                        {
                            MedicineInformationDic.Add(Hos, MedicineInformation);
                        }
                        MedicineInformation = "";
                    }
                }
                StringBuilder sb = new StringBuilder();
                List<string> Data = new List<string>();
                foreach (var v in HospitalInformationDic)
                {
                    Debug.WriteLine(v.Key);
                    string[] MedicineSplit = MedicineInformationDic[v.Key].Split('、');
                    foreach (string s in MedicineSplit)
                    {
                        if (!string.IsNullOrEmpty(s))
                        {
                            sb.Append(HospitalInformationDic[v.Key] + ";");
                            sb.Append(PatientInformationDic[v.Key] + ";");
                            sb.Append(s);
                        }
                        Data.Add(sb.ToString());
                        sb.Clear();
                    }
                }
                int number = 0;
                for (int r = 0; r <= Data.Count - 1; r++)
                {
                    BirthDate_L.Add("1997-01-01");
                    if (string.IsNullOrEmpty(Data[r]))
                        continue;
                    string[] DataSplit = Data[r].Split(';');
                    if (Settings.EN_FilterMedicineCode && !MedicineCodeGiven_L.Contains(DataSplit[6]))
                        continue;
                    AdminCode_S = DataSplit[9] + DataSplit[11].Trim();
                    if (JudgePackedMode(AdminCode_S))
                        continue;
                    //if (CrossDayAdminTIme.Contains(DataSplit[9] + DataSplit[11].Trim()))
                    //    continue;
                    if (AdminCode_S.Contains("TTS"))
                        AdminCode_S = "STTS";
                    if (!GOD.Is_Admin_Code_For_Multi_Created(AdminCode_S))
                    {
                        log.Write($"{FullFileName_S} 在OnCube中未建置此餐包頻率 {AdminCode_S}");
                        continue;
                    }
                    else
                        CalculationDays.Add($"{GOD.Get_Admin_Code_For_Multi(AdminCode_S).Count}");
                    if (!GOD.Is_Admin_Code_For_Combi_Created($"S{AdminCode_S}"))
                    {
                        log.Write($"{FullFileName_S} 在OnCube中未建置此種包頻率 S{AdminCode_S}");
                        continue;
                    }
                    DateTime.TryParseExact(DataSplit[0], "yyyyMMdd", null, DateTimeStyles.None, out DateTime _treatmentdate);
                    TreatmentDate.Add(_treatmentdate.ToString("yyyy-MM-dd"));
                    StayDay_L.Add(DataSplit[1].Trim());
                    BedNo_L.Add(DataSplit[2].Trim());
                    PrescriptionNo_L.Add(DataSplit[3].Trim());
                    PatientName_L.Add(DataSplit[4].Trim());
                    Class_L.Add(DataSplit[5].Trim());
                    MedicineCode_L.Add(DataSplit[6].Trim());
                    MedicineName_L.Add(DataSplit[7].Trim());
                    PerQty_L.Add(DataSplit[8].Trim());
                    AdminCode_L.Add(AdminCode_S);
                    Days_L.Add(DataSplit[10].Trim());
                    SumQty_L.Add(DataSplit[12].Trim());
                    PrescriptionCutTime.Add(DataSplit[14].Trim());
                    SpecialCode.Add(DataSplit[16].Trim());
                    DateTime.TryParseExact((Int32.Parse(DataSplit[13]) + 19110000).ToString(), "yyyyMMdd", null, DateTimeStyles.None, out DateTime _StartDay_L);
                    Debug.WriteLine(_StartDay_L);
                    StartDay_L.Add(_StartDay_L.ToString("yyMMdd"));
                    //if (!string.IsNullOrEmpty(DataSplit[15]) & int.TryParse(DataSplit[15], out int i))
                    //{
                    //    DateTime.TryParseExact((Int32.Parse(DataSplit[15]) + 19110000).ToString(), "yyyyMMdd", null, DateTimeStyles.None, out DateTime _EndDay_L);
                    //    EndDay_L.Add(_EndDay_L.ToString("yyMMdd"));
                    //}
                    //else
                    EndDay_L.Add("");
                    if (!BarcodeDic.ContainsKey(Int32.Parse(DataSplit[3])))
                        BarcodeDic.Add(Int32.Parse(DataSplit[3]), $"{DataSplit[3]},{DataSplit[2]},{DateTime.Now:yyyy-MM-dd},*{DataSplit[6]}#{DataSplit[9] + DataSplit[11].Trim()}#{Math.Ceiling(Convert.ToSingle(DataSplit[12]))}");
                    else
                        BarcodeDic[Int32.Parse(DataSplit[3])] += $"*{DataSplit[6]}#{DataSplit[9] + DataSplit[11].Trim()}#{Math.Ceiling(Convert.ToSingle(DataSplit[12]))}";
                    number += 1;
                }
                if (AdminCode_L.Count == 0)
                    return ResultType.全數過濾;
                return ResultType.成功;
            }
            catch (Exception ex)
            {
                log.Write($"{FullFileName_S} {ex}");
                ErrorContent = $"{FullFileName_S} 讀取處方籤時發生問題 {ex}";
                return ResultType.失敗;
            }
        }

        public ResultType UD_Stat_Logic()
        {
            if (AdminCode_L.Count == 0)
                return ResultType.全數過濾;
            log.Check();
            try
            {
                Dictionary<string, List<string>> DataDic = new Dictionary<string, List<string>>();
                List<bool> DoseType = new List<bool>();  //False > MultiDose，True > CombiDose
                List<bool> CrossAdminTimeType = new List<bool>();  //False > No，True > Yes
                List<int> TimesList = new List<int>();
                List<int> AdminTimeTimePointList = new List<int>();
                DateTime DateTemp = new DateTime();
                string JudgeTimer = "";
                int TimesCount = 0;
                string TimeTemp;
                string QODTEMP;
                List<string> QODDescription = new List<string>();
                List<string> CurrentDate = new List<string>();
                List<string> STAdminTime = new List<string>() { "STAC", "STPC", "STWM", "STPO", "STOTHER", "STOU", "STRECT" };
                List<string> AdminTimeList = new List<string>();
                for (int x = 0; x <= AdminCode_L.Count - 1; x++)
                {
                    TimesCount = 0;
                    QODTEMP = "";
                    CurrentDate.Add("");
                    CrossAdminTimeType.Add(false);
                    float PerQtyTemp = 0;
                    if (PerQty_L[x].Contains("."))
                        PerQtyTemp = Convert.ToSingle($"0{PerQty_L[x]}");
                    else
                        PerQtyTemp = Convert.ToSingle(PerQty_L[x]);
                    int Times = Convert.ToInt32(Convert.ToSingle(SumQty_L[x]) / PerQtyTemp);
                    DateTime.TryParseExact(TreatmentDate[x], "yyyy-MM-dd", null, DateTimeStyles.None, out DateTime TMD);
                    DateTime.TryParseExact(StartDay_L[x], "yyMMdd", null, DateTimeStyles.None, out DateTime SD);
                    Debug.WriteLine($"{TMD} {SD}");
                    if (AdminCode_L[x] == "STTS")  //途徑為TTS ，輸出種包
                    {
                        DoseType.Add(true);
                        EndDay_L[x] = SD.ToString("yyMMdd");
                        if (Properties.Settings.Default.DoseType == "Multi" || !PerQtyTemp.ToString().Contains("."))
                            QODDescription.Add($"每次劑量 : {PerQtyTemp}   ");
                        else
                            QODDescription.Add($"每次劑量 : {PerQtyTemp} 非整數");
                        DataDic.Add($"{x}_{EndDay_L[x]}", new List<string>() { "" });
                        PerQty_L[x] = Math.Ceiling(Convert.ToSingle(SumQty_L[x])).ToString();
                        continue;
                    }
                    if (Properties.Settings.Default.DoseType == "Combi" || !int.TryParse(PerQty_L[x], out int i) | STAdminTime.Contains(AdminCode_L[x]) | Settings.CrossDayAdminCode.Contains(AdminCode_L[x]))  //劑量為非整數 ，輸出種包
                    {
                        DoseType.Add(true);
                        EndDay_L[x] = SD.ToString("yyMMdd");
                        DataDic.Add($"{x}_{EndDay_L[x]}", new List<string>() { "" });
                        if (Properties.Settings.Default.DoseType == "Multi" || !PerQtyTemp.ToString().Contains("."))
                            QODDescription.Add($"每次劑量 : {PerQtyTemp}   ");
                        else
                            QODDescription.Add($"每次劑量 : {PerQtyTemp} 非整數");
                        AdminCode_L[x] = $"S{AdminCode_L[x]}";
                        PerQty_L[x] = Math.Ceiling(Convert.ToSingle(SumQty_L[x])).ToString();
                        continue;
                    }
                    JudgeTimer = PrescriptionCutTime[x].Insert(2, ":");
                    DoseType.Add(false);
                    QODDescription.Add(QODTEMP);
                    DateTemp = SD;
                    foreach (string s in GOD.Get_Admin_Code_For_Multi(AdminCode_L[x]))
                    {
                        AdminTimeTimePointList.Add(Convert.ToInt32(s.Substring(0, 2)));
                    }
                    AdminTimeList = GOD.Get_Admin_Code_For_Multi(AdminCode_L[x]);
                    foreach (int y in AdminTimeTimePointList)
                    {
                        if (TimesCount == Times)
                            break;
                        if (y.ToString().Length == 1)
                            TimeTemp = "0" + y;
                        else
                            TimeTemp = y.ToString();
                        DateTime PrescriptionTime = Convert.ToDateTime(JudgeTimer);
                        if (!DataDic.ContainsKey($"{x}_{DateTemp:yyMMdd}"))
                        {
                            string Time = "";
                            foreach (string s in AdminTimeList)
                            {
                                if (s.Substring(0, 2) == TimeTemp)
                                {
                                    Time = s;
                                    break;
                                }
                            }
                            DateTime OnCubeTime = Convert.ToDateTime(Time);
                            if (DateTime.Compare(PrescriptionTime, OnCubeTime) <= 0)
                            {
                                DataDic.Add($"{x}_{DateTemp:yyMMdd}", new List<string>() { TimeTemp });
                                TimesCount += 1;
                            }
                        }
                        else
                        {
                            DataDic[$"{x}_{DateTemp:yyMMdd}"].Add(TimeTemp);
                            TimesCount += 1;
                        }
                        if (TimesCount == Times)
                        {
                            EndDay_L[x] = DateTemp.ToString("yyMMdd");
                            continue;
                        }
                    }
                    DateTemp = DateTemp.AddDays(1);
                    if (TimesCount < Times)
                    {
                    Keep:
                        foreach (int y in AdminTimeTimePointList)
                        {
                            if (TimesCount == Times)
                                break;
                            if (y.ToString().Length == 1)
                                TimeTemp = "0" + y;
                            else
                                TimeTemp = y.ToString();
                            if (DataDic.ContainsKey($"{x}_{DateTemp:yyMMdd}"))
                                DataDic[$"{x}_{DateTemp:yyMMdd}"].Add(TimeTemp.ToString());
                            else
                                DataDic.Add($"{x}_{DateTemp:yyMMdd}", new List<string>() { TimeTemp });
                            TimesCount += 1;
                        }
                        if (TimesCount == Times)
                        {
                            EndDay_L[x] = DateTemp.ToString("yyMMdd");
                            continue;
                        }
                        DateTemp = DateTemp.AddDays(1);
                        goto Keep;
                    }
                }
                DateTime.TryParseExact(TreatmentDate[0], "yyyy-MM-dd", null, DateTimeStyles.None, out DateTime FirstDate);
                bool yn;
                FileNameOutput_S = $@"{OutputPath_S}\{PatientName_L[0]}_{PrescriptionNo_L[0]}_{Time_S}.txt";
                yn = oncube.KuangTien_UD(MedicineName_L, MedicineCode_L, AdminCode_L, PerQty_L, SumQty_L, Settings, StartDay_L, EndDay_L,
                    PatientName_L, PrescriptionNo_L, BedNo_L, BarcodeDic, FileNameOutput_S, Class_L, StayDay_L, DataDic, DoseType, CrossAdminTimeType, FirstDate.ToString("yyMMdd"), QODDescription,
                    CurrentDate, "即時", SpecialCode);
                if (yn)
                    return ResultType.成功;
                else
                {
                    List<string> DaysList = new List<string>();
                    for (int x = 0; x <= StartDay_L.Count - 1; x++)
                    {
                        DaysList.Add(StartDay_L[x] + "~" + EndDay_L[x]);
                    }
                    log.Prescription(FullFileName_S, PatientName_L[0].ToString(), "", MedicineCode_L, MedicineName_L, AdminCode_L, PerQty_L, DaysList);
                    ErrorContent = $"{FullFileName_S} 產生OCS時發生問題";
                    return ResultType.失敗;
                }
            }
            catch (Exception ex)
            {
                log.Write($"{FullFileName_S} {ex}");
                ErrorContent = $"{FullFileName_S} 處理邏輯時發生問題 {ex}";
                return ResultType.失敗;
            }
        }

        private void ClearList()
        {
            TreatmentDate.Clear();
            PrescriptionCutTime.Clear();
            CalculationDays.Clear();
            SpecialCode.Clear();
            TimesPerDay.Clear();
            Dic.Clear();
            JudgeMedicineGivenDic.Clear();
            HospitalInformationDic.Clear();
            PatientInformationDic.Clear();
            MedicineInformationDic.Clear();
            BarcodeDic.Clear();
        }
    }
}