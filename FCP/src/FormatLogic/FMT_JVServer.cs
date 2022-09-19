using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using FCP.src.Enum;
using FCP.Models;
using Helper;

namespace FCP.src.FormatLogic
{
    class FMT_JVServer : FormatCollection
    {
        private List<PrescriptionModel> _data = new List<PrescriptionModel>();

        public override void ProcessOPD()
        {
            try
            {
                string content = GetFileContent.Trim();
                int jvmPosition = content.IndexOf("|JVPEND||JVMHEAD|");
                EncodingHelper.SetBytes(content.Substring(9, jvmPosition - 9));
                string patientNo = EncodingHelper.GetString(1, 15);
                string prescriptionNo = EncodingHelper.GetString(16, 20);
                DateTime birthDate = DateTimeHelper.Convert(EncodingHelper.GetString(94, 8), "yyyyMMdd");
                string patientName = EncodingHelper.GetString(177, 20);
                if (patientName.Contains("?"))
                    patientName = patientName.Replace("?", " ");
                //有重疊風險
                string _class = EncodingHelper.GetString(197, 30);
                string hospitalName = EncodingHelper.GetString(229, 40).Replace("?", " ").Replace("？", "  ");
                string locationName = EncodingHelper.GetString(229, 30).Replace("?", " ").Replace("？", "  ");
                EncodingHelper.SetBytes(content.Substring(jvmPosition + 17, content.Length - 17 - jvmPosition));
                List<string> list = SeparateString(EncodingHelper.GetString(0, EncodingHelper.Length), 547);  //計算有多少種藥品資料
                Console.WriteLine($"{hospitalName} {locationName}");
                foreach (string s in list)
                {
                    EncodingHelper.SetBytes(s);
                    string adminCode = EncodingHelper.GetString(66, 10);
                    string medicineCode = EncodingHelper.GetString(1, 15);
                    if (FilterRule(adminCode, medicineCode))
                    {
                        continue;
                    }
                    if (WhetherToStopNotHasMultiAdminCode(adminCode))
                    {
                        return;
                    }
                    _data.Add(new PrescriptionModel()
                    {
                        PatientName = patientName,
                        PatientNo = patientNo,
                        PrescriptionNo = prescriptionNo,
                        BirthDate = birthDate.ToString("yyyy-MM-dd"),
                        Class = _class,
                        HospitalName = hospitalName,
                        LocationName = locationName,
                        MedicineCode = medicineCode,
                        MedicineName = EncodingHelper.GetString(16, 50),
                        AdminCode = adminCode,
                        PerQty = Convert.ToSingle(EncodingHelper.GetString(81, 6)),
                        SumQty = Convert.ToSingle(EncodingHelper.GetString(87, 8)),
                        StartDate = DateTimeHelper.Convert(EncodingHelper.GetString(507, 8), "yyyyMMdd"),
                        EndDate = DateTimeHelper.Convert(EncodingHelper.GetString(527, 8), "yyyyMMdd"),
                        Other1 = EncodingHelper.GetString(132, 30),  //random
                        Other2 = EncodingHelper.GetString(339, 20),  //mark
                        Other3 = EncodingHelper.GetString(107, 40),
                        Other4 = EncodingHelper.GetString(147, 40),
                        Other5 = EncodingHelper.GetString(187, 40),
                        Other6 = EncodingHelper.GetString(227, 40),
                        Other7 = EncodingHelper.GetString(267, 40),
                        Other8 = EncodingHelper.GetString(307, 40),
                        Other9 = EncodingHelper.GetString(347, 40),
                        Other10 = EncodingHelper.GetString(387, 40)
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
            Dictionary<string, List<PrescriptionModel>> dict = null;
            var splitEachMeal = Properties.Settings.Default.IsSplitEachMeal;
            if (splitEachMeal)
            {
                try
                {
                    dict = SplitEachMeal();
                }
                catch (Exception ex)
                {
                    ProgressLogicFail(ex);
                    return;
                }
            }
            try
            {
                if (splitEachMeal)
                {
                    string outputDirectoryWithoutSeconds = $@"{OutputDirectory}\{_data[0].PatientName}-{SourceFileNameWithoutExtension}_";
                    OP_OnCube.JVServer_SplitEachMeal(dict, outputDirectoryWithoutSeconds, CurrentSeconds);
                }
                else
                {
                    string outputDirectory = $@"{OutputDirectory}\{_data[0].PatientName}-{SourceFileNameWithoutExtension}_{CurrentSeconds}.txt";
                    OP_OnCube.JVServer(_data, outputDirectory);
                }
                Success();
            }
            catch (Exception ex)
            {
                GenerateOCSFileFail(ex);
            }
        }

        private Dictionary<string, List<PrescriptionModel>> SplitEachMeal()
        {
            Dictionary<string, List<PrescriptionModel>> dic = new Dictionary<string, List<PrescriptionModel>>();
            for (int x = 0; x <= 23; x++)
            {
                dic.Add(x.ToString().PadLeft(2, '0'), new List<PrescriptionModel>());
            }
            for (int i = 0; i <= _data.Count - 1; i++)
            {
                string adminCode = _data[i].AdminCode;
                List<string> adminCodeTimeList = GetMultiAdminCodeTimes(adminCode);
                foreach (var v in adminCodeTimeList)
                {
                    string hour = v.Substring(0, 2);
                    dic[hour].Add(_data[i]);
                }
            }
            return dic;
        }


        public override void ProcessUDBatch()
        {
            throw new NotImplementedException();
        }

        public override void LogicUDBatch()
        {
            throw new NotImplementedException();
        }

        public override void ProcessUDStat()
        {
            throw new NotImplementedException();
        }

        public override void LogicUDStat()
        {
            throw new NotImplementedException();
        }

        public override void ProcessPowder()
        {
            throw new NotImplementedException();
        }

        public override void LogicPowder()
        {
            throw new NotImplementedException();
        }

        public override void ProcessOther()
        {
            throw new NotImplementedException();
        }

        public override void LogicOther()
        {
            throw new NotImplementedException();
        }

        public override void ProcessCare()
        {
            throw new NotImplementedException();
        }

        public override void LogicCare()
        {
            throw new NotImplementedException();
        }

        public override ReturnsResultModel DepartmentShunt()
        {
            _data.Clear();
            return base.DepartmentShunt();
        }
    }

    internal class JVServerOPDBasic
    {
        public string PatientName { get; set; }
        public string PatientNo { get; set; }
        public string PrescriptionNo { get; set; }
        public string Class { get; set; }
        public string Mark { get; set; }
        public string Age { get; set; }
        public string ID { get; set; }
        public string Gender { get; set; }
        public string BirthDate { get; set; }
        public string LocationName { get; set; }
        public string HospitalName { get; set; }
    }

    internal class JVServerOPD
    {
        public string MedicineCode { get; set; }
        public string MedicineName { get; set; }
        public string PerQty { get; set; }
        public string AdminCode { get; set; }
        public string Days { get; set; }
        public string SumQty { get; set; }
        public string BedNo { get; set; }
        public string StartDay { get; set; }
        public string EndDay { get; set; }
    }
}
