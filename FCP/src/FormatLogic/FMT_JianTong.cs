using FCP.Models;
using Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
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
                        Days = days,
                        SumQty = Convert.ToSingle(EncodingHelper.GetString(144, 10)),
                        IsMultiDose = EncodingHelper.GetString(154, 1) == "N"
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
                string adminCode = GetMaxTimesAdminCode();
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

        public override void ProcessPowder()
        {
            ProcessOPD();
        }

        public override void LogicPowder()
        {
            LogicOPD();
        }

        public override void ProcessCare()
        {
            try
            {
                var list = Regex.Split(GetFileContent, "\r").Where(x => x.Trim().Length > 0).Select(x => x.Trim()).ToList();
                var basicModel = new PrescriptionModel();
                foreach (var v in list)
                {
                    if (v.StartsWith("CCPE"))
                    {
                        continue;
                    }
                    string[] splitDatas = v.Split(',');
                    if (list.IndexOf(v) == 0)
                    {
                        basicModel.StartDate = DateTimeHelper.Convert($"{Convert.ToInt32(splitDatas[1]) + 19110000}", "yyyyMMdd");
                        basicModel.HospitalName = splitDatas[8];
                        basicModel.PatientName = splitDatas[13];
                        continue;
                    }
                    var model = new PrescriptionModel();
                    model = basicModel.Clone();
                    string adminCode = splitDatas[2];
                    string medicineCode = splitDatas[0];
                    if (FilterRule(adminCode, medicineCode))
                    {
                        continue;
                    }
                    if (WhetherToStopNotHasMultiAdminCode(adminCode))
                    {
                        return;
                    }
                    model.MedicineCode = medicineCode;
                    model.MedicineName = splitDatas[1];
                    model.Days = Convert.ToInt32(splitDatas[3]);
                    model.PerQty = Convert.ToSingle(splitDatas[4]);
                    model.SumQty = Convert.ToSingle(splitDatas[5]);
                    model.AdminCode = adminCode;
                    model.EndDate = model.StartDate.AddDays(model.Days - 1);
                    _data.Add(model);
                    //LogService.Info(model.PerQty.ToString().Contains("."));
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

        public override void LogicCare()
        {
            try
            {
                string adminCode = GetMaxTimesAdminCode();
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

        private string GetMaxTimesAdminCode()
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
            return adminCode;
        }
    }
}
