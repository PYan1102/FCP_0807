﻿using FCP.Models;
using Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FCP.src.FormatLogic
{
    class FMT_ChengYu : FormatCollection
    {
        private List<PrescriptionModel> _data = new List<PrescriptionModel>();
        private bool _initialized;
        public override void ProcessCare()
        {
            throw new NotImplementedException();
        }

        public override void ProcessOPD()
        {
            _initialized = true;
            try
            {
                foreach (string s in GetPrescriptionInfoList)
                {
                    EncodingHelper.SetBytes(s.Trim());
                    string adminCode = EncodingHelper.GetString(307, 30);
                    string medicineCode = EncodingHelper.GetString(155, 20);
                    if (FilterRule(adminCode, medicineCode))
                    {
                        continue;
                    }
                    string unit = EncodingHelper.GetString(285, 10);
                    if (unit == "克" || unit == "包" || unit == "瓶" || unit == "袋")
                        continue;
                    float perQty = Convert.ToSingle(EncodingHelper.GetString(150, 5));
                    if (!int.TryParse(perQty.ToString("0.###"), out int i))
                    {
                        _initialized = false;
                    }
                    string adminCodeDescription = EncodingHelper.GetString(235, 50);
                    adminCodeDescription = adminCodeDescription.Replace("*", "+")
                                                               .Replace(":", "：")
                                                               .Replace("/", "／");
                    _data.Add(new PrescriptionModel()
                    {
                        PatientName = EncodingHelper.GetString(0, 20),
                        PatientNo = EncodingHelper.GetString(20, 30),
                        HospitalName = EncodingHelper.GetString(100, 50),
                        PerQty = Convert.ToSingle(EncodingHelper.GetString(150, 5)),
                        MedicineCode = medicineCode,
                        MedicineName = EncodingHelper.GetString(175, 50),
                        AdminCode = EncodingHelper.GetString(225, 10),
                        AdminCodeDescription = adminCodeDescription,
                        Unit = unit,
                        StartDate = DateTimeHelper.Convert(EncodingHelper.GetString(295, 6), "yyMMdd"),
                        EndDate = DateTimeHelper.Convert(EncodingHelper.GetString(301, 6), "yyMMdd"),
                        Random = adminCode,  //中醫的頻率是一天幾包藥，沒有像西醫有TID or QID等等的
                        Days = Convert.ToInt32(EncodingHelper.GetString(337, 30))
                    });
                }
                if (_data.Count == 0)
                {
                    Pass();
                }
            }
            catch (Exception ex)
            {
                ReadFileFail(ex);
            }
        }

        public override void LogicOPD()
        {
            #region 比對當天處方中是否有病患姓名及頻率(自一、自二)重複者，若重複則在檔名加上index
            string[] split = SourceFileNameWithoutExtension.Split('-');

            List<string> repeatPatientList = CommonModel.SqlHelper.Query_List($@"SELECT
                                                     E.JobName
                                                   ,    C.PrescriptionItemValue
                                                   ,	D.PrescriptionItemValue
                                                   FROM Prescription A
                                                   INNER JOIN PrescriptionItem B ON B.PrescriptionID = A.RawID
                                                   INNER JOIN PrescriptionItemDetail C ON C.PrescriptionItemID = B.RawID AND C.OCSFormatItemID=69 AND C.PrescriptionItemValue = N'{_data[0].PatientName}'  --病患姓名
                                                   INNER JOIN PrescriptionItemDetail D ON D.PrescriptionItemID = B.RawID AND D.OCSFormatItemID=68 AND D.PrescriptionItemValue = N'{_data[0].AdminCode}'  --自一or自二
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
                newFileName = $"{split[0]}-{split[2]}({index + 1})-{_data[0].Days}-{_data[0].AdminCodeDescription}.txt";
            }
            else
            {
                newFileName = $"{split[0]}-{split[2]}-{_data[0].Days}-{_data[0].AdminCodeDescription}.txt";
            }

            #endregion
            if (!_initialized)
            {
                newFileName = $"(錯誤) {newFileName}";
            }
            string outputDirectory = $@"{OutputDirectory}\{newFileName}";
            try
            {
                OP_OnCube.ChengYu(_data, outputDirectory);
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

        public override void ProcessPowder()
        {
            throw new NotImplementedException();
        }

        public override void ProcessUDBatch()
        {
            throw new NotImplementedException();
        }

        public override void ProcessUDStat()
        {
            throw new NotImplementedException();
        }

        public override void LogicCare()
        {
            throw new NotImplementedException();
        }

        public override void LogicOther()
        {
            throw new NotImplementedException();
        }

        public override void LogicPowder()
        {
            throw new NotImplementedException();
        }

        public override void LogicUDBatch()
        {
            throw new NotImplementedException();
        }

        public override void LogicUDStat()
        {
            throw new NotImplementedException();
        }

        public override ReturnsResultModel DepartmentShunt()
        {
            _data.Clear();
            return base.DepartmentShunt();
        }
    }
}