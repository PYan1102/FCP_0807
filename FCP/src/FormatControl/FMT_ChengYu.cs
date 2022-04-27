using Helper;
using FCP.Models;
using System;
using System.Collections.Generic;
using FCP.src.Enum;
using System.IO;
using System.Linq;

namespace FCP.src.FormatControl
{
    class FMT_ChengYu : FormatCollection
    {
        private List<ChengYuOPD> _OPD = new List<ChengYuOPD>();
        private bool _IsInt;
        public override bool ProcessCare()
        {
            throw new NotImplementedException();
        }

        public override bool ProcessOPD()
        {
            _IsInt = true;
            try
            {
                string[] list = GetContent.Split('\n');
                foreach (string s in list)
                {
                    if (s.Trim().Length == 0)
                        continue;
                    EncodingHelper.SetBytes(s.Trim());
                    string numberOfPackages = EncodingHelper.GetString(307, 30);
                    string medicineCode = EncodingHelper.GetString(155, 20);
                    if (IsFilterMedicineCode(medicineCode) || IsFilterAdminCode(numberOfPackages))
                    {
                        continue;
                    }
                    string unit = EncodingHelper.GetString(285, 10);
                    if (unit == "克" || unit == "包" || unit == "瓶" || unit == "袋")
                        continue;
                    float perQty = Convert.ToSingle(EncodingHelper.GetString(150, 5));
                    if (!int.TryParse(perQty.ToString("0.###"), out int i))
                    {
                        _IsInt = false;
                    }
                    //if (!IsExistsMultiAdminCode(numberOfPackages))
                    //{
                    //    Log.Write($"{FilePath} 在OnCube中未建置餐包頻率 {numberOfPackages}");
                    //    ReturnsResult.Shunt(ConvertResult.沒有餐包頻率, numberOfPackages);
                    //    return false;
                    //}
                    string adminCodeDescription = EncodingHelper.GetString(235, 50);
                    adminCodeDescription = adminCodeDescription.Replace("*", "+").Replace(":", "：").Replace("/", "／");
                    _OPD.Add(new ChengYuOPD()
                    {
                        PatientName = EncodingHelper.GetString(0, 20),
                        PatientNo = EncodingHelper.GetString(20, 30),
                        HospitalName = EncodingHelper.GetString(100, 50),
                        PerQty = EncodingHelper.GetString(150, 5),
                        MedicineCode = medicineCode,
                        MedicineName = EncodingHelper.GetString(175, 50),
                        AdminCode = EncodingHelper.GetString(225, 10),
                        AdminCodeDescription = adminCodeDescription,
                        Unit = unit,
                        StartDay = DateTimeHelper.Convert(EncodingHelper.GetString(295, 6), "yyMMdd"),
                        EndDay = DateTimeHelper.Convert(EncodingHelper.GetString(301, 6), "yyMMdd"),
                        NumOfPackages = numberOfPackages,
                        Days = EncodingHelper.GetString(337, 30)
                    });
                }
                if (_OPD.Count == 0)
                {
                    ReturnsResult.Shunt(eConvertResult.全數過濾, null);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Write($"{FilePath} {ex}");
                ReturnsResult.Shunt(eConvertResult.讀取檔案失敗, ex.ToString());
                return false;
            }
        }

        public override bool ProcessOther()
        {
            throw new NotImplementedException();
        }

        public override bool ProcessPOWDER()
        {
            throw new NotImplementedException();
        }

        public override bool ProcessUDBatch()
        {
            throw new NotImplementedException();
        }

        public override bool ProcessUDStat()
        {
            throw new NotImplementedException();
        }

        public override bool LogicCare()
        {
            throw new NotImplementedException();
        }

        public override bool LogicOPD()
        {
            string[] split = Path.GetFileNameWithoutExtension(FilePath).Split('-');
            List<string> repeatPatientList = MSSql.RunSQL_List($@"SELECT
	                                                    E.JobName
                                                   ,    C.PrescriptionItemValue
                                                   ,	D.PrescriptionItemValue
                                                   FROM Prescription A
                                                   INNER JOIN PrescriptionItem B ON B.PrescriptionID = A.RawID
                                                   INNER JOIN PrescriptionItemDetail C ON C.PrescriptionItemID = B.RawID AND C.OCSFormatItemID=69 AND C.PrescriptionItemValue = N'{_OPD[0].PatientName}'  --病患姓名
                                                   INNER JOIN PrescriptionItemDetail D ON D.PrescriptionItemID = B.RawID AND D.OCSFormatItemID=68 AND D.PrescriptionItemValue = N'{_OPD[0].AdminCode}'  --自一or自二
                                                   INNER JOIN Job E ON E.RawID = A.JobID
                                                   WHERE A.LastUpdatedDate BETWEEN '{DateTime.Now:yyyy-MM-dd}' AND '{DateTime.Now.AddDays(1):yyyy-MM-dd}' AND A.DeletedYN = 0
                                                   GROUP BY C.PrescriptionItemValue, D.PrescriptionItemValue, E.JobName, A.RawID
                                                   ORDER BY A.RawID DESC", "JobName");
            string newFileName = string.Empty;
            if (repeatPatientList.Count > 0)
            {
                int index = (from s in repeatPatientList
                             let temp = s.Split('-')
                             let startPosition = temp[1].IndexOf("(")
                             let endPosition = temp[1].IndexOf(")")
                             select startPosition > 0 && endPosition > 0 ? Convert.ToInt32(temp[1].Substring(startPosition + 1, endPosition - startPosition - 1)) : 0).ToList().Max();
                newFileName = $"{split[0]}-{split[2]}({index + 1})-{_OPD[0].Days}-{_OPD[0].AdminCodeDescription}.txt";
            }
            else
            {
                newFileName = $"{split[0]}-{split[2]}-{_OPD[0].Days}-{_OPD[0].AdminCodeDescription}.txt";
            }
            if (!_IsInt)
            {
                newFileName = $"(錯誤) {newFileName}";
            }
            string filePathOutput = $@"{OutputPath}\{newFileName}";
            try
            {
                OP_OnCube.ChengYu(_OPD, filePathOutput);
                return true;
            }
            catch (Exception ex)
            {
                Log.Write($"{FilePath} {ex}");
                ReturnsResult.Shunt(eConvertResult.產生OCS失敗, ex.ToString());
                return false;
            }
        }
        public override bool LogicOther()
        {
            throw new NotImplementedException();
        }

        public override bool LogicPOWDER()
        {
            throw new NotImplementedException();
        }

        public override bool LogicUDBatch()
        {
            throw new NotImplementedException();
        }

        public override bool LogicUDStat()
        {
            throw new NotImplementedException();
        }

        public override ReturnsResultFormat MethodShunt()
        {
            _OPD.Clear();
            return base.MethodShunt();
        }
    }
    internal class ChengYuOPD
    {
        public string PatientNo { get; set; }
        public string PatientName { get; set; }
        public string MedicineCode { get; set; }
        public string MedicineName { get; set; }
        public string PerQty { get; set; }
        public string AdminCode { get; set; }
        public string AdminCodeDescription { get; set; }
        public string Days { get; set; }
        public string NumOfPackages { get; set; }
        public DateTime StartDay { get; set; }
        public DateTime EndDay { get; set; }
        public string BedNo { get; set; }
        public string HospitalName { get; set; }
        public string Unit { get; set; }
    }
}
