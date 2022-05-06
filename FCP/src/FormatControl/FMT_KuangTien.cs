using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FCP.src.Enum;
using Helper;
using FCP.Models;
using System.Diagnostics;

namespace FCP.src.FormatControl
{
    class FMT_KuangTien : FormatCollection
    {
        private string HospitalInformation = "";
        private string PatientInformation = "";
        private string MedicineInformation = "";
        private List<string> TreatmentDate = new List<string>();
        private List<string> PrescriptionCutTime = new List<string>();
        private List<string> CalculationDays = new List<string>();
        private List<string> SpecialCode = new List<string>();
        private List<string> TimesPerDay = new List<string>();
        private Dictionary<string, string> JudgeMedicineGivenDic = new Dictionary<string, string>();
        private Dictionary<int, string> HospitalInformationDic = new Dictionary<int, string>();
        private Dictionary<int, string> PatientInformationDic = new Dictionary<int, string>();
        private Dictionary<int, string> MedicineInformationDic = new Dictionary<int, string>();
        private Dictionary<int, string> BarcodeDic = new Dictionary<int, string>();
        private Dictionary<string, List<KuangTienPowder>> _powder = new Dictionary<string, List<KuangTienPowder>>();
        private Dictionary<KuangTienUDBasic, List<KuangTienUD>> _udPrescriptions = new Dictionary<KuangTienUDBasic, List<KuangTienUD>>();
        private List<KuangTienStat> _stat = new List<KuangTienStat>();
        private KuangTienOPDBasic _opdBasic = new KuangTienOPDBasic();
        private List<KuangTienOPD> _opd = new List<KuangTienOPD>();
        private Stopwatch _sw = new Stopwatch();

        public override void ProcessOPD()
        {
            try
            {
                var ecd = Encoding.Default;
                string[] list = DeleteSpace(GetFileContent).Split('\n');
                EncodingHelper.SetBytes(list[0]);
                _opdBasic.WriteDate = EncodingHelper.GetString(9, 9);
                _opdBasic.GetMedicineNo = EncodingHelper.GetString(54, 8);
                _opdBasic.Age = EncodingHelper.GetString(67, 5);
                _opdBasic.DoctorName = EncodingHelper.GetString(77, 11);
                EncodingHelper.SetBytes(list[1]);
                _opdBasic.PatientNo = EncodingHelper.GetString(9, 11);
                _opdBasic.PatientName = EncodingHelper.GetString(25, 21);
                _opdBasic.Gender = EncodingHelper.GetString(47, 2);
                _opdBasic.Class = EncodingHelper.GetString(67, EncodingHelper.Length - 67);
                for (int x = 4; x <= list.ToList().Count - 3; x++)
                {
                    EncodingHelper.SetBytes(list[x]);
                    string adminCode = EncodingHelper.GetString(57, 11) + EncodingHelper.GetString(68, 9);
                    string medicineCode = EncodingHelper.GetString(2, 10);
                    int sumQty = Convert.ToInt32(EncodingHelper.GetString(82, 8));
                    int id1 = GetID1(medicineCode);
                    int id2 = GetID2(medicineCode);

                    if (IsFilterAdminCode(AdminCode_S))
                        continue;
                    if (IsFilterMedicineCode(medicineCode))
                        continue;
                    if (id1 >= 1 && sumQty % id1 == 0)  //總量可以被id1整除不包
                    {
                        continue;
                    }
                    if (id2 >= 1 && sumQty >= id2)  //總量超過id2(預包)數量不包
                        continue;
                    if (!IsExistsMultiAdminCode(adminCode))
                    {
                        LostMultiAdminCode(adminCode);
                        return;
                    }
                    _opd.Add(new KuangTienOPD()
                    {
                        MedicineCode = medicineCode,
                        MedicineName = EncodingHelper.GetString(12, 31),
                        AdminCode = adminCode,
                        PerQty = EncodingHelper.GetString(43, 8),
                        Days = EncodingHelper.GetString(77, 5),
                        SumQty = EncodingHelper.GetString(82, 8),
                        TimesPerDay = $"{GetMultiAdminCodeTimes(adminCode).Count}"
                    });
                }
                if (AdminCode_L.Count == 0)
                {
                    Pass();
                    return;
                }
                Success();
            }
            catch (Exception ex)
            {
                ReadFileFail(ex);
            }
        }

