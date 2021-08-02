using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Globalization;
using System.Collections.ObjectModel;

namespace FCP
{
    class OnputType_OnCube
    {
        Log log;
        public OnputType_OnCube(Log l)
        {
            log = l;
        }

        public bool JVS(List<string> MedicineName, List<string> MedicineCode, List<string> AdminCode, List<string> PerQty, List<string> SumQty,
            List<string> StartDay, List<string> EndDay, string FileName, Settings Settings, List<string> OnCubeRandom, string PatientName,
            string PatientNo, string HospitalName, string Location, string PrescriptionNo, string BirthDate, string Gender, string Random)
        {
            try
            {
                List<string> ExtraList = AssignExtraAdminTime(Settings.OppositeAdminCode);
                using (StreamWriter sw = new StreamWriter(FileName, false, Encoding.Default))
                {
                    for (int r = 0; r <= AdminCode.Count - 1; r++)
                    {
                        string DoseTypeOutput = JudgeDoseType(Settings.DoseMode, ExtraList, AdminCode[r]);  //劑量類型判斷Combi or Multi
                        sw.Write(ECD(PatientName, 20));
                        sw.Write(RightSpace(PatientNo, 30));
                        sw.Write(ECD(Location, 50));
                        sw.Write(RightSpace("", 29));
                        if (DoseTypeOutput == "M")
                            sw.Write(RightSpace(PerQty[r], 5));
                        else
                            sw.Write(RightSpace(SumQty[r], 5));
                        sw.Write(RightSpace(MedicineCode[r], 20));
                        sw.Write(ECD(MedicineName[r], 50));
                        sw.Write(RightSpace(AdminCode[r], 20));
                        if (DoseTypeOutput == "M")
                        {
                            sw.Write(StartDay[r]);
                            sw.Write(EndDay[r]);
                        }
                        else
                        {
                            sw.Write(EndDay[r]);
                            sw.Write(EndDay[r]);
                        }
                        sw.Write("3       ");
                        sw.Write(RightSpace("", 50));
                        sw.Write(RightSpace(PrescriptionNo, 50));
                        sw.Write(RightSpace("", 50));
                        sw.Write(BirthDate);
                        sw.Write(Gender == "1 " ? "男    " : "女    ");
                        sw.Write(RightSpace("", 20));
                        sw.Write(ECD(Random, 20));
                        sw.Write("0");
                        sw.Write(ECD(HospitalName, 30));
                        for (int ran = 0; ran <= 14; ran++)  //Random
                        {
                            if (OnCubeRandom.Count == 0 || string.IsNullOrEmpty(OnCubeRandom[ran]))
                                sw.Write(RightSpace("", 30));
                            else
                                sw.Write(ECD(OnCubeRandom[ran], 30));
                        }
                        sw.WriteLine(DoseTypeOutput);
                    }
                }
                return true;
            }
            catch (Exception a)
            {
                log.Write(a.ToString());
                return false;
            }
        }

        public bool ChuangSheng(List<string> medicineName, List<string> medicineCode, List<string> adminCode, List<string> perQty, List<string> sumQty,
            List<string> startDate, List<string> endDate, string fileNameOutput, Settings settings, string patientName, string birthDate, string hospitalName,
            string _class)
        {
            try
            {
                List<string> ExtraList = AssignExtraAdminTime(settings.OppositeAdminCode);
                using (StreamWriter sw = new StreamWriter(fileNameOutput, false, Encoding.Default))
                {
                    for (int r = 0; r <= adminCode.Count - 1; r++)
                    {
                        string DoseTypeOutput = JudgeDoseType(settings.DoseMode, ExtraList, adminCode[r]);  //劑量類型判斷Combi or Multi
                        sw.Write(ECD(patientName, 20));  //病患姓名
                        sw.Write("".PadRight(30));  //病例碼
                        sw.Write(ECD("門診", 50));  //醫院名稱
                        sw.Write(RightSpace("", 29));  // 醫師姓名
                        if (DoseTypeOutput == "M")  //劑量
                            sw.Write(RightSpace(perQty[r], 5));
                        else
                            sw.Write(RightSpace(sumQty[r], 5));
                        sw.Write(RightSpace(medicineCode[r], 20));  //健保碼或醫材代碼
                        sw.Write(ECD(medicineName[r], 50));  //藥品名稱
                        sw.Write(RightSpace(adminCode[r], 20));  //頻率
                        if (DoseTypeOutput == "M")  //C種包，M餐包
                        {
                            sw.Write(startDate[r]);  //給藥開始日期
                            sw.Write(endDate[r]);  //給藥結束日期
                        }
                        else
                        {
                            sw.Write(startDate[r]);  //給藥開始日期
                            sw.Write(startDate[r]);  //給藥結束日期
                        }
                        sw.Write("3       ");
                        sw.Write(RightSpace("", 150));
                        sw.Write(birthDate);  //生日
                        sw.Write("男    ");  //性別
                        sw.Write($"{RightSpace("", 40)}0");
                        sw.Write(ECD(hospitalName, 30));
                        sw.Write(ECD(_class, 30));
                        sw.Write(RightSpace("", 420));
                        if (r == adminCode.Count - 1)
                            sw.Write(DoseTypeOutput);
                        else if (r != adminCode.Count - 1)
                            sw.WriteLine(DoseTypeOutput);
                    }
                    sw.Close();
                }
                return true;
            }
            catch (Exception a)
            {
                log.Write(a.ToString());
                return false;
            }
        }

