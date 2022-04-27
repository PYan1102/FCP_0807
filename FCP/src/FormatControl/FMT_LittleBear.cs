using FCP.src.Enum;
using Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCP.src.FormatControl
{
    class FMT_LittleBear : FormatCollection
    {
        private List<LittleBearOPD> _OPD = new List<LittleBearOPD>();

        public override bool ProcessOPD()
        {
            try
            {
                _OPD.Clear();
                string[] list = GetContent.Trim().Split('\n');
                foreach (var s in list)
                {
                    string[] data = s.Trim().Split(',');
                    string medicineCode = RemoveStringDoubleQuotes(data[2]);
                    string adminCode = RemoveStringDoubleQuotes(data[6]);
                    if (IsFilterMedicineCode(medicineCode) || IsFilterAdminCode(adminCode))
                        continue;
                    if (!IsExistsMultiAdminCode(adminCode))
                    {
                        Log.Write($"{FilePath} 在OnCube中未建置此餐包頻率 {adminCode}");
                        ReturnsResult.Shunt(eConvertResult.沒有餐包頻率, adminCode);
                        return false;
                    }
                    DateTime startDay = DateTimeHelper.Convert((Convert.ToInt32(RemoveStringDoubleQuotes(data[0])) + 19110000).ToString(), "yyyyMMdd");
                    int days = Convert.ToInt32(RemoveStringDoubleQuotes(data[7]));
                    _OPD.Add(new LittleBearOPD()
                    {
                        PatientName = RemoveStringDoubleQuotes(data[13]),
                        PrescriptionNo = RemoveStringDoubleQuotes(data[1]),
                        MedicineCode = medicineCode,
                        MedicineName = RemoveStringDoubleQuotes(data[3]),
                        PerQty = Convert.ToSingle(RemoveStringDoubleQuotes(data[4])),
                        AdminCode = adminCode,
                        Days = days,
                        SumQty = Convert.ToSingle(RemoveStringDoubleQuotes(data[9])),
                        StartDay = startDay,
                        EndDay = startDay.AddDays(days - 1)
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
            string outputDirectory = $@"{OutputPath}\{_OPD[0].PatientName}-{Path.GetFileNameWithoutExtension(FilePath)}_{CurrentSeconds}.txt";
            try
            {
                OP_OnCube.LittleBear(_OPD, outputDirectory);
                return true;
            }
            catch (Exception ex)
            {
                Log.Write($"{FilePath} {ex}");
                ReturnsResult.Shunt(eConvertResult.產生OCS失敗, ex.ToString());
                return false;
            }
        }

        public override bool LogicCare()
        {
            throw new NotImplementedException();
        }

        public override bool LogicOther()
        {
            throw new NotImplementedException();
        }

        public override bool LogicPOWDER()
        {
            throw new NotImplementedException();
        }

        public override bool LogicUDBatch()
        {
            throw new NotImplementedException();
        }

        public override bool LogicUDStat()
        {
            throw new NotImplementedException();
        }

        public override bool ProcessCare()
        {
            throw new NotImplementedException();
        }

        public override bool ProcessOther()
        {
            throw new NotImplementedException();
        }

        public override bool ProcessPOWDER()
        {
            throw new NotImplementedException();
        }

        public override bool ProcessUDBatch()
        {
            throw new NotImplementedException();
        }

        public override bool ProcessUDStat()
        {
            throw new NotImplementedException();
        }

        private string RemoveStringDoubleQuotes(string data)
        {
            return data.Replace("\"", "");
        }
    }

    internal class LittleBearOPD
    {
        public string PatientName { get; set; }
        public string PrescriptionNo { get; set; }
        public string MedicineCode { get; set; }
        public string MedicineName { get; set; }
        public float PerQty { get; set; }
        public int Days { get; set; }
        public float SumQty { get; set; }
        public string AdminCode { get; set; }
        public DateTime StartDay { get; set; }
        public DateTime EndDay { get; set; }
    }
}
