using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Globalization;
using FCP.MVVM.Models;
using FCP.src.Enum;
using Helper;
using FCP.src.Factory.Models;
using FCP.src.FormatControl;

namespace FCP
{
    static class OP_OnCube
    {
        private static SettingsModel _SettingsModel { get => SettingsFactory.GenerateSettingsModel(); }

        public static void JVServer(List<JVServerOPD> OPD, JVServerOPDBasic basic, List<string> oncubeRandom, string random, string filePathOutput)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(filePathOutput, false, Encoding.Default))
                {
                    foreach (var v in OPD)
                    {
                        eDoseType doseType = GetDoseType(v.AdminCode);
                        sw.Write(ECD(basic.PatientName, 20));
                        sw.Write(basic.PatientNo.PadRight(30));
                        sw.Write(ECD(basic.LocationName, 50));
                        sw.Write("".PadRight(29));
                        if (doseType == eDoseType.餐包)
                            sw.Write(v.PerQty.PadRight(5));
                        else
                            sw.Write(v.SumQty.PadRight(5));
                        sw.Write(v.MedicineCode.PadRight(20));
                        sw.Write(ECD(v.MedicineName, 50));
                        sw.Write(v.AdminCode.PadRight(20));
                        if (doseType == eDoseType.餐包)
                        {
                            sw.Write(v.StartDay);
                            sw.Write(v.EndDay);
                        }
                        else
                        {
                            sw.Write(v.EndDay);
                            sw.Write(v.EndDay);
                        }
                        sw.Write("3       ");
                        sw.Write("".PadRight(50));
                        sw.Write(basic.PrescriptionNo.PadRight(50));
                        sw.Write(ECD(basic.Class, 50));
                        sw.Write(basic.BirthDate);
                        sw.Write(basic.Gender == "1 " ? "男    " : "女    ");
                        sw.Write(ECD(basic.Mark, 20));
                        sw.Write(ECD(random, 20));
                        sw.Write("0");
                        sw.Write(ECD(basic.HospitalName, 30));
                        for (int i = 0; i <= 14; i++)
                        {
                            if (oncubeRandom.Count == 0 || string.IsNullOrEmpty(oncubeRandom[i]))
                                sw.Write("".PadRight(30));
                            else
                                sw.Write(ECD(oncubeRandom[i], 30));
                        }
                        sw.WriteLine(ConvertDoseType(doseType));
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                throw;
            }
        }

