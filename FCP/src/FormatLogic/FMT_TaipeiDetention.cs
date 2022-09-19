using System;
using System.Collections.Generic;
using System.Text;
using FCP.src.Enum;
using FCP.Models;
using Helper;

namespace FCP.src.FormatLogic
{
    class FMT_TaipeiDetention : FormatCollection
    {
        private TaipeiDetentionOPDBasic _basic = new TaipeiDetentionOPDBasic();
        private List<TaipeiDetentionOPD> _opd = new List<TaipeiDetentionOPD>();

        public override void ProcessOPD()
        {
            try
            {
                string content = GetFileContent.Trim();
                int jvmPosition = content.IndexOf("|JVPEND||JVMHEAD|");
                EncodingHelper.SetBytes(content.Substring(9, jvmPosition - 9));
                var ecd = Encoding.Default;
                _basic.PatientNo = EncodingHelper.GetString(1, 15);
                _basic.PrescriptionNo = EncodingHelper.GetString(16, 20);
                _basic.Age = EncodingHelper.GetString(36, 5);
                _basic.ID = EncodingHelper.GetString(54, 10);
                _basic.BirthDate = DateTimeHelper.Convert(EncodingHelper.GetString(94, 8), "yyyyMMdd").ToString("yyyy-MM-dd");
                _basic.Class = EncodingHelper.GetString(132, 30);
                _basic.PatientName = EncodingHelper.GetString(177, 20);
                if (_basic.PatientName.Contains("?"))
                    _basic.PatientName = _basic.PatientName.Replace("?", " ");
                _basic.Gender = EncodingHelper.GetString(197, 2);
                _basic.HospitalName = EncodingHelper.GetString(229, 40);
                _basic.LocationName = EncodingHelper.GetString(229, 30);

                EncodingHelper.SetBytes(content.Substring(jvmPosition + 17, content.Length - 17 - jvmPosition));
                List<string> list = SeparateString(EncodingHelper.GetString(0, EncodingHelper.Length), 106);  //計算有多少種藥品資料
                foreach (string s in list)  //將藥品資料放入List<string>
                {
                    EncodingHelper.SetBytes(s);
                    string adminCode = EncodingHelper.GetString(66, 10).Trim().Replace(" ", "").ToUpper();
                    string medicineCode = EncodingHelper.GetString(1, 15);
                    if (FilterRule(adminCode, medicineCode))
                    {
                        continue;
                    }
                    if (WhetherToStopNotHasMultiAdminCode(adminCode))
                    {
                        return;
                    }
                    _opd.Add(new TaipeiDetentionOPD()
                    {
                        MedicineCode = medicineCode,
                        MedicineName = EncodingHelper.GetString(16, 50),
                        AdminCode = adminCode,
                        Days = EncodingHelper.GetString(76, 3),
                        PerQty = EncodingHelper.GetString(81, 6),
                        SumQty = EncodingHelper.GetString(87, 8)
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
            string outputDirectory = $@"{OutputDirectory}\{_basic.PatientName}-{_basic.PatientNo}-{_basic.Class}_{CurrentSeconds}.txt";
            List<string> putBackAdminCode = new List<string>() { "Q4H", "Q6H", "Q8H", "Q12H", "QDPRN", "QIDPRN", "PRN", "BIDPRN", "TIDPRN", "HSPRN" };
            try
            {
                OP_OnCube.TaipeiDentention(_opd, _basic, outputDirectory, putBackAdminCode);
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
            _basic = null;
            _basic = new TaipeiDetentionOPDBasic();
            _opd.Clear();
            return base.DepartmentShunt();
        }
    }
    internal class TaipeiDetentionOPDBasic
    {
        public string PatientName { get; set; }
        public string PatientNo { get; set; }
        public string PrescriptionNo { get; set; }
        public string Class { get; set; }
        public string Age { get; set; }
        public string ID { get; set; }
        public string Gender { get; set; }
        public string BirthDate { get; set; }
        public string LocationName { get; set; }
        public string HospitalName { get; set; }
    }

    internal class TaipeiDetentionOPD
    {
        public string MedicineCode { get; set; }
        public string MedicineName { get; set; }
        public string PerQty { get; set; }
        public string AdminCode { get; set; }
        public string Days { get; set; }
        public string SumQty { get; set; }
        public string BedNo { get; set; }
    }
}
