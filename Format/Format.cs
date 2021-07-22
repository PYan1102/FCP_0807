using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace Format
{
    public class Format
    {
        string OutputPath = "";
        string FileName = "";
        string PatientName = "";
        string FileNameBeforeProcess = "";
        string Time = "";
        string UsageCodePath = "";
        string ExtraSettingsPath = "";
        List<string> FilterSpecialTimeList = new List<string>();
        List<string> UseSpecialAdminTimeList = new List<string>();
        string[][] TimesPerDay = null;

        public void BasicData(string FNBP, string OP, string FN, string PN, string T, string P)
        {
            FileNameBeforeProcess = FNBP;
            OutputPath = OP;
            FileName = FN;
            PatientName = PN;
            Time = T;
            UsageCodePath = $@"Resources\UsageCode\{P}\UsageCode.txt";
            ExtraSettingsPath = $@"Resources\ExtraSettings\{P}\ExtraSettings.txt";
        }
        public Tuple<ReturnPackagedOfSeparateTime> PackagedOfSeparateTime(Dictionary<int, List<string>> dic, List<string> AdminTime)
        {
            Dictionary<string, List<int>> DispenseList = new Dictionary<string, List<int>>();
            List<int> Count = new List<int>();
            List<string> TimeList = new List<string>();
            string ReturnException = "";
            int count = 0;
            foreach (string MedicineAdminTime in AdminTime)
            {
                var a = from time in dic
                        from time1 in time.Value
                        where time1 == MedicineAdminTime.Trim()
                        select time.Key;
                foreach (var v in a)
                {
                    Count.Clear();
                    Count.Add(count);
                    string TimeOrder;
                    if (v.ToString().Length == 1)
                        TimeOrder = "0" + v;
                    else
                        TimeOrder = v.ToString();
                    if (!DispenseList.ContainsKey($@"{OutputPath}\{PatientName.Trim()}-{FileName}_{Time}_{v}.txt"))
                    {
                        DispenseList.Add($@"{OutputPath}\{PatientName.Trim()}-{FileName}_{Time}_{v}.txt", Count.ToList());
                        TimeList.Add(TimeOrder);
                    }
                    else
                        DispenseList[$@"{OutputPath}\{PatientName.Trim()}-{FileName}_{Time}_{v}.txt"].Add(count);
                }
                if (a.Count() == 0)
                    ReturnException = FileNameBeforeProcess + " UsageCode.txt缺少該檔案中的頻率 " + MedicineAdminTime.Trim();
                count += 1;
            }
            var SortDispenseList = from v in DispenseList
                                   let key = v.Key
                                   let s1 = key.Substring(key.LastIndexOf("_") + 1, key.LastIndexOf(".") - key.LastIndexOf("_") - 1)
                                   orderby Int32.Parse(s1)
                                   select v;
            TimeList.Sort();
            List<string> Name = new List<string>();
            Dictionary<string, List<int>> FinalDic = new Dictionary<string, List<int>>();
            foreach (var v in SortDispenseList)
                Name.Add(v.Key);
            int or = 0;
            foreach (var v in SortDispenseList)
            {
                string s = v.Key;
                FinalDic.Add(s.Insert(s.LastIndexOf("_") + 1, or.ToString(0 + "_")), v.Value);
                or += 1;
            }
            ReturnPackagedOfSeparateTime ReturnResult = new ReturnPackagedOfSeparateTime()
            {
                FinalDic = FinalDic,
                TimeList = TimeList,
                Exception = ReturnException
            };
            return new Tuple<ReturnPackagedOfSeparateTime>(ReturnResult);
        }  //早中晚功能
        public class ReturnPackagedOfSeparateTime
        {
            public Dictionary<string, List<int>> FinalDic { get; set; }
            public List<string> TimeList { get; set; }
            public string Exception { get; set; }
        }
        public void SetAdminTime(string FilterSpecial, string UseSpecial)
        {
            string[] FilterSpecialAdminTime;
            string[] UseSpecialAdminTime;
            FilterSpecialAdminTime = FilterSpecial.Split(',');
            UseSpecialAdminTime = UseSpecial.Split(',');
            foreach (string s in FilterSpecialAdminTime)
            {
                if (!string.IsNullOrEmpty(s))
                    FilterSpecialTimeList.Add(s);
            }
            foreach (string s in UseSpecialAdminTime)
            {
                if (!string.IsNullOrEmpty(s))
                    UseSpecialAdminTimeList.Add(s);
            }
        }  //設定使用特定頻率、過濾特定頻率的頻率
        public bool JudgePackedType(string PackedType, string AdminTime)
        {
            if (PackedType == "FilterSpecialAdminTime")
            {
                if (FilterSpecialTimeList.Contains(AdminTime))
                    return true;
            }
            else if (PackedType == "UseSpecialAdminTime")
            {
                if (UseSpecialAdminTimeList.Contains(AdminTime))
                    return false;
                else
                    return true;
            }
            return false;
        }  //判斷使用特定頻率及過濾特定頻率
        public Dictionary<int, List<string>> GetUsageCodeContent()
        {
            Dictionary<int, List<string>> dic = new Dictionary<int, List<string>>();
            List<string> list = new List<string>();
            List<string> list1;
            StreamReader sr = new StreamReader(UsageCodePath, Encoding.Default);
            string content = sr.ReadToEnd();
            sr.Close();
            string[] SplitRow = content.Split('\n');
            for (int x = 0; x <= 23; x++)
            {
                list.Clear();
                string[] ss = SplitRow[x].Substring(SplitRow[x].IndexOf("=") + 1, SplitRow[x].Length - SplitRow[x].IndexOf("=") - 1).Split(',');
                if (ss.Length > 0)
                {
                    foreach (string s in ss)
                        list.Add(s.Trim());
                }
                list1 = list.ToList();
                dic.Add(x, list1);
            }
            return dic;
        }
        public void UsageCodeFileCheck()
        {
            if (!File.Exists(UsageCodePath))
                ReWriteUsageCode();
            else
            {
                StreamReader read = new StreamReader(UsageCodePath);
                string s = read.ReadToEnd();
                read.Close();
                string[] list = s.Split('\n');
                if (list.Length <= 23)
                    ReWriteUsageCode();
            }
        }  //檢查是否有usagecode.txt
        private void ReWriteUsageCode()
        {
            using (StreamWriter admintime = new StreamWriter(UsageCodePath))
            {
                for (int x = 0; x <= 23; x++)
                {
                    if (x <= 22)
                        admintime.WriteLine(x + "=");
                    else
                        admintime.Write(x + "=");
                }
                admintime.Close();
            }
        }  //重寫usagecode.txt
        public float CalculationRemainder(List<int> order, List<int> ID1, List<string> TotalQuanity)
        {
            float a = 0;
            List<float> re = new List<float>();
            for (int r = 0; r <= order.Count - 1; r++)
            {
                float id1;
                id1 = Convert.ToSingle(ID1[r].ToString());
                float correcttotalquanity;
                float remainder;
                if (id1 > 0)
                {
                    correcttotalquanity = Convert.ToSingle(TotalQuanity[r].ToString());
                    remainder = Convert.ToSingle(Math.Floor(correcttotalquanity % id1));  //總量除以ID1的餘數
                }
                else
                    remainder = Convert.ToSingle(TotalQuanity[r].ToString());
                re.Add(remainder);
            }
            foreach (float f in re)
                a += f;
            return a;
        }  //計算總量除以ID1的餘數
        public Tuple<ReturnExtraSettings> FilterGivenMedicineCode(List<string> extralist)
        {
            TimesPerDay = new string[24][];
            int c = 0;
            Dictionary<string, string> JudgeMedicineGivenDic = new Dictionary<string, string>();
            foreach (string s in extralist)
            {
                if (s.Contains("MedicineCodeGiven"))
                {
                    int i = s.IndexOf("]");
                    string[] codegiven = s.Substring(i + 1, s.Length - i - 1).Trim().Split(',');
                    foreach (string a in codegiven)
                        JudgeMedicineGivenDic[a] = a;
                }
                else
                {
                    int j = s.IndexOf("]");
                    string[] admintimevalue = s.Substring(j + 1, s.Length - j - 1).Trim().Split('\n');
                    foreach (string a in admintimevalue)
                    {
                        int numberlocattion = a.IndexOf("=");
                        string number = a.Substring(0, numberlocattion);
                        TimesPerDay[Int32.Parse(number) - 1] = a.Substring(numberlocattion + 1, a.Length - numberlocattion - 1).Trim().Split(',');
                    }
                }
                c += 1;
            }
            ReturnExtraSettings re = new ReturnExtraSettings()
            {
                JudgeMedicineGivenDic = JudgeMedicineGivenDic,
                TimesPerDay = TimesPerDay
            };
            return new Tuple<ReturnExtraSettings>(re);
        }

        public class ReturnExtraSettings
        {
            public Dictionary<string, string> JudgeMedicineGivenDic { get; set; }
            public string[][] TimesPerDay { get; set; }
        }
        public string GetExteraSettingsAdminTimeTimes(string AdminTime)
        {
            for (int x = 1; x <= 24; x++)
            {
                foreach (string s in TimesPerDay[x - 1])
                {
                    if (AdminTime == s.Trim())
                        return x.ToString();
                }
            }
            return "ExtraSettings.txt 中沒有該頻率  " + AdminTime;
        }  //取得ExtraSettings.txt的頻率次數
        public string GetUsageCodeAdminTimeTimes(string AdminTime)
        {
            string Content;
            StreamReader sr = new StreamReader(UsageCodePath, Encoding.Default);
            Content = sr.ReadToEnd();
            sr.Close();
            string[] ContentSplit = Content.Split('\n');
            var v = from S in ContentSplit
                    let Data = S.Substring(S.IndexOf("=") + 1, S.Length - S.IndexOf("=") - 1).Trim()
                    from AdminTimeSplit in Data.Split(',')
                    where AdminTimeSplit == AdminTime
                    select AdminTimeSplit;
            return v.ToList().Count.ToString();
        }  //取得usagecode.txt的頻率次數
        public string GetFileInfo(string FilePath)
        {
            DateTime FileTime = Directory.GetLastWriteTime(FilePath);
            if (FileTime.ToString().Contains("下午"))
                return (Convert.ToInt32(Directory.GetLastWriteTime(FilePath).ToString("HH")) + 12).ToString() + FileTime.ToString("-mm");
            else
                return FileTime.ToString("HH-mm");
        }
    }
}
