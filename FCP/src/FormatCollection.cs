﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using FCP.Models;
using FCP.src.Enum;
using FCP.src.Interface;
using FCP.src.Factory.Models;
using Helper;

namespace FCP.src
{
    abstract class FormatCollection
    {
        public SettingJsonModel SettingModel { get; private set; }
        public string InputDirectory { get; private set; }
        public string OutputDirectory { get; private set; }
        public string SourceFilePath { get; private set; }
        public string SourceFileName { get; private set; }
        public string SourceFileNameWithoutExtension { get; private set; }
        public List<string> CrossDayAdminCode { get; private set; }
        public List<string> MultiAdminCode { get => _multiAdminCode; }
        public List<string> CombiAdminCode { get => _combiAdminCode; }
        public Dictionary<string, int> CrossDayAdminCodeDays { get => _crossDayAdminCodeDays; }
        public Dictionary<string, int> JVServerAdminCodeTimes { get => _jvserverAdminCodeTimes; }
        public string CurrentSeconds { get; set; }
        private IRetunrsResult _returnsResult;
        private eDepartment _department;
        private List<string> _filterAdminCode;
        private ReturnsResultModel _returnsResultFormat { get; set; }
        private List<string> _multiAdminCode;
        private List<string> _combiAdminCode;
        private Dictionary<string, List<string>> _multiAdminCodeTimes;
        private Dictionary<string, int> _crossDayAdminCodeDays;
        private Dictionary<string, int> _jvserverAdminCodeTimes;  //For JVServer
        private static readonly object _lockFile = new object();

        public FormatCollection()
        {
            SettingModel = ModelsFactory.GenerateSettingModel();
            _multiAdminCode = new List<string>();
            _combiAdminCode = new List<string>();
            _multiAdminCodeTimes = new Dictionary<string, List<string>>();
            _crossDayAdminCodeDays = new Dictionary<string, int>();
            _jvserverAdminCodeTimes = new Dictionary<string, int>();
            _filterAdminCode = new List<string>();
        }


        public List<string> GetPrescriptionInfoList
        {
            get => GetFileContent.Split('\n').Where(x => x.Trim().Length > 0).ToList();
        }

        public virtual void Init()
        {
            _returnsResultFormat = new ReturnsResultModel();
            _returnsResult = new ReturnsResult();
            _returnsResult.SetReturnsResultModel(_returnsResultFormat);
            InputDirectory = FileInfoModel.InputDirectory;
            OutputDirectory = SettingModel.OutputDirectory;
            SourceFilePath = FileInfoModel.SourceFilePath;
            SourceFileName = Path.GetFileName(FileInfoModel.SourceFilePath);
            SourceFileNameWithoutExtension = Path.GetFileNameWithoutExtension(FileInfoModel.SourceFilePath);
            CrossDayAdminCode = SettingModel.CrossDayAdminCode.Split(',').ToList();
            CurrentSeconds = FileInfoModel.CurrentDateTime.ToString("ss_fff");
            _department = FileInfoModel.Department;

            SetAdminCode();
        }

        /// <summary>
        /// 過濾規則
        /// </summary>
        /// <param name="adminCode">頻率</param>
        /// <param name="medicineCode">藥品代碼</param>
        /// <param name="filterAdminCode">使用設定中的使用特定頻率或過濾特定頻率功能</param>
        /// <param name="filterMedicine">使用設定中的過濾藥品功能及是否只讓有藥盒的藥品進來</param>
        /// <param name="filterCustomizeMedicineCode">使用設定中的過濾藥品功能</param>
        /// <returns>回傳 <see langword="true"/> 為過濾，<see langword="false"/> 為不過濾</returns>
        public virtual bool FilterRule(string adminCode = null, string medicineCode = null, bool filterAdminCode = true, bool filterMedicine = true, bool filterCustomizeMedicineCode = true)
        {
            if (filterAdminCode && adminCode == null)
            {
                throw new Exception("頻率不可為null");
            }
            if ((filterMedicine || filterCustomizeMedicineCode) && medicineCode == null)
            {
                throw new Exception("藥品代碼不可為null");
            }
            if (filterAdminCode && FilterAdminCode(adminCode))
            {
                return true;
            }
            if (filterMedicine && IsFilterMedicineCode(medicineCode))
            {
                return true;
            }
            if (filterCustomizeMedicineCode && FilterCustomizeMedicineCode(medicineCode))
            {
                return true;
            }
            return false;
        }