        public override void LogicOPD()
        {
            int year = Convert.ToInt32(DateTime.Now.ToString("yyyy")) - 1911;
            string outputDirectory = $@"{OutputDirectory}\{year}{DateTime.Now:MMdd}-{_opdBasic.GetMedicineNo}-{_opdBasic.PatientName}_{CurrentSeconds}.txt";
            try
            {
                OP_OnCube.KuangTien_OPD(_opdBasic, _opd, outputDirectory);
                Success();
            }
            catch (Exception ex)
            {
                GenerateOCSFileFail(ex);
            }
        }

        public override void ProcessUDBatch()
        {
            try
            {
                _sw.Restart();
                List<string> list = DeleteSpace(GetFileContent).Split('\n').ToList();
                StringBuilder sb = new StringBuilder();
                List<string> prescriptions = SeparateUD(list);
                Console.WriteLine($"separate {_sw.ElapsedMilliseconds}");
                _sw.Restart();
                foreach (var s in prescriptions)
                {
                    KuangTienUDBasic basic = new KuangTienUDBasic();
                    List<KuangTienUD> ud = new List<KuangTienUD>();
                    List<string> detail = s.Split('\n').ToList();
                    foreach (var v in detail)
                    {
                        if (v.Trim().Length == 0 || v.Contains("--------"))
                            continue;
                        int index = detail.IndexOf(v);
                        if (index == 2)  //診療日
                        {
                            EncodingHelper.SetBytes(v);
                            basic.TreatmentDate = DateTimeHelper.Convert($"{Convert.ToInt32(EncodingHelper.GetString(8, 9).Replace("/", "")) + 19110000}", "yyyyMMdd");
                        }
                        else if (index == 3)  //床號
                        {
                            EncodingHelper.SetBytes(v);
                            basic.BedNo = EncodingHelper.GetString(6, 9);
                            basic.PatientNo = EncodingHelper.GetString(22, 11);
                            basic.PatientName = EncodingHelper.GetString(38, 11);
                            basic.Barcode = $"{basic.PatientNo},{basic.BedNo},{DateTime.Now:yyyy-MM-dd},";
                        }
                        else if (index > 5 && !v.Contains("◎") && v.Trim().Length != 0)  //藥品
                        {
                            EncodingHelper.SetBytes(v);
                            DateTime startDate = DateTimeHelper.Convert($"{Convert.ToInt32(EncodingHelper.GetString(86, 7)) + 19110000} {EncodingHelper.GetString(94, 5)}", "yyyyMMdd HHmm");
                            string medicineCode = EncodingHelper.GetString(0, 10);
                            string adminCode = $"{EncodingHelper.GetString(53, 11)}{EncodingHelper.GetString(69, 9)}";
                            float sumQty = Convert.ToSingle(EncodingHelper.GetString(78, 8));
                            if (IsFilterMedicineCode(medicineCode) || IsFilterAdminCode(adminCode))
                                continue;
                            adminCode = adminCode.Contains("TTS") ? "STTS" : adminCode;
                            if (!IsExistsMultiAdminCode(adminCode))
                            {
                                LostMultiAdminCode(adminCode);
                                continue;
                            }
                            if (!IsExistsCombiAdminCode(adminCode))
                            {
                                LostCombiAdminCode(adminCode);
                                continue;
                            }
                            basic.Barcode += $"*{medicineCode}#{adminCode}#{Math.Ceiling(sumQty)}";
                            ud.Add(new KuangTienUD()
                            {
                                MedicineCode = medicineCode,
                                MedicineName = EncodingHelper.GetString(10, 31),
                                PerQty = Convert.ToSingle(EncodingHelper.GetString(47, 6)),
                                AdminCode = adminCode,
                                Days = Convert.ToInt32(EncodingHelper.GetString(64, 5)),
                                SumQty = sumQty,
                                StartDate = startDate,
                                PrintDate = startDate,
                                MedicineSerialNo = EncodingHelper.GetString(107, 7)
                            });
                            if (EncodingHelper.GetString(99, 7).Length != 0)
                            {
                                ud[ud.Count - 1].EndDate = DateTimeHelper.Convert($"{Convert.ToInt32(EncodingHelper.GetString(99, 7)) + 19110000}", "yyyyMMdd");
                            }
                            //var a = ud[ud.Count - 1];
                            //Console.WriteLine($"{a.MedicineCode} {a.MedicineName} {a.PerQty} {a.AdminCode} {a.Days} {a.SumQty} {a.StartDate} {a.EndDate}");
                        }
                    }
                    if (ud.Count > 0)
                    {
                        _udPrescriptions.Add(basic, ud);
                    }
                }
                Console.WriteLine($"progress {_sw.ElapsedMilliseconds}");
                _sw.Reset();
                if (_udPrescriptions.Count == 0)
                {
                    Pass();
                    return;
                }
                Success();
            }
            catch (Exception ex)
            {
                ReadFileFail(ex);
            }
        }

