using Helper;
using FCP.MVVM.Models;
using System;
using System.Collections.Generic;
using FCP.MVVM.Models.Enum;
using System.IO;

namespace FCP.MVVM.FormatControl
{
    class FMT_ChengYu : FormatCollection
    {
        private List<ChengYuOPD> _OPD = new List<ChengYuOPD>();
        public override bool ProcessCare()
        {
            throw new NotImplementedException();
        }

        public override bool ProcessOPD()
        {
            string[] list = GetContent.Split('\n');
            foreach (string s in list)
            {
                if (s.Trim().Length == 0)
                    continue;
                EncodingHelper.SetBytes(s.Trim());
                string numberOfPackages = EncodingHelper.GetString(287, 30);
                string medicineCode = EncodingHelper.GetString(155, 20);
                if (IsFilterMedicineCode(medicineCode) || IsFilterAdminCode(numberOfPackages))
                {
                    continue;
                }
                if (!IsExistsMultiAdminCode(numberOfPackages))
                {
                    Log.Write($"{FilePath} 在OnCube中未建置餐包頻率 {numberOfPackages}");
                    ReturnsResult.Shunt(ConvertResult.沒有餐包頻率, numberOfPackages);
                    return false;
                }
                _OPD.Add(new ChengYuOPD()
                {
                    PatientName = EncodingHelper.GetString(0, 20),
                    PatientNo = EncodingHelper.GetString(20, 30),
                    HospitalName = EncodingHelper.GetString(100, 50),
                    PerQty = EncodingHelper.GetString(150, 5),
                    MedicineCode = medicineCode,
                    MedicineName = EncodingHelper.GetString(175, 50),
                    AdminCode = EncodingHelper.GetString(225, 10),
                    AdminCodeDescription = EncodingHelper.GetString(235, 30),
                    Unit = EncodingHelper.GetString(265, 10),
                    StartDay = DateTimeHelper.Convert(EncodingHelper.GetString(275, 6), "yyMMdd"),
                    EndDay = DateTimeHelper.Convert(EncodingHelper.GetString(281, 6), "yyMMdd"),
                    NumOfPackages = numberOfPackages,
                    Days = EncodingHelper.GetString(307, 30)
                });
            }
            if (_OPD.Count == 0)
            {
                ReturnsResult.Shunt(ConvertResult.全數過濾, null);
                return false;
            }
            return true;
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

        public override bool LogicCare()
        {
            throw new NotImplementedException();
        }

        public override bool LogicOPD()
        {
            string filePathOutput = $@"{OutputPath}\{Path.GetFileName(FilePath)}";
            try
            {
                OP_OnCube.ChengYu(_OPD, filePathOutput);
                return true;
            }
            catch (Exception ex)
            {
                Log.Write($"{FilePath} {ex}");
                ReturnsResult.Shunt(ConvertResult.產生OCS失敗, ex.ToString());
                return false;
            }
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

        public override ReturnsResultFormat MethodShunt()
        {
            _OPD.Clear();
            return base.MethodShunt();
        }
    }
    internal class ChengYuOPD
    {
        public string PatientNo { get; set; }
        public string PatientName { get; set; }
        public string MedicineCode { get; set; }
        public string MedicineName { get; set; }
        public string PerQty { get; set; }
        public string AdminCode { get; set; }
        public string AdminCodeDescription { get; set; }
        public string Days { get; set; }
        public string NumOfPackages { get; set; }
        public DateTime StartDay { get; set; }
        public DateTime EndDay { get; set; }
        public string BedNo { get; set; }
        public string HospitalName { get; set; }
        public string Unit { get; set; }
    }
}
