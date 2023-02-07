using FCP.Models;
using Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace FCP.src.FormatLogic
{
    internal class FMT_JianTong : FormatCollection
    {
        private List<PrescriptionModel> _data = new List<PrescriptionModel>();

        public override void ProcessOPD()
        {
            try
            {
                var list = Regex.Split(GetFileContent, "\r").Where(x => x.Length > 0).ToList();
                foreach (var v in list)
                {
                    EncodingHelper.SetBytes(v);
                    string adminCode = EncodingHelper.GetString(131, 10);
                    string medicineCode = EncodingHelper.GetString(57, 10);
                    if (FilterRule(adminCode, medicineCode))
                    {
                        continue;
                    }
                    if (WhetherToStopNotHasMultiAdminCode(adminCode))
                    {
                        return;
                    }
                    DateTime startDate = DateTimeHelper.Convert((Convert.ToInt32(EncodingHelper.GetString(50, 7)) + 19110000).ToString(), "yyyyMMdd");
                    int days = Convert.ToInt32(EncodingHelper.GetString(141, 3));
                    _data.Add(new PrescriptionModel
                    {
                        PatientName = EncodingHelper.GetString(0, 20),
                        PrescriptionNo = EncodingHelper.GetString(20, 10),
                        LocationName = "一般",
                        HospitalName = "健通藥局",
                        StartDate = startDate,
                        EndDate = startDate.AddDays(days - 1),
                        MedicineCode = medicineCode,
                        MedicineName = EncodingHelper.GetString(67, 50),
                        Unit = EncodingHelper.GetString(117, 4),
                        PerQty = Convert.ToSingle(EncodingHelper.GetString(121, 10)),
                        AdminCode = adminCode,
                        Days = days
                    });
                }
                if (_data.Count == 0 || _data.Count == 1)
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
                var adminCodes = from v in _data
                                 group v by v.AdminCode into g
                                 select new
                                 {
                                     g.Key,
                                     Count = g.Count()
                                 };
                int maxTimes = 0;
                string adminCode = "";
                foreach (var v in adminCodes)
                {
                    int times = GetMultiAdminCodeTimes(v.Key).Count;
                    if (maxTimes == 0 || times > maxTimes)
                    {
                        maxTimes = times;
                        adminCode = v.Key;
                    }
                }
                _data.RemoveAll(x => x.AdminCode != adminCode);
                string outputDirectory = $@"{OutputDirectory}\{_data[0].PatientName}-{SourceFileNameWithoutExtension}_{CurrentSeconds}.txt";
                OP_OnCube.JianTong(_data, outputDirectory);
                Success();
            }
            catch (Exception ex)
            {
                GenerateOCSFileFail(ex);
            }
        }

        public override void ProcessCare()
        {
            ProcessOPD();
        }

        public override void ProcessPowder()
        {
            ProcessOPD();
        }

        public override void LogicCare()
        {
            LogicOPD();
        }

        public override void LogicPowder()
        {
            LogicOPD();
        }

        public override void LogicOther()
        {
            throw new System.NotImplementedException();
        }

        public override void LogicUDBatch()
        {
            throw new System.NotImplementedException();
        }

        public override void LogicUDStat()
        {
            throw new System.NotImplementedException();
        }

        public override void ProcessOther()
        {
            throw new System.NotImplementedException();
        }

        public override void ProcessUDBatch()
        {
            throw new System.NotImplementedException();
        }

        public override void ProcessUDStat()
        {
            throw new System.NotImplementedException();
        }

        public override ReturnsResultModel DepartmentShunt()
        {
            _data.Clear();
            return base.DepartmentShunt();
        }
    }
}