        //分流
        public virtual ReturnsResultModel DepartmentShunt()
        {
            Init();
            if (SettingModel.Format == eFormat.光田醫院JVS)
            {
                _jvserverAdminCodeTimes = GetJVServerAdminCodeTimes();
            }
            else if (SettingModel.Format != eFormat.JVS)
            {
                _multiAdminCode = GetAllMultiAdminCode();
                _combiAdminCode = GetAllCombiAdminCode();
                _multiAdminCodeTimes = GetAllMultiAdminCodeTimes();
                _crossDayAdminCodeDays = GetCrossDayAdminCodeDays();
            }
            switch (_department)
            {
                case eDepartment.OPD:
                    ProcessOPD();
                    if (_returnsResultFormat.Result == eConvertResult.成功)
                    {
                        LogicOPD();
                    }
                    break;
                case eDepartment.POWDER:
                    ProcessPowder();
                    if (_returnsResultFormat.Result == eConvertResult.成功)
                    {
                        LogicPowder();
                    }
                    break;
                case eDepartment.Stat:
                    ProcessUDStat();
                    if (_returnsResultFormat.Result == eConvertResult.成功)
                    {
                        LogicUDStat();
                    }
                    break;
                case eDepartment.Batch:
                    ProcessUDBatch();
                    if (_returnsResultFormat.Result == eConvertResult.成功)
                    {
                        LogicUDBatch();
                    }
                    break;
                case eDepartment.Other:
                    ProcessOther();
                    if (_returnsResultFormat.Result == eConvertResult.成功)
                    {
                        LogicOther();
                    }
                    break;
                default:
                    ProcessCare();
                    if (_returnsResultFormat.Result == eConvertResult.成功)
                    {
                        LogicCare();
                    }
                    break;
            }
            return _returnsResultFormat;
        }

        public void Success()
        {
            _returnsResult.Shunt(eConvertResult.成功);
        }

        public void Pass()
        {
            _returnsResult.Shunt(eConvertResult.全數過濾);
        }

        public void LostMultiAdminCode(string adminCode)
        {
            _returnsResult.Shunt(eConvertResult.缺少餐包頻率, adminCode);
        }

        public void LostCombiAdminCode(string adminCode)
        {
            _returnsResult.Shunt(eConvertResult.缺少種包頻率, adminCode);
        }

        public void ReadFileFail(Exception message)
        {
            _returnsResult.Shunt(eConvertResult.讀取檔案失敗, message);
        }

        public void ProgressLogicFail(Exception message)
        {
            _returnsResult.Shunt(eConvertResult.處理邏輯失敗, message);
        }

        public void GenerateOCSFileFail(Exception message)
        {
            _returnsResult.Shunt(eConvertResult.產生OCS失敗, message);
        }

        //特控1
        public int GetID1(string medicinecode)
        {
            try
            {
                int id1 = CommonModel.SqlHelper.Query_FirstInt($"SELECT CASE ISNULL(ExternalID1, 0) WHEN '' THEN 0 ELSE ISNULL(ExternalID1, 0) END FROM Item WHERE Mnemonic='{medicinecode}' AND DELETEDYN = 0");
                return id1;
            }
            catch
            {
                throw new ArgumentNullException($"在 Item 中找不到藥品代碼為 {medicinecode} 的藥品");
            }
        }

