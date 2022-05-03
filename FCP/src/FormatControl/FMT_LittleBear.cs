using FCP.Models;
using FCP.src.Enum;
using Helper;
using System;
using System.Collections.Generic;

namespace FCP.src.FormatControl
{
    class FMT_LittleBear : FormatCollection
    {
        private List<LittleBearOPD> _opd = new List<LittleBearOPD>();

        public override bool ProcessOPD()
        {
            try
            {
                foreach (var s in GetPrescriptionInfoList)
                {
                    string[] data = s.Trim().Split(',');
                    string medicineCode = RemoveStringDoubleQuotes(data[2]);
                    string adminCode = RemoveStringDoubleQuotes(data[6]);
                    if (IsFilterMedicineCode(medicineCode) || IsFilterAdminCode(adminCode))
                        continue;
                    if (!IsExistsMultiAdminCode(adminCode))
                    {
                        ReturnsResult.Shunt(eConvertResult.缺少餐包頻率, adminCode);
                        return false;
                    }
                    DateTime startDate = DateTimeHelper.Convert((Convert.ToInt32(RemoveStringDoubleQuotes(data[0])) + 19110000).ToString(), "yyyyMMdd");
                    int days = Convert.ToInt32(RemoveStringDoubleQuotes(data[7]));
                    _opd.Add(new LittleBearOPD()
                    {
                        PatientName = RemoveStringDoubleQuotes(data[13]),
                        PrescriptionNo = RemoveStringDoubleQuotes(data[1]),
                        MedicineCode = medicineCode,
                        MedicineName = RemoveStringDoubleQuotes(data[3]),
                        PerQty = Convert.ToSingle(RemoveStringDoubleQuotes(data[4])),
                        AdminCode = adminCode,
                        Days = days,
                        SumQty = Convert.ToSingle(RemoveStringDoubleQuotes(data[9])),
                        StartDay = startDate,
                        EndDay = startDate.AddDays(days - 1)
                    });
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
                ReturnsResult.Shunt(eConvertResult.讀取檔案失敗, ex);
                return false;
            }
        }

        public override bool LogicOPD()
        {
            string outputDirectory = $@"{OutputDirectory}\{_opd[0].PatientName}-{SourceFileNameWithoutExtension}_{CurrentSeconds}.txt";
            try
            {
                OP_OnCube.LittleBear(_opd, outputDirectory);
                return true;
            }
            catch (Exception ex)
            {
                ReturnsResult.Shunt(eConvertResult.產生OCS失敗, ex);
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

        public override ReturnsResultModel MethodShunt()
        {
            _opd.Clear();
            return base.MethodShunt();
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
