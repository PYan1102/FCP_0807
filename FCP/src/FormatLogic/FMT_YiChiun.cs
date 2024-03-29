﻿using FCP.Helpers;
using FCP.Models;
using Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace FCP.src.FormatLogic
{
    internal class FMT_YiChiun : FormatCollection
    {
        private List<PrescriptionModel> _up = new List<PrescriptionModel>();
        private List<PrescriptionModel> _down = new List<PrescriptionModel>();

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

                string _class = EncodingHelper.GetString(197, 30);
                string hospitalName = EncodingHelper.GetString(229, 40).Replace("?", " ").Replace("？", "  ");
                string locationName = EncodingHelper.GetString(229, 30).Replace("?", " ").Replace("？", "  ");
                EncodingHelper.SetBytes(content.Substring(jvmPosition + 17, content.Length - 17 - jvmPosition));
                List<string> list = SeparateString(EncodingHelper.GetString(0, EncodingHelper.Length), 547);  //計算有多少種藥品資料
                bool twoND = false;
                foreach (string s in list)
                {
                    EncodingHelper.SetBytes(s);
                    string adminCode = EncodingHelper.GetString(66, 10);
                    string medicineCode = EncodingHelper.GetString(1, 15);
                    if (medicineCode == "/7")  //以下限領一次藥
                    {
                        _up.Clear();
                        continue;
                    }
                    if (medicineCode == "2ND")
                    {
                        twoND = true;
                        continue;
                    }
                    if (medicineCode == "NND000" || medicineCode == "MORE")  //COVID-19、藥物天數超過三天
                    {
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
                    float sumQty = Convert.ToSingle(EncodingHelper.GetString(87, 8));
                    float perQty = Convert.ToSingle(EncodingHelper.GetString(81, 6));
                    int days = Convert.ToInt32(sumQty / perQty / GetMultiAdminCodeTimes(adminCode).Count);
                    PrescriptionModel model = new PrescriptionModel()
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
                        PerQty = perQty,
                        SumQty = sumQty,
                        Days = days,
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
                    };
                    if (!twoND)
                    {
                        _up.Add(model);
                    }
                    else
                    {
                        _down.Add(model);
                    }
                }
                if (_up.Count == 0 && _down.Count == 0)
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
                if (_up.Count > 0)
                {
                    Dictionary<string, List<PrescriptionModel>> up = new Dictionary<string, List<PrescriptionModel>>();
                    foreach (var v in _up)
                    {
                        string key = $"{v.AdminCode}^{v.Days}";
                        if (!up.ContainsKey(key))
                        {
                            up.Add(key, new List<PrescriptionModel>());
                        }
                        up[key].Add(v);
                    }
                    foreach (var v in up)
                    {
                        string outputDirectory = $@"{OutputDirectory}\{_up[0].PatientName}_UP_{v.Key}-{SourceFileNameWithoutExtension}_{CurrentSeconds}.txt";
                        OP_OnCube.JVServer(v.Value, outputDirectory);
                    }
                }
                if (_down.Count > 0)
                {
                    Dictionary<string, List<PrescriptionModel>> down = new Dictionary<string, List<PrescriptionModel>>();
                    foreach (var v in _down)
                    {
                        string key = $"{v.AdminCode}^{v.Days}";
                        if (!down.ContainsKey(key))
                        {
                            down.Add(key, new List<PrescriptionModel>());
                        }
                        down[key].Add(v);
                    }
                    foreach (var v in down)
                    {
                        string outputDirectory = $@"{OutputDirectory}\{_down[0].PatientName}_DOWN_{v.Key}-{SourceFileNameWithoutExtension}_{CurrentSeconds}.txt";
                        OP_OnCube.JVServer(v.Value, outputDirectory);
                    }
                }
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
            _up.Clear();
            _down.Clear();
            return base.DepartmentShunt();
        }
    }
}