        public override void LogicUDBatch()
        {
            try
            {
                _sw.Start();
                foreach(var prescription in _udPrescriptions)
                {
                    List<KuangTienUD> ud = new List<KuangTienUD>();
                    for (int i = 0; i <= prescription.Value.Count - 1; i++)
                    {
                        KuangTienUDBasic basic = prescription.Key;
                        var medicine = prescription.Value[i];
                        float perQty = medicine.PerQty;
                        float sumQty = medicine.SumQty;
                        int days = medicine.Days;
                        int times = Convert.ToInt32(sumQty / perQty);
                        int currentTimes = 1;
                        string adminCode = medicine.AdminCode;
                        string medicineCode = medicine.MedicineCode;
                        bool perQtyIsnteger = !$"{perQty}".Contains(".");
                        bool multiDose = SettingModel.DoseType == eDoseType.餐包;
                        DateTime startDate = medicine.StartDate;
                        DateTime treatmentDate = basic.TreatmentDate;
                        List<string> adminCodeTimes = GetMultiAdminCodeTimes(medicine.AdminCode);
                        if (basic.BedNo.Contains("111DAY"))
                        {
                            medicine.Description = $"每次劑量 : {perQty}";
                            medicine.Description = perQtyIsnteger ? "   " : " 非整數";
                            basic.Care = true;
                            medicine.DoseType = eDoseType.種包;
                            medicine.EndDate = medicine.StartDate;
                            medicine.AdminCode = $"S{medicine.AdminCode}";
                            medicine.PerQty = sumQty;
                            ud.Add(medicine.Clone());
                            //沙鹿
                            //CommonModel.SqlHelper.Execute("update PrintFormItem set DeletedYN=1 where RawID in (120180,120195)");

                            //大甲
                            CommonModel.SqlHelper.Execute("update PrintFormItem set DeletedYN=1 where RawID in (120156,120172)");
                            continue;
                        }
                        //種包
                        else if (!multiDose || !perQtyIsnteger || adminCode.Contains("STTS") || medicineCode == "23521")
                        {
                            medicine.DoseType = eDoseType.種包;
                            medicine.EndDate = medicine.StartDate;
                            medicine.PerQty = sumQty;
                            if (medicine.AdminCode == "QODAC" || medicine.AdminCode == "QODPC")
                            {
                                medicine.CrossDay = true;
                                while (currentTimes <= times)
                                {
                                    if (currentTimes == 1)
                                    {
                                        startDate = startDate.AddDays(2);
                                    }
                                    medicine.Description += $"{startDate:MM/dd} 劑量 : {perQty}";
                                    medicine.Description += multiDose || perQtyIsnteger ? "   " : " 非整數";
                                    currentTimes++;
                                }
                            }
                            else
                            {
                                medicine.Description += $"每次劑量 : {perQty}";
                                medicine.Description += multiDose || perQtyIsnteger ? "   " : " 非整數";
                            }
                            medicine.AdminCode = $"S{adminCode}";
                            ud.Add(medicine.Clone());
                            continue;
                        }
                        //跨天數
                        else if (SettingModel.CrossDayAdminCode.Contains(adminCode))
                        {
                            medicine.DoseType = eDoseType.餐包;
                            medicine.CrossDay = true;
                            medicine.TakingDescription = $"服用日 {startDate:yyyy/MM/dd}";
                            medicine.EndDate = DateTime.Compare(startDate, treatmentDate.AddDays(days)) == 1 ? startDate : treatmentDate.AddDays(days);
                            ud.Add(medicine.Clone());
                            continue;
                        }
                        while (currentTimes <= times)
                        {
                            medicine.DoseType = eDoseType.餐包;
                            foreach (var time in adminCodeTimes)
                            {
                                if (currentTimes > times)
                                {
                                    break;
                                }
                                DateTime adminDate = DateTimeHelper.Convert($"{startDate:yyyyMMdd} {time}", "yyyyMMdd HH:mm");
                                if (DateTime.Compare(adminDate, startDate) >= 0)
                                {
                                    medicine.TakingDescription = $"服用日 {adminDate:yyyy/MM/dd}";
                                    medicine.StartDate = adminDate;
                                    medicine.EndDate = adminDate;
                                    ud.Add(medicine.Clone());

                                    currentTimes++;
                                }
                            }
                            startDate = Convert.ToDateTime($"{startDate:yyyy/MM/dd} 00:00:00");
                            startDate = startDate.AddDays(1);
                        }
                    }
                    _udPrescriptions[prescription.Key].Clear();
                    _udPrescriptions[prescription.Key].AddRange(ud);
                }
                Console.WriteLine($"logic {_sw.ElapsedMilliseconds}");
            }
            catch (Exception ex)
            {
                ProgressLogicFail(ex);
                return;
            }
            try
            {
                string outputDirectory = $@"{OutputDirectory}\{SourceFileNameWithoutExtension}_{CurrentSeconds}.txt";
                _sw.Restart();
                OP_OnCube.KuangTien_UD(_udPrescriptions, outputDirectory);
                Console.WriteLine($"ocs {_sw.ElapsedMilliseconds}");
                Success();
            }
            catch (Exception ex)
            {
                GenerateOCSFileFail(ex);
            }
        }

