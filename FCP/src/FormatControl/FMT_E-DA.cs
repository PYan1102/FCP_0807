using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using FCP.src.Enum;
using FCP.Models;
using System.Globalization;
using Helper;

namespace FCP.src.FormatControl
{
    class FMT_E_DA : FormatCollection
    {
        private List<EDAUDBatch> _UDBatch = new List<EDAUDBatch>();

        public override bool ProcessOPD()
        {
            throw new NotImplementedException();
        }

        public override bool LogicOPD()
        {
            throw new NotImplementedException();
        }

        public override bool ProcessUDBatch()
        {
            try
            {
                bool isExists = false;
                List<string> list = GetContent.Split('\n').ToList();
                foreach (string s in list)
                {
                    isExists = false;
                    List<string> data = s.Split('	').Select(x => x.Trim()).ToList();
                    Console.WriteLine(data.Count);
                    if (data.Count <= 1 || data[5] == "BID+HS")
                        continue;
                    if (IsFilterMedicineCode(data[4]) || IsFilterAdminCode($"{data[5]}"))
                        continue;
                    if (!IsExistsCombiAdminCode($"{data[5]}"))
                    {
                        Log.Write($"{FilePath} 在OnCube中未建置此種包頻率 S{data[5]}");
                        ReturnsResult.Shunt(eConvertResult.沒有種包頻率, data[5]);
                        return false;
                    }
                    DateTime.TryParseExact($"{Convert.ToInt32(data[12]) + 19110000}", "yyyyMMdd", null, DateTimeStyles.None, out DateTime startDate);
                    for (int i = _UDBatch.Count - 1; i >= 0; i--)
                    {
                        if (_UDBatch[i].PatientName == data[1] & _UDBatch[i].MedicineCode == data[4] & _UDBatch[i].AdminTime == $"S{data[5]}")
                        {
                            isExists = true;
                            _UDBatch[i].SumQty = (float.Parse(_UDBatch[i].SumQty) + float.Parse(data[10])).ToString("0.###");
                            break;
                        }
                    }
                    if (!isExists)
                    {
                        _UDBatch.Add(new EDAUDBatch
                        {
                            PatientName = data[1],
                            MedicineCode = data[4],
                            AdminTime = $"S{data[5]}",
                            PerQty = data[9],
                            SumQty = float.Parse(data[10]).ToString("0.###"),
                            StartDate = startDate.ToString("yyMMdd"),
                            Days = data[13],
                            BedNo = data[15],
                            MedicineName = data[19],
                            PrescriptionNo = data[20],
                            StartTime = data[23],
                            BirthDate = data[39].Insert(3, "/").Insert(6, "/")
                        });
                    }
                }
                if (_UDBatch.Count == 0)
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

        public override bool LogicUDBatch()
        {
            string filePathOutput = $@"{OutputPath}\{Path.GetFileNameWithoutExtension(FilePath)}_{CurrentSeconds}.txt";
            try
            {
                OP_OnCube.E_DA_UD(_UDBatch, filePathOutput);
                return true;
            }
            catch (Exception ex)
            {
                Log.Write($"{FilePath} {ex}");
                ReturnsResult.Shunt(eConvertResult.產生OCS失敗, ex.ToString());
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
            _UDBatch.Clear();
            return base.MethodShunt();
        }
    }
    internal class EDAUDBatch
    {
        public string PrescriptionNo { get; set; }
        public string PatientName { get; set; }
        public string MedicineCode { get; set; }
        public string MedicineName { get; set; }
        public string AdminTime { get; set; }
        public string PerQty { get; set; }
        public string SumQty { get; set; }
        public string StartDate { get; set; }
        public string StartTime { get; set; }
        public string BedNo { get; set; }
        public string BirthDate { get; set; }
        public string Days { get; set; }
    }
}
