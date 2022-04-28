using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using FCP.Models;
using FCP.src.Enum;
using Helper;
using FCP.src.Factory.Models;
using FCP.src.FormatControl;

namespace FCP
{
    static class OP_OnCube
    {
        private static SettingsModel _SettingsModel { get => SettingsFactory.GenerateSettingsModel(); }

        public static void JVServer(List<JVServerOPD> opd, JVServerOPDBasic basic, List<string> oncubeRandom, string random, string outputDirectory)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                foreach (var v in opd)
                {
                    eDoseType doseType = GetDoseType(v.AdminCode);
                    sb.Append(ECD(basic.PatientName, 20));
                    sb.Append(basic.PatientNo.PadRight(30));
                    sb.Append(ECD(basic.LocationName, 50));
                    sb.Append("".PadRight(29));
                    if (doseType == eDoseType.餐包)
                        sb.Append(v.PerQty.PadRight(5));
                    else
                        sb.Append(v.SumQty.PadRight(5));
                    sb.Append(v.MedicineCode.PadRight(20));
                    sb.Append(ECD(v.MedicineName, 50));
                    sb.Append(v.AdminCode.PadRight(20));
                    if (doseType == eDoseType.餐包)
                    {
                        sb.Append(v.StartDay);
                        sb.Append(v.EndDay);
                    }
                    else
                    {
                        sb.Append(v.EndDay);
                        sb.Append(v.EndDay);
                    }
                    sb.Append("3       ");
                    sb.Append("".PadRight(50));
                    sb.Append(basic.PrescriptionNo.PadRight(50));
                    sb.Append(ECD(basic.Class, 50));
                    sb.Append(basic.BirthDate);
                    sb.Append(basic.Gender == "1 " ? "男    " : "女    ");
                    sb.Append(ECD(basic.Mark, 20));
                    sb.Append(ECD(random, 20));
                    sb.Append("0");
                    sb.Append(ECD(basic.HospitalName, 30));
                    for (int i = 0; i <= 14; i++)
                    {
                        if (oncubeRandom.Count == 0 || string.IsNullOrEmpty(oncubeRandom[i]))
                            sb.Append("".PadRight(30));
                        else
                            sb.Append(ECD(oncubeRandom[i], 30));
                    }
                    sb.AppendLine(ConvertDoseType(doseType));
                }
                using (StreamWriter sw = new StreamWriter(outputDirectory, false, Encoding.Default))
                {
                    sw.Write(sb.ToString());
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                throw;
            }
        }

        public static void JVServer_SplitEachMeal(Dictionary<string, List<JVServerOPD>> dic, JVServerOPDBasic basic, List<string> oncubeRandom, string random, string outputDirectoryWithoutSeconsds, string currentSeconds)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                foreach (var x in dic)
                {
                    sb.Clear();
                    if (x.Value.Count == 0)
                    {
                        continue;
                    }
                    string newOutputDirectory = $"{outputDirectoryWithoutSeconsds}{x.Key}_{currentSeconds}.txt";
                    foreach (var y in x.Value)
                    {
                        eDoseType doseType = GetDoseType(y.AdminCode);
                        sb.Append(ECD(basic.PatientName, 20));
                        sb.Append(basic.PatientNo.PadRight(30));
                        sb.Append(ECD(basic.LocationName, 50));
                        sb.Append("".PadRight(29));
                        if (doseType == eDoseType.餐包)
                            sb.Append(y.PerQty.PadRight(5));
                        else
                            sb.Append(y.SumQty.PadRight(5));
                        sb.Append(y.MedicineCode.PadRight(20));
                        sb.Append(ECD(y.MedicineName, 50));
                        if (doseType == eDoseType.餐包 && !_SettingsModel.CrossDayAdminCode.Contains(y.AdminCode))
                        {
                            sb.Append($"{y.AdminCode}{x.Key}".PadRight(20));
                        }
                        else
                        {
                            sb.Append(y.AdminCode.PadRight(20));
                        }
                        if (doseType == eDoseType.餐包)
                        {
                            sb.Append(y.StartDay);
                            sb.Append(y.EndDay);
                        }
                        else
                        {
                            sb.Append(y.EndDay);
                            sb.Append(y.EndDay);
                        }
                        sb.Append("3       ");
                        sb.Append("".PadRight(50));
                        sb.Append(basic.PrescriptionNo.PadRight(50));
                        sb.Append(ECD(basic.Class, 50));
                        sb.Append(basic.BirthDate);
                        sb.Append(basic.Gender == "1 " ? "男    " : "女    ");
                        sb.Append(ECD(basic.Mark, 20));
                        sb.Append(ECD(random, 20));
                        sb.Append("0");
                        sb.Append(ECD(basic.HospitalName, 30));
                        for (int i = 0; i <= 14; i++)
                        {
                            if (oncubeRandom.Count == 0 || string.IsNullOrEmpty(oncubeRandom[i]))
                                sb.Append("".PadRight(30));
                            else
                                sb.Append(ECD(oncubeRandom[i], 30));
                        }
                        sb.AppendLine(ConvertDoseType(doseType));
                    }
                    using (StreamWriter sw = new StreamWriter(newOutputDirectory, false, Encoding.Default))
                    {
                        sw.Write(sb.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                throw;
            }
        }

        public static void JVServerXML(JVServerXMLOPDBasic basic, List<JVServerXMLOPD> opd, string outputDirectory, string fileName)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                foreach (var v in opd)
                {
                    eDoseType DoseType = GetDoseType(v.AdminCode);
                    sb.Append(ECD($"{basic.PatientName}_{v.AdminCode}", 20));
                    sb.Append("".PadRight(30));
                    string temp = v.CorrectPatientName == string.Empty ? "空白" : "";
                    sb.Append(ECD($"{basic.LocationName}{temp}", 50));
                    sb.Append("".PadRight(29));
                    if (DoseType == eDoseType.餐包)
                        sb.Append(v.PerQty.PadRight(5));
                    else
                        sb.Append(v.SumQty.PadRight(5));
                    sb.Append(ECD(v.MedicineCode, 20));
                    sb.Append(ECD(v.MedicineName, 50));
                    sb.Append(ECD(v.AdminCode, 20));
                    if (DoseType == eDoseType.餐包)
                    {
                        sb.Append(v.StartDay);
                        sb.Append(v.EndDay);
                    }
                    else
                    {
                        sb.Append(v.EndDay);
                        sb.Append(v.EndDay);
                    }
                    sb.Append("3       ");
                    sb.Append("".PadRight(50));
                    sb.Append("".PadRight(50));
                    sb.Append("".PadRight(50));
                    sb.Append("1990-01-01");
                    sb.Append("男    ");
                    sb.Append(ECD(v.CorrectPatientName, 20));
                    sb.Append("".PadRight(20));
                    sb.Append("0");
                    sb.Append(ECD(basic.HospitalName, 30));
                    sb.Append(ECD(basic.Type, 30));
                    sb.Append(ECD(fileName, 30));
                    sb.Append("".PadRight(390));
                    sb.AppendLine(ConvertDoseType(DoseType));
                }
                using (StreamWriter sw = new StreamWriter(outputDirectory, false, Encoding.Default))
                {
                    sw.Write(sb.ToString());
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                throw;
            }
        }

