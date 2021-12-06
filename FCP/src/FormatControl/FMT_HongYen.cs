using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using FCP.src.Enum;
using FCP.MVVM.Models;
using Helper;

namespace FCP.src.FormatControl
{
    class FMT_HongYen : FormatCollection
    {
        private HongYenOPDBasic _Basic { get; set; }
        private List<HongYenOPD> _OPD = new List<HongYenOPD>();
        private int SpaceIndex = 0;

        public override bool ProcessOPD()
        {
            try
            {
                string content = GetContent;
                int jvmPosition = content.IndexOf("|JVPEND||JVMHEAD|");
                string FileName = Path.GetFileNameWithoutExtension(FilePath);
                EncodingHelper.SetBytes(content.Substring(9, jvmPosition - 9));
                _Basic.PatientNo = EncodingHelper.GetString(1, 15);
                _Basic.PrescriptionNo = EncodingHelper.GetString(16, 20);
                _Basic.Age = EncodingHelper.GetString(36, 5);
                _Basic.ID = EncodingHelper.GetString(54, 10);
                _Basic.BirthDate = EncodingHelper.GetString(94, 8);
                _Basic.PatientName = EncodingHelper.GetString(177, 20).Replace("?", " ");
                _Basic.Gender = EncodingHelper.GetString(197, 2);
                _Basic.HospitalName = EncodingHelper.GetString(229, 40);
                _Basic.LocationName = EncodingHelper.GetString(229, 30);

                EncodingHelper.SetBytes(content.Substring(jvmPosition + 17, content.Length - 17 - jvmPosition));
                List<string> list = SeparateString((EncodingHelper.GetString(0, EncodingHelper.Length)), 547);
                SpaceIndex = 0;
                foreach (string s in list)
                {
                    EncodingHelper.SetBytes(s);
                    string adminCode = EncodingHelper.GetString(66, 10);
                    string medicineCode = EncodingHelper.GetString(1, 15);
                    string medicineName = EncodingHelper.GetString(16, 50);
                    if (medicineName == "磨粉.")
                    {
                        ReturnsResult.Shunt(eConvertResult.全數過濾, null);
                        return false;
                    }
                    if (medicineCode.Length == 0 || (int.TryParse(medicineCode, out int i) && medicineCode.Length <= 4))
                    {
                        SpaceIndex = SpaceIndex == 0 ? _OPD.Count : SpaceIndex;
                        continue;
                    }
                    if (IsFilterMedicineCode(medicineCode) || IsFilterAdminCode(adminCode))
                        continue;
                    _OPD.Add(new HongYenOPD()
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

        public override bool LogicOPD()
        {
            try
            {
                List<int> order = new List<int>();
                List<string> FileNameOutputCount = new List<string>();
                List<string> OnCubeRandom = new List<string>();
                List<int> up = new List<int>();
                List<int> downList = new List<int>();
                List<HongYenOPD> OPDUp = new List<HongYenOPD>();
                List<HongYenOPD> OPDDown = new List<HongYenOPD>();
                OPDUp = SpaceIndex > 0 ? _OPD.Where(x => _OPD.IndexOf(x) < SpaceIndex).ToList() : _OPD;
                OPDDown = SpaceIndex > 0 ? _OPD.Where(x => _OPD.IndexOf(x) >= SpaceIndex & x.MedicineCode.Length > 0).ToList() : OPDDown;
                #region 排除天數重複2以下(不含)的
                var daysCount = from days in OPDUp.Select(x => x.Days)
                                group days by days into g
                                where g.Count() >= 2
                                select new
                                {
                                    g.Key,
                                    Num = g.Count()
                                };
                OPDUp = OPDUp.Where(x => daysCount.Select(y => y.Key).ToList().Contains(x.Days)).ToList();
                if (OPDDown.Count > 1)
                {
                    daysCount = from days in OPDDown.Select(x => x.Days)
                                group days by days into g
                                where g.Count() >= 2
                                select new
                                {
                                    g.Key,
                                    Num = g.Count()
                                };
                    OPDDown = OPDDown.Where(x => daysCount.Select(y => y.Key).ToList().Contains(x.Days)).ToList();
                }
                #endregion
                #region 排除頻率重複2以下(不含)的
                var adminCodesCount = from s in OPDUp.Select(x => x.AdminCode)
                                      group s by s into g
                                      where g.Count() >= 2
                                      select new
                                      {
                                          g.Key,
                                          Num = g.Count()
                                      };
                OPDUp = OPDUp.Where(x => adminCodesCount.Select(y => y.Key).ToList().Contains(x.AdminCode)).ToList();

                if (OPDDown.Count > 1)
                {
                    adminCodesCount = from s in OPDDown.Select(x => x.AdminCode)
                                      group s by s into g
                                      where g.Count() >= 2
                                      select new
                                      {
                                          g.Key,
                                          Num = g.Count()
                                      };
                    OPDDown = OPDDown.Where(x => adminCodesCount.Select(y => y.Key).ToList().Contains(x.AdminCode)).ToList();
                }
                #endregion
                #region 排除若有藥品代碼為A037598116，並且總品項數量為2
                if (OPDUp.Where(x => x.MedicineCode == "A037598116").Count() > 0 && OPDUp.Count <= 2)
                {
                    ReturnsResult.Shunt(eConvertResult.全數過濾, null);
                    return false;
                }
                #endregion

                if (OPDUp.Count == 0 & OPDDown.Count <= 1)
                {
                    ReturnsResult.Shunt(eConvertResult.全數過濾, null);
                    return false;
                }
                if (OPDUp.Count == 0 & OPDDown.Count >= 2)
                {
                    OPDUp.AddRange(OPDDown);
                    OPDDown.Clear();
                }
                List<string> filePathOutput = new List<string>();
                filePathOutput.Add($@"{OutputPath}\UP-{_Basic.PatientName}-{Path.GetFileNameWithoutExtension(FilePath)}_{CurrentSeconds}.txt");
                if (OPDDown.Count > 1)
                {
                    filePathOutput.Add($@"{OutputPath}\DOWN-{_Basic.PatientName}-{Path.GetFileNameWithoutExtension(FilePath)}_{CurrentSeconds}.txt");
                }
                DateTime.TryParseExact(_Basic.BirthDate, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime date);  //生日
                _Basic.BirthDate = date.ToString("yyyy-MM-dd");
                try
                {
                    OP_OnCube.HongYen(OPDUp, OPDDown, _Basic, filePathOutput);
                    return true;
                }
                catch (Exception ex)
                {
                    Log.Write($"{FilePath} {ex}");
                    ReturnsResult.Shunt(eConvertResult.產生OCS失敗, ex.ToString());
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.Write($"{FilePath} {ex}");
                ReturnsResult.Shunt(eConvertResult.處理邏輯失敗, ex.ToString());
                return false;
            }
        }

        public override bool ProcessUDBatch()
        {
            throw new NotImplementedException();
        }

        public override bool LogicUDBatch()
        {
            throw new NotImplementedException();
        }

        public override bool ProcessUDStat()
        {
            throw new NotImplementedException();
        }

        public override bool LogicUDStat()
        {
            throw new NotImplementedException();
        }

        public override bool ProcessPOWDER()
        {
            throw new NotImplementedException();
        }

        public override bool LogicPOWDER()
        {
            throw new NotImplementedException();
        }

        public override bool ProcessOther()
        {
            throw new NotImplementedException();
        }

        public override bool LogicOther()
        {
            throw new NotImplementedException();
        }

        public override bool ProcessCare()
        {
            throw new NotImplementedException();
        }

        public override bool LogicCare()
        {
            throw new NotImplementedException();
        }

        public override ReturnsResultFormat MethodShunt()
        {
            _Basic = null;
            _Basic = new HongYenOPDBasic();
            _OPD.Clear();
            return base.MethodShunt();
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
