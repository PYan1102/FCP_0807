using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using FCP.src.Enum;
using FCP.Models;
using Helper;

namespace FCP.src.FormatControl
{
    class FMT_HongYen : FormatCollection
    {
        private HongYenOPDBasic _basic = new HongYenOPDBasic();
        private List<HongYenOPD> _opd = new List<HongYenOPD>();
        private int _separateIndex = 0;  //紀錄分開上下筆的索引

        public override void ProcessOPD()
        {
            try
            {
                string content = GetFileContent;
                int jvmPosition = content.IndexOf("|JVPEND||JVMHEAD|");
                EncodingHelper.SetBytes(content.Substring(9, jvmPosition - 9));
                _basic.PatientNo = EncodingHelper.GetString(1, 15);
                _basic.PrescriptionNo = EncodingHelper.GetString(16, 20);
                _basic.Age = EncodingHelper.GetString(36, 5);
                _basic.ID = EncodingHelper.GetString(54, 10);
                _basic.BirthDate = DateTimeHelper.Convert(EncodingHelper.GetString(94, 8), "yyyyMMdd").ToString("yyyy-MM-dd");
                _basic.PatientName = EncodingHelper.GetString(177, 20).Replace("?", " ");
                _basic.Gender = EncodingHelper.GetString(197, 2);
                _basic.HospitalName = EncodingHelper.GetString(229, 40);
                _basic.LocationName = EncodingHelper.GetString(229, 30);

                EncodingHelper.SetBytes(content.Substring(jvmPosition + 17, content.Length - 17 - jvmPosition));
                List<string> list = SeparateString(EncodingHelper.GetString(0, EncodingHelper.Length), 547);
                foreach (string s in list)
                {
                    EncodingHelper.SetBytes(s);
                    string adminCode = EncodingHelper.GetString(66, 10);
                    string medicineCode = EncodingHelper.GetString(1, 15);
                    string medicineName = EncodingHelper.GetString(16, 50);
                    if (medicineName == "磨粉.")
                    {
                        Pass();
                        return;
                    }
                    if (medicineCode.Length == 0 || (int.TryParse(medicineCode, out int i) && medicineCode.Length <= 4))
                    {
                        _separateIndex = _separateIndex == 0 ? _opd.Count : _separateIndex;
                        continue;
                    }
                    if (IsFilterMedicineCode(medicineCode) || IsFilterAdminCode(adminCode))
                        continue;
                    _opd.Add(new HongYenOPD()
                    {
                        MedicineCode = medicineCode,
                        MedicineName = medicineName,
                        AdminCode = adminCode,
                        Days = EncodingHelper.GetString(76, 3),
                        PerQty = EncodingHelper.GetString(81, 6),
                        SumQty = EncodingHelper.GetString(87, 8),
                        StartDay = EncodingHelper.GetString(509, 6),
                        EndDay = EncodingHelper.GetString(529, 6)
                    });
                }
                if (_opd.Count == 0)
                {
                    Pass();
                    return;
                }
                Success();
            }
            catch (Exception ex)
            {
                ReadFileFail(ex);
            }
        }

        public override void LogicOPD()
        {
            List<HongYenOPD> opdUp = new List<HongYenOPD>();
            List<HongYenOPD> opdDown = new List<HongYenOPD>();
            List<string> outputDirectory = new List<string>();
            try
            {
                //若分離索引為 0 ，則 opdUp 為整個 _opd ，因為此處方沒有下筆
                opdUp = _separateIndex > 0 ? _opd.Where(x => _opd.IndexOf(x) < _separateIndex).ToList() : _opd;
                opdDown = _separateIndex > 0 ? _opd.Where(x => _opd.IndexOf(x) >= _separateIndex & x.MedicineCode.Length > 0).ToList() : opdDown;

                #region 排除天數重複2以下(不含)的

                opdUp = ExcludeDaysRepeatedLessThan2Times(opdUp);
                if (opdDown.Count > 1)
                {
                    opdDown = ExcludeDaysRepeatedLessThan2Times(opdDown);
                }

                #endregion

                #region 排除頻率重複2以下(不含)的

                opdUp = ExcludeAdminCodeRepeatedLessThan2Times(opdUp);
                if (opdDown.Count > 1)
                {
                    opdDown = ExcludeAdminCodeRepeatedLessThan2Times(opdDown);
                }

                #endregion

                #region 若藥品代碼為A037598116，並且上筆處方總品項數量 <= 2，則整筆過濾

                if (opdUp.Where(x => x.MedicineCode == "A037598116").Count() > 0 && opdUp.Count <= 2)
                {
                    Pass();
                    return;
                }

                #endregion

                //若上筆筆數為 0 ，並下筆筆數 <= 1 ，則整筆過濾
                if (opdUp.Count == 0 && opdDown.Count <= 1)
                {
                    Pass();
                    return;
                }

                //若上筆筆數為 0 ，並下筆筆數 >= 2 ，則將下筆處方移至上筆
                if (opdUp.Count == 0 && opdDown.Count >= 2)
                {
                    opdUp.AddRange(opdDown);
                    opdDown.Clear();
                }

                outputDirectory.Add($@"{OutputDirectory}\UP-{_basic.PatientName}-{SourceFileNameWithoutExtension}_{CurrentSeconds}.txt");
                if (opdDown.Count > 1)
                {
                    outputDirectory.Add($@"{OutputDirectory}\DOWN-{_basic.PatientName}-{SourceFileNameWithoutExtension}_{CurrentSeconds}.txt");
                }
            }
            catch (Exception ex)
            {
                ProgressLogicFail(ex);
                return;
            }
            try
            {
                OP_OnCube.HongYen(opdUp, opdDown, _basic, outputDirectory);
                Success();
            }
            catch (Exception ex)
            {
                GenerateOCSFileFail(ex);
            }
        }

        private List<HongYenOPD> ExcludeDaysRepeatedLessThan2Times(List<HongYenOPD> list)
        {
            var daysCount = from days in list.Select(x => x.Days)
                            group days by days into g
                            where g.Count() >= 2
                            select new
                            {
                                g.Key,
                                Num = g.Count()
                            };
            list.Where(x => daysCount.Select(y => y.Key).ToList().Contains(x.Days)).ToList();
            return list;
        }

        private List<HongYenOPD> ExcludeAdminCodeRepeatedLessThan2Times(List<HongYenOPD> list)
        {
            var adminCodeCount = from s in list.Select(x => x.AdminCode)
                                 group s by s into g
                                 where g.Count() >= 2
                                 select new
                                 {
                                     g.Key,
                                     Num = g.Count()
                                 };
            return list.Where(x => adminCodeCount.Select(y => y.Key).ToList().Contains(x.AdminCode)).ToList();
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
            _basic = null;
            _basic = new HongYenOPDBasic();
            _opd.Clear();
            _separateIndex = 0;
            return base.DepartmentShunt();
        }
    }

    internal class HongYenOPDBasic
    {
        public string PatientName { get; set; }
        public string PatientNo { get; set; }
        public string PrescriptionNo { get; set; }
        public string Age { get; set; }
        public string ID { get; set; }
        public string Gender { get; set; }
        public string BirthDate { get; set; }
        public string LocationName { get; set; }
        public string HospitalName { get; set; }
    }

    internal class HongYenOPD
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
