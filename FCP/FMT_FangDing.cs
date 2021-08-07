using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;

namespace FCP
{
    class FMT_FangDing : FormatCollection
    {
        private List<DetailItems> _DetailItems = new List<DetailItems>();
        public override void Load(string inp, string oup, string filename, string time, Settings settings, Log log)
        {
            base.Load(inp, oup, filename, time, settings, log);
            _DetailItems.Clear();
        }

        public override string MethodShunt(int? MethodID)
        {
            return Do(OPD_Process(), OPD_Logic());
        }

        private ResultType OPD_Process()
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
                    if (Settings.EN_FilterMedicineCode && !MedicineCodeGiven_L.Contains(medicineCode))
                        continue;
                    if (JudgePackedMode(adminCode))
                        continue;
                    if (!GOD.Is_Admin_Code_For_Multi_Created(adminCode))
                    {
                        log.Write($"{FullFileName_S} 在OnCube中未建置此餐包頻率 {adminCode}");
                        LoseContent = $"{FullFileName_S} 在OnCube中未建置此餐包頻率 {adminCode} 的頻率";
                        return ResultType.沒有頻率;
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
                    return ResultType.全數過濾;
                return ResultType.成功;
            }
            catch (Exception ex)
            {
                log.Write($"{FullFileName_S}  {ex}");
                ErrorContent = $"{FullFileName_S} 讀取處方籤時發生問題 {ex}";
                return ResultType.失敗;
            }
        }

        private ResultType OPD_Logic()
        {
            if (_DetailItems.Count == 0)
                return ResultType.全數過濾;
            oncube = new OnputType_OnCube(log);
            string patientName = _DetailItems[0].PatientName;
            string prescriptionNo = _DetailItems[0].PrescriptionNo;
            string fileNameOutput = $@"{OutputPath_S}\{patientName}-{prescriptionNo}-{Path.GetFileNameWithoutExtension(FullFileName_S)}_{Time_S}.txt";
            try
            {
                oncube.FangDing(_DetailItems, fileNameOutput);
                return ResultType.成功;
            }
            catch (Exception ex)
            {
                log.Write($"{FullFileName_S}  {ex}");
                ErrorContent = $"{FullFileName_S} 處理邏輯時發生問題 {ex}";
                return ResultType.失敗;
            }
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
