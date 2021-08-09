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
using FCP.MVVM.Models.Enum;
using FCP.MVVM.Factory;

namespace FCP.MVVM.Control
{
    public class Settings
    {
        public string Get
        {
            get
            {
                using (StreamReader sr = new StreamReader(_JsonPath, Encoding.Default))
                {
                    return sr.ReadToEnd();
                }
            }
        }
        private string _JsonPath = $@"{Environment.CurrentDirectory}\Settings.json";
        private Models.SettingsModel _SettingsModel { get; set; }

        public Settings()
        {
            _SettingsModel = SettingsFactory.GenerateSettingsModels();
            if (!IsJsonFileExists())
                CreateJsonFile();
            JObject json = JObject.Parse(Get);
            GetParameters(json);
        }

        private bool IsJsonFileExists()
        {
            bool result = File.Exists(_JsonPath);
            return result;
        }

        private void CreateJsonFile()
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
                DoseMode = 0,
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

        private void Save(object obj)
        {
            using (StreamWriter sw = new StreamWriter(_JsonPath, false, Encoding.Default))
            {
                sw.Write(JsonConvert.SerializeObject(obj));
            }
        }

        public void GetParameters(object obj)
        {
            var v = JObject.FromObject(obj);
            _SettingsModel.InputPath1 = $"{v["InputPath1"]}";
            _SettingsModel.InputPath2 = $"{v["InputPath2"]}";
            _SettingsModel.InputPath3 = $"{v["InputPath3"]}";
            _SettingsModel.OutputPath1 = $"{v["OutputPath1"]}";
            _SettingsModel.DeputyFileName = $"{v["DeputyFileName"]}";
            _SettingsModel.EN_AutoStart = bool.Parse($"{v["EN_AutoStart"]}");
            _SettingsModel.Mode = (Format)Enum.Parse(typeof(Format), $"{v["Mode"]}");
            _SettingsModel.Speed = Convert.ToInt32($"{v["Speed"]}");
            _SettingsModel.PackMode = (PackMode)Enum.Parse(typeof(PackMode), $"{v["PackMode"]}");
            _SettingsModel.AdminCodeFilter = $"{v["AdminCodeFilter"]}".Split(',').ToList();
            _SettingsModel.AdminCodeUse = $"{v["AdminCodeUse"]}".Split(',').ToList();
            _SettingsModel.ExtraRandom = $"{v["ExtraRandom"]}";
            _SettingsModel.DoseMode = (DoseMode)Enum.Parse(typeof(DoseMode), $"{v["DoseMode"]}");
            _SettingsModel.OppositeAdminCode = $"{v["OppositeAdminCode"]}".Split(',').ToList();
            _SettingsModel.StatOrBatch = $"{v["StatOrBatch"]}";
            _SettingsModel.CutTime = $"{v["CutTime"]}";
            _SettingsModel.CrossDayAdminCode = $"{v["CrossDayAdminCode"]}".Split(',').ToList();
            _SettingsModel.FilterMedicineCode = $"{v["FilterMedicineCode"]}".Split(',').ToList();
            _SettingsModel.EN_StatOrBatch = bool.Parse($"{v["EN_StatOrBatch"]}");
            _SettingsModel.EN_WindowMinimumWhenOpen = bool.Parse($"{v["EN_WindowMinimumWhenOpen"]}");
            _SettingsModel.EN_ShowControlButton = bool.Parse($"{v["EN_ShowControlButton"]}");
            _SettingsModel.EN_ShowXY = bool.Parse($"{v["EN_ShowXY"]}");
            _SettingsModel.EN_FilterMedicineCode = bool.Parse($"{v["EN_FilterMedicineCode"]}");
            _SettingsModel.EN_OnlyCanisterIn = bool.Parse($"{v["EN_OnlyCanisterIn"]}");
            _SettingsModel.FilterMedicineCode.Sort();
        }

        public void SaveMainWidow(string inputPath1,
            string inputPath2,
            string inputPath3,
            string outputPath,
            bool isAutoStart,
            string isStat)
        {
            var json = new
            {
                InputPath1 = inputPath1,
                InputPath2 = inputPath2,
                InputPath3 = inputPath3,
                OutputPath1 = outputPath,
                _SettingsModel.DeputyFileName,
                EN_AutoStart = isAutoStart,
                _SettingsModel.Mode,
                _SettingsModel.Speed,
                _SettingsModel.PackMode,
                AdminCodeFilter = ConvertListToString(_SettingsModel.AdminCodeFilter),
                AdminCodeUse = ConvertListToString(_SettingsModel.AdminCodeUse),
                _SettingsModel.ExtraRandom,
                _SettingsModel.DoseMode,
                OppositeAdminCode = ConvertListToString(_SettingsModel.OppositeAdminCode),
                StatOrBatch = isStat,
                _SettingsModel.CutTime,
                CrossDayAdminCode = ConvertListToString(_SettingsModel.CrossDayAdminCode),
                FilterMedicineCode = ConvertListToString(_SettingsModel.FilterMedicineCode),
                _SettingsModel.EN_StatOrBatch,
                _SettingsModel.EN_WindowMinimumWhenOpen,
                _SettingsModel.EN_ShowControlButton,
                _SettingsModel.EN_ShowXY,
                _SettingsModel.EN_FilterMedicineCode,
                _SettingsModel.EN_OnlyCanisterIn
            };
            Save(json);
            GetParameters(json);
        }  //Form1存檔

        public void SaveAdvancedSettings(string deputyFileName,
            Format mode,
            int speed,
            PackMode packMode,
            string adminCodeFilter,
            string adminCodeUse,
            string random,
            DoseMode doseMode,
            string oppositeAdminCode,
            string cutTime,
            string crossDayAdminCode,
            string filterMedicineCode,
            bool isOpenStatBatch,
            bool isOpenWindowMinimum,
            bool isOpenShowControlButton,
            bool isOpenShowXY,
            bool isOpenFilterMedicineCode,
            bool isOnlyLetCanisterMedicineEnter)
        {
            var json = new
            {
                _SettingsModel.InputPath1,
                _SettingsModel.InputPath2,
                _SettingsModel.InputPath3,
                _SettingsModel.OutputPath1,
                DeputyFileName = $"*.{deputyFileName}",
                _SettingsModel.EN_AutoStart,
                Mode = mode,
                Speed = speed,
                PackMode = packMode,
                AdminCodeFilter = adminCodeFilter,
                AdminCodeUse = adminCodeUse,
                ExtraRandom = random,
                DoseMode = doseMode,
                OppositeAdminCode = oppositeAdminCode,
                _SettingsModel.StatOrBatch,
                CutTime = cutTime,
                CrossDayAdminCode = crossDayAdminCode,
                FilterMedicineCode = filterMedicineCode,
                EN_StatOrBatch = isOpenStatBatch,
                EN_WindowMinimumWhenOpen = isOpenWindowMinimum,
                EN_ShowControlButton = isOpenShowControlButton,
                EN_ShowXY = isOpenShowXY,
                EN_FilterMedicineCode = isOpenFilterMedicineCode,
                EN_OnlyCanisterIn = isOnlyLetCanisterMedicineEnter
            };
            Save(json);
            GetParameters(json);
        }  //Form2存檔

        private string ConvertListToString(List<string> list)
        {
            StringBuilder stringBuilder = new StringBuilder();
            list.ForEach(x => stringBuilder.Append($"{x},"));
            string result = stringBuilder.ToString().TrimEnd(',');
            return result;
        }
    }
}
