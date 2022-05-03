using System;
using System.Collections.Generic;
using FCP.src.Enum;
using FCP.Models;
using FCP.src.SQL;
using Helper;

namespace FCP.src.FormatControl
{
    class FMT_MinSheng : FormatCollection
    {
        public int Index
        {
            get => _oledbIndex;
            set => _oledbIndex = value;
        }

        public int NewCount
        {
            get => _oledbCount;
            set => _oledbCount = value;
        }

        private Dictionary<string, decimal> _sumQtyOfMedicine = new Dictionary<string, decimal>();
        private List<MinShengUDBatch> _batch = new List<MinShengUDBatch>();
        private List<MinShengOPD> _opd = new List<MinShengOPD>();
        private List<string> _filterUnit = new List<string>() { "TUBE", "SUPP", "PCE", "BOT", "BTL", "FLEXPEN", "個", "IU", "適量", "CC" };
        private int _oledbIndex = 1;
        private int _oledbCount = 0;
        private string _location;

        public override bool ProcessOPD()
        {
            try
            {
                OLEDB_OPD();
                List<int> neeedRemoveList = new List<int>();
                foreach (var v in _opd)
                {
                    if (IsFilterMedicineCode(v.MedicineCode) || IsFilterAdminCode(v.AdminCode) || _filterUnit.Contains(v.Unit))
                    {
                        neeedRemoveList.Add(_opd.IndexOf(v));
                        continue;
                    }
                    if (!IsExistsMultiAdminCode(v.AdminCode))
                    {
                        NewCount = 0;
                        ReturnsResult.Shunt(eConvertResult.缺少餐包頻率, v.AdminCode);
                        return false;
                    }
                }
                for (int x = neeedRemoveList.Count - 1; x >= 0; x--)
                {
                    _opd.RemoveAt(neeedRemoveList[x]);
                }
                if (_opd.Count == 0)
                {
                    ReturnsResult.Shunt(eConvertResult.全數過濾);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                NewCount = 0;
                ReturnsResult.Shunt(eConvertResult.讀取檔案失敗, ex);
                return false;

            }
        }

        public override bool LogicOPD()
        {
            string outputDirectory = $@"{OutputDirectory}\{ SourceFileNameWithoutExtension}_{CurrentSeconds}.txt";
            try
            {
                OP_OnCube.MinSheng_OPD(_opd, outputDirectory, _location);
                return true;
            }
            catch (Exception ex)
            {
                ReturnsResult.Shunt(eConvertResult.產生OCS失敗, ex);
                return false;
            }
        }

        public override bool ProcessUDBatch()
        {
            try
            {
                OLEDB_UD();
                List<int> needRemoveList = new List<int>();
                foreach (var v in _batch)
                {
                    if (IsFilterMedicineCode(v.MedicineCode) || IsFilterAdminCode(v.AdminCode) || v.Unit == "BAG")
                    {
                        needRemoveList.Add(_batch.IndexOf(v));
                        continue;
                    }
                    if (_sumQtyOfMedicine.Count == 0 || !_sumQtyOfMedicine.ContainsKey($"{v.PrescriptionNo}|{v.MedicineName}{v.StartDay}{v.BeginTime}"))
                    {
                        _sumQtyOfMedicine[$"{v.PrescriptionNo}|{v.MedicineName}{v.StartDay}{v.BeginTime}"] = decimal.Parse(v.PerQty);
                    }
                    else
                    {
                        _sumQtyOfMedicine[$"{v.PrescriptionNo}|{v.MedicineName}{v.StartDay}{v.BeginTime}"] += decimal.Parse(v.PerQty);
                        needRemoveList.Add(_batch.IndexOf(v));
                        continue;
                    }
                    if (!IsExistsMultiAdminCode(v.AdminCode))
                    {
                        NewCount = 0;
                        ReturnsResult.Shunt(eConvertResult.缺少餐包頻率, v.AdminCode);
                        return false;
                    }
                }
                for (int x = needRemoveList.Count - 1; x >= 0; x--)
                {
                    _batch.RemoveAt(needRemoveList[x]);
                }
                if (_batch.Count == 0)
                {
                    NewCount = 0;
                    ReturnsResult.Shunt(eConvertResult.全數過濾);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                NewCount = 0;
                ReturnsResult.Shunt(eConvertResult.讀取檔案失敗, ex);
                return false;
            }
        }

        public override bool LogicUDBatch()
        {
            string outputDirectory = $@"{OutputDirectory}\{SourceFileNameWithoutExtension}_{CurrentSeconds}.txt";
            try
            {
                int count = _batch.Count;
                int times = 0;
                int currentTimes = 0;
                List<string> adminCodeList = new List<string>();
                for (int i = 0; i <= count - 1; i++)
                {
                    if (CrossDayAdminCode.Contains(_batch[i].AdminCode))
                    {
                        DataDic[$"{i}_{_batch[i].StartDay}"] = new List<string>() { nameof(eDoseType.種包) };
                        continue;
                    }
                    currentTimes = 0;
                    if (_sumQtyOfMedicine.TryGetValue($"{_batch[i].PrescriptionNo}|{_batch[i].MedicineName}{_batch[i].StartDay}{_batch[i].BeginTime}", out decimal sumQty))
                    {
                        times = (int)(sumQty / Convert.ToDecimal(_batch[i].PerQty));
                    }
                    adminCodeList = GetMultiAdminCodeTimes(_batch[i].AdminCode);
                    DateTime startDate = DateTimeHelper.Convert($"{_batch[i].StartDay} {_batch[i].BeginTime}", "yyMMdd HHmm");
                    DateTime startDateTemp = startDate;
                    while (currentTimes < times)
                    {
                        DataDic[$"{i}_{startDateTemp:yyMMdd}"] = new List<string>();
                        foreach (string time in adminCodeList)
                        {
                            if (currentTimes == times)
                                break;
                            DateTime adminDate = DateTimeHelper.Convert($"{_batch[i].StartDay} {time}", "yyMMdd HH:mm");
                            if (DateTime.Compare(startDate, adminDate) <= 0 & startDateTemp == startDate)
                            {
                                DataDic[$"{i}_{startDateTemp:yyMMdd}"].Add(time.Substring(0, 2));
                                currentTimes++;
                                continue;
                            }
                            if (startDate.CompareTo(startDateTemp) == -1)
                            {
                                DataDic[$"{i}_{startDateTemp:yyMMdd}"].Add(time.Substring(0, 2));
                                currentTimes++;
                                continue;
                            }
                        }
                        startDateTemp = Convert.ToDateTime($"{startDateTemp:yyyy/MM/dd} 00:00:00");
                        startDateTemp = startDateTemp.AddDays(1);
                    }
                }

            }
            catch (Exception ex)
            {
                NewCount = 0;
                ReturnsResult.Shunt(eConvertResult.處理邏輯失敗, ex);
                return false;
            }
            try
            {
                OP_OnCube.MinSheng_UD(DataDic, outputDirectory, _batch);
                return true;
            }
            catch (Exception ex)
            {
                NewCount = 0;
                ReturnsResult.Shunt(eConvertResult.產生OCS失敗, ex);
                return false;
            }
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

        private void OLEDB_UD()
        {
            try
            {
                _batch = OLEDB.GetMingSheng_UD(InputDirectory, SourceFilePath, Index);
                NewCount = _batch.Count > 0 ? _batch[_batch.Count - 1].RecNo : 0;
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                throw ex;
            }
        }

        public override bool ProcessCare()
        {
            throw new NotImplementedException();
        }

        public override bool LogicCare()
        {
            throw new NotImplementedException();
        }

        private void OLEDB_OPD()
        {
            try
            {
                _opd = OLEDB.GetMingSheng_OPD(InputDirectory, SourceFilePath, Index);
                NewCount = _opd.Count > 0 ? _opd[_opd.Count - 1].RecNo + 1 : 0;
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                throw ex;
            }
        }

        private void ClearList()
        {
            DataDic.Clear();
            _sumQtyOfMedicine.Clear();
        }

        public override ReturnsResultModel MethodShunt()
        {
            NewCount = 0;
            _batch.Clear();
            _opd.Clear();
            ClearList();
            _location = FileInfoModel.Department == eDepartment.OPD || FileInfoModel.Department == eDepartment.Care ? "門診" : "大寮";
            if (FileInfoModel.Department == eDepartment.Care || FileInfoModel.Department == eDepartment.Other)
            {
                FileInfoModel.Department = eDepartment.OPD;
            }
            return base.MethodShunt();
        }
    }


    internal class MinShengUDBatch
    {
        public int RecNo { get; set; }
        public string PrescriptionNo { get; set; }
        public string BedNo { get; set; }
        public string PatientName { get; set; }
        public string MedicineCode { get; set; }
        public string MedicineName { get; set; }
        public string PerQty { get; set; }
        public string AdminCode { get; set; }
        public string Description { get; set; }
        public string SumQty { get; set; }
        public string StartDay { get; set; }
        public string BeginTime { get; set; }
        public string Unit { get; set; }
    }

    internal class MinShengOPD
    {
        public int RecNo { get; set; }
        public string PrescriptionNo { get; set; }
        public string PatientName { get; set; }
        public string Age { get; set; }
        public string DrugNo { get; set; }
        public string MedicineCode { get; set; }
        public string MedicineName { get; set; }
        public string PerQty { get; set; }
        public string Unit { get; set; }
        public string AdminCode { get; set; }
        public string Days { get; set; }
        public string SumQty { get; set; }
        public string StartDay { get; set; }
        public string BeginTime { get; set; }
        public string EndDay { get; set; }
    }
}