using System;
using System.Collections.Generic;
using System.Linq;
using Helper;
using FCP.src.FormatControl;
using System.IO;
using System.Xml;
using System.Text;
using FCP.Models;

namespace FCP.src
{
    class ComparePrescription
    {
        private static List<JVServerXMLOPD> _OPD = new List<JVServerXMLOPD>();
        public static bool IsPrescriptionRepeat(string filePath, JVServerXMLOPDBasic basic, List<JVServerXMLOPD> OPD)
        {
            Log.Write("Washington got repeat prescriptions");
            _OPD.Clear();
            string fileName = Path.GetFileNameWithoutExtension(filePath).Substring(0, Path.GetFileNameWithoutExtension(filePath).Length - 1);
            int count = CommonModel.SqlHelper.Query_FirstInt($@"SELECT
	                                                                 COUNT(*) AS Count
                                                                 ,	D.PrescriptionItemValue
                                                                 FROM PrescriptionItem A
                                                                 LEFT OUTER JOIN Prescription B ON B.RawID=A.PrescriptionID
                                                                 LEFT OUTER JOIN Patient C ON C.RawID=B.PatientID AND C.PatientName LIKE N'%{basic.PatientName}%'
                                                                 LEFT OUTER JOIN PrescriptionItemDetail D ON A.RawID=D.PrescriptionItemID AND D.OCSFormatItemID=66
                                                                 --WHERE B.DeletedYN=0 AND B.LastUpdatedDate BETWEEN '{DateTime.Now:yyyy-MM-dd}' AND '{DateTime.Now.AddDays(1):yyyy-MM-dd}'
                                                                 WHERE B.DeletedYN=0 AND B.LastUpdatedDate BETWEEN '2021-11-25' AND '2021-11-26'
                                                                 AND D.PrescriptionItemValue LIKE '%{fileName}%'
                                                                 GROUP BY D.PrescriptionItemValue");
            Log.Write($"FileName:{fileName}, SearchStartDate:{DateTime.Now:yyyy-MM-dd}, SearchEndDate:{DateTime.Now.AddDays(1):yyyy-MM-dd}, SqlCount:{count}");
            if (count == 0)
                return false;
            XMLHelper.Load(filePath);
            DateTime startDate = DateTimeHelper.Convert(XMLHelper.GetParameterUseString("case", 0, "date"), "yyyyMMdd");
            var patientName = XMLHelper.GetParameterUseString("case/profile/person", 0, "name");
            var currentlabeno_type = XMLHelper.GetParameterUseString("case/insurance", 0, "labeno_type");
            foreach (XmlNode v in XMLHelper.GetNodeList("case/orders/item"))
            {
                Add(_OPD, v, patientName, startDate);
            }
            List<string> files = Directory.GetFiles($@"{CommonModel.FileBackupRootDirectory}\{DateTime.Now:yyyy-MM-dd}\Success", $"*{fileName}*").ToList();
            Log.Write($"{CommonModel.FileBackupRootDirectory} file count is {files.Count}");
            foreach (var file in files)
            {
                XMLHelper.Load(file);
                startDate = DateTimeHelper.Convert(XMLHelper.GetParameterUseString("case", 0, "date"), "yyyyMMdd");
                patientName = XMLHelper.GetParameterUseString("case/profile/person", 0, "name");
                var targetlabeno_type = XMLHelper.GetParameterUseString("case/insurance", 0, "labeno_type");
                List<JVServerXMLOPD> list = new List<JVServerXMLOPD>();
                foreach (XmlNode v in XMLHelper.GetNodeList("case/orders/item"))
                {
                    Add(list, v, patientName, startDate);
                }
                if (Equal(_OPD, list, currentlabeno_type, targetlabeno_type))
                {
                    Log.Write("Repeat");
                    return true;
                }
            }
            Log.Write("Not repeat");
            return false;
        }

        private static void Add(List<JVServerXMLOPD> list, XmlNode xmlNode, string patientName, DateTime startDate)
        {
            list.Add(new JVServerXMLOPD()
            {
                AdminCode = xmlNode.Attributes["freq"].Value,
                MedicineCode = xmlNode.Attributes["local_code"].Value,
                MedicineName = xmlNode.Attributes["desc"].Value,
                PerQty = xmlNode.Attributes["divided_dose"].Value,
                CorrectPatientName = patientName,
                Days = xmlNode.Attributes["days"].Value,
                StartDay = startDate.ToString("yyMMdd"),
                Memo = xmlNode.Attributes["memo"].Value,
                SumQty = xmlNode.Attributes["total_dose"].Value
            });
        }

        private static bool Equal(List<JVServerXMLOPD> current, List<JVServerXMLOPD> target, string currentlabeno_type, string targetlabeno_type)
        {
            Log.Write($"CurrentCount:{current.Count}, TargetCount:{target.Count}");
            if (current.Count != target.Count)
                return false;
            StringBuilder cur = new StringBuilder();
            StringBuilder targ = new StringBuilder();
            cur.Append($"{"MedicineCode".PadRight(20)}{"MedicineName".PadRight(30)}{"AdminCode".PadRight(10)}{"PerQty".PadRight(10)}{"SumQty".PadRight(10)}{"Days".PadRight(5)}{"StartDate".PadRight(12)}{"Memo".PadRight(20)}{"CorrectPatientName".PadRight(20)}");
            targ.Append($"{"MedicineCode".PadRight(20)}{"MedicineName".PadRight(30)}{"AdminCode".PadRight(10)}{"PerQty".PadRight(10)}{"SumQty".PadRight(10)}{"Days".PadRight(5)}{"StartDate".PadRight(12)}{"Memo".PadRight(20)}{"CorrectPatientName".PadRight(20)}");
            foreach (var a in current)
            {
                var b = target[current.IndexOf(a)];

                cur.Append($"{ECD(a.MedicineCode, 20)}{ECD(a.MedicineName, 30)}{ECD(a.AdminCode, 10)}{ECD(a.PerQty, 10)}{ECD(a.SumQty, 10)}{ECD(a.Days, 5)}{ECD(a.StartDay, 12)}{ECD(a.Memo, 20)}{ECD(a.CorrectPatientName, 20)}\n");
                targ.Append($"{ECD(b.MedicineCode, 20)}{ECD(b.MedicineName, 30)}{ECD(b.AdminCode, 10)}{ECD(b.PerQty, 10)}{ECD(b.SumQty, 10)}{ECD(b.Days, 5)}{ECD(b.StartDay, 12)}{ECD(b.Memo, 20)}{ECD(b.CorrectPatientName, 20)}\n");
            }
            Log.Write(cur.ToString().Trim());
            Log.Write(targ.ToString().Trim());
            var v = from t in target
                    from c in current
                    where t.AdminCode == c.AdminCode &&
                    t.MedicineCode == c.MedicineCode &&
                    t.MedicineName == c.MedicineName &&
                    t.PerQty == c.PerQty &&
                    t.CorrectPatientName == c.CorrectPatientName &&
                    t.Days == c.Days &&
                    t.StartDay == c.StartDay &&
                    t.Memo == c.Memo &&
                    t.SumQty == c.SumQty
                    select c;
            return v.Count() == current.Count && currentlabeno_type == targetlabeno_type;
        }

        private static string ECD(string data, int Length)  //處理中文
        {
            data = data.PadRight(Length, ' ');
            Byte[] Temp = Encoding.Default.GetBytes(data);
            return Encoding.Default.GetString(Temp, 0, Length);
        }
    }
}