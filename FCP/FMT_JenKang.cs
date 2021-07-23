using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.IO;

namespace FCP
{
    class FMT_JenKang : FormatCollection
    {
        ObservableCollection<Prescription> pre = new ObservableCollection<Prescription>();
        public override void Load(string inp, string oup, string filename, string time, Settings settings, Log log)
        {
            base.Load(inp, oup, filename, time, settings, log);
        }

        public override string MethodShunt(int? MethodID)
        {
            Console.WriteLine(MethodID);
            switch (MethodID)
            {
                case 0:
                    string processResult = Do(OPD_Process());
                    if (!processResult.StartsWith($"{(int)ResultType.成功}"))
                        return processResult;
                    string logicResult = Do(OPD_Logic());
                    if (!logicResult.StartsWith($"{(int)ResultType.成功}"))
                        return logicResult;
                    return logicResult;
                case 2:
                    string processResult1 = Do(UD_Process());
                    if (!processResult1.StartsWith($"{(int)ResultType.成功}"))
                        return processResult1;
                    string logicResult2 = Do(UD_Logic());
                    if (!logicResult2.StartsWith($"{(int)ResultType.成功}"))
                        return logicResult2;
                    return logicResult2;
                default:
                    return $"{FullFileName_S} {MethodID} {Settings.StatOrBatch}";
            }
        }

