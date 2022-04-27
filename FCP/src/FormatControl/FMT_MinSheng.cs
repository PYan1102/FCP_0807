using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Globalization;
using FCP.src.Enum;
using FCP.Models;
using FCP.src.SQL;
using Helper;

namespace FCP.src.FormatControl
{
    class FMT_MinSheng : FormatCollection
    {
        private Dictionary<string, decimal> _SumQtyOfMedicine = new Dictionary<string, decimal>();
        private List<MinShengUDBatch> _UDBatch = new List<MinShengUDBatch>();
        private List<MinShengOPD> _OPD = new List<MinShengOPD>();
        private List<string> _FilterUnit = new List<string>() { "TUBE", "SUPP", "PCE", "BOT", "BTL", "FLEXPEN", "個", "IU", "適量", "CC" };
        private int _OLEDBIndex = 1;
        private int _OLEDBCount = 0;
        private string location { get; set; }
        public int Index
        {
            get { return _OLEDBIndex; }
            set { _OLEDBIndex = value; }
        }
        public int newCount
        {
            get { return _OLEDBCount; }
            set { _OLEDBCount = value; }
        }

        public override ReturnsResultFormat MethodShunt()
        {
            newCount = 0;
            _UDBatch.Clear();
            _OPD.Clear();
            location = base.ConvertFileInformation.GetDepartment == eConvertLocation.OPD || base.ConvertFileInformation.GetDepartment == eConvertLocation.Care ? "門診" : "大寮";
            if (base.ConvertFileInformation.GetDepartment == eConvertLocation.Care || base.ConvertFileInformation.GetDepartment == eConvertLocation.Other)
            {
                base.ConvertFileInformation.SetDepartment(eConvertLocation.OPD);
            }
            return base.MethodShunt();
        }

