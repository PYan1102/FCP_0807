using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Collections.ObjectModel;

namespace FCP
{
    class OnputType_JVServer
    {
        Log log;
        public OnputType_JVServer(Log l)
        {
            log = l;
        }
        public bool KuangTien_磨粉(Dictionary<string, List<string>> Dic, List<string> DicDistinct, string PatientName, string DoctorName, string GetMedicineNumber, string PatientNumber, string Age, string Sex, string Class, string WriteDate,
            string FileOutputPath, string EffectiveDate)
        {
            try
            {
                foreach(string s in DicDistinct)
                {
                    string OutputPath= FileOutputPath.Insert(FileOutputPath.LastIndexOf("_") + 1, s);
                    OutputPath = OutputPath.Insert(OutputPath.LastIndexOf(".") - 7, "_");
                    string Year = (Convert.ToInt32(WriteDate.Substring(0, 3)) + 1911).ToString();
                    DateTime.TryParseExact(Year + WriteDate.Substring(3, 6), "yyyy/MM/dd", null, DateTimeStyles.None, out DateTime PrescriptionDate);
                    using (StreamWriter sw = new StreamWriter(OutputPath, false, Encoding.Default))
                    {
                        int SexCode;
                        if (Sex == "男")
                            SexCode = 1;
                        else
                            SexCode = 2;
                        sw.Write("|JVPHEAD|");
                        sw.Write("1");
                        sw.Write(RightSpace(PatientNumber, 15));
                        sw.Write(RightSpace("", 20));
                        sw.Write(RightSpace(Age, 5));
                        sw.Write(PrescriptionDate.ToString("yyyyMMdd"));
                        sw.Write("00:00");
                        sw.Write(RightSpace("D123456789", 40));
                        sw.Write("19970101");
                        sw.Write(RightSpace(GetMedicineNumber, 30));
                        sw.Write(RightSpace("", 45));
                        sw.Write(ECD(PatientName, 20));
                        sw.Write(RightSpace(SexCode.ToString(), 2));
                        sw.Write(RightSpace(EffectiveDate, 30));
                        sw.Write(ECD("光田醫院", 40));
                        sw.Write(ECD(DoctorName, 20));
                        sw.Write(RightSpace("", 100));
                        sw.Write(" ");  //帶藥類別，空白為門診用藥
                        sw.Write(RightSpace("", 20));
                        sw.Write("M  ");
                        sw.Write("|JVPEND|");
                        sw.Write("|JVMHEAD|");
                        for (int x = 0; x <= Dic[s].Count - 1; x+=8)
                        {
                            sw.Write("T");
                            sw.Write(RightSpace(Dic[s][x], 15));  //MedicineCode
                            sw.Write(ECD(Dic[s][x + 1], 50));  //MedicineName
                            sw.Write(RightSpace(Dic[s][x + 3], 10));  //AdminTime
                            sw.Write(RightSpace(Dic[s][x + 4], 3));  //Days
                            sw.Write(RightSpace(Dic[s][x + 7], 2));  //TimesPerDay
                            sw.Write(RightSpace(Dic[s][x + 2], 6));  //Dosage
                            sw.Write(RightSpace(Dic[s][x + 5], 8));  //TotalQuantiy
                            sw.Write(RightSpace("", 11));
                            sw.Write(PrescriptionDate.ToString("yyyy/MM/dd"));
                            sw.Write("00:00     ");
                            sw.Write(PrescriptionDate.AddDays(Int32.Parse(Dic[s][x + 4]) - 1).ToString("yyyy/MM/dd"));
                            sw.Write("00:00     ");
                        }
                        sw.Write("|JVMEND|");
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

        public bool ChangGung_POWDER(ObservableCollection<FMT_ChangGung_POWDER.POWDER> pow, string FileOutputName)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(FileOutputName, false, Encoding.Default))
                {
                    sw.Write("|JVPHEAD|");
                    sw.Write("1");
                    sw.Write(pow[0].PatientNo.PadRight(15));
                    sw.Write("".PadRight(20));
                    sw.Write("1".PadRight(5));
                    sw.Write($"{DateTime.Now:yyyyMMddHH:mm}");
                    sw.Write("A123456789".PadRight(40));
                    sw.Write("20210101");
                    sw.Write(pow[0].PrescriptionNo.PadRight(30));
                    sw.Write("".PadRight(45));
                    sw.Write(ECD(pow[0].PatientName, 20));
                    sw.Write("1 ");
                    sw.Write("".PadRight(191));
                    sw.Write(ECD(pow[0].Mediciner, 20));
                    sw.Write("   |JVPEND||JVMHEAD|");
                    foreach(var v in pow)
                    {
                        sw.Write("T");
                        sw.Write(v.MedicineCode.PadRight(15));
                        sw.Write(ECD(v.MedicineName, 50));
                        sw.Write("".PadRight(10));
                        sw.Write("0  0 0     0       ");
                        sw.Write("".PadRight(11));
                        sw.Write($"{DateTime.Now:yyyy/MM/ddHH:mm}     ");
                        sw.Write($"{DateTime.Now:yyyy/MM/ddHH:mm}     ");
                    }
                    sw.Write("|JVMEND|");
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

        private string ECD(string data, int Length)  //處理byte
        {
            data = data.PadRight(Length, ' ');
            Byte[] Temp = Encoding.Default.GetBytes(data);
            return Encoding.Default.GetString(Temp, 0, Length);
        }
    }
}