        public static void ChuangSheng(List<ChuangShengOPD> opd, string outputDirectory)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                foreach (var v in opd)
                {
                    eDoseType doseType = GetDoseType(v.AdminCode);
                    sb.Append(ECD(v.PatientName, 20));
                    sb.Append("".PadRight(30));
                    sb.Append(ECD("門診", 50));
                    sb.Append("".PadRight(29));
                    if (doseType == eDoseType.餐包)
                        sb.Append(v.PerQty.PadRight(5));
                    else
                        sb.Append(v.SumQty.PadRight(5));
                    sb.Append(v.MedicineCode.PadRight(20));
                    sb.Append(ECD(v.MedicineName, 50));
                    sb.Append(v.AdminCode.PadRight(20));
                    if (doseType == eDoseType.餐包)
                    {
                        sb.Append(v.StartDate);
                        sb.Append(v.EndDate);
                    }
                    else
                    {
                        sb.Append(v.StartDate);
                        sb.Append(v.StartDate);
                    }
                    sb.Append("3       ");
                    sb.Append("".PadRight(150));
                    sb.Append("1990-01-01");
                    sb.Append("男    ");
                    sb.Append("".PadRight(40));
                    sb.Append("0");
                    sb.Append(ECD(v.HospitalName, 30));
                    sb.Append(ECD(v.Class, 30));
                    sb.Append("".PadRight(420));
                    sb.AppendLine(ConvertDoseType(doseType));
                }
                using (StreamWriter sw = new StreamWriter(outputDirectory, false, Encoding.Default))
                {
                    sw.Write(sb.ToString());
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                throw;
            }
        }

        public static bool KuangTien_UD(List<string> MedicineName, List<string> MedicineCode, List<string> AdminTime, List<string> Dosage, List<string> TotalQuantity,
            List<string> StartDate, List<string> EndDate, List<string> PatientName, List<string> PrescriptionNo, List<string> BedNo, Dictionary<int, string> BarcodeDic,
            string outputDirectory, List<string> Class, List<string> StayDate, Dictionary<string, List<string>> DataDic, List<bool> DoseType, List<bool> CrossAdminTimeType, string FirstDate,
            List<string> QODDescription, List<string> CurrentDate, string Type, List<string> SpecialCode)
        {
            try
            {
                string MedicineCodeTemp = "";
                string Name = "";
                int InfoCount = 65;
                StringBuilder sb = new StringBuilder();
                foreach (var v in DataDic)
                {
                    int r = Convert.ToInt32(v.Key.Substring(0, v.Key.IndexOf("_")));
                    string DateTemp = v.Key.Substring(v.Key.IndexOf("_") + 1, v.Key.Length - v.Key.IndexOf("_") - 1);
                    DateTime.TryParseExact(DateTemp, "yyMMdd", null, DateTimeStyles.None, out DateTime Date);
                    if (Type == "即時" & _SettingsModel.DoseType == eDoseType.餐包)
                    {
                        if (Name == "")
                            Name = PatientName[r];
                        if (MedicineCodeTemp == "")
                        {
                            MedicineCodeTemp = MedicineCode[r];
                            PatientName[r] = $"{PatientName[r]}_{Convert.ToChar(InfoCount)}";
                        }
                        if (MedicineCodeTemp != MedicineCode[r])
                        {
                            InfoCount += 1;
                            MedicineCodeTemp = MedicineCode[r];
                            PatientName[r] = $"{PatientName[r]}_{Convert.ToChar(InfoCount)}";
                        }
                    }

                    if (_SettingsModel.DoseType == eDoseType.種包)
                    {
                        if (Name == "")
                            Name = PatientName[r];
                    }

                    foreach (string time in v.Value)
                    {
                        sb.Append(ECD(PatientName[r], 20));
                        sb.Append(PrescriptionNo[r].PadRight(30));
                        sb.Append(ECD(Type, 50));
                        sb.Append("".PadRight(29));
                        sb.Append(Dosage[r].PadRight(5));
                        sb.Append(MedicineCode[r].PadRight(20));
                        sb.Append(ECD(MedicineName[r], 50));
                        if (DoseType[r] | CrossAdminTimeType[r])
                            sb.Append(AdminTime[r].PadRight(20));
                        else
                        {
                            sb.Append($"{AdminTime[r]}{time}".PadRight(20));
                        }
                        if (DoseType[r])
                        {
                            sb.Append(FirstDate);
                            sb.Append(FirstDate);
                        }
                        else if (CrossAdminTimeType[r])
                        {
                            sb.Append(StartDate[r]);
                            sb.Append(Date.ToString("yyMMdd"));
                        }
                        else
                        {
                            sb.Append(Date.ToString("yyMMdd"));
                            sb.Append(Date.ToString("yyMMdd"));
                            //DateTime.TryParseExact(StartDate[r], "yyMMdd", null, DateTimeStyles.None,out DateTime d1);
                            //DateTime.TryParseExact(EndDate[r], "yyMMdd", null, DateTimeStyles.None, out DateTime d2);
                            //Debug.WriteLine(DateTime.Compare(d1,d2));
                        }
                        sb.Append("".PadRight(58));
                        sb.Append(PrescriptionNo[r].PadRight(50));
                        sb.Append("".PadRight(50));
                        sb.Append("1999-01-01");
                        sb.Append("男    ");
                        sb.Append(BedNo[r].PadRight(40));
                        sb.Append("0");
                        sb.Append(ECD("光田綜合醫院", 30));
                        sb.Append($"{Math.Ceiling(Convert.ToSingle(Dosage[r]))}".PadRight(30));
                        sb.Append(TotalQuantity[r].PadRight(30));
                        sb.Append(ECD(Class[r], 30));
                        sb.Append(StartDate[r].PadRight(30));
                        if (DoseType[r] | CrossAdminTimeType[r])
                            sb.Append(ECD(CurrentDate[r], 30));
                        else
                            sb.Append(ECD($"服用日{Date.ToString("yyyy/MM/dd")}", 30));
                        sb.Append(ECD(Name, 30));
                        sb.Append(ECD("", 30));
                        sb.Append(ECD(BarcodeDic[Int32.Parse(PrescriptionNo[r])], 240));
                        sb.Append(ECD(QODDescription[r].Trim(), 120));
                        sb.Append(SpecialCode[r].PadRight(30));
                        if (DoseType[r])
                            sb.AppendLine("C");
                        else
                            sb.AppendLine("M");
                    }
                }
                using (StreamWriter sw = new StreamWriter(outputDirectory, false, Encoding.Default))
                {
                    sw.Write(sb.ToString());
                }
                return true;
            }
            catch (Exception a)
            {
                Log.Write(a.ToString());
                return false;
            }
        }

        public static bool KuangTien_OPD(List<string> MedicineCode, List<string> MedicineName, List<string> AdminTime, List<string> Days, List<string> Dosage, List<string> TimesPerDay, List<string> TotalQuantity, string PatientName,
            string DoctorName, string GetMedicineNumber, string PatientNumber, string Age, string Sex, string Class, string WriteDate, string outputDirectory)
        {
            try
            {
                string Year = (Convert.ToInt32(WriteDate.Substring(0, 3)) + 1911).ToString();
                DateTime.TryParseExact(Year + WriteDate.Substring(3, 6), "yyyy/MM/dd", null, System.Globalization.DateTimeStyles.None, out DateTime PrescriptionDate);
                string DateTimeNow = $"{(Convert.ToInt32(DateTime.Now.ToString("yyyy")) - 1911).ToString()}/{DateTime.Now.ToString("MM/dd")} {DateTime.Now.ToString("HH:mm")}";
                string EffectiveDateTime = $"{Convert.ToInt32(DateTime.Now.AddDays(180).ToString("yyyy")) - 1911}/{DateTime.Now.AddDays(180).ToString("/MM/dd")}";
                StringBuilder sb = new StringBuilder();
                for (int r = 0; r <= MedicineCode.Count - 1; r++)
                {
                    sb.Append(ECD(PatientName, 20));
                    sb.Append(PatientNumber.PadRight(30));
                    sb.Append(ECD("門診", 50));
                    sb.Append(ECD(DoctorName, 29));
                    sb.Append(TotalQuantity[r].PadRight(5));
                    sb.Append(MedicineCode[r].PadRight(20));
                    sb.Append(ECD(MedicineName[r], 50));
                    sb.Append(AdminTime[r].PadRight(20));
                    sb.Append(PrescriptionDate.ToString("yyMMdd"));
                    sb.Append(PrescriptionDate.ToString("yyMMdd"));
                    //sb.Append(PrescriptionDate.AddDays(Int32.Parse(Days[r]) - 1).ToString("yyMMdd"));
                    sb.Append("".PadRight(158));
                    sb.Append("1997-01-01");
                    sb.Append("男    ");
                    sb.Append("".PadRight(40));
                    sb.Append("0");
                    sb.Append(ECD("光田綜合醫院", 30));
                    sb.Append($"{Math.Ceiling(Convert.ToSingle(Dosage[r]))}".PadRight(30));
                    sb.Append(TotalQuantity[r].PadRight(30));
                    sb.Append(DateTimeNow.PadRight(30));
                    sb.Append(EffectiveDateTime.PadRight(30));
                    sb.Append(ECD(Class, 30));
                    sb.Append("".PadRight(300));
                    sb.AppendLine("C");
                }
                using (StreamWriter sw = new StreamWriter(outputDirectory, false, Encoding.Default))
                {
                    sw.Write(sb.ToString());
                }
                return true;
            }
            catch (Exception a)
            {
                Log.Write(a.ToString());
                return false;
            }
        }

        public static bool YiSheng(List<YiShengOPD> opd, string outputDirectory)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                foreach (var v in opd)
                {
                    eDoseType doseType = GetDoseType(v.AdminCode);
                    sb.Append(ECD(v.PatientName, 20));
                    sb.Append(v.PatientNo.PadRight(30));
                    sb.Append(ECD("三杏藥局", 50));
                    sb.Append("".PadRight(29));
                    if (doseType == eDoseType.餐包)
                        sb.Append(v.PerQty.PadRight(5));
                    else
                        sb.Append(v.SumQty.PadRight(5));
                    sb.Append(v.MedicineCode.PadRight(20));
                    sb.Append(ECD(v.MedicineName, 50));
                    sb.Append(v.AdminCode.PadRight(20));
                    if (doseType == eDoseType.餐包)
                    {
                        sb.Append(v.StartDate);
                        sb.Append(v.EndDate);
                    }
                    else
                    {
                        sb.Append(v.StartDate);
                        sb.Append(v.StartDate);
                    }
                    sb.Append("".PadRight(158));
                    sb.Append("1990-01-01");
                    sb.Append("男    ");
                    sb.Append("".PadRight(40));
                    sb.Append("0");
                    sb.Append(ECD("三杏藥局", 30));
                    sb.Append("".PadRight(450));
                    sb.AppendLine(ConvertDoseType(doseType));
                }
                using (StreamWriter sw = new StreamWriter(outputDirectory, false, Encoding.Default))
                {
                    sw.Write(sb.ToString());
                }
                return true;
            }
            catch (Exception a)
            {
                Log.Write(a.ToString());
                return false;
            }
        }
        public static void HongYen(List<HongYenOPD> opdUp, List<HongYenOPD> opdDown, HongYenOPDBasic basic, List<string> outputDirectoryList)
        {

            try
            {

                foreach (string outputDirectory in outputDirectoryList)
                {
                    StringBuilder sb = new StringBuilder();
                    List<HongYenOPD> opd = outputDirectoryList.IndexOf(outputDirectory) == 0 ? opdUp.ToList() : opdDown.ToList();
                    foreach (var v in opd)
                    {
                        eDoseType doseType = GetDoseType(v.AdminCode);
                        sb.Append(ECD($"{basic.PatientName}-{v.AdminCode}", 20));
                        sb.Append(basic.PatientNo.PadRight(30));
                        sb.Append(ECD(basic.LocationName, 50));
                        sb.Append("".PadRight(29));
                        if (doseType == eDoseType.餐包)
                            sb.Append(v.PerQty.PadRight(5));
                        else
                            sb.Append(v.SumQty.PadRight(5));
                        sb.Append(v.MedicineCode.PadRight(20));
                        sb.Append(ECD(v.MedicineName, 50));
                        sb.Append(v.AdminCode.PadRight(20));
                        if (doseType == eDoseType.餐包)
                        {
                            sb.Append(v.StartDay);
                            sb.Append(v.EndDay);
                        }
                        else
                        {
                            sb.Append(v.EndDay);
                            sb.Append(v.EndDay);
                        }
                        sb.Append("".PadRight(58));
                        sb.Append(basic.PrescriptionNo.PadRight(50));
                        sb.Append("".PadRight(50));
                        sb.Append(basic.BirthDate);
                        if (basic.Gender == "1 ")
                            sb.Append("男    ");
                        else
                            sb.Append("女    ");
                        sb.Append("".PadRight(40));
                        sb.Append("0");
                        sb.Append(ECD(basic.HospitalName, 30));
                        sb.Append("".PadRight(420));
                        sb.Append(ECD(basic.PatientName, 30));
                        sb.AppendLine(ConvertDoseType(doseType));
                    }
                    using (StreamWriter sw = new StreamWriter(outputDirectory, false, Encoding.Default))
                    {
                        sw.Write(sb.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                throw;
            }
        }
        public static void MinSheng_UD(Dictionary<string, List<string>> dic, string outputDirectory, List<MinShengUDBatch> ud)
        {
            try
            {
                string directoryName = Path.GetDirectoryName(outputDirectory);
                foreach (var v in dic)
                {
                    int index = Convert.ToInt32(v.Key.Substring(0, v.Key.IndexOf("_")));
                    string dateTemp = v.Key.Substring(v.Key.IndexOf("_") + 1, v.Key.Length - v.Key.IndexOf("_") - 1);
                    DateTime.TryParseExact(dateTemp, "yyMMdd", null, DateTimeStyles.None, out DateTime date);
                    bool isCombi = false;
                    StringBuilder sb = new StringBuilder();
                    foreach (string time in v.Value)
                    {
                        isCombi = time == nameof(eDoseType.種包);
                        sb.Append(ECD(ud[index].PatientName, 20));
                        sb.Append(ud[index].PrescriptionNo.PadRight(30));
                        sb.Append(ECD("住院", 50));
                        sb.Append("".PadRight(29));
                        if (isCombi)
                            sb.Append(float.Parse(ud[index].PerQty).ToString("0.###").PadRight(5));
                        else
                            sb.Append(float.Parse(ud[index].PerQty).ToString("0.###").PadRight(5));
                        sb.Append(ud[index].MedicineCode.PadRight(20));
                        sb.Append(ECD(ud[index].MedicineName, 50));
                        if (isCombi)
                        {
                            sb.Append(ud[index].AdminCode.PadRight(20));
                            //int Days = (int)(Convert.ToDecimal(MS_UD[r].Dosage) / Convert.ToDecimal(MS_UD[r].Dosage));
                            sb.Append(date.ToString("yyMMdd"));
                            sb.Append(date.ToString("yyMMdd"));
                        }
                        else
                        {
                            sb.Append($"{ud[index].AdminCode}{time}".PadRight(20));
                            sb.Append(date.ToString("yyMMdd"));
                            sb.Append(date.ToString("yyMMdd"));
                        }
                        sb.Append("".PadRight(58));
                        sb.Append(ud[index].PrescriptionNo.PadRight(50));
                        sb.Append("".PadRight(50));
                        sb.Append("1999-01-01");
                        sb.Append("男    ");
                        sb.Append(ud[index].BedNo.PadRight(20));
                        sb.Append("".PadRight(20));
                        sb.Append("0");
                        sb.Append(ECD("民生醫院", 30));
                        sb.Append("".PadRight(450));
                        sb.AppendLine(isCombi ? "C" : "M");
                    }
                    using (StreamWriter sw = new StreamWriter($@"{directoryName}\{ud[index].BedNo}_{ud[index].PatientName}.txt", true, Encoding.Default))
                    {
                        sw.Write(sb.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                throw;
            }
        }

        public static void MinSheng_OPD(List<MinShengOPD> opd, string outputDirectory, string location)
        {
            try
            {
                string directoryName = Path.GetDirectoryName(outputDirectory);
                int maxDays = opd.Select(x => Convert.ToInt32(x.Days)).ToList().Max();
                StringBuilder sb = new StringBuilder();
                foreach (var v in opd)
                {
                    sb.Clear();
                    DateTime.TryParse((Int32.Parse(v.StartDay) + 19110000).ToString(), out DateTime startdate);
                    sb.Append(ECD(v.PatientName, 20));
                    sb.Append(v.PrescriptionNo.PadRight(30));
                    sb.Append(ECD(location, 50));
                    sb.Append("".PadRight(29));
                    sb.Append(float.Parse(v.PerQty).ToString("0.###").PadRight(5));
                    sb.Append(v.MedicineCode.PadRight(20));
                    sb.Append(ECD(v.MedicineName, 50));
                    sb.Append(v.AdminCode.PadRight(20));
                    sb.Append(v.StartDay);
                    sb.Append(v.EndDay);
                    sb.Append("".PadRight(58));
                    sb.Append(v.PrescriptionNo.PadRight(50));
                    sb.Append("".PadRight(50));
                    sb.Append("1999-01-01");
                    sb.Append("男    ");
                    sb.Append("".PadRight(40));
                    sb.Append("0");
                    sb.Append(ECD("民生醫院", 30));
                    sb.Append(v.DrugNo.PadRight(30));
                    sb.Append($"{maxDays}".PadRight(30));
                    sb.Append("".PadRight(30));
                    sb.AppendLine("M");
                    using (StreamWriter sw = new StreamWriter($@"{directoryName}\{v.DrugNo}_{v.PatientName}.txt", true, Encoding.Default))
                    {
                        sw.Write(sb.ToString());
                    }
                }

            }
            catch (Exception ex)
            {
                Log.Write(ex);
                throw;
            }
        }

        public static void E_DA_UD(List<EDAUDBatch> ud, string outputDirectory)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                foreach (var v in ud)
                {
                    sb.Append(ECD(v.PatientName, 20));
                    sb.Append(v.PrescriptionNo.PadRight(30));
                    sb.Append(ECD("住院", 50));
                    sb.Append("".PadRight(29));
                    sb.Append(float.Parse(v.SumQty).ToString("0.###").PadRight(5));
                    sb.Append(v.MedicineCode.PadRight(20));
                    sb.Append(ECD(v.MedicineName, 50));
                    sb.Append(v.AdminTime.PadRight(20));
                    sb.Append(v.StartDate);
                    sb.Append(v.StartDate);
                    sb.Append("".PadRight(58));
                    sb.Append(v.PrescriptionNo.PadRight(50));
                    sb.Append("".PadRight(50));
                    sb.Append("1999-01-01");
                    sb.Append("男    ");
                    sb.Append("".PadRight(20));
                    sb.Append(v.BedNo.PadRight(20));
                    sb.Append("0");
                    sb.Append(ECD("義大醫院", 30));
                    sb.Append(float.Parse(v.PerQty).ToString("0.###").PadRight(30));
                    sb.Append(v.BirthDate.PadRight(30));
                    sb.Append("".PadRight(390));
                    sb.AppendLine("C");
                }
                using (StreamWriter sw = new StreamWriter(outputDirectory, false, Encoding.Default))
                {
                    sw.Write(sb.ToString());
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                throw;
            }
        }

        public static void TaipeiDentention(List<TaipeiDetentionOPD> opd, TaipeiDetentionOPDBasic basic, string outputDirectory, List<string> putBackAdminCode)
        {
            try
            {
                int maxDays = opd.Select(x => Convert.ToInt32(x.Days)).Max();
                StringBuilder sb = new StringBuilder();
                foreach (var v in opd)
                {
                    eDoseType doseType = GetDoseType(v.AdminCode);
                    sb.Append(ECD(basic.PatientName, 20));
                    sb.Append(basic.PatientNo.PadRight(30));
                    sb.Append(ECD(basic.LocationName, 50));
                    sb.Append("".PadRight(29));
                    if (doseType == eDoseType.餐包)
                        sb.Append(v.PerQty.PadRight(5));
                    else
                        sb.Append(v.SumQty.PadRight(5));
                    sb.Append(v.MedicineCode.PadRight(20));
                    sb.Append(ECD(v.MedicineName, 50));
                    sb.Append(v.AdminCode.PadRight(20));
                    DateTime currentDate = DateTime.Now;
                    if (putBackAdminCode.Contains(v.AdminCode))
                    {
                        sb.Append($"{currentDate.AddDays(maxDays):yyMMdd}");
                        sb.Append($"{currentDate.AddDays(maxDays + Convert.ToInt32(v.Days) - 1):yyMMdd}");
                    }
                    else
                    {
                        sb.Append($"{currentDate:yyMMdd}");
                        sb.Append($"{currentDate.AddDays(Convert.ToInt32(v.Days) - 1):yyMMdd}");
                    }
                    sb.Append("3       ");
                    sb.Append("".PadRight(50));
                    sb.Append(basic.PrescriptionNo.PadRight(50));
                    sb.Append("".PadRight(50));
                    sb.Append(basic.BirthDate);
                    sb.Append(basic.Gender == "1 " ? "男    " : "女    ");
                    sb.Append("".PadRight(20));
                    sb.Append(ECD(basic.Class, 20));
                    sb.Append("0");
                    sb.Append(ECD(basic.HospitalName, 30));
                    sb.Append("".PadRight(450));
                    sb.AppendLine(ConvertDoseType(doseType));
                }
                using (StreamWriter sw = new StreamWriter(outputDirectory, false, Encoding.Default))
                {
                    sw.Write(sb.ToString());
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                throw;
            }
        }

        public static void JenKang_OPD(List<JenKang> opd, string outputDirectory)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                foreach (var v in opd)
                {
                    sb.Append(ECD(v.PatientName, 20));
                    sb.Append("".PadRight(30));
                    sb.Append(ECD("門診", 50));
                    sb.Append("".PadRight(29));
                    sb.Append(v.SumQty.PadRight(5));
                    sb.Append(v.MedicineCode.PadRight(20));
                    sb.Append(ECD(v.MedicineName, 50));
                    sb.Append(v.AdminCode.PadRight(20));
                    sb.Append(v.StartDay.ToString("yyMMdd"));
                    sb.Append(v.StartDay.ToString("yyMMdd"));
                    sb.Append("3       ");
                    sb.Append("".PadRight(50));
                    sb.Append(v.PrescriptionNo.PadRight(50));
                    sb.Append("".PadRight(50));
                    sb.Append("2000-01-01");
                    sb.Append(ECD("男", 6));
                    sb.Append("".PadRight(40));
                    sb.Append("0");
                    sb.Append(ECD("新北仁康醫院", 30));
                    sb.Append(Math.Ceiling(Convert.ToSingle(v.SumQty)).ToString().PadRight(30));
                    sb.Append(ECD(v.PatientName, 30));
                    sb.Append("".PadRight(390));
                    sb.AppendLine("C");
                }
                using (StreamWriter sw = new StreamWriter(outputDirectory, false, Encoding.Default))
                {
                    sw.Write(sb.ToString());
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                throw;
            }
        }

        public static bool JenKang_UD(List<JenKang> ud, string outputDirectory, DateTime minStartDate)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                foreach (var v in ud)
                {
                    //bool isInteger = !v.PerQty.Contains('.');
                    //string perQty = !isInteger ? Math.Ceiling(Convert.ToSingle(v.PerQty)).ToString() : v.PerQty;
                    //if (isInteger)
                    //    sb.Append(ECD($"{v.PatientName} 整數", 20));
                    //else
                    //    sb.Append(ECD($"{v.PatientName} 非整數", 20));
                    sb.Append(ECD(v.PatientName, 20));
                    sb.Append("".PadRight(30));
                    sb.Append(ECD(v.Location, 50));
                    sb.Append("".PadRight(29));
                    sb.Append(v.PerQty.PadRight(5));
                    sb.Append(ECD(v.MedicineCode, 20));
                    sb.Append(ECD(v.MedicineName, 50));
                    sb.Append(v.AdminCode.PadRight(20));
                    sb.Append(minStartDate.ToString("yyMMdd"));
                    sb.Append(v.EndDay.ToString("yyMMdd"));
                    sb.Append("3       ");
                    sb.Append("".PadRight(50));
                    sb.Append(v.PrescriptionNo.PadRight(50));
                    sb.Append("".PadRight(50));
                    sb.Append("2000-01-01");
                    sb.Append(ECD("男", 6));
                    sb.Append("".PadRight(20));
                    sb.Append(v.BedNo.PadRight(20));
                    sb.Append("0");
                    sb.Append(ECD("新北仁康醫院", 30));
                    sb.Append(Math.Ceiling(Convert.ToSingle(v.PerQty)).ToString().PadRight(30));
                    sb.Append(ECD(v.PatientName, 30));
                    sb.Append("".PadRight(390));
                    sb.AppendLine("M");
                }
                using (StreamWriter sw = new StreamWriter(outputDirectory, false, Encoding.Default))
                {
                    sw.Write(sb.ToString());
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return false;
            }
        }

        public static void FangDing(List<FangDingOPD> OPD, string outputDirectory)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                foreach (var v in OPD)
                {
                    sb.Append(ECD(v.PatientName, 20));
                    sb.Append("".PadRight(30));
                    sb.Append(ECD("門診", 50));
                    sb.Append("".PadRight(29));
                    sb.Append(v.PerQty.PadRight(5));
                    sb.Append(v.MedicineCode.PadRight(20));
                    sb.Append(ECD(v.MedicineName, 50));
                    sb.Append(v.AdminCode.PadRight(20));
                    sb.Append(v.StartDate);
                    sb.Append(v.EndDate);
                    sb.Append("".PadRight(58));
                    sb.Append(v.PrescriptionNo.PadRight(50));
                    sb.Append("".PadRight(50));
                    sb.Append("1999-01-01");
                    sb.Append("男    ");
                    sb.Append("".PadRight(40));
                    sb.Append("0");
                    sb.Append(ECD("艾森曼", 30));
                    sb.Append("".PadRight(450));
                    sb.AppendLine("M");
                }
                using (StreamWriter sw = new StreamWriter(outputDirectory, false, Encoding.Default))
                {
                    sw.Write(sb.ToString());
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                throw;
            }
        }

        public static void ChengYu(List<ChengYuOPD> OPD, string outputDirectory)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                foreach (var v in OPD)
                {
                    eDoseType doseType = GetDoseType(v.AdminCode);
                    sb.Append(ECD($"{v.PatientName}-{v.Unit}", 20));
                    sb.Append("".PadRight(30));
                    sb.Append(ECD("門診", 50));
                    sb.Append("".PadRight(29));
                    sb.Append(v.PerQty.PadRight(5));
                    sb.Append(v.MedicineCode.PadRight(20));
                    sb.Append(ECD(v.MedicineName, 50));
                    sb.Append(v.NumOfPackages.PadRight(20));
                    sb.Append(v.StartDay.ToString("yyMMdd"));
                    sb.Append(v.EndDay.ToString("yyMMdd"));
                    sb.Append("".PadRight(8));
                    sb.Append(ECD(v.AdminCodeDescription, 50));
                    sb.Append("".PadRight(100));
                    sb.Append("1999-01-01");
                    sb.Append("男    ");
                    sb.Append("".PadRight(40));
                    sb.Append("0");
                    sb.Append(ECD(v.HospitalName, 30));
                    sb.Append(ECD(v.Unit, 30));
                    sb.Append(v.Days.PadRight(30));
                    sb.Append(ECD(v.AdminCode, 30));
                    sb.Append(ECD(v.PatientName, 30));
                    sb.Append("".PadRight(300));
                    sb.Append(ECD(v.PatientName, 30));
                    sb.AppendLine("M");
                }
                using (StreamWriter sw = new StreamWriter(outputDirectory, false, Encoding.Default))
                {
                    sw.Write(sb.ToString());
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                throw;
            }
        }

        public static void OnCube(List<OnCubeOPD> OPD, string outputDirectory)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                foreach (var v in OPD)
                {
                    eDoseType doseType = GetDoseType(v.AdminCode);
                    sb.Append(ECD(v.PatientName, 20));
                    sb.Append("".PadRight(30));
                    sb.Append(ECD("門診", 50));
                    sb.Append("".PadRight(29));
                    sb.Append(v.PerQty.PadRight(5));
                    sb.Append(v.MedicineCode.PadRight(20));
                    sb.Append(ECD(v.MedicineName, 50));
                    sb.Append(ECD(v.AdminCode, 20));
                    sb.Append(v.StartDay.ToString("yyMMdd"));
                    sb.Append(v.EndDay.ToString("yyMMdd"));
                    sb.Append("".PadRight(158));
                    sb.Append("1999-01-01");
                    sb.Append("男    ");
                    sb.Append(v.RoomNo.PadRight(20));
                    sb.Append("".PadRight(20));
                    sb.Append("0");
                    sb.Append(ECD(v.Hospital, 30));
                    sb.Append(ECD(v.Handler, 30));
                    sb.Append("".PadRight(420));
                    sb.AppendLine("M");
                }
                using (StreamWriter sw = new StreamWriter(outputDirectory, false, Encoding.Default))
                {
                    sw.Write(sb.ToString());
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                throw;
            }
        }

        public static void LittleBear(List<LittleBearOPD> OPD, string outputDirectory)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                foreach (var v in OPD)
                {
                    eDoseType doseType = GetDoseType(v.AdminCode);
                    sb.Append(ECD(v.PatientName, 20));
                    sb.Append("".PadRight(30));
                    sb.Append(ECD("門診", 50));
                    sb.Append("".PadRight(29));
                    sb.Append($"{v.PerQty}".PadRight(5));
                    sb.Append(v.MedicineCode.PadRight(20));
                    sb.Append(ECD(v.MedicineName, 50));
                    sb.Append(ECD(v.AdminCode, 20));
                    sb.Append(v.StartDay.ToString("yyMMdd"));
                    sb.Append(v.EndDay.ToString("yyMMdd"));
                    sb.Append("".PadRight(158));
                    sb.Append("1999-01-01");
                    sb.Append("男    ");
                    sb.Append("".PadRight(40));
                    sb.Append("0");
                    sb.Append(ECD("小熊藥局", 30));
                    sb.Append("".PadRight(450));
                    sb.AppendLine("M");
                }
                using (StreamWriter sw = new StreamWriter(outputDirectory, false, Encoding.Default))
                {
                    sw.Write(sb.ToString());
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                throw;
            }
        }

        private static string ECD(string data, int Length)  //處理中文
        {
            data = data.PadRight(Length, ' ');
            Byte[] Temp = Encoding.Default.GetBytes(data);
            return Encoding.Default.GetString(Temp, 0, Length);
        }

        private static string ConvertDoseType(eDoseType type)
        {
            string result = type == eDoseType.餐包 ? "M" : "C";
            return result;
        }

        private static eDoseType GetDoseType(string code)
        {
            List<string> list = AssignExtraAdminTime(_SettingsModel.OutputSpecialAdminCode);
            var type = JudgeDoseType(_SettingsModel.DoseType, list, code);
            return type;
        }

        private static List<string> AssignExtraAdminTime(List<string> OppositeAdminCode)
        {
            List<string> extraList = new List<string>();
            foreach (string s in OppositeAdminCode)
            {
                if (!string.IsNullOrEmpty(s.Trim()))
                    extraList.Add(s.Trim());
            }
            return extraList;
        }

        private static eDoseType JudgeDoseType(eDoseType doseType, List<string> extraList, string AdminTime)
        {
            if (doseType == eDoseType.種包)
            {
                if (extraList.Count >= 1)
                {
                    if (extraList.Contains(AdminTime.Trim()))
                        return eDoseType.餐包;
                    else
                        return eDoseType.種包;
                }
                else
                    return eDoseType.種包;
            }
            else
            {
                if (extraList.Count >= 1)
                {
                    if (extraList.Contains(AdminTime.Trim()))
                        return eDoseType.種包;
                    else
                        return eDoseType.餐包;
                }
                else
                    return eDoseType.餐包;
            }
        }
    }
}
