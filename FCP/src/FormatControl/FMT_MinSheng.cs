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

        public int NewPrescriptionCount
        {
            get => _oledbCount;
            set => _oledbCount = value;
        }

        private Dictionary<string, float> _sumQtyOfMedicine = new Dictionary<string, float>();
        private List<MinShengUDBatch> _batch = new List<MinShengUDBatch>();
        private List<MinShengOPD> _opd = new List<MinShengOPD>();
        private List<string> _filterUnit = new List<string>() { "TUBE", "SUPP", "PCE", "BOT", "BTL", "FLEXPEN", "個", "IU", "適量", "CC" };
        private int _oledbIndex = 1;
        private int _oledbCount = 0;
        private string _location;

        public override void ProcessOPD()
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
                        NewPrescriptionCount = 0;
                        LostMultiAdminCode(v.AdminCode);
                        return;
                    }
                }
                for (int x = neeedRemoveList.Count - 1; x >= 0; x--)
                {
                    _opd.RemoveAt(neeedRemoveList[x]);
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
                NewPrescriptionCount = 0;
                ReadFileFail(ex);

            }
        }

        public override void LogicOPD()
        {
            string outputDirectory = $@"{OutputDirectory}\{ SourceFileNameWithoutExtension}_{CurrentSeconds}.txt";
            try
            {
                OP_OnCube.MinSheng_OPD(_opd, outputDirectory, _location);
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
                OLEDB_UD();
                List<int> needRemoveList = new List<int>();
                foreach (var v in _batch)
                {
                    if (IsFilterMedicineCode(v.MedicineCode) || IsFilterAdminCode(v.AdminCode) || v.Unit == "BAG")
                    {
                        needRemoveList.Add(_batch.IndexOf(v));
                        continue;
                    }
                    if (_sumQtyOfMedicine.Count == 0 || !_sumQtyOfMedicine.ContainsKey($"{v.PrescriptionNo}|{v.MedicineName}{v.StartDate}{v.BeginTime}"))
                    {
                        _sumQtyOfMedicine[$"{v.PrescriptionNo}|{v.MedicineName}{v.StartDate}{v.BeginTime}"] = v.PerQty;
                    }
                    else
                    {
                        _sumQtyOfMedicine[$"{v.PrescriptionNo}|{v.MedicineName}{v.StartDate}{v.BeginTime}"] += v.PerQty;
                        needRemoveList.Add(_batch.IndexOf(v));
                        continue;
                    }
                    if (!IsExistsMultiAdminCode(v.AdminCode))
                    {
                        NewPrescriptionCount = 0;
                        LostMultiAdminCode(v.AdminCode);
                        return;
                    }
                }
                for (int x = needRemoveList.Count - 1; x >= 0; x--)
                {
                    _batch.RemoveAt(needRemoveList[x]);
                }
                if (_batch.Count == 0)
                {
                    NewPrescriptionCount = 0;
                    Pass();
                }
            }
            catch (Exception ex)
            {
                NewPrescriptionCount = 0;
                ReadFileFail(ex);
            }
        }

        public override void LogicUDBatch()
        {
            List<MinShengUDBatch> batch = new List<MinShengUDBatch>();
            try
            {
                int count = _batch.Count;
                List<string> adminCodeTimes = new List<string>();
                for (int i = 0; i <= count - 1; i++)
                {
                    if (CrossDayAdminCode.Contains(_batch[i].AdminCode))
                    {
                        batch.Add(_batch[i].Clone());
                        continue;
                    }
                    int currentTimes = 1;
                    int times = 0;
                    if (_sumQtyOfMedicine.TryGetValue($"{_batch[i].PrescriptionNo}|{_batch[i].MedicineName}{_batch[i].StartDate}{_batch[i].BeginTime}", out float sumQty))
                    {
                        times = Convert.ToInt32(sumQty / Convert.ToSingle(_batch[i].PerQty));
                    }
                    adminCodeTimes = GetMultiAdminCodeTimes(_batch[i].AdminCode);
                    DateTime startDate = DateTimeHelper.Convert($"{_batch[i].StartDate:yyMMdd} {_batch[i].BeginTime}", "yyMMdd HHmm");
                    DateTime startDateTemp = startDate;
                    while (currentTimes <= times)
                    {
                        foreach (var time in adminCodeTimes)
                        {
                            if (currentTimes > times)
                            {
                                break;
                            }
                            DateTime adminDate = DateTimeHelper.Convert($"{startDate:yyMMdd} {time}", "yyMMdd HH:mm");
                            if (DateTime.Compare(adminDate, startDate) >= 0)
                            {
                                _batch[i].StartDate = adminDate;
                                batch.Add(_batch[i].Clone());
                                currentTimes++;
                            }
                        }
                        startDate = Convert.ToDateTime($"{startDate:yyyy/MM/dd} 00:00:00");
                        startDate = startDate.AddDays(1);
                    }
                }
            }
            catch (Exception ex)
            {
                NewPrescriptionCount = 0;
                ProgressLogicFail(ex);
                return;
            }
            try
            {
                string outputDirectory = $@"{OutputDirectory}\{SourceFileNameWithoutExtension}_{CurrentSeconds}.txt";
                OP_OnCube.MinSheng_UD(batch, outputDirectory);
                Success();
            }
            catch (Exception ex)
            {
                NewPrescriptionCount = 0;
                GenerateOCSFileFail(ex);
            }
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

        private void OLEDB_UD()
        {
            try
            {
                _batch = OLEDB.GetMingSheng_UD(InputDirectory, SourceFilePath, Index);
                NewPrescriptionCount = _batch.Count > 0 ? _batch[_batch.Count - 1].RecNo : 0;
            }
            catch (Exception ex)
            {
                LogService.Exception(ex);
                throw ex;
            }
        }

        public override void ProcessCare()
        {
            throw new NotImplementedException();
        }

        public override void LogicCare()
        {
            throw new NotImplementedException();
        }

        private void OLEDB_OPD()
        {
            try
            {
                _opd = OLEDB.GetMingSheng_OPD(InputDirectory, SourceFilePath, Index);
                NewPrescriptionCount = _opd.Count > 0 ? _opd[_opd.Count - 1].RecNo + 1 : 0;
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                throw ex;
            }
        }

        public override ReturnsResultModel DepartmentShunt()
        {
            NewPrescriptionCount = 0;
            _batch.Clear();
            _opd.Clear();
            _sumQtyOfMedicine.Clear();

            _location = FileInfoModel.Department == eDepartment.OPD || FileInfoModel.Department == eDepartment.Care ? "門診" : "大寮";
            if (FileInfoModel.Department == eDepartment.Care || FileInfoModel.Department == eDepartment.Other)
            {
                FileInfoModel.Department = eDepartment.OPD;
            }
            return base.DepartmentShunt();
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
        public float PerQty { get; set; }
        public string AdminCode { get; set; }
        public string Description { get; set; }
        public float SumQty { get; set; }
        public DateTime StartDate { get; set; }
        public string BeginTime { get; set; }
        public string Unit { get; set; }
        public eDoseType DoseType { get; set; } = eDoseType.餐包;

        public MinShengUDBatch Clone()
        {
            return (MinShengUDBatch)this.MemberwiseClone();
        }
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
        public float PerQty { get; set; }
        public string Unit { get; set; }
        public string AdminCode { get; set; }
        public int Days { get; set; }
        public float SumQty { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string BeginTime { get; set; }
    }
}