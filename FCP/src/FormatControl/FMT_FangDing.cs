using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Globalization;
using FCP.src.Enum;
using FCP.Models;
using Helper;

namespace FCP.src.FormatControl
{
    class FMT_FangDing : FormatCollection
    {
        private List<FangDingOPD> _opd = new List<FangDingOPD>();

        public override bool ProcessOPD()
        {
            try
            {
                foreach (string s in GetPrescriptionInfoList)
                {
                    List<string> properties = s.Split('|').Where(x => !string.IsNullOrEmpty(x)).ToList();
                    string adminCode = properties[9];
                    string medicineCode = properties[5];
                    if (IsFilterMedicineCode(medicineCode) || IsFilterAdminCode(adminCode))
                        continue;
                    if (!IsExistsMultiAdminCode(adminCode))
                    {
                        ReturnsResult.Shunt(eConvertResult.缺少餐包頻率, adminCode);
                        return false;
                    }
                    int days = Convert.ToInt32(properties[10]);
                    DateTime endDate = DateTimeHelper.Convert(properties[4], "yyyyMMdd").AddDays(days - 1);
                    _opd.Add(new FangDingOPD
                    {
                        PrescriptionNo = properties[2],
                        PatientName = properties[3],
                        StartDate = properties[4].Substring(2),
                        EndDate = endDate.ToString("yyMMdd"),
                        MedicineCode = medicineCode,
                        MedicineName = properties[6],
                        PerQty = properties[7],
                        AdminCode = adminCode,
                        Days = days,
                        SumQty = properties[11],
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
            string outputDirectory = $@"{OutputDirectory}\{_opd[0].PatientName}-{_opd[0].PrescriptionNo}-{SourceFileNameWithoutExtension}_{CurrentSeconds}.txt";
            try
            {
                OP_OnCube.FangDing(_opd, outputDirectory);
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
            throw new NotImplementedException();
        }

        public override bool LogicUDBatch()
        {
            throw new NotImplementedException();
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

        public override ReturnsResultModel MethodShunt()
        {
            _opd.Clear();
            return base.MethodShunt();
        }
    }

    internal class FangDingOPD
    {
        public string PrescriptionNo { get; set; }
        public string PatientName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string MedicineCode { get; set; }
        public string MedicineName { get; set; }
        public string PerQty { get; set; }
        public string AdminCode { get; set; }
        public int Days { get; set; }
        public string SumQty { get; set; }
    }
}