        public override void ProcessUDStat()
        {
            try
            {
                bool _JudgeMedicinePosition = false;
                var ecd = Encoding.Default;
                string[] FileContentSplit = DeleteSpace(GetFileContent).Split('\n');
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
                    if (IsFilterMedicineCode(DataSplit[6]))
                        continue;
                    AdminCode_S = DataSplit[9] + DataSplit[11].Trim();
                    if (IsFilterAdminCode(AdminCode_S))
                    {
                        continue;
                    }
                    if (AdminCode_S.Contains("TTS"))
                        AdminCode_S = "STTS";
                    if (!IsExistsMultiAdminCode(AdminCode_S))
                    {
                        Log.Write($"{SourceFilePath} 在OnCube中未建置此餐包頻率 {AdminCode_S}");
                        continue;
                    }
                    else
                        CalculationDays.Add($"{GetMultiAdminCodeTimes(AdminCode_S).Count}");
                    if (!IsExistsCombiAdminCode($"{AdminCode_S}"))
                    {
                        Log.Write($"{SourceFilePath} 在OnCube中未建置此種包頻率 S{AdminCode_S}");
                        continue;
                    }
                    DateTime treatmentDate = DateTimeHelper.Convert(DataSplit[0], "yyyyMMdd");
                    TreatmentDate.Add(treatmentDate.ToString("yyyy-MM-dd"));
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
                    DateTime startDate = DateTimeHelper.Convert((Convert.ToInt32(DataSplit[13]) + 19110000).ToString(), "yyyyMMdd");
                    StartDay_L.Add(startDate.ToString("yyMMdd"));
                    EndDay_L.Add("");
                    if (!BarcodeDic.ContainsKey(Convert.ToInt32(DataSplit[3])))
                        BarcodeDic.Add(Convert.ToInt32(DataSplit[3]), $"{DataSplit[3]},{DataSplit[2]},{DateTime.Now:yyyy-MM-dd},*{DataSplit[6]}#{DataSplit[9] + DataSplit[11].Trim()}#{Math.Ceiling(Convert.ToSingle(DataSplit[12]))}");
                    else
                        BarcodeDic[Convert.ToInt32(DataSplit[3])] += $"*{DataSplit[6]}#{DataSplit[9] + DataSplit[11].Trim()}#{Math.Ceiling(Convert.ToSingle(DataSplit[12]))}";
                    number += 1;
                }
                if (AdminCode_L.Count == 0)
                {
                    Pass();
                    return;
                }
                Success();
            }
            catch (Exception ex)
            {
                ReadFileFail(ex);
            }
        }

