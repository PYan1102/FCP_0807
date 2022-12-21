using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FCP.src.Enum;
using Helper;
using FCP.Models;

namespace FCP.src.FormatLogic
{
    class FMT_KuangTien : FormatCollection
    {
        private Dictionary<string, List<KuangTienPowder>> _powder = new Dictionary<string, List<KuangTienPowder>>();
        private Dictionary<KuangTienUDBasic, List<KuangTienUD>> _udPrescriptions = new Dictionary<KuangTienUDBasic, List<KuangTienUD>>();
        private KuangTienOPDBasic _opdBasic = new KuangTienOPDBasic();
        private List<KuangTienOPD> _opd = new List<KuangTienOPD>();

        public override void ProcessOPD()
        {
            try
            {
                string[] list = DeleteSpace(GetFileContent).Split('\n');
                EncodingHelper.SetBytes(list[0]);
                _opdBasic.WriteDate = EncodingHelper.GetString(9, 9);
                _opdBasic.GetMedicineNo = EncodingHelper.GetString(54, 8);
                _opdBasic.Age = EncodingHelper.GetString(67, 5);
                _opdBasic.DoctorName = EncodingHelper.GetString(77, 11);
                EncodingHelper.SetBytes(list[1]);
                _opdBasic.PatientNo = EncodingHelper.GetString(9, 11);
                _opdBasic.PatientName = EncodingHelper.GetString(25, 21);
                _opdBasic.Gender = EncodingHelper.GetString(47, 2);
                _opdBasic.Class = EncodingHelper.GetString(67, EncodingHelper.Length - 67);
                for (int x = 4; x <= list.ToList().Count - 3; x++)
                {
                    EncodingHelper.SetBytes(list[x]);
                    string adminCode = EncodingHelper.GetString(57, 11) + EncodingHelper.GetString(68, 9);
                    string medicineCode = EncodingHelper.GetString(2, 10);
                    int sumQty = Convert.ToInt32(EncodingHelper.GetString(82, 8));
                    int id1 = GetID1(medicineCode);
                    int id2 = GetID2(medicineCode);

                    if (FilterRule(adminCode, medicineCode))
                    {
                        continue;
                    }
                    if (id1 >= 1 && sumQty % id1 == 0)  //總量可以被id1整除不包
                    {
                        continue;
                    }
                    if (id2 >= 1 && sumQty >= id2)  //總量超過id2(預包)數量不包
                    {
                        continue;
                    }
                    if (WhetherToStopNotHasMultiAdminCode(adminCode))
                    {
                        return;
                    }
                    _opd.Add(new KuangTienOPD()
                    {
                        MedicineCode = medicineCode,
                        MedicineName = EncodingHelper.GetString(12, 31),
                        AdminCode = adminCode,
                        PerQty = EncodingHelper.GetString(43, 8),
                        Days = EncodingHelper.GetString(77, 5),
                        SumQty = EncodingHelper.GetString(82, 8),
                        TimesPerDay = $"{GetMultiAdminCodeTimes(adminCode).Count}"
                    });
                }
                if (_opd.Count == 0)
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
            int year = Convert.ToInt32(DateTime.Now.ToString("yyyy")) - 1911;
            string outputDirectory = $@"{OutputDirectory}\{year}{DateTime.Now:MMdd}-{_opdBasic.GetMedicineNo}-{_opdBasic.PatientName}_{CurrentSeconds}.txt";
            try
            {
                OP_OnCube.KuangTien_OPD(_opdBasic, _opd, outputDirectory);
                Success();
            }
            catch (Exception ex)
            {
                GenerateOCSFileFail(ex);
            }
        }

        public override void ProcessUDBatch()
        {
            try
            {
                List<string> list = DeleteSpace(GetFileContent).Split('\n').ToList();
                StringBuilder sb = new StringBuilder();
                List<string> prescriptions = SeparateUD(list);
                foreach (var s in prescriptions)
                {
                    KuangTienUDBasic basic = new KuangTienUDBasic();
                    List<KuangTienUD> ud = new List<KuangTienUD>();
                    List<string> detail = s.Split('\n').ToList();
                    foreach (var v in detail)
                    {
                        if (v.Trim().Length == 0 || v.Contains("--------"))
                            continue;
                        int index = detail.IndexOf(v);
                        if (index == 2)  //診療日
                        {
                            EncodingHelper.SetBytes(v);
                            basic.TreatmentDate = DateTimeHelper.Convert($"{Convert.ToInt32(EncodingHelper.GetString(8, 9).Replace("/", "")) + 19110000}", "yyyyMMdd");
                        }
                        else if (index == 3)  //床號
                        {
                            EncodingHelper.SetBytes(v);
                            basic.BedNo = EncodingHelper.GetString(6, 9);
                            basic.PatientNo = EncodingHelper.GetString(22, 11);
                            basic.PatientName = EncodingHelper.GetString(38, 11);
                            basic.Barcode = $"{basic.PatientNo},{basic.BedNo},{DateTime.Now:yyyy-MM-dd},";
                        }
                        else if (index > 5 && !v.Contains("◎") && v.Trim().Length != 0)  //藥品
                        {
                            EncodingHelper.SetBytes(v);
                            DateTime startDate = DateTimeHelper.Convert($"{Convert.ToInt32(EncodingHelper.GetString(86, 7)) + 19110000} {EncodingHelper.GetString(94, 5)}", "yyyyMMdd HHmm");
                            string medicineCode = EncodingHelper.GetString(0, 10);
                            string adminCode = $"{EncodingHelper.GetString(53, 11)}{EncodingHelper.GetString(69, 9)}";
                            float sumQty = Convert.ToSingle(EncodingHelper.GetString(78, 8));
                            if (FilterRule(adminCode, medicineCode))
                            {
                                continue;
                            }
                            adminCode = adminCode.Contains("TTS") ? "STTS" : adminCode;
                            if (WhetherToStopNotHasMultiAdminCode(adminCode) || WhetherToStopNotHasCombiAdminCode(adminCode))
                            {
                                continue;
                            }
                            basic.Barcode += $"*{medicineCode}#{adminCode}#{Math.Ceiling(sumQty)}";
                            ud.Add(new KuangTienUD()
                            {
                                MedicineCode = medicineCode,
                                MedicineName = EncodingHelper.GetString(10, 31),
                                PerQty = Convert.ToSingle(EncodingHelper.GetString(47, 6)),
                                AdminCode = adminCode,
                                Days = Convert.ToInt32(EncodingHelper.GetString(64, 5)),
                                SumQty = sumQty,
                                StartDate = startDate,
                                PrintDate = startDate,
                                MedicineSerialNo = EncodingHelper.GetString(107, EncodingHelper.Length - 107)
                            });
                            if (EncodingHelper.GetString(99, 7).Length != 0)
                            {
                                ud[ud.Count - 1].EndDate = DateTimeHelper.Convert($"{Convert.ToInt32(EncodingHelper.GetString(99, 7)) + 19110000}", "yyyyMMdd");
                            }
                            //var a = ud[ud.Count - 1];
                            //Console.WriteLine($"{a.MedicineCode} {a.MedicineName} {a.PerQty} {a.AdminCode} {a.Days} {a.SumQty} {a.StartDate} {a.EndDate}");
                        }
                    }
                    if (ud.Count > 0)
                    {
                        _udPrescriptions.Add(basic, ud);
                    }
                }
                if (_udPrescriptions.Count == 0)
                {
                    Pass();
                }
            }
            catch (Exception ex)
            {
                ReadFileFail(ex);
            }
        }

        public override void LogicUDBatch()
        {
            try
            {
                foreach (var prescription in _udPrescriptions)
                {
                    List<KuangTienUD> ud = new List<KuangTienUD>();
                    for (int i = 0; i <= prescription.Value.Count - 1; i++)
                    {
                        KuangTienUDBasic basic = prescription.Key;
                        var medicine = prescription.Value[i];
                        float perQty = medicine.PerQty;
                        float sumQty = medicine.SumQty;
                        int days = medicine.Days;
                        int times = Convert.ToInt32(sumQty / perQty);
                        int currentTimes = 1;
                        string adminCode = medicine.AdminCode;
                        string medicineCode = medicine.MedicineCode;
                        bool perQtyIsnteger = !$"{perQty}".Contains(".");
                        bool multiDose = Properties.Settings.Default.DoseType == "Multi";
                        DateTime startDate = medicine.StartDate;
                        DateTime treatmentDate = basic.TreatmentDate;
                        List<string> adminCodeTimes = GetMultiAdminCodeTimes(medicine.AdminCode);
                        if (basic.BedNo.Contains("111DAY"))
                        {
                            medicine.Description += $"每次劑量 : {perQty}";
                            medicine.Description += perQtyIsnteger ? "   " : " 非整數";
                            basic.Care = true;
                            medicine.DoseType = eDoseType.種包;
                            medicine.EndDate = medicine.StartDate;
                            medicine.AdminCode = $"S{medicine.AdminCode}";
                            medicine.PerQty = sumQty;
                            ud.Add(medicine.Clone());
                            //沙鹿
                            CommonModel.SqlHelper.Execute("update PrintFormItem set DeletedYN=1 where RawID in (120180,120195)");

                            //大甲
                            //CommonModel.SqlHelper.Execute("update PrintFormItem set DeletedYN=1 where RawID in (120156,120172)");
                            continue;
                        }
                        //種包
                        //medicineCode == "23521" 為大甲需求
                        else if (!multiDose || !perQtyIsnteger || adminCode.Contains("STTS"))
                        {
                            medicine.DoseType = eDoseType.種包;
                            medicine.EndDate = medicine.StartDate;
                            medicine.PerQty = sumQty;
                            if (medicine.AdminCode == "QODAC" || medicine.AdminCode == "QODPC")
                            {
                                medicine.CrossDay = true;
                                while (currentTimes <= times)
                                {
                                    bool startDateOutOfTreatmentDate = DateTime.Compare(startDate.AddDays(2), treatmentDate.AddDays(days)) == 1;
                                    if (startDateOutOfTreatmentDate)
                                        break;
                                    if (currentTimes >= 2)
                                    {
                                        startDate = startDate.AddDays(2);
                                    }
                                    medicine.Description += $"{startDate:MM/dd} 劑量 : {perQty}";
                                    medicine.Description += multiDose || perQtyIsnteger ? "   " : " 非整數";
                                    currentTimes++;
                                }
                            }
                            else
                            {
                                medicine.Description += $"每次劑量 : {perQty}";
                                medicine.Description += multiDose || perQtyIsnteger ? "   " : " 非整數";
                            }
                            medicine.AdminCode = $"S{adminCode}";
                            ud.Add(medicine.Clone());
                            continue;
                        }
                        //跨天數
                        else if (SettingModel.CrossDayAdminCode.Contains(adminCode))
                        {
                            medicine.DoseType = eDoseType.餐包;
                            medicine.CrossDay = true;
                            medicine.TakingDescription = $"服用日 {startDate:yyyy/MM/dd}";
                            medicine.EndDate = DateTime.Compare(startDate, treatmentDate.AddDays(days)) == 1 ? startDate : treatmentDate.AddDays(days);
                            ud.Add(medicine.Clone());
                            continue;
                        }
                        CalculatedMedicineStartAndEndDate(ud, medicine);
                    }
                    _udPrescriptions[prescription.Key].Clear();
                    _udPrescriptions[prescription.Key].AddRange(ud);
                }
            }
            catch (Exception ex)
            {
                ProgressLogicFail(ex);
                return;
            }
            try
            {
                string outputDirectory = $@"{OutputDirectory}\{SourceFileNameWithoutExtension}_{CurrentSeconds}.txt";
                OP_OnCube.KuangTien_Batch(_udPrescriptions, outputDirectory);
                Success();
            }
            catch (Exception ex)
            {
                GenerateOCSFileFail(ex);
            }
        }

        public override void ProcessUDStat()
        {
            try
            {
                List<string> list = DeleteSpace(GetFileContent).Split('\n').ToList();
                StringBuilder sb = new StringBuilder();
                List<string> prescriptions = SeparateUD(list);
                foreach (var s in prescriptions)
                {
                    KuangTienUDBasic basic = new KuangTienUDBasic();
                    List<KuangTienUD> ud = new List<KuangTienUD>();
                    List<string> detail = s.Split('\n').ToList();
                    foreach (var v in detail)
                    {
                        if (v.Trim().Length == 0 || v.Contains("--------"))
                            continue;
                        int index = detail.IndexOf(v);
                        if (index == 2)  //診療日
                        {
                            EncodingHelper.SetBytes(v);
                            basic.TreatmentDate = DateTimeHelper.Convert($"{Convert.ToInt32(EncodingHelper.GetString(8, 9).Replace("/", "")) + 19110000}", "yyyyMMdd");
                        }
                        else if (index == 3)  //床號
                        {
                            EncodingHelper.SetBytes(v);
                            basic.BedNo = EncodingHelper.GetString(6, 9);
                            basic.PatientNo = EncodingHelper.GetString(22, 11);
                            basic.PatientName = EncodingHelper.GetString(38, 11);
                            basic.Barcode = $"{basic.PatientNo},{basic.BedNo},{DateTime.Now:yyyy-MM-dd},";
                        }
                        else if (index > 5 && !v.Contains("◎") && v.Trim().Length != 0)  //藥品
                        {
                            EncodingHelper.SetBytes(v);
                            DateTime startDate = DateTimeHelper.Convert($"{Convert.ToInt32(EncodingHelper.GetString(66, 7)) + 19110000} {EncodingHelper.GetString(74, 5)}", "yyyyMMdd HHmm");
                            string medicineCode = EncodingHelper.GetString(0, 10);
                            string adminCode = $"{EncodingHelper.GetString(43, 5)}{EncodingHelper.GetString(53, 5)}";
                            float sumQty = Convert.ToSingle(EncodingHelper.GetString(58, 8));
                            if (FilterRule(adminCode, medicineCode))
                            {
                                continue;
                            }
                            adminCode = adminCode.Contains("TTS") ? "STTS" : adminCode;
                            if ((!adminCode.StartsWith("ST") && WhetherToStopNotHasMultiAdminCode(adminCode))
                                || WhetherToStopNotHasCombiAdminCode(adminCode))
                            {
                                continue;
                            }
                            basic.Barcode += $"*{medicineCode}#{adminCode}#{Math.Ceiling(sumQty)}";
                            ud.Add(new KuangTienUD()
                            {
                                MedicineCode = medicineCode,
                                MedicineName = EncodingHelper.GetString(10, 21),
                                PerQty = Convert.ToSingle(EncodingHelper.GetString(36, 6)),
                                AdminCode = adminCode,
                                Days = Convert.ToInt32(EncodingHelper.GetString(48, 5)),
                                SumQty = sumQty,
                                StartDate = startDate,
                                PrintDate = startDate,
                                MedicineSerialNo = EncodingHelper.GetString(86, EncodingHelper.Length - 86)
                            });
                            if (EncodingHelper.GetString(79, 7).Length != 0)
                            {
                                ud[ud.Count - 1].EndDate = DateTimeHelper.Convert($"{Convert.ToInt32(EncodingHelper.GetString(79, 7)) + 19110000}", "yyyyMMdd");
                            }
                            //var a = ud[ud.Count - 1];
                            //Console.WriteLine($"{a.MedicineCode} {a.MedicineName} {a.PerQty} {a.AdminCode} {a.Days} {a.SumQty} {a.StartDate} {a.EndDate}");
                        }
                    }
                    if (ud.Count > 0)
                    {
                        _udPrescriptions.Add(basic, ud);
                    }
                }
                if (_udPrescriptions.Count == 0)
                {
                    Pass();
                }
            }
            catch (Exception ex)
            {
                ReadFileFail(ex);
            }
        }

        public override void LogicUDStat()
        {
            try
            {
                List<string> statAdminCode = new List<string>()
                {
                    "STAC",
                    "STPC",
                    "STWM",
                    "STPO",
                    "STOTHER",
                    "STOU",
                    "STRECT"
                };
                foreach (var prescription in _udPrescriptions)
                {
                    List<KuangTienUD> ud = new List<KuangTienUD>();
                    for (int i = 0; i <= prescription.Value.Count - 1; i++)
                    {
                        KuangTienUDBasic basic = prescription.Key;
                        var medicine = prescription.Value[i];
                        float perQty = medicine.PerQty;
                        float sumQty = medicine.SumQty;
                        int days = medicine.Days;
                        string adminCode = medicine.AdminCode;
                        string medicineCode = medicine.MedicineCode;
                        bool perQtyIsnteger = !$"{perQty}".Contains(".");
                        bool multiDose = Properties.Settings.Default.DoseType == "Multi";
                        DateTime startDate = medicine.StartDate;
                        DateTime treatmentDate = basic.TreatmentDate;
                        if (!multiDose || !perQtyIsnteger || statAdminCode.Contains(adminCode) || CrossDayAdminCodeDays.TryGetValue(adminCode, out int value) || adminCode == "STTS")
                        {
                            medicine.DoseType = eDoseType.種包;
                            medicine.EndDate = startDate;
                            medicine.Description += $"每次劑量 : {perQty}";
                            medicine.Description += multiDose || perQtyIsnteger ? "   " : " 非整數";
                            medicine.PerQty = Convert.ToSingle(Math.Ceiling(sumQty));
                            if (adminCode != "STTS")
                            {
                                medicine.AdminCode = $"S{adminCode}";
                            }
                            ud.Add(medicine.Clone());
                            continue;
                        }
                        CalculatedMedicineStartAndEndDate(ud, medicine);
                    }
                    _udPrescriptions[prescription.Key].Clear();
                    _udPrescriptions[prescription.Key].AddRange(ud);
                }
            }
            catch (Exception ex)
            {
                ProgressLogicFail(ex);
                return;
            }
            try
            {
                var firstPatientBasicInfo = _udPrescriptions.ToList().Select(x => (
                    PatientName: x.Key.PatientName,
                    PatientNo: x.Key.PatientNo
                )).First();
                string outputDirectory = $@"{OutputDirectory}\{firstPatientBasicInfo.PatientName}-{firstPatientBasicInfo.PatientNo}_{CurrentSeconds}.txt";
                OP_OnCube.KuangTien_Stat(_udPrescriptions, outputDirectory);
                Success();
            }
            catch (Exception ex)
            {
                GenerateOCSFileFail(ex);
            }
        }

        public override void ProcessPowder()
        {
            try
            {
                List<string> list = DeleteSpace(GetFileContent).Split('\n').ToList();
                EncodingHelper.SetBytes(list[0]);
                int year = Convert.ToInt32(EncodingHelper.GetString(9, 3)) + 1911;
                DateTime startDate = Convert.ToDateTime($"{year}{EncodingHelper.GetString(12, 6)}");
                string getMedicineNumber = EncodingHelper.GetString(54, 8);
                EncodingHelper.SetBytes(list[1]);
                string patientNo = EncodingHelper.GetString(9, 11);
                string patientName = EncodingHelper.GetString(25, 21);
                string gender = EncodingHelper.GetString(47, 2);
                string _class = EncodingHelper.GetString(67, EncodingHelper.Length - 67);
                int barcodeIndex = list.Where(x => x.Contains("BARCODE")).Select(x => list.IndexOf(x)).First();
                EncodingHelper.SetBytes(list[barcodeIndex]);
                string barcode = EncodingHelper.GetString(10, EncodingHelper.Length - 10);
                list.RemoveRange(0, 4);
                list.RemoveRange(list.Count - 2, 2);
                foreach (string s in list)
                {
                    if (s.Contains("=====")
                        || s.Contains("光田綜合醫院")
                        || s.Contains("BARCODE")
                        || s.Trim().Length == 0)
                        continue;
                    EncodingHelper.SetBytes(s.TrimEnd('\n'));
                    string adminCode = EncodingHelper.GetString(57, 11) + EncodingHelper.GetString(68, 9);
                    string grindTable = EncodingHelper.GetString(100, 8);
                    if (grindTable.Length == 0)
                        continue;
                    if (FilterRule(adminCode, null, true, false, false))
                    {
                        continue;
                    }
                    var powder = new KuangTienPowder()
                    {
                        StartDate = startDate,
                        GetMedicineNo = getMedicineNumber,
                        PatientNo = patientNo,
                        PatientName = patientName,
                        Gender = gender,
                        Class = _class,
                        MedicineCode = EncodingHelper.GetString(2, 10),
                        MedicineName = EncodingHelper.GetString(12, 31),
                        PerQty = EncodingHelper.GetString(43, 8),
                        AdminCode = adminCode,
                        Days = EncodingHelper.GetString(77, 5),
                        SumQty = EncodingHelper.GetString(82, 8),
                        GrindTable = grindTable,
                        EffectiveDate = DateTime.Now.AddDays(Convert.ToInt32(EncodingHelper.GetString(77, 5))),
                        Barcode = barcode
                    };
                    Console.WriteLine(adminCode);
                    if (adminCode.Contains("PRN"))
                    {
                        JVServerAdminCodeTimes.TryGetValue(adminCode, out int times);
                        times = times > 0 ? times : 1;
                        powder.TimesPerDay = Math.Round(Convert.ToSingle(powder.SumQty) / times / Convert.ToSingle(powder.PerQty), 0, MidpointRounding.AwayFromZero).ToString();
                    }
                    else
                    {
                        powder.TimesPerDay = Math.Round(Convert.ToSingle(powder.SumQty) / Convert.ToInt32(powder.Days) / Convert.ToSingle(powder.PerQty), 0, MidpointRounding.AwayFromZero).ToString();
                    }
                    if (!_powder.ContainsKey(grindTable))
                        _powder[grindTable] = new List<KuangTienPowder>();
                    _powder[grindTable].Add(powder);
                }
                if (_powder.Count == 0)
                {
                    Pass();
                }
            }
            catch (Exception ex)
            {
                ReadFileFail(ex);
            }
        }

        public override void LogicPowder()
        {
            int year = Convert.ToInt32(DateTime.Now.ToString("yyyy")) - 1911;
            var firstPowder = _powder.Select(x => x).First().Value[0];
            string outputDirectory = $@"{OutputDirectory}\{year}{DateTime.Now:MMdd}_{firstPowder.GetMedicineNo}_{firstPowder.PatientName}_{CurrentSeconds}.txt";
            List<string> grindTableList = new List<string>();
            grindTableList.AddRange(_powder.Where(x => !grindTableList.Contains(x.Key)).Select(x => x.Key));
            try
            {
                OP_JVServer.KuangTien_磨粉(_powder, grindTableList, outputDirectory);
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
            _powder.Clear();
            _opd.Clear();
            _udPrescriptions.Clear();
            return base.DepartmentShunt();
        }

        private List<string> SeparateUD(List<string> list)
        {
            StringBuilder sb = new StringBuilder();
            List<string> prescriptions = new List<string>();
            foreach (var s in list)
            {
                if (s.Contains("C!") && sb.ToString().Trim().Length > 0)
                {
                    prescriptions.Add(sb.ToString());
                    sb.Clear();
                }
                if (s.Contains("C!") && s.Trim().Length == 3)
                {
                    continue;
                }
                sb.AppendLine(s.Trim());
                if (list.IndexOf(s) == list.Count - 1)
                {
                    prescriptions.Add(sb.ToString());
                }
            }
            return prescriptions;
        }

        private void CalculatedMedicineStartAndEndDate(List<KuangTienUD> ud, KuangTienUD medicine)
        {
            int currentTimes = 1;
            float sumQty = medicine.SumQty;
            float perQty = medicine.PerQty;
            int times = Convert.ToInt32(sumQty / perQty);
            DateTime startDate = medicine.StartDate;
            List<string> adminCodeTimes = GetMultiAdminCodeTimes(medicine.AdminCode);
            while (currentTimes <= times)
            {
                medicine.DoseType = eDoseType.餐包;
                foreach (var time in adminCodeTimes)
                {
                    if (currentTimes > times)
                    {
                        break;
                    }
                    DateTime adminDate = DateTimeHelper.Convert($"{startDate:yyyyMMdd} {time}", "yyyyMMdd HH:mm");
                    if (DateTime.Compare(adminDate, startDate) >= 0)
                    {
                        medicine.StartDate = adminDate;
                        medicine.EndDate = adminDate;
                        ud.Add(medicine.Clone());

                        currentTimes++;
                    }
                }
                startDate = Convert.ToDateTime($"{startDate:yyyy/MM/dd} 00:00:00");
                startDate = startDate.AddDays(1);
            }
        }
    }


    internal class KuangTienOPDBasic
    {
        public string WriteDate { get; set; }
        public string GetMedicineNo { get; set; }
        public string Age { get; set; }
        public string DoctorName { get; set; }
        public string PatientNo { get; set; }
        public string PatientName { get; set; }
        public string Gender { get; set; }
        public string Class { get; set; }
    }

    internal class KuangTienPowder
    {
        public DateTime StartDate { get; set; }
        public string GetMedicineNo { get; set; }
        public string PatientNo { get; set; }
        public string PatientName { get; set; }
        public string Gender { get; set; }
        public string Class { get; set; }
        public string MedicineCode { get; set; }
        public string MedicineName { get; set; }
        public string PerQty { get; set; }
        public string AdminCode { get; set; }
        public string Days { get; set; }
        public string SumQty { get; set; }
        public string GrindTable { get; set; }
        public string TimesPerDay { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string Barcode { get; set; }
    }

    internal class KuangTienUDBasic
    {
        public DateTime TreatmentDate { get; set; }
        public string BedNo { get; set; }
        public string PatientNo { get; set; }
        public string PatientName { get; set; }
        public string Barcode { get; set; }
        public bool Care { get; set; }
    }

    internal class KuangTienUD
    {
        public string MedicineCode { get; set; }
        public string MedicineName { get; set; }
        public float PerQty { get; set; }
        public string AdminCode { get; set; }
        public int Days { get; set; }
        public float SumQty { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime PrintDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public eDoseType DoseType { get; set; }
        public bool CrossDay { get; set; }
        public string TakingDescription { get; set; } = string.Empty;
        public string MedicineSerialNo { get; set; }

        public KuangTienUD Clone()
        {
            return (KuangTienUD)this.MemberwiseClone();
        }
    }

    internal class KuangTienStat
    {

    }

    internal class KuangTienOPD
    {
        public string MedicineCode { get; set; }
        public string MedicineName { get; set; }
        public string AdminCode { get; set; }
        public string PerQty { get; set; }
        public string Days { get; set; }
        public string SumQty { get; set; }
        public string TimesPerDay { get; set; }
    }
}