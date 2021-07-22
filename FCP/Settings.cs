using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FCP
{
    public class Settings
    {
        string JsonPath = $@"{Environment.CurrentDirectory}\Settings.json";
        public string InputPath1 {get;set;}
        public string InputPath2 {get;set;}
        public string InputPath3 {get;set;}
        public string OutputPath1 {get;set;}
        public string DeputyFileName {get;set;}
        public bool EN_AutoStart {get;set;}
        public int Mode {get;set;}
        public int Speed {get;set;}
        public int PackMode { get; set; }
        public List<string> AdminCodeFilter {get;set;}
        public List<string> AdminCodeUse {get;set;}
        public string ExtraRandom {get;set;}
        public string DoseMode {get;set;}
        public List<string> OppositeAdminCode {get;set;}
        public string StatOrBatch {get;set;}
        public string CutTime {get;set;}
        public List<string> CrossDayAdminCode {get;set;}
        public List<string> FilterMedicineCode { get; set; }
        public bool EN_StatOrBatch { get;set;}
        public bool EN_WindowMinimumWhenOpen { get;set;}
        public bool EN_ShowControlButton { get; set; }
        public bool EN_ShowXY { get; set; }
        public bool EN_FilterMedicineCode { get; set; }
        public bool EN_OnlyCanisterIn { get; set; }
        public enum ModeEnum
        {
            JVS = 0, 創聖 = 1, 醫聖 = 2, 小港醫院 = 3, 光田OnCube = 4, 光田JVS = 5, 民生醫院 = 6, 宏彥診所 = 7, 義大醫院 = 8, 長庚磨粉 = 9, 長庚醫院 = 10, 台北看守所 = 11,
            仁康醫院 = 12
        }

        public enum PackModeEnum
        {
            正常 = 0, 使用特殊 = 1, 過濾特殊 = 2
        }

        public string Get
        {
            get
            {
                using (StreamReader sr = new StreamReader(JsonPath, Encoding.Default))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        public void Check() //檢查Settings.json
        {
            if (!File.Exists(JsonPath))
                Create();
            var v = JObject.Parse(Get);
            Analysis(v);
        }

        public void Create() //重新建立Settings.json
        {
            var json = new
            {
                InputPath1 = "",
                InputPath2 = "",
                InputPath3 = "",
                OutputPath1 = "",
                DeputyFileName = "*.txt",
                EN_AutoStart = false,
                Mode = 0,
                Speed = 100,
                PackMode = 0,
                AdminCodeFilter = "",
                AdminCodeUse = "",
                ExtraRandom = "",
                DoseMode = "M",
                OppositeAdminCode = "",
                StatOrBatch = "S",
                CutTime = "",
                CrossDayAdminCode = "",
                FilterMedicineCode = "",
                EN_StatOrBatch = false,
                EN_WindowMinimumWhenOpen = false,
                EN_ShowControlButton = true,
                EN_ShowXY = false,
                EN_FilterMedicineCode = false,
                EN_OnlyCanisterIn = false
            };
            Save(json);
        }

        private void Save(object o)
        {
            using (StreamWriter sw = new StreamWriter(JsonPath, false, Encoding.Default))
            {
                sw.Write(JsonConvert.SerializeObject(o));
            }
        }

        public void Analysis(object o) //分析Settings.json的資料
        {
            var v = JObject.FromObject(o);
            InputPath1 = $"{v["InputPath1"]}";
            InputPath2 = $"{v["InputPath2"]}";
            InputPath3 = $"{v["InputPath3"]}";
            OutputPath1 = $"{v["OutputPath1"]}";
            DeputyFileName = $"{v["DeputyFileName"]}";
            EN_AutoStart = bool.Parse($"{v["EN_AutoStart"]}");
            Mode = Convert.ToInt32($"{v["Mode"]}");
            Speed = Convert.ToInt32($"{v["Speed"]}");
            PackMode = Convert.ToInt32($"{v["PackMode"]}");
            AdminCodeFilter = $"{v["AdminCodeFilter"]}".Split(',').ToList();
            AdminCodeUse = $"{v["AdminCodeUse"]}".Split(',').ToList();
            ExtraRandom = $"{v["ExtraRandom"]}";
            DoseMode = $"{v["DoseMode"]}";
            OppositeAdminCode = $"{v["OppositeAdminCode"]}".Split(',').ToList();
            StatOrBatch = $"{v["StatOrBatch"]}";
            CutTime = $"{v["CutTime"]}";
            CrossDayAdminCode = $"{v["CrossDayAdminCode"]}".Split(',').ToList();
            FilterMedicineCode = $"{v["FilterMedicineCode"]}".Split(',').ToList();
            EN_StatOrBatch = bool.Parse($"{v["EN_StatOrBatch"]}");
            EN_WindowMinimumWhenOpen = bool.Parse($"{v["EN_WindowMinimumWhenOpen"]}");
            EN_ShowControlButton = bool.Parse($"{v["EN_ShowControlButton"]}");
            EN_ShowXY = bool.Parse($"{v["EN_ShowXY"]}");
            EN_FilterMedicineCode = bool.Parse($"{v["EN_FilterMedicineCode"]}");
            EN_OnlyCanisterIn = bool.Parse($"{v["EN_OnlyCanisterIn"]}");
            FilterMedicineCode.Sort();
        }

        public void SaveForm1(string IP1, string IP2, string IP3, string OP, bool EN_AutoStart, string IsStat)
        {
            string A = "", B = "", C = "", D = "", E = "";
            AdminCodeFilter.ForEach(x => A += $"{x},");
            AdminCodeUse.ForEach(x => B += $"{x},");
            OppositeAdminCode.ForEach(x => C += $"{x},");
            CrossDayAdminCode.ForEach(x => D += $"{x},");
            FilterMedicineCode.ForEach(x => E += $"{x},");
            A = A.TrimEnd(',');
            B = B.TrimEnd(',');
            C = C.TrimEnd(',');
            D = D.TrimEnd(',');
            E = E.TrimEnd(',');
            var json = new
            {
                InputPath1 = IP1,
                InputPath2 = IP2,
                InputPath3 = IP3,
                OutputPath1 = OP,
                DeputyFileName = DeputyFileName,
                EN_AutoStart = EN_AutoStart,
                Mode,
                Speed,
                PackMode,
                AdminCodeFilter = A,
                AdminCodeUse = B,
                ExtraRandom,
                DoseMode,
                OppositeAdminCode = C,
                StatOrBatch = IsStat,
                CutTime,
                CrossDayAdminCode = D,
                FilterMedicineCode = E,
                EN_StatOrBatch,
                EN_WindowMinimumWhenOpen,
                EN_ShowControlButton,
                EN_ShowXY,
                EN_FilterMedicineCode,
                EN_OnlyCanisterIn
            };
            Save(json);
            Analysis(json);
        }  //Form1存檔

        public void SaveForm2(string DeputyFileName, int Mode, int Speed, int PackMode, string Filter, string Use, string Random, string DoseMode, string OppositeAdminCode,
            string CutTime, string CrossDayAdminCode,string FilterMedicineCode, bool EN_StatOrBatch, bool EN_WindowMinimum, bool EN_ShowControlButton, bool EN_ShowXY,
            bool EN_FilterMedicineCode, bool EN_OnlyCanisterIn)
        {
            Debug.WriteLine(FilterMedicineCode);
            var json = new
            {
                InputPath1,
                InputPath2,
                InputPath3,
                OutputPath1,
                DeputyFileName = $"*.{DeputyFileName}",
                EN_AutoStart,
                Mode = Mode,
                Speed = Speed,
                PackMode = PackMode,
                AdminCodeFilter = Filter,
                AdminCodeUse = Use,
                ExtraRandom = Random,
                DoseMode = DoseMode,
                OppositeAdminCode = OppositeAdminCode,
                StatOrBatch,
                CutTime = CutTime,
                CrossDayAdminCode = CrossDayAdminCode,
                FilterMedicineCode = FilterMedicineCode,
                EN_StatOrBatch = EN_StatOrBatch,
                EN_WindowMinimumWhenOpen = EN_WindowMinimum,
                EN_ShowControlButton = EN_ShowControlButton,
                EN_ShowXY = EN_ShowXY,
                EN_FilterMedicineCode = EN_FilterMedicineCode,
                EN_OnlyCanisterIn = EN_OnlyCanisterIn
            };
            Save(json);
            Analysis(json);
        }  //Form2存檔
    }
}