        public override void LogicUDStat()
        {
            if (AdminCode_L.Count == 0)
            {
                Pass();
                return;
            }
            try
            {
                Dictionary<string, List<string>> DataDic = new Dictionary<string, List<string>>();
                List<bool> doseType = new List<bool>();  //False > MultiDose，True > CombiDose
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
                    Console.WriteLine(AdminCode_L[x]);
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

                    DateTime tmd = DateTimeHelper.Convert(TreatmentDate[x], "yyyy-MM-dd");
                    DateTime sd = DateTimeHelper.Convert(StartDay_L[x], "yyMMdd");
                    if (AdminCode_L[x] == "STTS")  //途徑為TTS ，輸出種包
                    {
                        doseType.Add(true);
                        EndDay_L[x] = sd.ToString("yyMMdd");
                        if (DoseType == eDoseType.餐包 || !PerQtyTemp.ToString().Contains("."))
                            QODDescription.Add($"每次劑量 : {PerQtyTemp}   ");
                        else
                            QODDescription.Add($"每次劑量 : {PerQtyTemp} 非整數");
                        DataDic.Add($"{x}_{EndDay_L[x]}", new List<string>() { "" });
                        PerQty_L[x] = Math.Ceiling(Convert.ToSingle(SumQty_L[x])).ToString();
                        continue;
                    }
                    if (DoseType == eDoseType.種包 || !int.TryParse(PerQty_L[x], out int i) | STAdminTime.Contains(AdminCode_L[x]) | CrossDayAdminCode.Contains(AdminCode_L[x]))  //劑量為非整數 ，輸出種包
                    {
                        doseType.Add(true);
                        EndDay_L[x] = sd.ToString("yyMMdd");
                        DataDic.Add($"{x}_{EndDay_L[x]}", new List<string>() { "" });
                        if (DoseType == eDoseType.餐包 || !PerQtyTemp.ToString().Contains("."))
                            QODDescription.Add($"每次劑量 : {PerQtyTemp}   ");
                        else
                            QODDescription.Add($"每次劑量 : {PerQtyTemp} 非整數");
                        AdminCode_L[x] = $"S{AdminCode_L[x]}";
                        PerQty_L[x] = Math.Ceiling(Convert.ToSingle(SumQty_L[x])).ToString();
                        continue;
                    }
                    JudgeTimer = PrescriptionCutTime[x].Insert(2, ":");
                    doseType.Add(false);
                    QODDescription.Add(QODTEMP);
                    DateTemp = sd;
                    AdminTimeTimePointList.Clear();
                    foreach (string s in GetMultiAdminCodeTimes(AdminCode_L[x]))
                    {
                        AdminTimeTimePointList.Add(Convert.ToInt32(s.Substring(0, 2)));
                    }
                    AdminTimeList = GetMultiAdminCodeTimes(AdminCode_L[x]);
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
                            {
                                DataDic[$"{x}_{DateTemp:yyMMdd}"].Add(TimeTemp.ToString());
                            }
                            else
                            {
                                DataDic.Add($"{x}_{DateTemp:yyMMdd}", new List<string>() { TimeTemp });
                            }
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
                DateTime firstDate = DateTimeHelper.Convert(TreatmentDate[0], "yyyy-MM-dd");
                bool yn = true;
                string outputDirectory = $@"{OutputDirectory}\{PatientName_L[0]}_{PrescriptionNo_L[0]}_{CurrentSeconds}.txt";
                //yn = OP_OnCube.KuangTien_UD(MedicineName_L, MedicineCode_L, AdminCode_L, PerQty_L, SumQty_L, StartDay_L, EndDay_L,
                //    PatientName_L, PrescriptionNo_L, BedNo_L, BarcodeDic, outputDirectory, Class_L, StayDay_L, DataDic, doseType, CrossAdminTimeType, firstDate.ToString("yyMMdd"), QODDescription,
                //    CurrentDate, "即時", SpecialCode);
                if (yn)
                    Success();
                else
                {
                    GenerateOCSFileFail();
                }
            }
            catch (Exception ex)
            {
                ProgressLogicFail(ex);
            }
        }