        //特控2
        public int GetID2(string medicinecode)
        {
            try
            {
                int id2 = CommonModel.SqlHelper.Query_FirstInt($"SELECT CASE ISNULL(ExternalID2, 0) WHEN '' THEN 0 ELSE ISNULL(ExternalID2, 0) END FROM Item WHERE Mnemonic='{medicinecode}' AND DELETEDYN = 0");
                return id2;
            }
            catch
            {
                throw new ArgumentNullException($"在 Item 中找不到藥品代碼為 {medicinecode} 的藥品");
            }
        }

        //計算共有幾筆藥 for JVServer
        public List<string> SeparateString(string content, int length)
        {
            List<string> list = new List<string>();
            byte[] bytes = Encoding.Default.GetBytes(content);
            int x = 0;
            while (true)
            {
                if (x < Encoding.Default.GetString(bytes, 0, bytes.Length).Length & bytes.Length - x >= length)
                {
                    string a = Encoding.Default.GetString(bytes, x, length);
                    if (!string.IsNullOrWhiteSpace(a))
                    {
                        list.Add(a);
                        x += length;
                    }
                }
                else
                    break;
            }
            return list;
        }

        //刪除空白
        public string DeleteSpace(string content)
        {
            string[] split = content.Split('\n');
            StringBuilder sb = new StringBuilder();
            foreach (string s in split)
            {
                if (!string.IsNullOrEmpty(s.Trim()))
                {
                    sb.AppendLine(s);
                }
            }
            return sb.ToString();
        }

