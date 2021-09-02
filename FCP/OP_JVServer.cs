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
using Helper;
using FCP.MVVM.FormatControl;

namespace FCP
{
    static class OP_JVServer
    {
        public static void KuangTien_磨粉(Dictionary<string, List<KuangTienPowder>> powder, List<string> grindTableList, string filePathOutput)
        {
            try
            {
                foreach (string grindTable in grindTableList)
                {
                    string newFilePath = filePathOutput.Insert(filePathOutput.LastIndexOf("_") + 1, grindTable);
                    newFilePath = newFilePath.Insert(newFilePath.LastIndexOf(".") - 7, "_");
                    using (StreamWriter sw = new StreamWriter(newFilePath, false, Encoding.Default))
                    {
                        var firstPowder = powder.Select(x => x).First().Value[0];
                        sw.Write("|JVPHEAD|");
                        sw.Write("1");
                        sw.Write(firstPowder.PatientNo.PadRight(15));
                        sw.Write("".PadRight(20));
                        sw.Write("20   ");
                        sw.Write(firstPowder.StartDate.ToString("yyyyMMdd"));
                        sw.Write("00:00");
                        sw.Write("D123456789".PadRight(40));
                        sw.Write("19970101");
                        sw.Write(firstPowder.GetMedicineNo.PadRight(30));
                        sw.Write("".PadRight(45));
                        sw.Write(ECD(firstPowder.PatientName, 20));
                        sw.Write("1 ");
                        sw.Write(firstPowder.EffectiveDate.ToString("yyyy/MM/dd").PadRight(30));
                        sw.Write(ECD("光田醫院", 40));
                        sw.Write("".PadRight(20));
                        sw.Write("".PadRight(121));
                        sw.Write("M  ");
                        sw.Write("|JVPEND|");
                        sw.Write("|JVMHEAD|");
                        foreach (var v in powder[grindTable])
                        {
                            sw.Write("T");
                            sw.Write(v.MedicineCode.PadRight(15));
                            sw.Write(ECD(v.MedicineName, 50));
                            sw.Write(v.AdminCode.PadRight(10));
                            sw.Write(v.Days.PadRight(3));
                            sw.Write(v.TimesPerDay.PadRight(2));
                            sw.Write(v.PerQty.PadRight(6));
                            sw.Write(v.SumQty.PadRight(8));
                            sw.Write("".PadRight(11));
                            sw.Write(v.StartDate.ToString("yyyy/MM/dd"));
                            sw.Write("00:00     ");
                            sw.Write(v.StartDate.AddDays(Convert.ToInt32(v.Days) - 1).ToString("yyyy/MM/dd"));
                            sw.Write("00:00     ");
                        }
                        sw.Write("|JVMEND|");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex.ToString());
                throw ex;
            }
        }

        public static void ChangGung_POWDER(List<ChangGungPowder> Powder, string filePathOutput)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(filePathOutput, false, Encoding.Default))
                {
                    sw.Write("|JVPHEAD|");
                    sw.Write("1");
                    sw.Write(Powder[0].PatientNo.PadRight(15));
                    sw.Write("".PadRight(20));
                    sw.Write("1".PadRight(5));
                    sw.Write($"{DateTime.Now:yyyyMMddHH:mm}");
                    sw.Write("A123456789".PadRight(40));
                    sw.Write("20210101");
                    sw.Write(Powder[0].PrescriptionNo.PadRight(30));
                    sw.Write("".PadRight(45));
                    sw.Write(ECD(Powder[0].PatientName, 20));
                    sw.Write("1 ");
                    sw.Write("".PadRight(191));
                    sw.Write(ECD(Powder[0].Mediciner, 20));
                    sw.Write("   |JVPEND||JVMHEAD|");
                    foreach(var v in Powder)
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
            }
            catch (Exception ex)
            {
                Log.Write(ex.ToString());
                throw ex;
            }
        }

        private static string ECD(string data, int Length)  //處理byte
        {
            data = data.PadRight(Length, ' ');
            Byte[] Temp = Encoding.Default.GetBytes(data);
            return Encoding.Default.GetString(Temp, 0, Length);
        }
    }
}