        public override bool ProcessOPD()
        {
            try
            {
                OLEDB_OPD();
                List<int> neeedRemoveList = new List<int>();
                foreach (var v in _OPD)
                {
                    if (IsFilterMedicineCode(v.MedicineCode) || IsFilterAdminCode(v.AdminCode) || _FilterUnit.Contains(v.Unit))
                    {
                        neeedRemoveList.Add(_OPD.IndexOf(v));
                        continue;
                    }
                    //if (!int.TryParse(float.Parse(v.PerQty).ToString("0.##"), out int i))  //劑量為小數點不包
                    //{
                    //    Remove.Add(MS_OPD.IndexOf(v));
                    //    continue;
                    //}
                    if (!IsExistsMultiAdminCode(v.AdminCode))
                    {
                        newCount = 0;
                        Log.Write($"{FilePath} 在OnCube中未建置此餐包頻率 {v.AdminCode}");
                        ReturnsResult.Shunt(eConvertResult.沒有餐包頻率, v.AdminCode);
                        return false;
                    }
                }
                for (int x = neeedRemoveList.Count - 1; x >= 0; x--)
                {
                    _OPD.RemoveAt(neeedRemoveList[x]);
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
                newCount = 0;
                ReturnsResult.Shunt(eConvertResult.讀取檔案失敗, ex.ToString());
                return false;

            }
        }

        public override bool LogicOPD()
        {
            int count = _OPD.Count;
            string filePathOutput = $@"{OutputPath}\{ Path.GetFileNameWithoutExtension(FilePath)}_{CurrentSeconds}.txt";
            try
            {
                OP_OnCube.MinSheng_OPD(_OPD, filePathOutput, location);
                return true;
            }
            catch (Exception ex)
            {
                Log.Write($"{FilePath} {ex}");
                ReturnsResult.Shunt(eConvertResult.產生OCS失敗, ex.ToString());
                return false;
            }
        }

        public override bool ProcessUDBatch()
        {
            try
            {
                OLEDB_UD();
                ClearList();
                List<int> needRemoveList = new List<int>();
                foreach (var v in _UDBatch)
                {
                    if (IsFilterMedicineCode(v.MedicineCode) || IsFilterAdminCode(v.AdminCode) || v.Unit == "BAG")
                    {
                        needRemoveList.Add(_UDBatch.IndexOf(v));
                        continue;
                    }
                    //if (!int.TryParse(float.Parse(v.Dosage).ToString("0.##"), out int i))  //劑量為小數點不包
                    //{
                    //    Remove.Add(MS_UD.IndexOf(v));
                    //    continue;
                    //}
                    //Debug.WriteLine($"{v.PrescriptionNo}|{v.MedicineName}{v.StartDay}{v.BeginTime}");
                    if (_SumQtyOfMedicine.Count == 0 || !_SumQtyOfMedicine.ContainsKey($"{v.PrescriptionNo}|{v.MedicineName}{v.StartDay}{v.BeginTime}"))
                    {
                        //Debug.WriteLine("New");
                        _SumQtyOfMedicine[$"{v.PrescriptionNo}|{v.MedicineName}{v.StartDay}{v.BeginTime}"] = decimal.Parse(v.PerQty);
                    }
                    else
                    {
                        //Debug.WriteLine("Old");
                        _SumQtyOfMedicine[$"{v.PrescriptionNo}|{v.MedicineName}{v.StartDay}{v.BeginTime}"] += decimal.Parse(v.PerQty);
                        needRemoveList.Add(_UDBatch.IndexOf(v));
                        continue;
                    }
                    if (!IsExistsMultiAdminCode(v.AdminCode))
                    {
                        newCount = 0;
                        Log.Write($"{FilePath} 在OnCube中未建置此餐包頻率 {v.AdminCode}");
                        ReturnsResult.Shunt(eConvertResult.沒有餐包頻率, v.AdminCode);
                        return false;
                    }
                }
                for (int x = needRemoveList.Count - 1; x >= 0; x--)
                {
                    _UDBatch.RemoveAt(needRemoveList[x]);
                }
                if (_UDBatch.Count == 0)
                {
                    newCount = 0;
                    ReturnsResult.Shunt(eConvertResult.全數過濾, null);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Write($"{FilePath} {ex}");
                newCount = 0;
                ReturnsResult.Shunt(eConvertResult.讀取檔案失敗, ex.ToString());
                return false;
            }
        }

        public override bool LogicUDBatch()
        {
            string filePathOutput = $@"{OutputPath}\{Path.GetFileNameWithoutExtension(FilePath)}_{CurrentSeconds}.txt";
            try
            {
                int count = _UDBatch.Count;
                int times = 0;
                int currentTimes = 0;
                List<string> adminCodeList = new List<string>();
                for (int i = 0; i <= count - 1; i++)
                {
                    if (SettingsModel.CrossDayAdminCode.Contains(_UDBatch[i].AdminCode))
                    {
                        DataDic[$"{i}_{_UDBatch[i].StartDay}"] = new List<string>() { nameof(eDoseType.種包) };
                        continue;
                    }
                    currentTimes = 0;
                    if (_SumQtyOfMedicine.TryGetValue($"{_UDBatch[i].PrescriptionNo}|{_UDBatch[i].MedicineName}{_UDBatch[i].StartDay}{_UDBatch[i].BeginTime}", out decimal sumQty))
                    {
                        times = (int)(sumQty / Convert.ToDecimal(_UDBatch[i].PerQty));
                    }
                    adminCodeList = GetMultiAdminCodeTimes(_UDBatch[i].AdminCode);
                    DateTime.TryParseExact($"{_UDBatch[i].StartDay} {_UDBatch[i].BeginTime}", "yyMMdd HHmm", null, DateTimeStyles.None, out DateTime startTime);
                    DateTime dateTemp = startTime;
                    //Debug.WriteLine($"{MedicineName_L[i]} {AdminTime_L[i]}");
                    //Debug.WriteLine($"日期 {DateTemp}");
                    while (currentTimes < times)
                    {
                        DataDic[$"{i}_{dateTemp:yyMMdd}"] = new List<string>();
                        foreach (string s in adminCodeList)
                        {
                            if (currentTimes == times)
                                break;
                            DateTime.TryParseExact($"{_UDBatch[i].StartDay} {s}", "yyMMdd HH:mm", null, DateTimeStyles.None, out DateTime adminTime);
                            //Debug.WriteLine($"時間點 {s}");
                            if (DateTime.Compare(startTime, adminTime) <= 0 & dateTemp == startTime)
                            {
                                //Debug.WriteLine($"符合 {s}");
                                DataDic[$"{i}_{dateTemp:yyMMdd}"].Add(s.Substring(0, 2));
                                currentTimes++;
                                continue;
                            }
                            if (startTime.CompareTo(dateTemp) == -1)
                            {
                                //Debug.WriteLine($"符合 {s}");
                                DataDic[$"{i}_{dateTemp:yyMMdd}"].Add(s.Substring(0, 2));
                                currentTimes++;
                                continue;
                            }
                        }
                        dateTemp = Convert.ToDateTime($"{dateTemp:yyyy/MM/dd} 00:00:00");
                        dateTemp = dateTemp.AddDays(1);
                        //Debug.WriteLine($"日期 {DateTemp}");
                    }
                }

            }
            catch (Exception ex)
            {
                Log.Write($"{FilePath} {ex}");
                newCount = 0;
                ReturnsResult.Shunt(eConvertResult.處理邏輯失敗, ex.ToString());
                return false;
            }
            try
            {
                OP_OnCube.MinSheng_UD(DataDic, filePathOutput, _UDBatch);
                return true;
            }
            catch (Exception ex)
            {
                Log.Write($"{FilePath} {ex}");
                newCount = 0;
                ReturnsResult.Shunt(eConvertResult.產生OCS失敗, null);
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
                _UDBatch = OLEDB.GetMingSheng_UD(InputPath, FilePath, Index);
                newCount = _UDBatch.Count > 0 ? _UDBatch[_UDBatch.Count - 1].RecNo : 0;
                //newCount = 0;
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                throw ex;
            }
        }

        private void OLEDB_OPD()
        {
            try
            {
                _OPD = OLEDB.GetMingSheng_OPD(InputPath, FilePath, Index);
                newCount = _OPD.Count > 0 ? _OPD[_OPD.Count - 1].RecNo + 1 : 0;
                //Debug.WriteLine($"The number of new count is {newCount}");
                //newCount = 0;
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
            _SumQtyOfMedicine.Clear();
        }

        public override bool ProcessCare()
        {
            throw new NotImplementedException();
        }

        public override bool LogicCare()
        {
            throw new NotImplementedException();
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