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

        private ConvertResult OPD_Process()
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
                        FailMessage = $"{FullFileName_S} 在OnCube中未建置此餐包頻率 {adminCode} 的頻率";
                        return ConvertResult.沒有頻率;
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
                    return ConvertResult.全數過濾;
                return ConvertResult.成功;
            }
            catch (Exception ex)
            {
                Log.Write($"{FullFileName_S}  {ex}");
                ErrorContent = $"{FullFileName_S} 讀取處方籤時發生問題 {ex}";
                return ConvertResult.失敗;
            }
        }

        private ConvertResult OPD_Logic()
        {
            if (_DetailItems.Count == 0)
                return ConvertResult.全數過濾;
            oncube = new OnputType_OnCube(Log);
            string patientName = _DetailItems[0].PatientName;
            string prescriptionNo = _DetailItems[0].PrescriptionNo;
            string fileNameOutput = $@"{OutputPath_S}\{patientName}-{prescriptionNo}-{Path.GetFileNameWithoutExtension(FullFileName_S)}_{Time_S}.txt";
            try
            {
                oncube.FangDing(_DetailItems, fileNameOutput);
                return ConvertResult.成功;
            }
            catch (Exception ex)
            {
                Log.Write($"{FullFileName_S}  {ex}");
                ErrorContent = $"{FullFileName_S} 處理邏輯時發生問題 {ex}";
                return ConvertResult.失敗;
            }
        }

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
