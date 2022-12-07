using FCP.Helpers;
using FCP.Models;
using Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FCP.src.FormatLogic
{
    internal class FMT_LiChiun : FormatCollection
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
                string getMedicineNo = EncodingHelper.GetString(102, 30);
                string bedNo = EncodingHelper.GetString(132, 30);
                string patientName = RegexHelper.FilterSpecialSymbols(EncodingHelper.GetString(177, 20));

                //有重疊風險
                string _class = EncodingHelper.GetString(197, 30);
                string hospitalName = EncodingHelper.GetString(229, 40).Replace("?", " ").Replace("？", "  ");
                string locationName = EncodingHelper.GetString(229, 30).Replace("?", " ").Replace("？", "  ");
                EncodingHelper.SetBytes(content.Substring(jvmPosition + 17, content.Length - 17 - jvmPosition));
                List<string> list = SeparateString(EncodingHelper.GetString(0, EncodingHelper.Length), 547);  //計算有多少種藥品資料
                foreach (string s in list)
                {
                    EncodingHelper.SetBytes(s);
                    string adminCode = EncodingHelper.GetString(66, 10);
                    string medicineCode = EncodingHelper.GetString(1, 15);
                    if (adminCode.Length == 0)
                    {
                        if(list.IndexOf(s) != list.Count - 1)
                        {
                            _data.Clear();
                        }
                        continue;
                    }
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
                        GetMedicineNo = getMedicineNo,
                        BedNo = bedNo,
                        Class = _class,
                        HospitalName = hospitalName,
                        LocationName = locationName,
                        MedicineCode = medicineCode,
                        MedicineName = EncodingHelper.GetString(16, 50),
                        AdminCode = adminCode,
                        PerQty = Convert.ToSingle(EncodingHelper.GetString(81, 6)),
                        SumQty = Convert.ToSingle(EncodingHelper.GetString(87, 8)),
                        Random = EncodingHelper.GetString(132, 30),
                        Mark = EncodingHelper.GetString(339, 20),
                        Other1 = EncodingHelper.GetString(107, 40),
                        Other2 = EncodingHelper.GetString(147, 40),
                        Other3 = EncodingHelper.GetString(187, 40),
                        Other4 = EncodingHelper.GetString(227, 40),
                        Other5 = EncodingHelper.GetString(267, 40),
                        Other6 = EncodingHelper.GetString(307, 40),
                        Other7 = EncodingHelper.GetString(347, 40),
                        Other8 = EncodingHelper.GetString(387, 40),
                        StartDate = DateTimeHelper.Convert(EncodingHelper.GetString(507, 8), "yyyyMMdd"),
                        EndDate = DateTimeHelper.Convert(EncodingHelper.GetString(527, 8), "yyyyMMdd")
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
            try
            {
                Dictionary<int, PrescriptionModel> temp = new Dictionary<int, PrescriptionModel>();
                _data.ForEach(x =>
                {
                    if (x.AdminCode.Contains("HS"))
                    {
                        x.PatientName = $"{x.PatientName}_HS";
                        temp.Add(_data.IndexOf(x), x.Clone());
                    }
                });
                _data.RemoveAll(x => temp.Select(y => y.Key).Contains(_data.IndexOf(x)));
                _data.AddRange(temp.Select(x => x.Value));
                string outputDirectory = $@"{OutputDirectory}\{_data[0].PatientName}-{SourceFileNameWithoutExtension}_{CurrentSeconds}.txt";
                OP_OnCube.JVServer(_data, outputDirectory);
                Success();
            }
            catch (Exception ex)
            {
                GenerateOCSFileFail(ex);
            }
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
}
