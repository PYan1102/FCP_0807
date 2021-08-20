using FCP.MVVM.Models;
using System;
using System.Collections.Generic;
using FCP.MVVM.Helper;
using FCP.MVVM.Models.Enum;
using System.IO;

namespace FCP.MVVM.FormatControl
{
    class FMT_OnCube : FormatCollection
    {
        private List<OnCubeOPD> _OPD = new List<OnCubeOPD>();

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
                //原檔會有病患姓名為問號的問題，半形問號，少一個byte
                if (EncodingHelper.GetString(0, 20).Contains("?"))
                {
                    string newData = s.Trim().Replace('?', ' ').Insert(17, " ");
                    EncodingHelper.SetBytes(newData);
                }
                string adminCode = EncodingHelper.GetString(204, 20);
                string medicineCode = EncodingHelper.GetString(134, 20);
                if (IsFilterMedicineCode(medicineCode) || IsFilterAdminCode(adminCode))
                    continue;
                if (!IsExistsMultiAdminCode(adminCode))
                {
                    Log.Write($"{FilePath} 在OnCube中未建置此餐包頻率 {adminCode}");
                    ReturnsResult.Shunt(ConvertResult.沒有餐包頻率, adminCode);
                    return false;
                }
                _OPD.Add(new OnCubeOPD()
                {
                    PatientName = EncodingHelper.GetString(0, 20),
                    PatientNo = EncodingHelper.GetString(20, 30),
                    Department = EncodingHelper.GetString(50, 50),
                    PerQty = EncodingHelper.GetString(129, 5),
                    MedicineCode = medicineCode,
                    MedicineName = EncodingHelper.GetString(154, 50),
                    AdminCode = adminCode,
                    StartDay = DateTimeHelper.Convert(EncodingHelper.GetString(224, 6), "yyMMdd"),
                    EndDay = DateTimeHelper.Convert(EncodingHelper.GetString(230, 6), "yyMMdd"),
                    RoomNo = EncodingHelper.GetString(410, 20),
                    BedNo = EncodingHelper.GetString(430, 20),
                    Hospital = EncodingHelper.GetString(451, 30)
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
                OP_OnCube.OnCube(_OPD, filePathOutput);
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
    internal class OnCubeOPD
    {
        public string PatientNo { get; set; }
        public string PrescriptionNo { get; set; }
        public string PatientName { get; set; }
        public string MedicineCode { get; set; }
        public string MedicineName { get; set; }
        public string PerQty { get; set; }
        public string AdminCode { get; set; }
        public string Days { get; set; }
        public string SumQty { get; set; }
        public DateTime StartDay { get; set; }
        public DateTime EndDay { get; set; }
        public string RoomNo { get; set; }
        public string BedNo { get; set; }
        public string Department { get; set; }
        public string Hospital { get; set; }
    }
}
