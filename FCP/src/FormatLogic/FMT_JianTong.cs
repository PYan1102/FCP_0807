using FCP.Models;
using Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace FCP.src.FormatLogic
{
    internal class FMT_JianTong : FormatCollection
    {
        private List<PrescriptionModel> _data = new List<PrescriptionModel>();
        /// <summary>
        /// 展望格式若遇到不磨粉的，只包頻率為QD BID TID QID Q6H Q4H 其他過濾
        /// </summary>
        private List<string> _needToPackAdminCode = new List<string>() { "QD", "BID", "TID", "QID", "Q6H", "Q4H", "Q8H", "HS" };

        /// <summary>
        /// 展望格式
        /// </summary>
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
                    bool isMultiDose = EncodingHelper.GetString(154, 1) == "N";
                    int days = Convert.ToInt32(EncodingHelper.GetString(141, 3));
                    // 為餐包(不磨粉) 並且頻率不在須包出的頻率列表中則過濾
                    if (isMultiDose && !_needToPackAdminCode.Contains(adminCode))
                    {
                        continue;
                    }
                    // 天數 >= 20天不包
                    if (days >= 20)
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
                    };
                    DateTime startDate = DateTimeHelper.Convert((Convert.ToInt32(EncodingHelper.GetString(50, 7)) + 19110000).ToString(), "yyyyMMdd");
                    float sumQty = Convert.ToSingle(EncodingHelper.GetString(144, 10));
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
                        SumQty = isMultiDose ? sumQty : Convert.ToSingle(Math.Ceiling(sumQty)),
                        IsMultiDose = isMultiDose
                    });
                }
                if (_data.Count <= 1)
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
                _data.RemoveAll(x => x.IsMultiDose && x.AdminCode != adminCode);
                LogService.Info(_data.Count);
                if (_data.Count <= 1)
                {
                    Pass();
                    return;
                }
                string outputDirectory = $@"{OutputDirectory}\{_data[0].PatientName}-{SourceFileNameWithoutExtension}_{CurrentSeconds}.txt";
                OP_OnCube.JianTong(_data, outputDirectory);
                Success();
            }
            catch (Exception ex)
            {
                GenerateOCSFileFail(ex);
            }
        }
        /// <summary>
        /// 寶寶格式
        /// </summary>
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
                    List<string> splitDatas = v.Split(',').Select(x => x.Trim()).ToList();
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
                    int days = Convert.ToInt32(splitDatas[3]);
                    float perQty = Convert.ToSingle(splitDatas[4]);
                    float sumQty = Convert.ToSingle(splitDatas[5]);
                    bool isMultiDose = perQty < 1;
                    // 天數 >= 20天不包
                    if (days >= 20)
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
                    model.MedicineCode = medicineCode;
                    model.MedicineName = splitDatas[1];
                    model.Days = days;
                    model.PerQty = perQty;
                    model.SumQty = isMultiDose ? sumQty : Convert.ToSingle(Math.Ceiling(sumQty));
                    model.AdminCode = adminCode;
                    model.EndDate = model.StartDate.AddDays(model.Days - 1);
                    model.IsMultiDose = isMultiDose;
                    _data.Add(model);
                }
                if (_data.Count <= 1)
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
                //總量小數點不包
                for (int i = _data.Count - 1; i >= 0; i--)
                {
                    if (_data[i].SumQty.ToString().Contains("."))
                    {
                        _data.RemoveAt(i);
                    }
                }

                string adminCode = GetMaxTimesAdminCode();
                _data.RemoveAll(x => x.IsMultiDose && x.AdminCode != adminCode);
                if (_data.Count <= 1)
                {
                    Pass();
                    return;
                }
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

        /// <summary>
        /// 取得一天中服用頻率最高的頻率
        /// </summary>
        /// <returns>服用頻率</returns>
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
