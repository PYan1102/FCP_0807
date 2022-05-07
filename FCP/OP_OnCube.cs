using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using FCP.src.Enum;
using Helper;
using FCP.src.FormatControl;
using FCP.src.Factory.Models;
using FCP.Models;

namespace FCP
{
    internal static class OP_OnCube
    {
        private static SettingModel _settingModel => ModelsFactory.GenerateSettingModel();
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
                LogService.Exception(ex);
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
                        if (doseType == eDoseType.餐包 && !_settingModel.CrossDayAdminCode.Contains(y.AdminCode))
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
                LogService.Exception(ex);
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
                LogService.Exception(ex);
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
                LogService.Exception(ex);
                throw;
            }
        }

        public static void KuangTien_UD(Dictionary<KuangTienUDBasic, List<KuangTienUD>> ud, string outputDirectory)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                bool stat = FileInfoModel.Department == eDepartment.Stat;
                foreach (var prescription in ud)
                {
                    var basic = prescription.Key;
                    string patientName = basic.PatientName;
                    int alphaIndex = 65;
                    foreach (var medicine in prescription.Value)
                    {
                        bool multiDose = medicine.DoseType == eDoseType.餐包;
                        bool crossDay = medicine.CrossDay;
                        string adminCode = medicine.AdminCode;
                        DateTime startDate = medicine.StartDate;
                        DateTime endDate = medicine.EndDate;
                        if (stat && _settingModel.DoseType == eDoseType.餐包)
                        {
                            basic.PatientName = $"{patientName}_{Convert.ToChar(alphaIndex++)}";
                        }
                        string type = stat ? "即時" : "住院";
                        sb.Append(ECD(basic.PatientName, 20));
                        sb.Append(basic.PatientNo.PadRight(30));
                        sb.Append(ECD(type, 50));
                        sb.Append("".PadRight(29));
                        sb.Append($"{medicine.PerQty}".PadRight(5));
                        sb.Append(medicine.MedicineCode.PadRight(20));
                        sb.Append(ECD(medicine.MedicineName, 50));
                        sb.Append((multiDose && !crossDay ? $"{adminCode}{endDate:HH}" : adminCode).PadRight(20));
                        sb.Append(!multiDose ? $"{basic.TreatmentDate:yyMMdd}" : $"{medicine.StartDate:yyMMdd}");
                        sb.Append(!multiDose ? $"{basic.TreatmentDate:yyMMdd}" : $"{medicine.EndDate:yyMMdd}");
                        sb.Append("".PadRight(158));
                        sb.Append("1999-01-01");
                        sb.Append("男    ");
                        sb.Append(basic.BedNo.PadRight(40));
                        sb.Append("0");
                        sb.Append(ECD("光田綜合醫院", 30));
                        sb.Append($"{Math.Ceiling(Convert.ToSingle(medicine.PerQty))}".PadRight(30));
                        sb.Append($"{medicine.SumQty}".PadRight(30));
                        sb.Append("".PadRight(30));
                        sb.Append($"{medicine.PrintDate:yyMMdd}".PadRight(30));
                        sb.Append(ECD(!multiDose || crossDay ? medicine.TakingDescription : $"服用日{medicine.EndDate:yyyy/MM/dd}", 30));
                        sb.Append(ECD(patientName, 30));
                        sb.Append("".PadRight(30));
                        sb.Append(ECD(basic.Barcode, 240));
                        sb.Append(ECD(medicine.Description.Trim(), 120));
                        sb.Append(medicine.MedicineSerialNo.PadRight(30));
                        sb.AppendLine(multiDose ? "M" : "C");
                    }
                }
                using (StreamWriter sw = new StreamWriter(outputDirectory, false, Encoding.Default))
                {
                    sw.Write(sb.ToString());
                }
            }
            catch (Exception ex)
            {
                LogService.Exception(ex);
                throw;
            }
        }

        public static void KuangTien_OPD(KuangTienOPDBasic basic, List<KuangTienOPD> _opd, string outputDirectory)
        {
            try
            {
                int year = Convert.ToInt32(basic.WriteDate.Substring(0, 3)) + 1911;  //民國
                DateTime prescriptionDate = DateTimeHelper.Convert($"{year}{basic.WriteDate.Substring(3, 6)}", "yyyy/MM/dd");
                string currentDateTime = $"{Convert.ToInt32(DateTime.Now.ToString("yyyy")) - 1911}/{DateTime.Now:MM/dd HH:mm}";
                string effectivedDateTime = $"{Convert.ToInt32(DateTime.Now.AddDays(180).ToString("yyyy")) - 1911}/{DateTime.Now.AddDays(180):/MM/dd}";
                StringBuilder sb = new StringBuilder();
                foreach (var v in _opd)
                {
                    sb.Append(ECD(basic.PatientName, 20));
                    sb.Append(basic.PatientNo.PadRight(30));
                    sb.Append(ECD("門診", 50));
                    sb.Append(ECD(basic.DoctorName, 29));
                    sb.Append(v.SumQty.PadRight(5));
                    sb.Append(v.MedicineCode.PadRight(20));
                    sb.Append(ECD(v.MedicineName, 50));
                    sb.Append(v.AdminCode.PadRight(20));
                    sb.Append(prescriptionDate.ToString("yyMMdd"));
                    sb.Append(prescriptionDate.ToString("yyMMdd"));
                    sb.Append("".PadRight(158));
                    sb.Append("1997-01-01");
                    sb.Append("男    ");
                    sb.Append("".PadRight(40));
                    sb.Append("0");
                    sb.Append(ECD("光田綜合醫院", 30));
                    sb.Append($"{Math.Ceiling(Convert.ToSingle(v.PerQty))}".PadRight(30));
                    sb.Append(v.SumQty.PadRight(30));
                    sb.Append(currentDateTime.PadRight(30));
                    sb.Append(effectivedDateTime.PadRight(30));
                    sb.Append(ECD(basic.Class, 30));
                    sb.Append("".PadRight(300));
                    sb.AppendLine("C");
                }
                using (StreamWriter sw = new StreamWriter(outputDirectory, false, Encoding.Default))
                {
                    sw.Write(sb.ToString());
                }
            }
            catch (Exception ex)
            {
                LogService.Exception(ex);
                throw;
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
            catch (Exception ex)
            {
                LogService.Exception(ex);
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
                LogService.Exception(ex);
                throw;
            }
        }

        public static void MinSheng_UD(List<MinShengUDBatch> batch, string outputDirectory)
        {
            try
            {
                string directoryName = Path.GetDirectoryName(outputDirectory);
                foreach (var v in batch)
                {
                    StringBuilder sb = new StringBuilder();
                    bool multi = v.DoseType == eDoseType.餐包;
                    sb.Append(ECD(v.PatientName, 20));
                    sb.Append(v.PrescriptionNo.PadRight(30));
                    sb.Append(ECD("住院", 50));
                    sb.Append("".PadRight(29));
                    sb.Append(v.PerQty.ToString("0.###").PadRight(5));
                    sb.Append(v.MedicineCode.PadRight(20));
                    sb.Append(ECD(v.MedicineName, 50));
                    sb.Append((multi ? $"{v.AdminCode}{v.StartDate:HH}" : v.AdminCode).PadRight(20));
                    sb.Append(v.StartDate.ToString("yyMMdd"));
                    sb.Append(v.StartDate.ToString("yyMMdd"));
                    sb.Append("".PadRight(58));
                    sb.Append(v.PrescriptionNo.PadRight(50));
                    sb.Append("".PadRight(50));
                    sb.Append("1999-01-01");
                    sb.Append("男    ");
                    sb.Append(v.BedNo.PadRight(20));
                    sb.Append("".PadRight(20));
                    sb.Append("0");
                    sb.Append(ECD("民生醫院", 30));
                    sb.Append("".PadRight(450));
                    sb.AppendLine(multi ? "M" : "C");
                    using (StreamWriter sw = new StreamWriter($@"{directoryName}\{v.BedNo}-{v.PatientName}.txt", true, Encoding.Default))
                    {
                        sw.Write(sb.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Exception(ex);
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
                    sb.Append(ECD(v.PatientName, 20));
                    sb.Append(v.PrescriptionNo.PadRight(30));
                    sb.Append(ECD(location, 50));
                    sb.Append("".PadRight(29));
                    sb.Append(v.PerQty.ToString("0.###").PadRight(5));
                    sb.Append(v.MedicineCode.PadRight(20));
                    sb.Append(ECD(v.MedicineName, 50));
                    sb.Append(v.AdminCode.PadRight(20));
                    sb.Append(v.StartDate.ToString("yyMMdd"));
                    sb.Append(v.EndDate.ToString("yyMMdd"));
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
                LogService.Exception(ex);
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
                    sb.Append(v.AdminCode.PadRight(20));
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
                LogService.Exception(ex);
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
                LogService.Exception(ex);
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
                    sb.Append(v.StartDate.ToString("yyMMdd"));
                    sb.Append(v.StartDate.ToString("yyMMdd"));
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
                LogService.Exception(ex);
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
                    sb.Append(v.EndDate.ToString("yyMMdd"));
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
                LogService.Exception(ex);
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
                LogService.Exception(ex);
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
                LogService.Exception(ex);
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
                LogService.Exception(ex);
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
                LogService.Exception(ex);
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
            List<string> list = AssignExtraAdminTime(_settingModel.OutputSpecialAdminCode);
            var type = JudgeDoseType(_settingModel.DoseType, list, code);
            return type;
        }

        private static List<string> AssignExtraAdminTime(string adminCode)
        {
            adminCode = adminCode.Replace(" ", "");
            List<string> extraList = new List<string>();
            foreach (string code in adminCode.Split(','))
            {
                if (!string.IsNullOrEmpty(code))
                    extraList.Add(code);
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