        public override void ProcessPowder()
        {
            try
            {
                List<string> list = DeleteSpace(GetFileContent).Split('\n').ToList();
                EncodingHelper.SetBytes(list[0]);
                int year = Convert.ToInt32(EncodingHelper.GetString(9, 3)) + 1911;
                DateTime startDate = Convert.ToDateTime($"{year}{EncodingHelper.GetString(12, 6)}");
                string getMedicineNumber = EncodingHelper.GetString(54, 8);
                EncodingHelper.SetBytes(list[1]);
                string patientNo = EncodingHelper.GetString(9, 11);
                string patientName = EncodingHelper.GetString(25, 21);
                string gender = EncodingHelper.GetString(47, 2);
                string _class = EncodingHelper.GetString(67, EncodingHelper.Length - 67);
                list.RemoveRange(0, 4);
                list.RemoveRange(list.Count - 2, 2);
                foreach (string s in list)
                {
                    if (s.Contains("=====") || s.Trim().Length == 0)
                        continue;
                    EncodingHelper.SetBytes(s.TrimEnd('\n'));
                    string adminCode = EncodingHelper.GetString(57, 11) + EncodingHelper.GetString(68, 9);
                    string grindTable = EncodingHelper.GetString(100, 8);
                    if (grindTable.Length == 0)
                        continue;
                    if (IsFilterAdminCode(adminCode))
                        continue;
                    var powder = new KuangTienPowder()
                    {
                        StartDate = startDate,
                        GetMedicineNo = getMedicineNumber,
                        PatientNo = patientNo,
                        PatientName = patientName,
                        Gender = gender,
                        Class = _class,
                        MedicineCode = EncodingHelper.GetString(2, 10),
                        MedicineName = EncodingHelper.GetString(12, 31),
                        PerQty = EncodingHelper.GetString(43, 8),
                        AdminCode = adminCode,
                        Days = EncodingHelper.GetString(77, 5),
                        SumQty = EncodingHelper.GetString(82, 8),
                        GrindTable = grindTable,
                        EffectiveDate = DateTime.Now.AddDays(Convert.ToInt32(EncodingHelper.GetString(77, 5)))
                    };
                    powder.TimesPerDay = Math.Round((Convert.ToSingle(powder.SumQty) / Convert.ToInt32(powder.Days) / Convert.ToSingle(powder.PerQty)), 0, MidpointRounding.AwayFromZero).ToString();
                    if (!_powder.ContainsKey(grindTable))
                        _powder[grindTable] = new List<KuangTienPowder>();
                    _powder[grindTable].Add(powder);
                }
                if (_powder.Count == 0)
                {
                    Pass();
                    return;
                }
                Success();
            }
            catch (Exception ex)
            {
                ReadFileFail(ex);
            }
        }

