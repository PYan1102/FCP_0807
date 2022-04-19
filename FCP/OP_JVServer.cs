﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using Helper;
using FCP.src.FormatControl;

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
                    var firstPowder = powder.Select(x => x).First().Value[0];
                    StringBuilder sb = new StringBuilder();
                    sb.Append("|JVPHEAD|");
                    sb.Append("1");
                    sb.Append(firstPowder.PatientNo.PadRight(15));
                    sb.Append("20   ");
                    sb.Append(firstPowder.StartDate.ToString("yyyyMMdd"));
                    sb.Append("00:00");
                    sb.Append("D123456789".PadRight(40));
                    sb.Append("19970101");
                    sb.Append(firstPowder.GetMedicineNo.PadRight(30));
                    sb.Append("".PadRight(45));
                    sb.Append(ECD(firstPowder.PatientName, 20));
                    sb.Append("1 ");
                    sb.Append(firstPowder.EffectiveDate.ToString("yyyy/MM/dd").PadRight(30));
                    sb.Append(ECD("光田醫院", 40));
                    sb.Append("".PadRight(20));
                    sb.Append("".PadRight(121));
                    sb.Append("M  ");
                    sb.Append("|JVPEND|");
                    sb.Append("|JVMHEAD|");
                    foreach (var v in powder[grindTable])
                    {
                        sb.Append("T");
                        sb.Append(v.MedicineCode.PadRight(15));
                        sb.Append(ECD(v.MedicineName, 50));
                        sb.Append(v.AdminCode.PadRight(10));
                        sb.Append(v.Days.PadRight(3));
                        sb.Append(v.TimesPerDay.PadRight(2));
                        sb.Append(v.PerQty.PadRight(6));
                        sb.Append(v.SumQty.PadRight(8));
                        sb.Append("".PadRight(11));
                        sb.Append(v.StartDate.ToString("yyyy/MM/dd"));
                        sb.Append("00:00     ");
                        sb.Append(v.StartDate.AddDays(Convert.ToInt32(v.Days) - 1).ToString("yyyy/MM/dd"));
                        sb.Append("00:00     ");
                    }
                    sb.Append("|JVMEND|");
                    using (StreamWriter sw = new StreamWriter(newFilePath, false, Encoding.Default))
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
                Log.Write(ex);
                throw;
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