        public bool XiaoGang_UD(List<string> medicinename, List<string> medicinecode, List<string> admintime, List<string> dosage, List<string> totalqunity, string startday,
            List<string> endday, List<string> filenameoutputcount, List<string> format, List<string> unit, List<int> ID1, List<string> patientname, List<string> patientnumber,
            string doctorname, List<string> prescriptionnumber, List<string> birthday, List<string> roomnumber, List<string> bednumber, List<string> Class, List<string> warehouse, bool IsStat,
            List<string> autonumber, List<string> numberdetail, List<string> nursingstationnumber, string effectivedate, List<string> medicalunit, Settings Settings)
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
                            sw.Write(RightSpace(patientnumber[r], 30));  //病歷號
                            if (roomnumber[r] == "8DH" & Settings.StatOrBatch == "B")
                                sw.Write(ECD("住院8DH", 50));
                            else
                            {
                                if (IsStat)
                                    sw.Write(ECD("住院即時", 50));
                                else
                                    sw.Write(ECD("住院長期", 50));
                            }
                            sw.Write(ECD(doctorname, 29));  // 醫師姓名
                            if (roomnumber[r] == "8DH" & Settings.StatOrBatch == "B")
                                sw.Write(RightSpace($"{Convert.ToSingle($"{totalqunity[r]}")}", 5));  //8DH 幒藥量
                            else
                                sw.Write($"{Convert.ToSingle($"{remainder}".Trim())}", 5);  //此次幒藥量
                            sw.Write(ECD(medicinecode[r], 20));  //藥品碼
                            sw.Write(ECD(medicinename[r], 50));  //藥品名稱
                            sw.Write(ECD(admintime[r], 20));  //頻率
                            sw.Write(startday.Substring(2));  //起日
                            sw.Write(endday[r].Substring(2));  //迄日
                            sw.Write(RightSpace("", 158));
                            sw.Write("1997-01-01");  //生日
                            sw.Write("男    ");  //性別
                            sw.Write(RightSpace(roomnumber[r], 20));  //病房號碼
                            sw.Write(RightSpace(bednumber[r], 20));  //床號
                            sw.Write("0");
                            sw.Write(ECD("小港醫院", 30));  //位置or場所
                            sw.Write(RightSpace(dosage[r], 30));  //次量
                            sw.Write(ECD(prescriptionnumber[r], 30));  //領藥序號
                            sw.Write(RightSpace(medicinecode[r].Substring(1), 30));  //藥品代碼
                            sw.Write(ECD(format[r], 30));  //規格
                            sw.Write(RightSpace($"/{medicalunit[r]}",30));  //單位
                            sw.Write(RightSpace(medicinecode[r], 30));  //庫房
                            if (IsStat)
                                sw.Write(ECD("即", 30));  //長期or即時
                            else
                                sw.Write(ECD("長", 30));
                            sw.Write(RightSpace(autonumber[r], 30));  //自動編號
                            sw.Write(RightSpace(numberdetail[r], 30));  //編號細項
                            sw.Write(RightSpace(nursingstationnumber[r], 30));  //護理站編號
                            sw.Write(RightSpace($"{medicinecode[r].Substring(0, 1)}_{nursingstationnumber[r]}{RoomNo}-{bednumber[r]} {medicinecode[r].Trim().Substring(1)}", 30));
                            sw.Write(RightSpace($"{roomnumber[r]}-{bednumber[r]}", 30));
                            sw.Write(RightSpace(Birthdaynew, 30));
                            sw.Write(RightSpace(effectivedate, 30));  //有效日期
                            if (quotient > 0)
                                sw.Write(RightSpace($"{quotient}", 30));
                            else
                                sw.Write(RightSpace("0", 30));
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
                log.Write(a.ToString());
                return false;
            }
        }
        public bool XiaoGang_OPD(List<string> medicinename, List<string> medicinecode, List<string> admintime, List<string> dosage, List<string> totalquanity, List<string> startday,
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
                            sw.Write(RightSpace(patientnumber, 30));  //病歷號
                            sw.Write(ECD("門診", 50));
                            sw.Write(ECD(doctorname, 29));  // 醫師姓名
                            sw.Write(RightSpace($"{remainder}", 5));  //幒量
                            sw.Write(RightSpace(medicinecode[r], 20));  //藥品碼
                            sw.Write(ECD(medicinename[r], 50));  //藥品名稱
                            sw.Write(RightSpace(admintime[r], 20));  //頻率
                            sw.Write(startday[r].Substring(2));  //寫入日期
                            sw.Write(startday[r].Substring(2));  //寫入日期
                            sw.Write(RightSpace("", 158));
                            sw.Write("1997-01-01");  //生日
                            if (patientsex == "F")  //性別
                                sw.Write("女    ");
                            else
                                sw.Write("男    ");
                            sw.Write(RightSpace(prescriptionnumber, 40));
                            sw.Write("0");
                            sw.Write(ECD(hospitalname, 30));  //位置or場所
                            sw.Write(RightSpace(dosage[r], 30));  //次量
                            sw.Write(RightSpace(medicinebagsnumber[r], 30));  //藥袋數
                            sw.Write(ECD(Class, 30));  //科別
                            sw.Write(ECD(medicinecontent[r], 30));  //藥品含量
                            sw.Write(RightSpace(millingmark[r], 30));  //磨粉註記
                            sw.Write(RightSpace(modifymark[r], 30));  //修改註記
                            sw.Write(ECD($"台{mixingtable[1]}", 30));  //調劑台
                            sw.Write(RightSpace("", 120));
                            sw.Write(RightSpace(effectivedate, 30));  //有效日期
                            sw.Write(RightSpace(birthday, 60));
                            if (quotient > 0)
                                sw.Write(RightSpace($"{quotient}", 30));
                            else
                                sw.Write(RightSpace("0", 30));
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
                log.Write(a.ToString());
                return false;
            }
        }
        public bool XiaoGang_POWDER(List<string> medicinename, List<string> medicinecode, List<string> admintime, List<string> dosage, List<string> totalquanity, List<string> startday,
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
                            sw.Write(RightSpace(patientnumber, 30));  //病歷號
                            sw.Write(RightSpace("OPD_Powder", 30));
                            sw.Write(ECD(doctorname, 29));  // 醫師姓名
                            sw.Write(RightSpace($"{remainder}", 5));  //幒量
                            sw.Write(RightSpace(medicinecode[r], 20));  //藥品碼
                            sw.Write(ECD(medicinename[r], 50));  //藥品名稱
                            sw.Write(RightSpace(admintime[r], 20));  //頻率
                            sw.Write(startday[r].Substring(2));  //寫入日期
                            sw.Write(startday[r].Substring(2)); ;  //寫入日期
                            sw.Write(RightSpace("", 158));
                            sw.Write("1997-01-01");  //生日
                            if (patientsex == "F")  //性別
                                sw.Write("女    ");
                            else
                                sw.Write("男    ");
                            sw.Write(RightSpace(prescriptionnumber, 40));
                            sw.Write("0");
                            sw.Write(ECD(hospitalname, 30));  //位置or場所
                            sw.Write(RightSpace(dosage[r], 30));  //次量
                            sw.Write(RightSpace(medicinebagsnumber[r], 30));  //藥袋數
                            sw.Write(ECD(Class, 30));  //科別
                            sw.Write(ECD(medicinecontent[r], 30));  //藥品含量
                            sw.Write(RightSpace($"/{unit[r]}", 30));  //藥品單位
                            sw.Write(RightSpace("", 120));
                            sw.Write(RightSpace(birthday, 30));
                            sw.Write(RightSpace(effectivedate, 30));  //有效日期
                            if (quotient > 0)
                                sw.Write(RightSpace($"{quotient}", 30));
                            else
                                sw.Write(RightSpace("0", 30));
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
                log.Write(a.ToString());
                return false;
            }
        }
        public bool KuangTien_UD(List<string> MedicineName, List<string> MedicineCode, List<string> AdminTime, List<string> Dosage, List<string> TotalQuantity, Settings Settings,
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
                        if (Type == "即時" & Properties.Settings.Default.DoseType == "M")
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

                        if (Properties.Settings.Default.DoseType == "C")
                        {
                            if (Name == "")
                                Name = PatientName[r];
                        }

                        foreach (string time in v.Value)
                        {
                            sw.Write(ECD(PatientName[r], 20));
                            sw.Write(RightSpace(PrescriptionNo[r], 30));
                            sw.Write(ECD(Type, 50));
                            sw.Write(RightSpace("", 29));
                            sw.Write(RightSpace(Dosage[r], 5));
                            sw.Write(RightSpace(MedicineCode[r], 20));
                            sw.Write(ECD(MedicineName[r], 50));
                            if (DoseType[r] | CrossAdminTimeType[r])
                                sw.Write(RightSpace(AdminTime[r], 20));
                            else
                            {
                                sw.Write(RightSpace($"{AdminTime[r]}{time}", 20));
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
                            sw.Write(RightSpace("", 58));
                            sw.Write(RightSpace(PrescriptionNo[r], 50));
                            sw.Write(RightSpace("", 50));
                            sw.Write("1999-01-01");
                            sw.Write("男    ");
                            sw.Write($"{RightSpace(BedNo[r], 40)}0");
                            sw.Write(ECD("光田綜合醫院", 30));
                            sw.Write(RightSpace($"{Math.Ceiling(Convert.ToSingle(Dosage[r]))}", 30));
                            sw.Write(RightSpace(TotalQuantity[r], 30));
                            sw.Write(ECD(Class[r], 30));
                            sw.Write(RightSpace(StartDate[r], 30));
                            if (DoseType[r] | CrossAdminTimeType[r])
                                sw.Write(ECD(CurrentDate[r], 30));
                            else
                                sw.Write(ECD($"服用日{Date.ToString("yyyy/MM/dd")}", 30));
                            sw.Write(ECD(Name, 30));
                            sw.Write(ECD("", 30));
                            sw.Write(ECD(BarcodeDic[Int32.Parse(PrescriptionNo[r])], 240));
                            sw.Write(ECD(QODDescription[r].Trim(),120));
                            sw.Write(RightSpace(SpecialCode[r], 30));
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
                log.Write(a.ToString());
                return false;
            }
        }
        public bool KuangTien_OPD(List<string> MedicineCode, List<string> MedicineName, List<string> AdminTime, List<string> Days, List<string> Dosage, List<string> TimesPerDay, List<string> TotalQuantity, string PatientName,
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
                        sw.Write(RightSpace(PatientNumber, 30));
                        sw.Write(ECD("門診", 50));
                        sw.Write(ECD(DoctorName, 29));
                        sw.Write(RightSpace(TotalQuantity[r], 5));
                        sw.Write(RightSpace(MedicineCode[r], 20));
                        sw.Write(ECD(MedicineName[r], 50));
                        sw.Write(RightSpace(AdminTime[r], 20));
                        sw.Write(PrescriptionDate.ToString("yyMMdd"));
                        sw.Write(PrescriptionDate.ToString("yyMMdd"));
                        //sw.Write(PrescriptionDate.AddDays(Int32.Parse(Days[r]) - 1).ToString("yyMMdd"));
                        sw.Write(RightSpace("", 158));
                        sw.Write("1997-01-01");
                        sw.Write("男    ");
                        sw.Write($"{RightSpace("", 40)}0");
                        sw.Write(ECD("光田綜合醫院", 30));
                        sw.Write(RightSpace($"{Math.Ceiling(Convert.ToSingle(Dosage[r]))}", 30));
                        sw.Write(RightSpace(TotalQuantity[r], 30));
                        sw.Write(RightSpace(DateTimeNow, 30));
                        sw.Write(RightSpace(EffectiveDateTime, 30));
                        sw.Write(ECD(Class, 30));
                        sw.Write(RightSpace("", 300));
                        sw.WriteLine("C");
                    }
                }
                    return true;
            }
            catch(Exception a)
            {
                log.Write(a.ToString());
                return false;
            }
        }
        public bool YiSheng(List<string> medicinename, List<string> medicinecode, List<string> admintime, List<string> dosage, List<string> totalquantity,
                                List<string> startday, List<string> endday, string filenameoutputcount, Settings Settings, string patientname, string birthday, string patientno)
        {
            try
            {
                List<string> ExtraList = AssignExtraAdminTime(Settings.OppositeAdminCode);
                using (StreamWriter sw = new StreamWriter(filenameoutputcount.ToString(), false, Encoding.Default))
                {
                    for (int r = 0; r <= admintime.Count - 1; r++)
                    {
                        string DoseTypeOutput = JudgeDoseType(Settings.DoseMode, ExtraList, admintime[r]);  //劑量類型判斷Combi or Multi
                        sw.Write(ECD(patientname, 20));  //病患姓名
                        sw.Write(RightSpace(patientno, 30));  //病例碼
                        sw.Write(ECD("三杏藥局", 50));  //醫院名稱
                        sw.Write(RightSpace("", 29));
                        if (DoseTypeOutput == "M")  //劑量
                            sw.Write(RightSpace(dosage[r].Substring(0, 5), 5));
                        else
                            sw.Write(RightSpace(totalquantity[r].Substring(0, 5), 5));
                        sw.Write(RightSpace(medicinecode[r], 20));  //健保碼或醫材代碼
                        sw.Write(ECD(medicinename[r], 50));  //藥品名稱
                        sw.Write(RightSpace(admintime[r], 20));  //頻率
                        if (DoseTypeOutput == "M")  //C種包，M餐包
                        {
                            sw.Write(startday[r]);  //給藥開始日期
                            sw.Write(endday[r]);  //給藥結束日期
                        }
                        else
                        {
                            sw.Write(startday[r]);  //給藥開始日期
                            sw.Write(startday[r]);  //給藥結束日期
                        }
                        sw.Write(RightSpace("", 158));
                        sw.Write(birthday);  //生日
                        sw.Write("男    ");  //性別
                        sw.Write(RightSpace("", 40));
                        sw.Write("0");
                        sw.Write(ECD("三杏藥局", 30));  //位置or場所
                        sw.Write(RightSpace("", 450));
                        if (r == admintime.Count - 1)
                            sw.Write(DoseTypeOutput);
                        else if (r != admintime.Count - 1)
                            sw.WriteLine(DoseTypeOutput);
                    }
                    sw.Close();
                }
                return true;
            }
            catch (Exception a)
            {
                log.Write(a.ToString());
                return false;
            }
        }
        public bool HongYen(List<int> up, List<int> down, List<string>filterDayList, List<string>FilterAdminTime, List<string> medicinename, List<string> medicinecode, List<string> admintime, List<string> dosage, List<string> totalqunity,
            List<string> startday, List<string> endday, List<string> filenameoutputcount, Settings Settings, List<string> oncuberandom, List<string> days, string patientname,
            string patientnumber, string hospitalname, string hospitalname0, string doctorname, string prescriptionnumber, string birthday, string sex, string random,
            bool isTwo, int Position)
        {

            List<int> A = new List<int>();
            try
            {
                List<string> ExtraList = AssignExtraAdminTime(Settings.OppositeAdminCode);
                foreach (string filenameoutput in filenameoutputcount)
                {
                    
                    if (filenameoutputcount.IndexOf(filenameoutput) == 0)
                    {
                        if (FilterAdminTime.Count == 0)
                            continue;
                        A = up;
                    }
                    else
                    {
                        if (down.Count <= 1)
                            continue;
                        A = down;
                    }
                    using (StreamWriter sw = new StreamWriter(filenameoutput.ToString(), false, Encoding.Default))
                    {

                        for (int r = A[0]; r <= A[A.Count - 1]; r++)
                        {
                            if (!FilterAdminTime.Contains(admintime[r]) & filenameoutputcount.IndexOf(filenameoutput) == 0)
                                continue;
                            if (filterDayList.Contains(days[r]) & filenameoutputcount.IndexOf(filenameoutput) == 0)
                                continue;
                            string DoseTypeOutput = JudgeDoseType(Settings.DoseMode, ExtraList, admintime[r]);  //劑量類型判斷Combi or Multi
                            sw.Write(ECD($"{patientname}-{admintime[r]}", 20));
                            sw.Write(RightSpace(patientnumber, 30));
                            sw.Write(ECD(hospitalname, 50));  //醫院名稱
                            sw.Write(ECD(doctorname, 29));  // 醫師姓名
                            if (DoseTypeOutput == "M")  //劑量
                                sw.Write(RightSpace(dosage[r], 5));
                            else
                                sw.Write(RightSpace(totalqunity[r], 5));
                            sw.Write(RightSpace(medicinecode[r], 20));  //健保碼或醫材代碼
                            sw.Write(ECD(medicinename[r], 50));  //藥品名稱
                            sw.Write(RightSpace(admintime[r], 20));  //頻率
                            if (DoseTypeOutput == "M")  //C種包，M餐包
                            {
                                sw.Write(startday[r]);  //給藥開始日期
                                sw.Write(endday[r]);  //給藥結束日期
                            }
                            else
                            {
                                sw.Write(endday[r]);  //給藥開始日期
                                sw.Write(endday[r]);  //給藥結束日期
                            }
                            sw.Write(RightSpace("", 58));
                            sw.Write(string.Format("{0,-50}", prescriptionnumber));  //處方籤號碼
                            sw.Write(RightSpace("", 50));
                            sw.Write(birthday);  //生日
                            if (sex == "1 ")  //性別
                                sw.Write("男    ");
                            else
                                sw.Write("女    ");
                            sw.Write(RightSpace("", 20));
                            sw.Write(ECD(random, 20));
                            sw.Write("0");
                            sw.Write(ECD(hospitalname0, 30));  //位置or場所
                            for (int ran = 0; ran <= 14; ran++)
                            {
                                if (oncuberandom.Count == 0 || string.IsNullOrEmpty(oncuberandom[ran]))
                                    sw.Write(RightSpace("", 30));
                                else
                                    sw.Write(ECD(oncuberandom[ran], 30));
                            }
                            if (r == admintime.Count - 1)
                                sw.Write(DoseTypeOutput);
                            else if (r != admintime.Count - 1)
                                sw.WriteLine(DoseTypeOutput);
                        }
                        continue;
                    }
                }
                return true;
            }
            catch (Exception a)
            {
                log.Write(a.ToString());
                return false;
            }
        }
        public bool MinSheng_UD(Dictionary<string, List<string>> DataDic, string FileNameOutput, ObservableCollection<OLEDB.MinSheng_UD> MS_UD, string Type)
        {
            try
            {
                string RootDirectory = Path.GetDirectoryName(FileNameOutput);
                foreach (var v in DataDic)
                {
                    int r = Int32.Parse(v.Key.Substring(0, v.Key.IndexOf("_")));
                    using (StreamWriter sw = new StreamWriter($@"{RootDirectory}\{MS_UD[r].BedNo}_{MS_UD[r].PatientName}.txt", true, Encoding.Default))
                    {
                        string DateTemp = v.Key.Substring(v.Key.IndexOf("_") + 1, v.Key.Length - v.Key.IndexOf("_") - 1);
                        DateTime.TryParseExact(DateTemp, "yyMMdd", null, DateTimeStyles.None, out DateTime Date);
                        bool _combi = false;
                        foreach (string time in v.Value)
                        {
                            _combi = time == "Combi";
                            sw.Write(ECD(MS_UD[r].PatientName, 20));
                            sw.Write(RightSpace(MS_UD[r].PrescriptionNo, 30));
                            sw.Write(ECD(Type, 50));
                            sw.Write(RightSpace("", 29));
                            if (_combi)
                                sw.Write(RightSpace(float.Parse(MS_UD[r].PerQty).ToString("0.###"), 5));
                            else
                                sw.Write(RightSpace(float.Parse(MS_UD[r].PerQty).ToString("0.###"), 5));
                            sw.Write(RightSpace(MS_UD[r].MedicineCode, 20));
                            sw.Write(ECD(MS_UD[r].MedicineName, 50));
                            if (_combi)
                            {
                                sw.Write(RightSpace(MS_UD[r].AdminCode, 20));
                                //int Days = (int)(Convert.ToDecimal(MS_UD[r].Dosage) / Convert.ToDecimal(MS_UD[r].Dosage));
                                sw.Write(Date.ToString("yyMMdd"));
                                sw.Write(Date.ToString("yyMMdd"));
                            }
                            else
                            {
                                sw.Write(RightSpace(MS_UD[r].AdminCode + time, 20));
                                sw.Write(Date.ToString("yyMMdd"));
                                sw.Write(Date.ToString("yyMMdd"));
                            }
                            sw.Write(RightSpace("", 58));
                            sw.Write(RightSpace(MS_UD[r].PrescriptionNo, 50));
                            sw.Write(RightSpace("", 50));
                            sw.Write("1999-01-01");
                            sw.Write("男    ");
                            sw.Write(RightSpace(MS_UD[r].BedNo, 20));
                            sw.Write(RightSpace("", 20));
                            sw.Write("0");
                            sw.Write(ECD("民生醫院", 30));
                            sw.Write(RightSpace("", 450));
                            if (_combi)
                                sw.WriteLine("C");
                            else
                                sw.WriteLine("M");
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                log.Write(ex.ToString());
                return false;
            }
        }

        public bool MinSheng_OPD(string FileNameOutput, ObservableCollection<OLEDB.MinSheng_OPD> MS_OPD, string Type)
        {
            try
            {
                string RootDirectory = Path.GetDirectoryName(FileNameOutput);
                int MaxDays = MS_OPD.Select(x => Convert.ToInt32(x.Days)).ToList().Max();
                foreach (var v in MS_OPD)
                {
                    DateTime.TryParse((Int32.Parse(v.StartDay) + 19110000).ToString(), out DateTime Startdate);
                    //Debug.WriteLine(Startdate);
                    using (StreamWriter sw = new StreamWriter($@"{RootDirectory}\{v.DrugNo}_{v.PatientName}.txt", true, Encoding.Default))
                    {
                        sw.Write(ECD(v.PatientName, 20));
                        sw.Write(RightSpace(v.PrescriptionNo, 30));
                        sw.Write(ECD(Type, 50));
                        sw.Write(RightSpace("", 29));
                        sw.Write(RightSpace(float.Parse(v.PerQty).ToString("0.###"), 5));
                        sw.Write(RightSpace(v.MedicineCode, 20));
                        sw.Write(ECD(v.MedicineName, 50));
                        sw.Write(RightSpace(v.AdminCode, 20));
                        sw.Write(v.StartDay);
                        sw.Write(v.EndDay);
                        sw.Write(RightSpace("", 58));
                        sw.Write(RightSpace(v.PrescriptionNo, 50));
                        sw.Write(RightSpace("", 50));
                        sw.Write("1999-01-01");
                        sw.Write("男    ");
                        sw.Write(RightSpace("", 40));
                        sw.Write("0");
                        sw.Write(ECD("民生醫院", 30));
                        sw.Write(RightSpace(v.DrugNo, 30));
                        sw.Write(RightSpace($"{MaxDays}", 30));
                        sw.Write(RightSpace("", 390));
                        sw.WriteLine("M");
                    }
                }
                
                return true;
            }
            catch (Exception ex)
            {
                log.Write(ex.ToString());
                return false;
            }
        }

        public bool E_DA_UD(ObservableCollection<InputType_E_DA.Data> EDA_UD, string FileNameOutput)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(FileNameOutput, false, Encoding.Default))
                {
                    foreach (var v in EDA_UD)
                    {
                        sw.Write(ECD(v.PatientName, 20));
                        sw.Write(RightSpace(v.PrescriptionNo, 30));
                        sw.Write(ECD("住院", 50));
                        sw.Write(RightSpace("", 29));
                        sw.Write(RightSpace(float.Parse(v.SumQty).ToString("0.###"), 5));
                        sw.Write(RightSpace(v.MedicineCode, 20));
                        sw.Write(ECD(v.MedicineName, 50));
                        sw.Write(RightSpace(v.AdminTime, 20));
                        sw.Write(v.StartDate);
                        sw.Write(v.StartDate);
                        sw.Write(RightSpace("", 58));
                        sw.Write(RightSpace(v.PrescriptionNo, 50));
                        sw.Write(RightSpace("", 50));
                        sw.Write("1999-01-01");
                        sw.Write("男    ");
                        sw.Write(RightSpace("", 20));
                        sw.Write(RightSpace(v.BedNo, 20));
                        sw.Write("0");
                        sw.Write(ECD("義大醫院" ,30));
                        sw.Write(RightSpace(float.Parse(v.PerQty).ToString("0.###"), 30));
                        sw.Write(RightSpace(v.BirthDate, 30));
                        sw.Write(RightSpace("", 390));
                        sw.WriteLine("C");
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                log.Write(ex.ToString());
                return false;
            }
        }

        public bool ChangGung_OPD(ObservableCollection<FMT_ChangGung.OPD> opd, string FileNameOutput, string Type) 
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(FileNameOutput, false, Encoding.Default))
                {
                    foreach (var v in opd)
                    {
                        sw.Write(ECD(v.PatientName, 20));
                        sw.Write(RightSpace(v.PatientNo, 30));
                        sw.Write(ECD(Type, 50));
                        sw.Write(ECD(v.Mediciner, 26));
                        sw.Write("   ");
                        sw.Write(RightSpace(v.SumQty, 5));
                        sw.Write(RightSpace(v.MedicineCode, 20));
                        sw.Write(ECD(v.MedicineName, 50));
                        sw.Write(RightSpace("OPD-ONLINE", 20));
                        sw.Write(DateTime.Now.ToString("yyMMdd"));
                        sw.Write(DateTime.Now.ToString("yyMMdd"));
                        sw.Write(RightSpace("", 58));
                        sw.Write(RightSpace(v.PatientNo, 50));
                        sw.Write(RightSpace("", 50));
                        sw.Write("1999-01-01");
                        sw.Write("男    ");
                        sw.Write(RightSpace(v.PrescriptionNo, 20));
                        sw.Write(RightSpace("", 20));
                        sw.Write("0");
                        sw.Write(ECD("高雄長庚紀念醫院", 30));
                        sw.Write(RightSpace(v.Other, 30));
                        sw.Write(RightSpace("", 420));
                        sw.WriteLine("C");
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                log.Write(ex.ToString());
                return false;
            }
        }

        public bool ChangGung_Other(ObservableCollection<FMT_ChangGung.OPD> opd, string FileNameOutput, string Type)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(FileNameOutput, false, Encoding.Default))
                {
                    foreach (var v in opd)
                    {
                        sw.Write(ECD(v.PatientName, 20));
                        sw.Write(RightSpace(v.PatientNo, 30));
                        sw.Write(ECD(Type, 50));
                        sw.Write(RightSpace("", 29)); 
                        sw.Write(RightSpace(v.SumQty, 5));
                        sw.Write(RightSpace(v.MedicineCode, 20));
                        sw.Write(ECD(v.MedicineName, 50));
                        sw.Write(RightSpace("OPD-BATCH", 20));
                        sw.Write(DateTime.Now.ToString("yyMMdd"));
                        sw.Write(DateTime.Now.ToString("yyMMdd"));
                        sw.Write(RightSpace("", 58));
                        sw.Write(RightSpace(v.PatientNo, 50));
                        sw.Write(RightSpace("", 50));
                        sw.Write("1999-01-01");
                        sw.Write("男    ");
                        sw.Write(RightSpace(v.PrescriptionNo, 20));
                        sw.Write(RightSpace("", 20));
                        sw.Write("0");
                        sw.Write(ECD("高雄長庚紀念醫院", 30));
                        sw.Write(RightSpace(v.Other, 30));
                        sw.Write(RightSpace("", 420));
                        sw.WriteLine("C");
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                log.Write(ex.ToString());
                return false;
            }
        }

        public bool ChangGung_UD_Stat()
        {
            return false;
        }

        public bool ChangGung_UD_Batch(ObservableCollection<FMT_ChangGung.Batch> batch, string FileNameOutput, string Type)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(FileNameOutput, false, Encoding.Default))
                {
                    bool _isOneDay;
                    foreach (var v in batch)
                    {
                        _isOneDay = Convert.ToInt32(v.Days) == 1;
                        sw.Write(ECD($"{v.BedNo}-{v.PatientName}", 20));
                        sw.Write(RightSpace(v.PatientNo, 30));
                        sw.Write(ECD(Type, 50));
                        sw.Write(RightSpace("", 29));
                        if (!_isOneDay)
                        {
                            Debug.WriteLine(v.PatientName);
                            Debug.WriteLine(v.MedicineName);
                            Debug.WriteLine($"首日量 {v.FirstDayQty} 天數 {v.Days}");
                        }
                        if (_isOneDay)
                            sw.Write(RightSpace((Convert.ToDecimal(v.FirstDayQty)).ToString("0.##"), 5));
                        else
                            sw.Write(RightSpace((Convert.ToDecimal(v.FirstDayQty) / Convert.ToDecimal(v.Days)).ToString("0.##"), 5));
                        sw.Write(RightSpace(v.MedicineCode, 20));
                        sw.Write(ECD(v.MedicineName, 50));
                        sw.Write(RightSpace("UD-Batch", 20));
                        sw.Write(v.Date.ToString("yyMMdd"));
                        if (_isOneDay)
                            sw.Write(v.Date.ToString("yyMMdd"));
                        else
                            sw.Write(v.Date.AddDays(Convert.ToInt32(v.Days) - 1).ToString("yyMMdd"));
                        sw.Write(RightSpace("", 58));
                        sw.Write(RightSpace(v.PatientNo, 50));
                        sw.Write(v.BirthDate.ToString("yyyy-MM-dd"));
                        sw.Write("男    ");
                        sw.Write(RightSpace(v.BedNo.Substring(0, 4), 20));
                        sw.Write(RightSpace(v.BedNo.Substring(4), 20));
                        sw.Write("0");
                        sw.Write(ECD("高雄長庚紀念醫院", 30));
                        sw.Write(RightSpace(v.MaterialNo, 30));
                        sw.Write(RightSpace(v.NursingStationNo, 30));
                        sw.Write(RightSpace(v.MedicineShape, 30));
                        sw.Write(ECD(v.PerQty, 30));
                        sw.Write(ECD(v.ProductName, 30));
                        sw.Write(RightSpace(v.MedicineCode, 30));
                        sw.Write(RightSpace("", 270));
                        sw.WriteLine("C");
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                log.Write(ex.ToString());
                return false;
            }
        }

        public bool TaipeiDentention(List<string> MedicineName, List<string> MedicineCode, List<string> AdminCode, List<string> Days, List<string> PerQty,
            List<string> SumQty, string FileName, Settings Settings, string PatientName, string PatientNo, string HospitalName, string Location,
            string PrescriptionNo, string BirthDate, string Gender, string Random, List<string> PutBackAdminCode)
        {
            try
            {
                List<int> DaysInt = new List<int>();
                Days.ForEach(x => DaysInt.Add(Convert.ToInt32(x)));
                int MaxDay = DaysInt.Max();
                List<string> ExtraList = AssignExtraAdminTime(Settings.OppositeAdminCode);
                using (StreamWriter sw = new StreamWriter(FileName, false, Encoding.Default))
                {
                    for (int r = 0; r <= AdminCode.Count - 1; r++)
                    {
                        string DoseTypeOutput = JudgeDoseType(Settings.DoseMode, ExtraList, AdminCode[r]);  //劑量類型判斷Combi or Multi
                        sw.Write(ECD(PatientName, 20));
                        sw.Write(RightSpace(PatientNo, 30));
                        sw.Write(ECD(Location, 50));
                        sw.Write(RightSpace("", 29));
                        if (DoseTypeOutput == "M")
                            sw.Write(RightSpace(PerQty[r], 5));
                        else
                            sw.Write(RightSpace(SumQty[r], 5));
                        sw.Write(RightSpace(MedicineCode[r], 20));
                        sw.Write(ECD(MedicineName[r], 50));
                        sw.Write(RightSpace(AdminCode[r], 20));
                        if (PutBackAdminCode.Contains(AdminCode[r]))
                        {
                            sw.Write($"{DateTime.Now.AddDays(MaxDay - 1):yyMMdd}");
                            sw.Write($"{DateTime.Now.AddDays(MaxDay - 1):yyMMdd}");
                        }
                        else
                        {
                            sw.Write($"{DateTime.Now:yyMMdd}");
                            sw.Write($"{DateTime.Now.AddDays(Convert.ToInt32(Days[r]) - 1):yyMMdd}");
                        }
                        sw.Write("3       ");
                        sw.Write(RightSpace("", 50));
                        sw.Write(RightSpace(PrescriptionNo, 50));
                        sw.Write(RightSpace("", 50));
                        sw.Write(BirthDate);
                        sw.Write(Gender == "1 " ? "男    " : "女    ");
                        sw.Write(RightSpace("", 20));
                        sw.Write(ECD(Random, 20));
                        sw.Write("0");
                        sw.Write(ECD(HospitalName, 30));
                        sw.Write("".PadRight(450));
                        sw.WriteLine(DoseTypeOutput);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                log.Write(ex.ToString());
                return false;
            }
        }

        public bool JenKang_OPD(ObservableCollection<FMT_JenKang.Prescription> pre, string OutputFileName)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(OutputFileName, false, Encoding.Default))
                {
                    foreach (var v in pre)
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
                        sw.Write(v.PerQty.PadRight(30));
                        sw.Write(ECD(v.PatientName, 30));
                        sw.Write("".PadRight(390));
                        sw.WriteLine("C");
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                log.Write(ex.ToString());
                return false;
            }
        }

        public bool JenKang_UD(ObservableCollection<FMT_JenKang.Prescription> pre, string outputFileName)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(outputFileName, false, Encoding.Default))
                {
                    DateTime minStartDate = DateTime.Now;
                    foreach (var a in pre)
                    {
                        DateTime dateTemp = a.StartDay;
                        if (pre.IndexOf(a) == 0)
                        {
                            minStartDate = dateTemp;
                            continue;
                        }
                        if (DateTime.Compare(minStartDate, dateTemp) == 1)
                        {
                            minStartDate = dateTemp;
                        }
                    }
                    foreach(var a in pre)
                    {
                        a.EndDay = minStartDate.AddDays(Convert.ToInt32(a.Days) - 1);
                    }
                    foreach (var v in pre)
                    {
                        bool _IsInteger = !v.PerQty.Contains('.');
                        string Qty = !_IsInteger ? Math.Ceiling(Convert.ToSingle(v.PerQty)).ToString() : v.PerQty;
                        if (_IsInteger)
                            sw.Write(ECD($"{v.PatientName} 整數", 20));
                        else
                            sw.Write(ECD($"{v.PatientName} 非整數", 20));
                        sw.Write("".PadRight(30));
                        sw.Write(ECD(v.Location, 50));
                        sw.Write("".PadRight(29));
                        sw.Write(Qty.PadRight(5));
                        sw.Write(v.MedicineCode.PadRight(20));
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
                        sw.Write(v.PerQty.PadRight(30));
                        sw.Write(ECD(v.PatientName, 30));
                        sw.Write("".PadRight(390));
                        sw.WriteLine("M");
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                log.Write(ex.ToString());
                return false;
            }
        }

        private string RightSpace(string Content, int Length)
        {
            return Content.PadRight(Length, ' ');
        }

        private string ECD(string data, int Length)  //處理中文
        {
            data = data.PadRight(Length, ' ');
            Byte[] Temp = Encoding.Default.GetBytes(data);
            return Encoding.Default.GetString(Temp, 0, Length);
        }

        private string JudgeDoseType(string DoseMode, List<string> ExtraList, string AdminTime)
        {
            if (DoseMode == "C")
            {
                if (ExtraList.Count >= 1)
                {
                    if (ExtraList.Contains(AdminTime.Trim()))
                        return "M";
                    else
                        return "C";
                }
                else
                    return "C";
            }
            else
            {
                if (ExtraList.Count >= 1)
                {
                    if (ExtraList.Contains(AdminTime.Trim()))
                        return "C";
                    else
                        return "M";
                }
                else
                    return "M";
            }
        }

        private List<string> AssignExtraAdminTime(List<string> OppositeAdminCode)
        {
            List<string> ExtraList = new List<string>();
            foreach (string s in OppositeAdminCode)
            {
                if (!string.IsNullOrEmpty(s.Trim()))
                    ExtraList.Add(s.Trim());
            }
            return ExtraList;
        }
    }
}