        public static void JVServer_SplitEachMeal(Dictionary<string, List<JVServerOPD>> dic, JVServerOPDBasic basic, List<string> oncubeRandom, string random, string filePathOutPutNoSeconds, string currentSeconds)
        {
            try
            {
                foreach (var x in dic)
                {
                    if (x.Value.Count == 0)
                    {
                        continue;
                    }
                    string newfilePathOutPut = $"{filePathOutPutNoSeconds}{x.Key}_{currentSeconds}.txt";
                    using (StreamWriter sw = new StreamWriter(newfilePathOutPut, false, Encoding.Default))
                    {
                        foreach (var y in x.Value)
                        {
                            eDoseType doseType = GetDoseType(y.AdminCode);
                            sw.Write(ECD(basic.PatientName, 20));
                            sw.Write(basic.PatientNo.PadRight(30));
                            sw.Write(ECD(basic.LocationName, 50));
                            sw.Write("".PadRight(29));
                            if (doseType == eDoseType.餐包)
                                sw.Write(y.PerQty.PadRight(5));
                            else
                                sw.Write(y.SumQty.PadRight(5));
                            sw.Write(y.MedicineCode.PadRight(20));
                            sw.Write(ECD(y.MedicineName, 50));
                            if (doseType == eDoseType.餐包 && !_SettingsModel.CrossDayAdminCode.Contains(y.AdminCode))
                            {
                                sw.Write($"{y.AdminCode}{x.Key}".PadRight(20));
                            }
                            else if(_SettingsModel.CrossDayAdminCode.Contains(y.AdminCode))
                            {
                                sw.Write(y.AdminCode.PadRight(20));
                            }
                            if (doseType == eDoseType.餐包)
                            {
                                sw.Write(y.StartDay);
                                sw.Write(y.EndDay);
                            }
                            else
                            {
                                sw.Write(y.EndDay);
                                sw.Write(y.EndDay);
                            }
                            sw.Write("3       ");
                            sw.Write("".PadRight(50));
                            sw.Write(basic.PrescriptionNo.PadRight(50));
                            sw.Write(ECD(basic.Class, 50));
                            sw.Write(basic.BirthDate);
                            sw.Write(basic.Gender == "1 " ? "男    " : "女    ");
                            sw.Write(ECD(basic.Mark, 20));
                            sw.Write(ECD(random, 20));
                            sw.Write("0");
                            sw.Write(ECD(basic.HospitalName, 30));
                            for (int i = 0; i <= 14; i++)
                            {
                                if (oncubeRandom.Count == 0 || string.IsNullOrEmpty(oncubeRandom[i]))
                                    sw.Write("".PadRight(30));
                                else
                                    sw.Write(ECD(oncubeRandom[i], 30));
                            }
                            sw.WriteLine(ConvertDoseType(doseType));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                throw;
            }
        }

        public static void JVServerXML(JVServerXMLOPDBasic basic, List<JVServerXMLOPD> OPD, string filePathOutput, string fileName)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(filePathOutput, false, Encoding.Default))
                {
                    foreach (var v in OPD)
                    {
                        eDoseType DoseType = GetDoseType(v.AdminCode);
                        sw.Write(ECD($"{basic.PatientName}_{v.AdminCode}", 20));
                        sw.Write("".PadRight(30));
                        string temp = v.CorrectPatientName == string.Empty ? "空白" : "";
                        sw.Write(ECD($"{basic.LocationName}{temp}", 50));
                        sw.Write("".PadRight(29));
                        if (DoseType == eDoseType.餐包)
                            sw.Write(v.PerQty.PadRight(5));
                        else
                            sw.Write(v.SumQty.PadRight(5));
                        sw.Write(ECD(v.MedicineCode, 20));
                        sw.Write(ECD(v.MedicineName, 50));
                        sw.Write(ECD(v.AdminCode, 20));
                        if (DoseType == eDoseType.餐包)
                        {
                            sw.Write(v.StartDay);
                            sw.Write(v.EndDay);
                        }
                        else
                        {
                            sw.Write(v.EndDay);
                            sw.Write(v.EndDay);
                        }
                        sw.Write("3       ");
                        sw.Write("".PadRight(50));
                        sw.Write("".PadRight(50));
                        sw.Write("".PadRight(50));
                        sw.Write("1990-01-01");
                        sw.Write("男    ");
                        sw.Write(ECD(v.CorrectPatientName, 20));
                        sw.Write("".PadRight(20));
                        sw.Write("0");
                        sw.Write(ECD(basic.HospitalName, 30));
                        sw.Write(ECD(basic.Type, 30));
                        sw.Write(ECD(fileName, 30));
                        sw.Write("".PadRight(390));
                        sw.WriteLine(ConvertDoseType(DoseType));
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                throw;
            }
        }

        public static void ChuangSheng(List<ChuangShengOPD> OPD, string filePathOutput)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(filePathOutput, false, Encoding.Default))
                {
                    foreach (var v in OPD)
                    {
                        eDoseType doseType = GetDoseType(v.AdminCode);
                        sw.Write(ECD(v.PatientName, 20));
                        sw.Write("".PadRight(30));
                        sw.Write(ECD("門診", 50));
                        sw.Write("".PadRight(29));
                        if (doseType == eDoseType.餐包)
                            sw.Write(v.PerQty.PadRight(5));
                        else
                            sw.Write(v.SumQty.PadRight(5));
                        sw.Write(v.MedicineCode.PadRight(20));
                        sw.Write(ECD(v.MedicineName, 50));
                        sw.Write(v.AdminCode.PadRight(20));
                        if (doseType == eDoseType.餐包)
                        {
                            sw.Write(v.StartDate);
                            sw.Write(v.EndDate);
                        }
                        else
                        {
                            sw.Write(v.StartDate);
                            sw.Write(v.StartDate);
                        }
                        sw.Write("3       ");
                        sw.Write("".PadRight(150));
                        sw.Write("1990-01-01");
                        sw.Write("男    ");
                        sw.Write("".PadRight(40));
                        sw.Write("0");
                        sw.Write(ECD(v.HospitalName, 30));
                        sw.Write(ECD(v.Class, 30));
                        sw.Write("".PadRight(420));
                        sw.WriteLine(ConvertDoseType(doseType));
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                throw;
            }
        }

        public static bool XiaoGang_UD(List<string> medicinename, List<string> medicinecode, List<string> admintime, List<string> dosage, List<string> totalqunity, string startday,
            List<string> endday, List<string> filenameoutputcount, List<string> format, List<string> unit, List<int> ID1, List<string> patientname, List<string> patientnumber,
            string doctorname, List<string> prescriptionnumber, List<string> birthday, List<string> roomnumber, List<string> bednumber, List<string> Class, List<string> warehouse, bool IsStat,
            List<string> autonumber, List<string> numberdetail, List<string> nursingstationnumber, string effectivedate, List<string> medicalunit)
        {
            try
            {
                foreach (string filenameoutput in filenameoutputcount)
                {
                    using (StreamWriter sw = new StreamWriter(filenameoutput.ToString(), false, Encoding.Default))
                    {
                        for (int r = 0; r <= admintime.Count - 1; r++)
                        {
                            string RoomNo;
                            if (roomnumber[r].Substring(2).Length == 1)
                                RoomNo = "";
                            else
                                RoomNo = roomnumber[r].Substring(2);
                            DateTime.TryParseExact(birthday[r], "yyyy/MM/dd", null, System.Globalization.DateTimeStyles.None, out DateTime _date);  //生日
                            string Birthdaynew = string.Format("{0:d}", _date);
                            float quotient = 0;
                            float remainder = 0;
                            quotient = 0;
                            remainder = Convert.ToSingle(totalqunity[r].ToString());
                            if (remainder == 0)
                                continue;
                            if (dosage[r].IndexOf(".") == -1)
                                remainder = Convert.ToSingle(dosage[r].ToString());
                            sw.Write(ECD(patientname[r], 20));  //病患姓名
                            sw.Write(patientnumber[r].PadRight(30));  //病歷號
                            if (roomnumber[r] == "8DH" & _SettingsModel.StatOrBatch == "B")
                                sw.Write(ECD("住院8DH", 50));
                            else
                            {
                                if (IsStat)
                                    sw.Write(ECD("住院即時", 50));
                                else
                                    sw.Write(ECD("住院長期", 50));
                            }
                            sw.Write(ECD(doctorname, 29));  // 醫師姓名
                            if (roomnumber[r] == "8DH" & _SettingsModel.StatOrBatch == "B")
                                sw.Write($"{Convert.ToSingle($"{totalqunity[r]}")}".PadRight(5));  //8DH 幒藥量
                            else
                                sw.Write($"{Convert.ToSingle($"{remainder}".Trim())}", 5);  //此次幒藥量
                            sw.Write(ECD(medicinecode[r], 20));  //藥品碼
                            sw.Write(ECD(medicinename[r], 50));  //藥品名稱
                            sw.Write(ECD(admintime[r], 20));  //頻率
                            sw.Write(startday.Substring(2));  //起日
                            sw.Write(endday[r].Substring(2));  //迄日
                            sw.Write("".PadRight(158));
                            sw.Write("1997-01-01");  //生日
                            sw.Write("男    ");  //性別
                            sw.Write(roomnumber[r].PadRight(20));  //病房號碼
                            sw.Write(bednumber[r].PadRight(20));  //床號
                            sw.Write("0");
                            sw.Write(ECD("小港醫院", 30));  //位置or場所
                            sw.Write(dosage[r].PadRight(30));  //次量
                            sw.Write(ECD(prescriptionnumber[r], 30));  //領藥序號
                            sw.Write(medicinecode[r].Substring(1).PadRight(30));  //藥品代碼
                            sw.Write(ECD(format[r], 30));  //規格
                            sw.Write($"/{medicalunit[r]}".PadRight(30));  //單位
                            sw.Write(medicinecode[r].PadRight(30));  //庫房
                            if (IsStat)
                                sw.Write(ECD("即", 30));  //長期or即時
                            else
                                sw.Write(ECD("長", 30));
                            sw.Write(autonumber[r].PadRight(30));  //自動編號
                            sw.Write(numberdetail[r].PadRight(30));  //編號細項
                            sw.Write(nursingstationnumber[r].PadRight(30));  //護理站編號
                            sw.Write($"{medicinecode[r].Substring(0, 1)}_{nursingstationnumber[r]}{RoomNo}-{bednumber[r]} {medicinecode[r].Trim().Substring(1)}".PadRight(30));
                            sw.Write($"{roomnumber[r]}-{bednumber[r]}".PadRight(30));
                            sw.Write(Birthdaynew.PadRight(30));
                            sw.Write(effectivedate.PadRight(30));  //有效日期
                            if (quotient > 0)
                                sw.Write($"{quotient}".PadRight(30));
                            else
                                sw.Write("0".PadRight(30));
                            if (r + 1 == admintime.Count)
                                sw.Write("C");  //CombiDose
                            else
                                sw.WriteLine("C");  //CombiDose
                        }
                        sw.Close();
                    }
                }
                return true;
            }

            catch (Exception a)
            {
                Log.Write(a.ToString());
                return false;
            }
        }
        public static bool XiaoGang_OPD(List<string> medicinename, List<string> medicinecode, List<string> admintime, List<string> dosage, List<string> totalquanity, List<string> startday,
            List<string> filenameoutputcount, List<int> ID1, List<string> medicinebagsnumber, List<string> medicinecontent, List<string> millingmark,
            List<string> modifymark, List<string> patientname, string patientnumber, string hospitalname, string doctorname, string prescriptionnumber, string birthday,
            string Class, string patientsex, string mixingtable, string effectivedate)
        {
            try
            {
                foreach (string filenameoutput in filenameoutputcount)
                {
                    using (StreamWriter sw = new StreamWriter(filenameoutput.ToString(), false, Encoding.Default))
                    {
                        for (int r = 0; r <= admintime.Count - 1; r++)
                        {
                            float id1 = 0;
                            float correcttotalquanity;
                            float quotient = 0;
                            float remainder = 0;
                            correcttotalquanity = Convert.ToSingle(totalquanity[r].ToString());
                            id1 = Convert.ToSingle(ID1[r]);
                            if (id1 > 0)
                            {
                                quotient = Convert.ToSingle(Math.Floor(correcttotalquanity / id1));  //總量除以ID1的商值
                                remainder = Convert.ToSingle(Math.Floor(correcttotalquanity % id1));  //總量除以ID1的餘數
                            }
                            else
                            {
                                quotient = 0;
                                remainder = correcttotalquanity;
                            }
                            if (remainder == 0)
                                continue;
                            sw.Write(ECD(patientname[r], 20));  //病患姓名
                            sw.Write(patientnumber.PadRight(30));  //病歷號
                            sw.Write(ECD("門診", 50));
                            sw.Write(ECD(doctorname, 29));  // 醫師姓名
                            sw.Write($"{remainder}".PadRight(5));  //幒量
                            sw.Write(medicinecode[r].PadRight(20));  //藥品碼
                            sw.Write(ECD(medicinename[r], 50));  //藥品名稱
                            sw.Write(admintime[r].PadRight(20));  //頻率
                            sw.Write(startday[r].Substring(2));  //寫入日期
                            sw.Write(startday[r].Substring(2));  //寫入日期
                            sw.Write("".PadRight(158));
                            sw.Write("1997-01-01");  //生日
                            if (patientsex == "F")  //性別
                                sw.Write("女    ");
                            else
                                sw.Write("男    ");
                            sw.Write(prescriptionnumber.PadRight(40));
                            sw.Write("0");
                            sw.Write(ECD(hospitalname, 30));  //位置or場所
                            sw.Write(dosage[r].PadRight(30));  //次量
                            sw.Write(medicinebagsnumber[r].PadRight(30));  //藥袋數
                            sw.Write(ECD(Class, 30));  //科別
                            sw.Write(ECD(medicinecontent[r], 30));  //藥品含量
                            sw.Write(millingmark[r].PadRight(30));  //磨粉註記
                            sw.Write(modifymark[r].PadRight(30));  //修改註記
                            sw.Write(ECD($"台{mixingtable[1]}", 30));  //調劑台
                            sw.Write("".PadRight(120));
                            sw.Write(effectivedate.PadRight(30));  //有效日期
                            sw.Write(birthday.PadRight(60));
                            if (quotient > 0)
                                sw.Write($"{quotient}".PadRight(30));
                            else
                                sw.Write("0".PadRight(30));
                            if (r + 1 == admintime.Count)
                                sw.Write("C");
                            else
                                sw.WriteLine("C");
                        }
                        sw.Close();
                    }
                }
                return true;
            }
            catch (Exception a)
            {
                Log.Write(a.ToString());
                return false;
            }
        }
        public static bool XiaoGang_POWDER(List<string> medicinename, List<string> medicinecode, List<string> admintime, List<string> dosage, List<string> totalquanity, List<string> startday,
            List<string> filenameoutputcount, List<string> unit, List<int> ID1, List<string> medicinebagsnumber, List<string> medicinecontent, List<string> patientname,
            string patientnumber, string hospitalname, string doctorname, string prescriptionnumber, string birthday, string Class, string patientsex, string effectivedate)
        {
            try
            {
                foreach (string filenameoutput in filenameoutputcount)
                {
                    using (StreamWriter sw = new StreamWriter(filenameoutput.ToString(), false, Encoding.Default))
                    {
                        for (int r = 0; r <= admintime.Count - 1; r++)
                        {
                            float id1 = 0;
                            id1 = Convert.ToSingle(ID1[r].ToString());
                            float tq1 = Convert.ToSingle(totalquanity[r]);
                            float tq2 = Convert.ToInt16(Math.Ceiling(tq1));  //無條件進位
                            float quotient = 0;
                            float remainder = 0;
                            if (id1 > 0)  //ID2不為0才計算
                            {
                                quotient = Convert.ToSingle(Math.Floor(tq2 / id1));  //總量除以ID1的商值
                                remainder = Convert.ToSingle(Math.Floor(tq2 % id1));  //總量除以ID1的餘數
                            }
                            else
                            {
                                quotient = 0;
                                remainder = tq2;
                            }
                            if (remainder == 0)
                                continue;
                            sw.Write(ECD(patientname[r], 20));  //病患姓名
                            sw.Write(patientnumber.PadRight(30));  //病歷號
                            sw.Write("OPD_Powder".PadRight(30));
                            sw.Write(ECD(doctorname, 29));  // 醫師姓名
                            sw.Write($"{remainder}".PadRight(5));  //幒量
                            sw.Write(medicinecode[r].PadRight(30));  //藥品碼
                            sw.Write(ECD(medicinename[r], 50));  //藥品名稱
                            sw.Write(admintime[r].PadRight(20));  //頻率
                            sw.Write(startday[r].Substring(2));  //寫入日期
                            sw.Write(startday[r].Substring(2)); ;  //寫入日期
                            sw.Write("".PadRight(158));
                            sw.Write("1997-01-01");  //生日
                            if (patientsex == "F")  //性別
                                sw.Write("女    ");
                            else
                                sw.Write("男    ");
                            sw.Write(prescriptionnumber.PadRight(40));
                            sw.Write("0");
                            sw.Write(ECD(hospitalname, 30));  //位置or場所
                            sw.Write(dosage[r].PadRight(30));  //次量
                            sw.Write(medicinebagsnumber[r].PadRight(30));  //藥袋數
                            sw.Write(ECD(Class, 30));  //科別
                            sw.Write(ECD(medicinecontent[r], 30));  //藥品含量
                            sw.Write($"/{unit[r]}".PadRight(30));  //藥品單位
                            sw.Write("".PadRight(120));
                            sw.Write(birthday.PadRight(30));
                            sw.Write(effectivedate.PadRight(30));  //有效日期
                            if (quotient > 0)
                                sw.Write($"{quotient}".PadRight(30));
                            else
                                sw.Write("0".PadRight(30));
                            if (r + 1 == admintime.Count)
                                sw.Write("C");
                            else
                                sw.WriteLine("C");
                        }
                        sw.Close();
                    }
                }
                return true;
            }
            catch (Exception a)
            {
                Log.Write(a.ToString());
                return false;
            }
        }
        public static bool KuangTien_UD(List<string> MedicineName, List<string> MedicineCode, List<string> AdminTime, List<string> Dosage, List<string> TotalQuantity,
            List<string> StartDate, List<string> EndDate, List<string> PatientName, List<string> PrescriptionNo, List<string> BedNo, Dictionary<int, string> BarcodeDic,
            string FileNameOutputCount, List<string> Class, List<string> StayDate, Dictionary<string, List<string>> DataDic, List<bool> DoseType, List<bool> CrossAdminTimeType, string FirstDate,
            List<string> QODDescription, List<string> CurrentDate, string Type, List<string> SpecialCode)
        {
            try
            {
                string MedicineCodeTemp = "";
                string Name = "";
                int InfoCount = 65;
                using (StreamWriter sw = new StreamWriter(FileNameOutputCount, false, Encoding.Default))
                {
                    foreach (var v in DataDic)
                    {
                        int r = Int32.Parse(v.Key.Substring(0, v.Key.IndexOf("_")));
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
                            sw.Write(ECD(PatientName[r], 20));
                            sw.Write(PrescriptionNo[r].PadRight(30));
                            sw.Write(ECD(Type, 50));
                            sw.Write("".PadRight(29));
                            sw.Write(Dosage[r].PadRight(5));
                            sw.Write(MedicineCode[r].PadRight(20));
                            sw.Write(ECD(MedicineName[r], 50));
                            if (DoseType[r] | CrossAdminTimeType[r])
                                sw.Write(AdminTime[r].PadRight(20));
                            else
                            {
                                sw.Write($"{AdminTime[r]}{time}".PadRight(20));
                            }
                            if (DoseType[r])
                            {
                                sw.Write(FirstDate);
                                sw.Write(FirstDate);
                            }
                            else if (CrossAdminTimeType[r])
                            {
                                sw.Write(StartDate[r]);
                                sw.Write(Date.ToString("yyMMdd"));
                            }
                            else
                            {
                                sw.Write(Date.ToString("yyMMdd"));
                                sw.Write(Date.ToString("yyMMdd"));
                                //DateTime.TryParseExact(StartDate[r], "yyMMdd", null, DateTimeStyles.None,out DateTime d1);
                                //DateTime.TryParseExact(EndDate[r], "yyMMdd", null, DateTimeStyles.None, out DateTime d2);
                                //Debug.WriteLine(DateTime.Compare(d1,d2));
                            }
                            sw.Write("".PadRight(58));
                            sw.Write(PrescriptionNo[r].PadRight(50));
                            sw.Write("".PadRight(50));
                            sw.Write("1999-01-01");
                            sw.Write("男    ");
                            sw.Write(BedNo[r].PadRight(40));
                            sw.Write("0");
                            sw.Write(ECD("光田綜合醫院", 30));
                            sw.Write($"{Math.Ceiling(Convert.ToSingle(Dosage[r]))}".PadRight(30));
                            sw.Write(TotalQuantity[r].PadRight(30));
                            sw.Write(ECD(Class[r], 30));
                            sw.Write(StartDate[r].PadRight(30));
                            if (DoseType[r] | CrossAdminTimeType[r])
                                sw.Write(ECD(CurrentDate[r], 30));
                            else
                                sw.Write(ECD($"服用日{Date.ToString("yyyy/MM/dd")}", 30));
                            sw.Write(ECD(Name, 30));
                            sw.Write(ECD("", 30));
                            sw.Write(ECD(BarcodeDic[Int32.Parse(PrescriptionNo[r])], 240));
                            sw.Write(ECD(QODDescription[r].Trim(), 120));
                            sw.Write(SpecialCode[r].PadRight(30));
                            if (DoseType[r])
                                sw.WriteLine("C");
                            else
                                sw.WriteLine("M");
                        }
                    }
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
            string DoctorName, string GetMedicineNumber, string PatientNumber, string Age, string Sex, string Class, string WriteDate, string FileOutputPath)
        {
            try
            {
                string Year = (Convert.ToInt32(WriteDate.Substring(0, 3)) + 1911).ToString();
                DateTime.TryParseExact(Year + WriteDate.Substring(3, 6), "yyyy/MM/dd", null, System.Globalization.DateTimeStyles.None, out DateTime PrescriptionDate);
                string DateTimeNow = $"{(Convert.ToInt32(DateTime.Now.ToString("yyyy")) - 1911).ToString()}/{DateTime.Now.ToString("MM/dd")} {DateTime.Now.ToString("HH:mm")}";
                string EffectiveDateTime = $"{Convert.ToInt32(DateTime.Now.AddDays(180).ToString("yyyy")) - 1911}/{DateTime.Now.AddDays(180).ToString("/MM/dd")}";
                using (StreamWriter sw = new StreamWriter(FileOutputPath, false, Encoding.Default))
                {
                    for (int r = 0; r <= MedicineCode.Count - 1; r++)
                    {
                        sw.Write(ECD(PatientName, 20));
                        sw.Write(PatientNumber.PadRight(30));
                        sw.Write(ECD("門診", 50));
                        sw.Write(ECD(DoctorName, 29));
                        sw.Write(TotalQuantity[r].PadRight(5));
                        sw.Write(MedicineCode[r].PadRight(20));
                        sw.Write(ECD(MedicineName[r], 50));
                        sw.Write(AdminTime[r].PadRight(20));
                        sw.Write(PrescriptionDate.ToString("yyMMdd"));
                        sw.Write(PrescriptionDate.ToString("yyMMdd"));
                        //sw.Write(PrescriptionDate.AddDays(Int32.Parse(Days[r]) - 1).ToString("yyMMdd"));
                        sw.Write("".PadRight(158));
                        sw.Write("1997-01-01");
                        sw.Write("男    ");
                        sw.Write("".PadRight(40));
                        sw.Write("0");
                        sw.Write(ECD("光田綜合醫院", 30));
                        sw.Write($"{Math.Ceiling(Convert.ToSingle(Dosage[r]))}".PadRight(30));
                        sw.Write(TotalQuantity[r].PadRight(30));
                        sw.Write(DateTimeNow.PadRight(30));
                        sw.Write(EffectiveDateTime.PadRight(30));
                        sw.Write(ECD(Class, 30));
                        sw.Write("".PadRight(300));
                        sw.WriteLine("C");
                    }
                }
                return true;
            }
            catch (Exception a)
            {
                Log.Write(a.ToString());
                return false;
            }
        }
        public static bool YiSheng(List<YiShengOPD> OPD, string filePathOutput)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(filePathOutput, false, Encoding.Default))
                {
                    foreach (var v in OPD)
                    {
                        eDoseType doseType = GetDoseType(v.AdminCode);
                        sw.Write(ECD(v.PatientName, 20));
                        sw.Write(v.PatientNo.PadRight(30));
                        sw.Write(ECD("三杏藥局", 50));
                        sw.Write("".PadRight(29));
                        if (doseType == eDoseType.餐包)
                            sw.Write(v.PerQty.PadRight(5));
                        else
                            sw.Write(v.SumQty.PadRight(5));
                        sw.Write(v.MedicineCode.PadRight(20));
                        sw.Write(ECD(v.MedicineName, 50));
                        sw.Write(v.AdminCode.PadRight(20));
                        if (doseType == eDoseType.餐包)
                        {
                            sw.Write(v.StartDate);
                            sw.Write(v.EndDate);
                        }
                        else
                        {
                            sw.Write(v.StartDate);
                            sw.Write(v.StartDate);
                        }
                        sw.Write("".PadRight(158));
                        sw.Write("1990-01-01");
                        sw.Write("男    ");
                        sw.Write("".PadRight(40));
                        sw.Write("0");
                        sw.Write(ECD("三杏藥局", 30));
                        sw.Write("".PadRight(450));
                        sw.WriteLine(ConvertDoseType(doseType));
                    }
                    sw.Close();
                }
                return true;
            }
            catch (Exception a)
            {
                Log.Write(a.ToString());
                return false;
            }
        }
        public static void HongYen(List<HongYenOPD> OPDUp, List<HongYenOPD> OPDDown, HongYenOPDBasic basic, List<string> filePathOutput)
        {

            try
            {
                foreach (string filenameoutput in filePathOutput)
                {
                    List<HongYenOPD> OPD = filePathOutput.IndexOf(filenameoutput) == 0 ? OPDUp.ToList() : OPDDown.ToList();
                    using (StreamWriter sw = new StreamWriter(filenameoutput, false, Encoding.Default))
                    {
                        foreach (var v in OPD)
                        {
                            eDoseType doseType = GetDoseType(v.AdminCode);
                            sw.Write(ECD($"{basic.PatientName}-{v.AdminCode}", 20));
                            sw.Write(basic.PatientNo.PadRight(30));
                            sw.Write(ECD(basic.LocationName, 50));
                            sw.Write("".PadRight(29));
                            if (doseType == eDoseType.餐包)
                                sw.Write(v.PerQty.PadRight(5));
                            else
                                sw.Write(v.SumQty.PadRight(5));
                            sw.Write(v.MedicineCode.PadRight(20));
                            sw.Write(ECD(v.MedicineName, 50));
                            sw.Write(v.AdminCode.PadRight(20));
                            if (doseType == eDoseType.餐包)
                            {
                                sw.Write(v.StartDay);
                                sw.Write(v.EndDay);
                            }
                            else
                            {
                                sw.Write(v.EndDay);
                                sw.Write(v.EndDay);
                            }
                            sw.Write("".PadRight(58));
                            sw.Write(basic.PrescriptionNo.PadRight(50));
                            sw.Write("".PadRight(50));
                            sw.Write(basic.BirthDate);
                            if (basic.Gender == "1 ")
                                sw.Write("男    ");
                            else
                                sw.Write("女    ");
                            sw.Write("".PadRight(40));
                            sw.Write("0");
                            sw.Write(ECD(basic.HospitalName, 30));
                            sw.Write("".PadRight(420));
                            sw.Write(ECD(basic.PatientName, 30));
                            sw.WriteLine(ConvertDoseType(doseType));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                throw;
            }
        }
        public static void MinSheng_UD(Dictionary<string, List<string>> dataDic, string filePathOutput, List<MinShengUDBatch> UDBatch)
        {
            try
            {
                string directoryName = Path.GetDirectoryName(filePathOutput);
                foreach (var v in dataDic)
                {
                    int index = Int32.Parse(v.Key.Substring(0, v.Key.IndexOf("_")));
                    using (StreamWriter sw = new StreamWriter($@"{directoryName}\{UDBatch[index].BedNo}_{UDBatch[index].PatientName}.txt", true, Encoding.Default))
                    {
                        string dateTemp = v.Key.Substring(v.Key.IndexOf("_") + 1, v.Key.Length - v.Key.IndexOf("_") - 1);
                        DateTime.TryParseExact(dateTemp, "yyMMdd", null, DateTimeStyles.None, out DateTime date);
                        bool isCombi = false;
                        foreach (string time in v.Value)
                        {
                            isCombi = time == nameof(eDoseType.種包);
                            sw.Write(ECD(UDBatch[index].PatientName, 20));
                            sw.Write(UDBatch[index].PrescriptionNo.PadRight(30));
                            sw.Write(ECD("住院", 50));
                            sw.Write("".PadRight(29));
                            if (isCombi)
                                sw.Write(float.Parse(UDBatch[index].PerQty).ToString("0.###").PadRight(5));
                            else
                                sw.Write(float.Parse(UDBatch[index].PerQty).ToString("0.###").PadRight(5));
                            sw.Write(UDBatch[index].MedicineCode.PadRight(20));
                            sw.Write(ECD(UDBatch[index].MedicineName, 50));
                            if (isCombi)
                            {
                                sw.Write(UDBatch[index].AdminCode.PadRight(20));
                                //int Days = (int)(Convert.ToDecimal(MS_UD[r].Dosage) / Convert.ToDecimal(MS_UD[r].Dosage));
                                sw.Write(date.ToString("yyMMdd"));
                                sw.Write(date.ToString("yyMMdd"));
                            }
                            else
                            {
                                sw.Write($"{UDBatch[index].AdminCode}{time}".PadRight(20));
                                sw.Write(date.ToString("yyMMdd"));
                                sw.Write(date.ToString("yyMMdd"));
                            }
                            sw.Write("".PadRight(58));
                            sw.Write(UDBatch[index].PrescriptionNo.PadRight(50));
                            sw.Write("".PadRight(50));
                            sw.Write("1999-01-01");
                            sw.Write("男    ");
                            sw.Write(UDBatch[index].BedNo.PadRight(20));
                            sw.Write("".PadRight(20));
                            sw.Write("0");
                            sw.Write(ECD("民生醫院", 30));
                            sw.Write("".PadRight(450));
                            sw.WriteLine(isCombi ? "C" : "M");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                throw;
            }
        }

        public static void MinSheng_OPD(List<MinShengOPD> MS_OPD, string filePathOutput, string location)
        {
            try
            {
                string directoryName = Path.GetDirectoryName(filePathOutput);
                int maxDays = MS_OPD.Select(x => Convert.ToInt32(x.Days)).ToList().Max();
                foreach (var v in MS_OPD)
                {
                    DateTime.TryParse((Int32.Parse(v.StartDay) + 19110000).ToString(), out DateTime startdate);
                    using (StreamWriter sw = new StreamWriter($@"{directoryName}\{v.DrugNo}_{v.PatientName}.txt", true, Encoding.Default))
                    {
                        sw.Write(ECD(v.PatientName, 20));
                        sw.Write(v.PrescriptionNo.PadRight(30));
                        sw.Write(ECD(location, 50));
                        sw.Write("".PadRight(29));
                        sw.Write(float.Parse(v.PerQty).ToString("0.###").PadRight(5));
                        sw.Write(v.MedicineCode.PadRight(20));
                        sw.Write(ECD(v.MedicineName, 50));
                        sw.Write(v.AdminCode.PadRight(20));
                        sw.Write(v.StartDay);
                        sw.Write(v.EndDay);
                        sw.Write("".PadRight(58));
                        sw.Write(v.PrescriptionNo.PadRight(50));
                        sw.Write("".PadRight(50));
                        sw.Write("1999-01-01");
                        sw.Write("男    ");
                        sw.Write("".PadRight(40));
                        sw.Write("0");
                        sw.Write(ECD("民生醫院", 30));
                        sw.Write(v.DrugNo.PadRight(30));
                        sw.Write($"{maxDays}".PadRight(30));
                        sw.Write("".PadRight(30));
                        sw.WriteLine("M");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                throw;
            }
        }

        public static void E_DA_UD(List<EDAUDBatch> UDBatch, string filePathOutput)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(filePathOutput, false, Encoding.Default))
                {
                    foreach (var v in UDBatch)
                    {
                        sw.Write(ECD(v.PatientName, 20));
                        sw.Write(v.PrescriptionNo.PadRight(30));
                        sw.Write(ECD("住院", 50));
                        sw.Write("".PadRight(29));
                        sw.Write(float.Parse(v.SumQty).ToString("0.###").PadRight(5));
                        sw.Write(v.MedicineCode.PadRight(20));
                        sw.Write(ECD(v.MedicineName, 50));
                        sw.Write(v.AdminTime.PadRight(20));
                        sw.Write(v.StartDate);
                        sw.Write(v.StartDate);
                        sw.Write("".PadRight(58));
                        sw.Write(v.PrescriptionNo.PadRight(50));
                        sw.Write("".PadRight(50));
                        sw.Write("1999-01-01");
                        sw.Write("男    ");
                        sw.Write("".PadRight(20));
                        sw.Write(v.BedNo.PadRight(20));
                        sw.Write("0");
                        sw.Write(ECD("義大醫院", 30));
                        sw.Write(float.Parse(v.PerQty).ToString("0.###").PadRight(30));
                        sw.Write(v.BirthDate.PadRight(30));
                        sw.Write("".PadRight(390));
                        sw.WriteLine("C");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                throw;
            }
        }

        public static bool ChangGung_OPD(List<FMT_ChangGung.OPD> OPD, string filePathOutput, string type)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(filePathOutput, false, Encoding.Default))
                {
                    foreach (var v in OPD)
                    {
                        sw.Write(ECD(v.PatientName, 20));
                        sw.Write(v.PatientNo.PadRight(30));
                        sw.Write(ECD(type, 50));
                        sw.Write(ECD(v.Mediciner, 26));
                        sw.Write("   ");
                        sw.Write(v.SumQty.PadRight(5));
                        sw.Write(v.MedicineCode.PadRight(20));
                        sw.Write(ECD(v.MedicineName, 50));
                        sw.Write("OPD-ONLINE".PadRight(20));
                        sw.Write(DateTime.Now.ToString("yyMMdd"));
                        sw.Write(DateTime.Now.ToString("yyMMdd"));
                        sw.Write("".PadRight(58));
                        sw.Write(v.PatientNo.PadRight(50));
                        sw.Write("".PadRight(50));
                        sw.Write("1999-01-01");
                        sw.Write("男    ");
                        sw.Write(v.PrescriptionNo.PadRight(20));
                        sw.Write("".PadRight(20));
                        sw.Write("0");
                        sw.Write(ECD("高雄長庚紀念醫院", 30));
                        sw.Write(v.Other.PadRight(30));
                        sw.Write("".PadRight(420));
                        sw.WriteLine("C");
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return false;
            }
        }

        public static bool ChangGung_Other(List<FMT_ChangGung.OPD> OPD, string filePathOutput, string type)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(filePathOutput, false, Encoding.Default))
                {
                    foreach (var v in OPD)
                    {
                        sw.Write(ECD(v.PatientName, 20));
                        sw.Write(v.PatientNo.PadRight(30));
                        sw.Write(ECD(type, 50));
                        sw.Write("".PadRight(29));
                        sw.Write(v.SumQty.PadRight(5));
                        sw.Write(v.MedicineCode.PadRight(20));
                        sw.Write(ECD(v.MedicineName, 50));
                        sw.Write("OPD-BATCH".PadRight(20));
                        sw.Write(DateTime.Now.ToString("yyMMdd"));
                        sw.Write(DateTime.Now.ToString("yyMMdd"));
                        sw.Write("".PadRight(58));
                        sw.Write(v.PatientNo.PadRight(50));
                        sw.Write("".PadRight(50));
                        sw.Write("1999-01-01");
                        sw.Write("男    ");
                        sw.Write(v.PrescriptionNo.PadRight(20));
                        sw.Write("".PadRight(20));
                        sw.Write("0");
                        sw.Write(ECD("高雄長庚紀念醫院", 30));
                        sw.Write(v.Other.PadRight(30));
                        sw.Write("".PadRight(420));
                        sw.WriteLine("C");
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return false;
            }
        }

        public static bool ChangGung_UD_Stat()
        {
            return false;
        }

        public static bool ChangGung_UD_Batch(List<FMT_ChangGung.Batch> UDBatch, string filePathOutput, string type)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(filePathOutput, false, Encoding.Default))
                {
                    bool _isOneDay;
                    foreach (var v in UDBatch)
                    {
                        _isOneDay = Convert.ToInt32(v.Days) == 1;
                        sw.Write(ECD($"{v.BedNo}-{v.PatientName}", 20));
                        sw.Write(v.PatientNo.PadRight(30));
                        sw.Write(ECD(type, 50));
                        sw.Write("".PadRight(29));
                        if (!_isOneDay)
                        {
                            Debug.WriteLine(v.PatientName);
                            Debug.WriteLine(v.MedicineName);
                            Debug.WriteLine($"首日量 {v.FirstDayQty} 天數 {v.Days}");
                        }
                        if (_isOneDay)
                            sw.Write(Convert.ToDecimal(v.FirstDayQty).ToString("0.##").PadRight(5));
                        else
                            sw.Write((Convert.ToDecimal(v.FirstDayQty) / Convert.ToDecimal(v.Days)).ToString("0.##").PadRight(5));
                        sw.Write(v.MedicineCode.PadRight(20));
                        sw.Write(ECD(v.MedicineName, 50));
                        sw.Write("UD-Batch".PadRight(20));
                        sw.Write(v.Date.ToString("yyMMdd"));
                        if (_isOneDay)
                            sw.Write(v.Date.ToString("yyMMdd"));
                        else
                            sw.Write(v.Date.AddDays(Convert.ToInt32(v.Days) - 1).ToString("yyMMdd"));
                        sw.Write("".PadRight(58));
                        sw.Write(v.PatientNo.PadRight(50));
                        sw.Write(v.BirthDate.ToString("yyyy-MM-dd"));
                        sw.Write("男    ");
                        sw.Write(v.BedNo.Substring(0, 4).PadRight(20));
                        sw.Write(v.BedNo.Substring(4).PadRight(20));
                        sw.Write("0");
                        sw.Write(ECD("高雄長庚紀念醫院", 30));
                        sw.Write(v.MaterialNo.PadRight(30));
                        sw.Write(v.NursingStationNo.PadRight(30));
                        sw.Write(v.MedicineShape.PadRight(30));
                        sw.Write(ECD(v.PerQty, 30));
                        sw.Write(ECD(v.ProductName, 30));
                        sw.Write(v.MedicineCode.PadRight(30));
                        sw.Write("".PadRight(270));
                        sw.WriteLine("C");
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return false;
            }
        }

        public static void TaipeiDentention(List<TaipeiDetentionOPD> OPD, TaipeiDetentionOPDBasic basic, string filePathOutput, List<string> putBackAdminCode)
        {
            try
            {
                int maxDays = OPD.Select(x => Convert.ToInt32(x.Days)).Max();
                using (StreamWriter sw = new StreamWriter(filePathOutput, false, Encoding.Default))
                {
                    foreach (var v in OPD)
                    {
                        eDoseType doseType = GetDoseType(v.AdminCode);
                        sw.Write(ECD(basic.PatientName, 20));
                        sw.Write(basic.PatientNo.PadRight(30));
                        sw.Write(ECD(basic.LocationName, 50));
                        sw.Write("".PadRight(29));
                        if (doseType == eDoseType.餐包)
                            sw.Write(v.PerQty.PadRight(5));
                        else
                            sw.Write(v.SumQty.PadRight(5));
                        sw.Write(v.MedicineCode.PadRight(20));
                        sw.Write(ECD(v.MedicineName, 50));
                        sw.Write(v.AdminCode.PadRight(20));
                        DateTime currentDate = DateTime.Now;
                        if (putBackAdminCode.Contains(v.AdminCode))
                        {
                            sw.Write($"{currentDate.AddDays(maxDays):yyMMdd}");
                            sw.Write($"{currentDate.AddDays(maxDays + Convert.ToInt32(v.Days) - 1):yyMMdd}");
                        }
                        else
                        {
                            sw.Write($"{currentDate:yyMMdd}");
                            sw.Write($"{currentDate.AddDays(Convert.ToInt32(v.Days) - 1):yyMMdd}");
                        }
                        sw.Write("3       ");
                        sw.Write("".PadRight(50));
                        sw.Write(basic.PrescriptionNo.PadRight(50));
                        sw.Write("".PadRight(50));
                        sw.Write(basic.BirthDate);
                        sw.Write(basic.Gender == "1 " ? "男    " : "女    ");
                        sw.Write("".PadRight(20));
                        sw.Write(ECD(basic.Class, 20));
                        sw.Write("0");
                        sw.Write(ECD(basic.HospitalName, 30));
                        sw.Write("".PadRight(450));
                        sw.WriteLine(ConvertDoseType(doseType));
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                throw;
            }
        }

        public static void JenKang_OPD(List<JenKang> OPD, string filePathOutput)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(filePathOutput, false, Encoding.Default))
                {
                    foreach (var v in OPD)
                    {
                        sw.Write(ECD(v.PatientName, 20));
                        sw.Write("".PadRight(30));
                        sw.Write(ECD("門診", 50));
                        sw.Write("".PadRight(29));
                        sw.Write(v.SumQty.PadRight(5));
                        sw.Write(v.MedicineCode.PadRight(20));
                        sw.Write(ECD(v.MedicineName, 50));
                        sw.Write(v.AdminCode.PadRight(20));
                        sw.Write(v.StartDay.ToString("yyMMdd"));
                        sw.Write(v.StartDay.ToString("yyMMdd"));
                        sw.Write("3       ");
                        sw.Write("".PadRight(50));
                        sw.Write(v.PrescriptionNo.PadRight(50));
                        sw.Write("".PadRight(50));
                        sw.Write("2000-01-01");
                        sw.Write(ECD("男", 6));
                        sw.Write("".PadRight(40));
                        sw.Write("0");
                        sw.Write(ECD("新北仁康醫院", 30));
                        sw.Write(Math.Ceiling(Convert.ToSingle(v.SumQty)).ToString().PadRight(30));
                        sw.Write(ECD(v.PatientName, 30));
                        sw.Write("".PadRight(390));
                        sw.WriteLine("C");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                throw;
            }
        }

        public static bool JenKang_UD(List<JenKang> UD, string filePathOutput, DateTime minStartDate)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(filePathOutput, false, Encoding.Default))
                {
                    foreach (var v in UD)
                    {
                        //bool isInteger = !v.PerQty.Contains('.');
                        //string perQty = !isInteger ? Math.Ceiling(Convert.ToSingle(v.PerQty)).ToString() : v.PerQty;
                        //if (isInteger)
                        //    sw.Write(ECD($"{v.PatientName} 整數", 20));
                        //else
                        //    sw.Write(ECD($"{v.PatientName} 非整數", 20));
                        sw.Write(ECD(v.PatientName, 20));
                        sw.Write("".PadRight(30));
                        sw.Write(ECD(v.Location, 50));
                        sw.Write("".PadRight(29));
                        sw.Write(v.PerQty.PadRight(5));
                        sw.Write(ECD(v.MedicineCode, 20));
                        sw.Write(ECD(v.MedicineName, 50));
                        sw.Write(v.AdminCode.PadRight(20));
                        sw.Write(minStartDate.ToString("yyMMdd"));
                        sw.Write(v.EndDay.ToString("yyMMdd"));
                        sw.Write("3       ");
                        sw.Write("".PadRight(50));
                        sw.Write(v.PrescriptionNo.PadRight(50));
                        sw.Write("".PadRight(50));
                        sw.Write("2000-01-01");
                        sw.Write(ECD("男", 6));
                        sw.Write("".PadRight(20));
                        sw.Write(v.BedNo.PadRight(20));
                        sw.Write("0");
                        sw.Write(ECD("新北仁康醫院", 30));
                        sw.Write(Math.Ceiling(Convert.ToSingle(v.PerQty)).ToString().PadRight(30));
                        sw.Write(ECD(v.PatientName, 30));
                        sw.Write("".PadRight(390));
                        sw.WriteLine("M");
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return false;
            }
        }

        public static void FangDing(List<FangDingOPD> OPD, string filePathOutput)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(filePathOutput, false, Encoding.Default))
                {
                    foreach (var v in OPD)
                    {
                        sw.Write(ECD(v.PatientName, 20));
                        sw.Write("".PadRight(30));
                        sw.Write(ECD("門診", 50));
                        sw.Write("".PadRight(29));
                        sw.Write(v.PerQty.PadRight(5));
                        sw.Write(v.MedicineCode.PadRight(20));
                        sw.Write(ECD(v.MedicineName, 50));
                        sw.Write(v.AdminCode.PadRight(20));
                        sw.Write(v.StartDate);
                        sw.Write(v.EndDate);
                        sw.Write("".PadRight(58));
                        sw.Write(v.PrescriptionNo.PadRight(50));
                        sw.Write("".PadRight(50));
                        sw.Write("1999-01-01");
                        sw.Write("男    ");
                        sw.Write("".PadRight(40));
                        sw.Write("0");
                        sw.Write(ECD("艾森曼", 30));
                        sw.Write("".PadRight(450));
                        sw.WriteLine("M");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                throw;
            }
        }

        public static void ChengYu(List<ChengYuOPD> OPD, string filePathOutput)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(filePathOutput, false, Encoding.Default))
                {
                    foreach (var v in OPD)
                    {
                        eDoseType doseType = GetDoseType(v.AdminCode);
                        sw.Write(ECD($"{v.PatientName}-{v.Unit}", 20));
                        sw.Write("".PadRight(30));
                        sw.Write(ECD("門診", 50));
                        sw.Write("".PadRight(29));
                        sw.Write(v.PerQty.PadRight(5));
                        sw.Write(v.MedicineCode.PadRight(20));
                        sw.Write(ECD(v.MedicineName, 50));
                        sw.Write(v.NumOfPackages.PadRight(20));
                        sw.Write(v.StartDay.ToString("yyMMdd"));
                        sw.Write(v.EndDay.ToString("yyMMdd"));
                        sw.Write("".PadRight(8));
                        sw.Write(ECD(v.AdminCodeDescription, 50));
                        sw.Write("".PadRight(100));
                        sw.Write("1999-01-01");
                        sw.Write("男    ");
                        sw.Write("".PadRight(40));
                        sw.Write("0");
                        sw.Write(ECD(v.HospitalName, 30));
                        sw.Write(ECD(v.Unit, 30));
                        sw.Write(v.Days.PadRight(30));
                        sw.Write(ECD(v.AdminCode, 30));
                        sw.Write(ECD(v.PatientName, 30));
                        sw.Write("".PadRight(300));
                        sw.Write(ECD(v.PatientName, 30));
                        sw.WriteLine("M");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                throw;
            }
        }

        public static void OnCube(List<OnCubeOPD> OPD, string filePathOutput)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(filePathOutput, false, Encoding.Default))
                {
                    foreach (var v in OPD)
                    {
                        eDoseType doseType = GetDoseType(v.AdminCode);
                        sw.Write(ECD(v.PatientName, 20));
                        sw.Write("".PadRight(30));
                        sw.Write(ECD("門診", 50));
                        sw.Write("".PadRight(29));
                        sw.Write(v.PerQty.PadRight(5));
                        sw.Write(v.MedicineCode.PadRight(20));
                        sw.Write(ECD(v.MedicineName, 50));
                        sw.Write(ECD(v.AdminCode, 20));
                        sw.Write(v.StartDay.ToString("yyMMdd"));
                        sw.Write(v.EndDay.ToString("yyMMdd"));
                        sw.Write("".PadRight(158));
                        sw.Write("1999-01-01");
                        sw.Write("男    ");
                        sw.Write(v.RoomNo.PadRight(20));
                        sw.Write("".PadRight(20));
                        sw.Write("0");
                        sw.Write(ECD(v.Hospital, 30));
                        sw.Write(ECD(v.Handler, 30));
                        sw.Write("".PadRight(420));
                        sw.WriteLine("M");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                throw;
            }
        }

        public static void LittleBear(List<LittleBearOPD> OPD, string filePathOutput)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(filePathOutput, false, Encoding.Default))
                {
                    foreach (var v in OPD)
                    {
                        eDoseType doseType = GetDoseType(v.AdminCode);
                        sw.Write(ECD(v.PatientName, 20));
                        sw.Write("".PadRight(30));
                        sw.Write(ECD("門診", 50));
                        sw.Write("".PadRight(29));
                        sw.Write($"{v.PerQty}".PadRight(5));
                        sw.Write(v.MedicineCode.PadRight(20));
                        sw.Write(ECD(v.MedicineName, 50));
                        sw.Write(ECD(v.AdminCode, 20));
                        sw.Write(v.StartDay.ToString("yyMMdd"));
                        sw.Write(v.EndDay.ToString("yyMMdd"));
                        sw.Write("".PadRight(158));
                        sw.Write("1999-01-01");
                        sw.Write("男    ");
                        sw.Write("".PadRight(40));
                        sw.Write("0");
                        sw.Write(ECD("小熊藥局", 30));
                        sw.Write("".PadRight(450));
                        sw.WriteLine("M");
                    }
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