        public override void LogicPowder()
        {
            int year = Convert.ToInt32(DateTime.Now.ToString("yyyy")) - 1911;
            var firstPowder = _powder.Select(x => x).First().Value[0];
            string outputDirectory = $@"{OutputDirectory}\{year}{DateTime.Now:MMdd}_{firstPowder.GetMedicineNo}_{firstPowder.PatientName}_{CurrentSeconds}.txt";
            List<string> grindTableList = new List<string>();
            grindTableList.AddRange(_powder.Where(x => !grindTableList.Contains(x.Key)).Select(x => x.Key));
            try
            {
                OP_JVServer.KuangTien_磨粉(_powder, grindTableList, outputDirectory);
                Success();
            }
            catch (Exception ex)
            {
                GenerateOCSFileFail(ex);
            }
        }

        public override void ProcessOther()
        {
            throw new NotImplementedException();
        }

        public override void LogicOther()
        {
            throw new NotImplementedException();
        }

        public override void ProcessCare()
        {
            throw new NotImplementedException();
        }

        public override void LogicCare()
        {
            throw new NotImplementedException();
        }

        public override ReturnsResultModel DepartmentShunt()
        {
            ClearList();
            _udPrescriptions.Clear();
            return base.DepartmentShunt();
        }

        private List<string> SeparateUD(List<string> list)
        {
            StringBuilder sb = new StringBuilder();
            List<string> prescriptions = new List<string>();
            foreach (var s in list)
            {
                if (s.Contains("C!") && sb.ToString().Trim().Length > 0)
                {
                    prescriptions.Add(sb.ToString());
                    sb.Clear();
                }
                sb.AppendLine(s.Trim());
                if (list.IndexOf(s) == list.Count - 1)
                {
                    prescriptions.Add(sb.ToString());
                }
            }
            return prescriptions;
        }

        private void ClearList()
        {
            TreatmentDate.Clear();
            PrescriptionCutTime.Clear();
            CalculationDays.Clear();
            SpecialCode.Clear();
            TimesPerDay.Clear();
            _powder.Clear();
            JudgeMedicineGivenDic.Clear();
            HospitalInformationDic.Clear();
            PatientInformationDic.Clear();
            MedicineInformationDic.Clear();
            BarcodeDic.Clear();
        }
    }


    internal class KuangTienOPDBasic
    {
        public string WriteDate { get; set; }
        public string GetMedicineNo { get; set; }
        public string Age { get; set; }
        public string DoctorName { get; set; }
        public string PatientNo { get; set; }
        public string PatientName { get; set; }
        public string Gender { get; set; }
        public string Class { get; set; }
    }

    internal class KuangTienPowder
    {
        public DateTime StartDate { get; set; }
        public string GetMedicineNo { get; set; }
        public string PatientNo { get; set; }
        public string PatientName { get; set; }
        public string Gender { get; set; }
        public string Class { get; set; }
        public string MedicineCode { get; set; }
        public string MedicineName { get; set; }
        public string PerQty { get; set; }
        public string AdminCode { get; set; }
        public string Days { get; set; }
        public string SumQty { get; set; }
        public string GrindTable { get; set; }
        public string TimesPerDay { get; set; }
        public DateTime EffectiveDate { get; set; }
    }

    internal class KuangTienUDBasic
    {
        public DateTime TreatmentDate { get; set; }
        public string BedNo { get; set; }
        public string PatientNo { get; set; }
        public string PatientName { get; set; }
        public string Barcode { get; set; }
        public bool Care { get; set; }
    }

    internal class KuangTienUD
    {
        public string MedicineCode { get; set; }
        public string MedicineName { get; set; }
        public float PerQty { get; set; }
        public string AdminCode { get; set; }
        public int Days { get; set; }
        public float SumQty { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime PrintDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public eDoseType DoseType { get; set; }
        public bool CrossDay { get; set; }
        public string TakingDescription { get; set; } = string.Empty;
        public string MedicineSerialNo { get; set; }

        public KuangTienUD Clone()
        {
            return (KuangTienUD)this.MemberwiseClone();
        }
    }

    internal class KuangTienStat
    {

    }

    internal class KuangTienOPD
    {
        public string MedicineCode { get; set; }
        public string MedicineName { get; set; }
        public string AdminCode { get; set; }
        public string PerQty { get; set; }
        public string Days { get; set; }
        public string SumQty { get; set; }
        public string TimesPerDay { get; set; }
    }
}