using FCP.Models;
using System;
using System.Collections.Generic;
using Helper;
using FCP.src.Enum;

namespace FCP.src.FormatControl
{
    class FMT_OnCube : FormatCollection
    {
        private List<OnCubeOPD> _opd = new List<OnCubeOPD>();

        public override bool ProcessOPD()
        {
            try
            {
                foreach (string s in GetPrescriptionInfoList)
                {
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
                        ReturnsResult.Shunt(eConvertResult.缺少餐包頻率, adminCode);
                        return false;
                    }
                    _opd.Add(new OnCubeOPD()
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
                        Hospital = EncodingHelper.GetString(451, 30),
                        Handler = EncodingHelper.GetString(481, 30)
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
            string outputDirectory = $@"{OutputDirectory}\{SourceFileName}";
            try
            {
                OP_OnCube.OnCube(_opd, outputDirectory);
                return true;
            }
            catch (Exception ex)
            {
                ReturnsResult.Shunt(eConvertResult.產生OCS失敗, ex);
                return false;
            }
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

        public override ReturnsResultModel MethodShunt()
        {
            _opd.Clear();
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
        public string Handler { get; set; }
    }
}