        public ResultType OPD_Process()
        {
            try
            {
                pre.Clear();
                var ecd = Encoding.Default;
                string content = GetContent;
                List<string> contentSplit = content.Split('\n').ToList();
                List<string> medicineList = Get_Medicine_Code_If_Weight_Is_10_Gram();
                foreach (string s in contentSplit)
                {
                    if (s == "")
                        continue;
                    byte[] temp = ecd.GetBytes(s.Trim());
                    //途徑PO轉為PC
                    string way = ecd.GetString(temp, 193, 4).Trim().Equals("PO") ? "PC" : ecd.GetString(temp, 193, 4).Trim();
                    string adminCode = $"S{ecd.GetString(temp, 183, 10).Trim() + way}";
                    string medicineCode = ecd.GetString(temp, 57, 10).Trim();
                    //if (Settings.EN_FilterMedicineCode && !MedicineCodeGiven_L.Contains(medicineCode))
                    //    continue;
                    if (!medicineList.Contains(medicineCode))
                        continue;
                    if (JudgePackedMode(adminCode))
                        continue;
                    if (!CheckCombiCode(adminCode))
                    {
                        log.Write($"{FullFileName_S} 在OnCube中未建置種包頻率 {adminCode}");
                        LoseContent = $"{FullFileName_S} 在OnCube中未建置此種包頻率 {adminCode} 的頻率";
                        return ResultType.沒有頻率;
                    }
                    string startDateTemp = ecd.GetString(temp, 41, 8).Trim();
                    DateTime.TryParseExact(startDateTemp, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime startDate);
                    pre.Add(new Prescription
                    {
                        DrugNo = ecd.GetString(temp, 0, 10).Trim(),
                        PatientName = ecd.GetString(temp, 11, 20).Trim(),
                        PrescriptionNo = ecd.GetString(temp, 31, 10).Trim(),
                        StartDay = startDate,
                        MedicineCode = medicineCode,
                        MedicineName = ecd.GetString(temp, 67, 50).Trim(),
                        PerQty = Convert.ToSingle(ecd.GetString(temp, 167, 10).Trim()).ToString("0.##"),
                        AdminCode = adminCode,
                        Days = ecd.GetString(temp, 197, 3).Trim(),
                        SumQty = ecd.GetString(temp, 200, 10).Trim(),
                        Location = string.Empty
                    });
                    try
                    {
                        pre[pre.Count - 1]._IsPowder = ecd.GetString(temp, 236, 1) != "";
                    }
                    catch (Exception) { pre[pre.Count - 1]._IsPowder = false; }
                }
                if (pre.Count == 0)
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

        public ResultType OPD_Logic()
        {
            if (pre.Count == 0)
                return ResultType.全數過濾;
            try
            {
                bool yn = false;
                string outputFileName = $@"{OutputPath_S}\{Path.GetFileNameWithoutExtension(FullFileName_S)}_{pre[0].PatientName}_{Time_S}.txt";
                oncube = new OnputType_OnCube(log);
                yn = oncube.JenKang_OPD(pre, outputFileName);
                if (yn)
                    return ResultType.成功;
                else
                {
                    List<string> day = new List<string>();
                    for (int x = 0; x <= pre.Count - 1; x++)
                    {
                        day.Add(pre[x].StartDay + "~" + pre[x].StartDay);
                    }
                    log.Prescription(FullFileName_S, pre.Select(x => x.PatientName).ToList(), pre.Select(x => x.PrescriptionNo).ToList(), pre.Select(x => x.MedicineCode).ToList(), pre.Select(x => x.MedicineName).ToList(),
                        pre.Select(x => x.AdminCode).ToList(), pre.Select(x => x.PerQty).ToList(), pre.Select(x => x.SumQty).ToList(), day);
                    ErrorContent = $"{FullFileName_S} 產生OCS時發生問題";
                    return ResultType.失敗;
                }
            }
            catch (Exception ex)
            {
                log.Write($"{FullFileName_S} {ex}");
                ErrorContent = $"{FullFileName_S} 處理邏輯時發生問題 {ex}";
                return ResultType.失敗;
            }
        }

        public ResultType UD_Process()
        {
            try
            {
                pre.Clear();
                var ecd = Encoding.Default;
                string content = GetContent;
                List<string> contentSplit = content.Split('\n').ToList();
                foreach (string s in contentSplit)
                {
                    if (s == "")
                        continue;
                    byte[] temp = ecd.GetBytes(s.Trim());
                    //途徑PO轉為PC
                    string way = ecd.GetString(temp, 193, 4).Trim().Equals("PO") ? "PC" : ecd.GetString(temp, 193, 4).Trim();
                    string adminCode = ecd.GetString(temp, 183, 10).Trim() + way;
                    string medicineCode = ecd.GetString(temp, 57, 10).Trim();
                    if (Settings.EN_FilterMedicineCode && !MedicineCodeGiven_L.Contains(medicineCode))
                        continue;
                    if (JudgePackedMode(adminCode))
                        continue;
                    if (!CheckMultiCode(adminCode))
                    {
                        log.Write($"{FullFileName_S} 在OnCube中未建置餐包頻率 {adminCode}");
                        LoseContent = $"{FullFileName_S} 在OnCube中未建置此餐包頻率 {adminCode} 的頻率";
                        return ResultType.沒有頻率;
                    }
                    string location = "住院";
                    if (temp.Length > 240)
                        location = ecd.GetString(temp, 247, temp.Length - 247);
                    int adminCodeTimePerDay = GOD.Get_Admin_Code_For_Multi(adminCode).Count;
                    Single SumQty = Convert.ToSingle(ecd.GetString(temp, 200, 10).Trim());
                    Single PerQty = Convert.ToSingle(ecd.GetString(temp, 167, 10).Trim());
                    pre.Add(new Prescription
                    {
                        BedNo = ecd.GetString(temp, 0, 10).Trim(),
                        PatientName = ecd.GetString(temp, 11, 20).Trim(),
                        PrescriptionNo = ecd.GetString(temp, 31, 10).Trim(),
                        MedicineCode = medicineCode,
                        MedicineName = ecd.GetString(temp, 67, 50).Trim(),
                        PerQty = Convert.ToSingle(ecd.GetString(temp, 167, 10).Trim()).ToString("0.##"),
                        AdminCode = adminCode,
                        Days = Math.Ceiling(SumQty / PerQty / adminCodeTimePerDay).ToString(),
                        SumQty = ecd.GetString(temp, 200, 10).Trim(),
                        Location = location
                    });
                    var preTemp = pre[pre.Count - 1];
                    string startDateTemp = ecd.GetString(temp, 41, 8).Trim();
                    DateTime.TryParseExact(startDateTemp, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime startDate);
                    preTemp.StartDay = startDate.AddDays(location == "住院" ? 2 : 1);  //住院+2天，養護+1天
                    //preTemp.EndDay = startDate.AddDays(2 + Convert.ToInt32(preTemp.Days) - 1).ToString("yyyyMMdd");
                    try
                    {
                        pre[pre.Count - 1]._IsPowder = ecd.GetString(temp, 236, 1) != "";
                    }
                    catch (Exception) { pre[pre.Count - 1]._IsPowder = false; }
                }
                if (pre.Count == 0)
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

        public ResultType UD_Logic()
        {
            if (pre.Count == 0)
                return ResultType.全數過濾;
            try
            {
                bool yn = false;
                string outputFileName = $@"{OutputPath_S}\{Path.GetFileNameWithoutExtension(FullFileName_S)}_{Time_S}.txt";
                oncube = new OnputType_OnCube(log);
                yn = oncube.JenKang_UD(pre, outputFileName);
                if (yn)
                    return ResultType.成功;
                else
                {
                    List<string> day = new List<string>();
                    for (int x = 0; x <= pre.Count - 1; x++)
                    {
                        day.Add(pre[x].StartDay + "~" + pre[x].StartDay);
                    }
                    log.Prescription(FullFileName_S, pre.Select(x => x.PatientName).ToList(), pre.Select(x => x.PrescriptionNo).ToList(), pre.Select(x => x.MedicineCode).ToList(), pre.Select(x => x.MedicineName).ToList(),
                        pre.Select(x => x.AdminCode).ToList(), pre.Select(x => x.PerQty).ToList(), pre.Select(x => x.SumQty).ToList(), day);
                    ErrorContent = $"{FullFileName_S} 產生OCS時發生問題";
                    return ResultType.失敗;
                }
            }
            catch (Exception ex)
            {
                log.Write($"{FullFileName_S} {ex}");
                ErrorContent = $"{FullFileName_S} 處理邏輯時發生問題 {ex}";
                return ResultType.失敗;
            }
        }

        public class Prescription
        {
            public string PrescriptionNo { get; set; }
            public string PatientName { get; set; }
            public string DrugNo { get; set; }
            public string MedicineCode { get; set; }
            public string MedicineName { get; set; }
            public string PerQty { get; set; }
            public string AdminCode { get; set; }
            public string Days { get; set; }
            public string SumQty { get; set; }
            public DateTime StartDay { get; set; }
            public DateTime EndDay { get; set; }
            public bool _IsPowder { get; set; }
            public string BedNo { get; set; }
            public string Location { get; set; }
        }
    }
}