        //取得檔案內容
        public string GetFileContent
        {
            get
            {
                try
                {
                    using (FileStream fs = new FileStream(SourceFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        using (StreamReader sr = new StreamReader(fs, Encoding.Default))
                        {
                            lock (_lockFile)
                            {
                                return sr.ReadToEnd();
                            }
                        }
                    }
                }
                catch
                {
                    LogService.Exception($"讀取目標檔案錯誤 {SourceFilePath}");
                    return GetFileContent;
                }
            }
        }

        //設定使用特定頻率、過濾特定頻率的頻率
        public void SetAdminCode()
        {
            _filterAdminCode.Clear();
            foreach (string s in SettingModel.NeedToFilterAdminCode)
            {
                if (!string.IsNullOrEmpty(s))
                    _filterAdminCode.Add(s);
            }
        }

        /// <summary>
        /// 取得指定頻率在餐包裡的時間點
        /// </summary>
        /// <param name="adminCode">頻率</param>
        /// <returns>時間點的陣列 (ex:08:00,13:00,18:00)</returns>
        public List<string> GetMultiAdminCodeTimes(string adminCode)
        {
            _multiAdminCodeTimes.TryGetValue(adminCode, out List<string> value);
            return value;
        }

        /// <summary>
        /// 若指定之餐包頻率不存在是否暫停
        /// </summary>
        /// <param name="adminCode">頻率</param>
        /// <returns>存在為 <see langword="true"/> ，不存在為 <see langword="false"/></returns>
        public bool WhetherToStopNotHasMultiAdminCode(string adminCode)
        {
            bool result = _multiAdminCode.Contains(adminCode);
            if (!result)
            {
                LostMultiAdminCode(adminCode);
            }
            return !result && !SettingModel.IgnoreAdminCodeIfNotInOnCube;
        }

        /// <summary>
        /// 若指定之種包頻率不存在是否暫停
        /// </summary>
        /// <param name="adminCode">頻率</param>
        /// <returns>存在為 <see langword="true"/> ，不存在為 <see langword="false"/></returns>
        public bool WhetherToStopNotHasCombiAdminCode(string adminCode)
        {
            bool result = _combiAdminCode.Contains($"S{adminCode}");
            if (!result)
            {
                LostCombiAdminCode(adminCode);
            }
            return !result && !SettingModel.IgnoreAdminCodeIfNotInOnCube;
        }

        public int GetCrossDayAdminCodeDays(string adminCode)
        {
            if (!_crossDayAdminCodeDays.TryGetValue(adminCode, out int days))
            {
                throw new Exception($"OnCube沒有設定該跨天數的頻率 {adminCode}");
            }
            return days;
        }

        public List<string> GetMedicineCodeWhenWeightIs10g()
        {
            List<string> list = CommonModel.SqlHelper.Query_List(@"SELECT
	                                                                   B.Mnemonic + ',' + ISNULL(D.MultiMnemonic, '') AS Mnemonic
                                                                   FROM Medicine A
                                                                   INNER JOIN Item B ON A.RawID = B.RawID AND A.DeletedYN = 0 AND A.WeightMedicine = 10
                                                                   LEFT OUTER JOIN ItemMultiCode D ON B.RawID = D.ItemID AND D.DeletedYN = 0", "Mnemonic");
            List<string> newList = new List<string>();
            foreach (string s in list)
            {
                string[] temp = s.Split(',');
                if (temp[1].Length > 0)
                    newList.Add(temp[1]);
                newList.Add(temp[0]);
            }
            return newList;
        }

        private Dictionary<string, int> GetCrossDayAdminCodeDays()
        {
            string value = CommonModel.SqlHelper.Query_FirstString("SELECT Value FROM SettingValue WHERE Name = 'OCS_DispensePeriodByAdminCode'");
            string[] split1 = value.Split('|');
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            foreach (var v in split1)
            {
                if (v.Trim().Length == 0)
                {
                    continue;
                }
                string[] split2 = v.Split('^');
                string rawID = split2[0];
                int times = Convert.ToInt32(split2[2]);
                string adminCode = CommonModel.SqlHelper.Query_FirstString($"SELECT AdminCode FROM AdminTime WHERE RawID = {rawID}");
                if (times != 0)
                {
                    dictionary.Add(adminCode, times);
                }
            }
            return dictionary;
        }

        //判斷使用特定頻率及過濾特定頻率
        private bool FilterAdminCode(string adminCode)
        {
            if (SettingModel.PackMode == ePackMode.正常)
                return false;
            bool result = _filterAdminCode.Contains(adminCode);
            return SettingModel.PackMode == ePackMode.過濾特殊 ? result : !result;
        }

        /// <summary>
        /// 判斷指定藥品代碼是否被設為需過濾
        /// </summary>
        /// <param name="medicineCode">頻率</param>
        /// <returns>如果為 <see langword="true"/> 則為需過濾，為 <see langword="false"/> 則為不需過濾</returns>
        private bool FilterCustomizeMedicineCode(string medicineCode)
        {
            return SettingModel.NeedToFilterMedicineCode.Contains(medicineCode);
        }

        private List<string> GetAllMultiAdminCode()
        {
            List<string> result = CommonModel.SqlHelper.Query_List(@"SELECT
	                                                                     AdminCode
                                                                     FROM AdminTime A
                                                                     WHERE DeletedYN = 0 AND A.UpperAdminTimeID is null
                                                                     GROUP BY AdminCode", "AdminCode");
            return result;
        }

        private List<string> GetAllCombiAdminCode()
        {
            List<string> result = CommonModel.SqlHelper.Query_List(@"SELECT
	                                                                     AdminCode
                                                                     FROM AdminTime A
                                                                     WHERE A.AdminCode LIKE 'S%' AND AdminTimeDiv = 1 AND DeletedYN = 0
                                                                     GROUP BY AdminCode", "AdminCode");
            return result;
        }

        private Dictionary<string, List<string>> GetAllMultiAdminCodeTimes()
        {
            List<string> list = CommonModel.SqlHelper.Query_List($@"SELECT
	                                                                    #UpperAdminTime.AdminCode
                                                                    ,	LEFT(CONVERT(VARCHAR,A.StandardTime,108), 5) AS Time
                                                                    ,	A.RawID
                                                                    FROM AdminTime A,
                                                                    (
                                                                        SELECT
	                                                                        RawID
	                                                                    ,	AdminCode
                                                                        FROM AdminTime A
                                                                        WHERE DeletedYN = 0 AND A.UpperAdminTimeID is null AND AdminTimeDiv = 2
                                                                    ) #UpperAdminTime
                                                                    WHERE A.UpperAdminTimeID = #UpperAdminTime.RawID AND A.DeletedYN = 0
                                                                    ORDER BY A.AdminCode,Time", new List<string>() { "AdminCode", "Time" });
            list = list.Where(x => x.Length != 0).Select(x => x).ToList();
            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();
            foreach (var v in list)
            {
                string[] split = v.Split('|');
                string adminCode = split[0];
                string time = split[1];
                if (!result.ContainsKey(adminCode))
                {
                    result.Add(adminCode, new List<string>());
                }
                result[adminCode].Add(time);
            }
            return result;
        }

        /// <summary>
        /// 判斷頻率在餐包及種包的集合
        /// </summary>
        /// <param name="adminCode">頻率</param>
        /// <returns>都存在為 <see langword="true"/> ，有任一個不存在為 <see langword="false"/></returns>
        public bool IsExistsMultiAndCombiAdminCode(string adminCode)
        {
            return WhetherToStopNotHasMultiAdminCode(adminCode) && WhetherToStopNotHasCombiAdminCode(adminCode);
        }

        public bool IsFilterMedicineCode(string medicineCode)
        {
            if (!SettingModel.FilterMedicineCode)
                return false;
            return SettingModel.OnlyCanisterIn ? !GetMedicineCodeHasCanister().Contains(medicineCode) : !GetAllMedicineCode().Contains(medicineCode);
        }

        private List<string> GetMedicineCodeHasCanister()
        {
            List<string> list = CommonModel.SqlHelper.Query_List(@"SELECT
                                                                       A.Mnemonic + ',' + ISNULL(C.MultiMnemonic, '') AS Mnemonic
                                                                   FROM Item A
                                                                   INNER JOIN InventoryContainer B ON A.DeletedYN = 0 AND A.UseYN = 1 AND B.ContainerID IS NOT NULL AND A.RawID = B.ItemID
                                                                   LEFT OUTER JOIN ItemMultiCode C ON A.RawID = C.ItemID AND C.DeletedYN = 0", "Mnemonic");
            List<string> newList = new List<string>();
            foreach (string s in list)
            {
                string[] temp = s.Split(',');
                if (temp[1].Length > 0)
                    newList.Add(temp[1]);
                newList.Add(temp[0]);
            }
            return newList;
        }

        private List<string> GetAllMedicineCode()
        {
            List<string> list = CommonModel.SqlHelper.Query_List(@"SELECT
                                                                       A.Mnemonic + ',' + ISNULL(B.MultiMnemonic, '') AS Mnemonic
                                                                   FROM Item A
                                                                   LEFT OUTER JOIN ItemMultiCode B ON A.RawID = B.ItemID AND B.DeletedYN = 0
                                                                   WHERE A.DeletedYN = 0 AND A.UseYN = 1", "Mnemonic");
            List<string> newList = new List<string>();
            foreach (string s in list)
            {
                string[] temp = s.Split(',');
                if (temp[1].Length > 0)
                    newList.Add(temp[1]);
                newList.Add(temp[0]);
            }
            return newList;
        }

        private Dictionary<string, int> GetJVServerAdminCodeTimes()
        {
            List<string> list = CommonModel.SqlHelper.Query_List(@"SELECT admin_code AS AdminCode, admin_pattern AS Times FROM admincode WHERE time_check = 'S'", new List<string>() { "AdminCode", "Times" });
            Dictionary<string, int> dict = new Dictionary<string, int>();
            foreach (var v in list)
            {
                string[] split = v.Split('|');
                int times = split[1].Count(x => x == '1');
                dict.Add(split[0], times);
            }
            return dict;
        }

        public abstract void ProcessOPD();
        public abstract void LogicOPD();
        public abstract void ProcessUDBatch();
        public abstract void LogicUDBatch();
        public abstract void ProcessUDStat();
        public abstract void LogicUDStat();
        public abstract void ProcessPowder();
        public abstract void LogicPowder();
        public abstract void ProcessOther();
        public abstract void LogicOther();
        public abstract void ProcessCare();
        public abstract void LogicCare();
    }
}