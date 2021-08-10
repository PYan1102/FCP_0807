using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Globalization;
using FCP.MVVM.Models.Enum;
using FCP.MVVM.Models;

namespace FCP
{
    class FMT_FangDing : FormatCollection
    {
        private List<DetailItems> _DetailItems = new List<DetailItems>();

        public override ReturnsResultFormat MethodShunt()
        {
            _DetailItems.Clear();
            return base.MethodShunt();
        }

        public override bool ProcessOPD()
        {
            try
            {
                string fileContent = GetContent;
                List<string> data = fileContent.Split('\n').Where(x => !string.IsNullOrEmpty(x)).ToList();
                foreach (string s in data)
                {
                    List<string> properties = s.Split('|').Where(x => !string.IsNullOrEmpty(x)).ToList();
                    string adminCode = properties[9];
                    string medicineCode = properties[5];
                    if (SettingsModel.EN_FilterMedicineCode && !MedicineCodeGiven_L.Contains(medicineCode))
                        continue;
                    if (JudgePackedMode(adminCode))
                        continue;
                    if (!GOD.Is_Admin_Code_For_Multi_Created(adminCode))
                    {
                        Log.Write($"{FullFileName_S} 在OnCube中未建置此餐包頻率 {adminCode}");
                        ReturnsResult.Shunt(ConvertResult.沒有餐包頻率, adminCode);
                        return false;
                    }
                    int days = Convert.ToInt32(properties[10]);
                    DateTime.TryParseExact(properties[4], "yyyyMMdd", null, DateTimeStyles.None, out DateTime endDate);
                    endDate = endDate.AddDays(days - 1);
                    _DetailItems.Add(new DetailItems
                    {
                        PrescriptionNo = properties[2],
                        PatientName = properties[3],
                        StartDate = properties[4].Substring(2),
                        EndDate = endDate.ToString("yyMMdd"),
                        MedicineCode = medicineCode,
                        MedicineName = properties[6],
                        PerQty = properties[7],
                        AdminCode = adminCode,
                        Days = Convert.ToInt32(properties[10]),
                        SumQty = properties[11],
                    });
                }
                if (_DetailItems.Count == 0)
                {
                    ReturnsResult.Shunt(ConvertResult.全數過濾, null);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Write($"{FullFileName_S}  {ex}");
                ReturnsResult.Shunt(ConvertResult.讀取檔案失敗, ex.ToString());
                return false;
            }
        }

        public override bool LogicOPD()
        {
            oncube = new OnputType_OnCube(Log);
            string patientName = _DetailItems[0].PatientName;
            string prescriptionNo = _DetailItems[0].PrescriptionNo;
            string fileNameOutput = $@"{OutputPath_S}\{patientName}-{prescriptionNo}-{Path.GetFileNameWithoutExtension(FullFileName_S)}_{Time_S}.txt";
            try
            {
                oncube.FangDing(_DetailItems, fileNameOutput);
                return true;
            }
            catch (Exception ex)
            {
                Log.Write($"{FullFileName_S}  {ex}");
                ReturnsResult.Shunt(ConvertResult.產生OCS失敗, null);
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
    }

    public class DetailItems
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
