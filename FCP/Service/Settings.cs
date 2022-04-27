using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json.Linq;
using FCP.src.Enum;
using Helper;
using FCP.src.Factory.Models;
using FCP.Models;

namespace FCP.Service
{
    public class Settings
    {
        private string _JsonPath = $@"{Environment.CurrentDirectory}\Settings.json";
        private SettingsModel _SettingsModel;

        public Settings()
        {
            _SettingsModel = SettingsFactory.GenerateSettingsModel();
            JsonHelper.SetJsonPath(_JsonPath);
            if (!IsJsonFileExists())
                CreateJsonFile();
            GetParameters();
        }

        private bool IsJsonFileExists()
        {
            return File.Exists(_JsonPath);
        }

        private void CreateJsonFile()
        {
            var json = new
            {
                InputPath1 = "",
                InputPath2 = "",
                InputPath3 = "",
                OutputPath = "",
                FileExtensionName = "txt",
                EN_AutoStart = false,
                Mode = 0,
                Speed = 500,
                PackMode = 0,
                FilterAdminCode = "",
                ExtraRandom = "",
                DoseType = 0,
                OutputSpecialAdminCode = "",
                StatOrBatch = "S",
                CutTime = "",
                CrossDayAdminCode = "",
                FilterMedicineCode = "",
                EN_StatOrBatch = false,
                EN_WindowMinimumWhenOpen = false,
                EN_ShowControlButton = true,
                EN_ShowXY = false,
                EN_FilterMedicineCode = false,
                EN_OnlyCanisterIn = false,
                EN_WhenCompeletedMoveFile = true,
                EN_WhenCompeletedStop = false
            };
            Save(json);
        }

        private void Save(object obj)
        {
            JsonHelper.Save(JObject.FromObject(obj));
        }

        public void GetParameters()
        {
            var v = JsonHelper.Analysis();
            _SettingsModel.InputPath1 = $"{v["InputPath1"]}";
            _SettingsModel.InputPath2 = $"{v["InputPath2"]}";
            _SettingsModel.InputPath3 = $"{v["InputPath3"]}";
            _SettingsModel.OutputPath = $"{v["OutputPath"]}";
            _SettingsModel.FileExtensionName = $"{v["FileExtensionName"]}";
            _SettingsModel.EN_AutoStart = bool.Parse($"{v["EN_AutoStart"]}");
            _SettingsModel.Mode = (eFormat)Enum.Parse(typeof(eFormat), $"{v["Mode"]}");
            _SettingsModel.Speed = Convert.ToInt32($"{v["Speed"]}");
            _SettingsModel.PackMode = (ePackMode)Enum.Parse(typeof(ePackMode), $"{v["PackMode"]}");
            _SettingsModel.FilterAdminCode = $"{v["FilterAdminCode"]}".Split(',').ToList();
            _SettingsModel.ExtraRandom = $"{v["ExtraRandom"]}";
            _SettingsModel.DoseType = (eDoseType)Enum.Parse(typeof(eDoseType), $"{v["DoseType"]}");
            _SettingsModel.OutputSpecialAdminCode = $"{v["OutputSpecialAdminCode"]}".Split(',').ToList();
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
            _SettingsModel.EN_WhenCompeletedMoveFile = bool.Parse($"{v["EN_WhenCompeletedMoveFile"]}");
            _SettingsModel.EN_WhenCompeletedStop = bool.Parse($"{v["EN_WhenCompeletedStop"]}");

            _SettingsModel.FilterAdminCode.RemoveAll(x => x.Length == 0);
            _SettingsModel.OutputSpecialAdminCode.RemoveAll(x => x.Length == 0);
            _SettingsModel.CrossDayAdminCode.RemoveAll(x => x.Length == 0);
            _SettingsModel.FilterMedicineCode.RemoveAll(x => x.Length == 0);
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
                OutputPath = outputPath,
                _SettingsModel.FileExtensionName,
                EN_AutoStart = isAutoStart,
                _SettingsModel.Mode,
                _SettingsModel.Speed,
                _SettingsModel.PackMode,
                FilterAdminCode = ConvertListToString(_SettingsModel.FilterAdminCode),
                _SettingsModel.ExtraRandom,
                _SettingsModel.DoseType,
                OutputSpecialAdminCode = ConvertListToString(_SettingsModel.OutputSpecialAdminCode),
                StatOrBatch = isStat,
                _SettingsModel.CutTime,
                CrossDayAdminCode = ConvertListToString(_SettingsModel.CrossDayAdminCode),
                FilterMedicineCode = ConvertListToString(_SettingsModel.FilterMedicineCode),
                _SettingsModel.EN_StatOrBatch,
                _SettingsModel.EN_WindowMinimumWhenOpen,
                _SettingsModel.EN_ShowControlButton,
                _SettingsModel.EN_ShowXY,
                _SettingsModel.EN_FilterMedicineCode,
                _SettingsModel.EN_OnlyCanisterIn,
                _SettingsModel.EN_WhenCompeletedMoveFile,
                _SettingsModel.EN_WhenCompeletedStop
            };
            Save(json);
            GetParameters();
        }  //Form1存檔

        public void SaveAdvancedSettings(
            eFormat mode,
            int speed,
            ePackMode packMode,
            string filterAdminCode,
            string extraRandom,
            eDoseType doseType,
            string outputSpecialAdminCode,
            string cutTime,
            string crossDayAdminCode,
            string filterMedicineCode,
            string fileExtensionName,
            bool isOpenStatBatch,
            bool isOpenWindowMinimum,
            bool isOpenShowControlButton,
            bool isOpenShowXY,
            bool isOpenFilterMedicineCode,
            bool isOnlyLetCanisterMedicineEnter,
            bool isWhenCompeletedMoveFile,
            bool isWhenCompeletedStop)
        {
            var json = new
            {
                _SettingsModel.InputPath1,
                _SettingsModel.InputPath2,
                _SettingsModel.InputPath3,
                _SettingsModel.OutputPath,
                FileExtensionName = fileExtensionName,
                _SettingsModel.EN_AutoStart,
                Mode = mode,
                Speed = speed,
                PackMode = packMode,
                FilterAdminCode = filterAdminCode,
                ExtraRandom = extraRandom,
                DoseType = doseType,
                OutputSpecialAdminCode = outputSpecialAdminCode,
                _SettingsModel.StatOrBatch,
                CutTime = cutTime,
                CrossDayAdminCode = crossDayAdminCode,
                FilterMedicineCode = filterMedicineCode,
                EN_StatOrBatch = isOpenStatBatch,
                EN_WindowMinimumWhenOpen = isOpenWindowMinimum,
                EN_ShowControlButton = isOpenShowControlButton,
                EN_ShowXY = isOpenShowXY,
                EN_FilterMedicineCode = isOpenFilterMedicineCode,
                EN_OnlyCanisterIn = isOnlyLetCanisterMedicineEnter,
                EN_WhenCompeletedMoveFile = isWhenCompeletedMoveFile,
                EN_WhenCompeletedStop = isWhenCompeletedStop
            };
            Save(json);
            GetParameters();
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
