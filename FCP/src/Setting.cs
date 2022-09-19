using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using FCP.src.Enum;
using Helper;
using FCP.Models;
using FCP.src.Factory.Models;

namespace FCP.Services
{
    public class Setting
    {
        private static string _settingFilePath = $@"{Environment.CurrentDirectory}\Setting.json";
        private static SettingJsonModel _settingModel => ModelsFactory.GenerateSettingModel();

        public static void Init()
        {
            JsonHelper.SetJsonPath(_settingFilePath);
            if (!IsSettingFileExists())
                GenerateSettingFile();
            SetNewValueIntoSettingModel();
        }

        public static void Save(object obj)
        {
            JsonHelper.Save(JObject.FromObject(obj));
            SetNewValueIntoSettingModel();
        }

        private static bool IsSettingFileExists()
        {
            return File.Exists(_settingFilePath);
        }

        private static void GenerateSettingFile()
        {
            Save(new SettingJsonModel());
        }

        private static void SetNewValueIntoSettingModel()
        {
            try
            {
                var jObject = JsonHelper.Analysis();
                _settingModel.InputDirectory1 = $"{jObject[nameof(SettingJsonModel.InputDirectory1)]}";
                _settingModel.InputDirectory2 = $"{jObject[nameof(SettingJsonModel.InputDirectory2)]}";
                _settingModel.InputDirectory3 = $"{jObject[nameof(SettingJsonModel.InputDirectory3)]}";
                _settingModel.InputDirectory4 = $"{jObject[nameof(SettingJsonModel.InputDirectory4)]}";
                _settingModel.InputDirectory5 = $"{jObject[nameof(SettingJsonModel.InputDirectory5)]}";
                _settingModel.InputDirectory6 = $"{jObject[nameof(SettingJsonModel.InputDirectory6)]}";
                _settingModel.OutputDirectory = $"{jObject[nameof(SettingJsonModel.OutputDirectory)]}";
                _settingModel.FileExtensionName = $"{jObject[nameof(SettingJsonModel.FileExtensionName)]}";
                _settingModel.AutoStart = bool.Parse($"{jObject[nameof(SettingJsonModel.AutoStart)]}");
                _settingModel.Format = (eFormat)Enum.Parse(typeof(eFormat), $"{jObject[nameof(SettingJsonModel.Format)]}");
                _settingModel.Speed = Convert.ToInt32($"{jObject[nameof(SettingJsonModel.Speed)]}");
                _settingModel.PackMode = (ePackMode)Enum.Parse(typeof(ePackMode), $"{jObject[nameof(SettingJsonModel.PackMode)]}");
                _settingModel.NeedToFilterAdminCode = jObject[nameof(SettingJsonModel.NeedToFilterAdminCode)].ToObject<List<string>>();
                _settingModel.ExtraRandom = jObject[nameof(SettingJsonModel.ExtraRandom)].ToObject<List<RandomInfo>>();
                _settingModel.DoseType = (eDoseType)Enum.Parse(typeof(eDoseType), $"{jObject[nameof(SettingJsonModel.DoseType)]}");
                _settingModel.OutputSpecialAdminCode = $"{jObject[nameof(SettingJsonModel.OutputSpecialAdminCode)]}";
                _settingModel.StatOrBatch = (eDepartment)Enum.Parse(typeof(eDepartment), $"{jObject[nameof(SettingJsonModel.StatOrBatch)]}");
                _settingModel.CrossDayAdminCode = $"{jObject[nameof(SettingJsonModel.CrossDayAdminCode)]}";
                _settingModel.NeedToFilterMedicineCode = jObject[nameof(SettingJsonModel.NeedToFilterMedicineCode)].ToObject<List<string>>();
                _settingModel.UseStatAndBatchOption = bool.Parse($"{jObject[nameof(SettingJsonModel.UseStatAndBatchOption)]}");
                _settingModel.MinimizeWindowWhenProgramStart = bool.Parse($"{jObject[nameof(SettingJsonModel.MinimizeWindowWhenProgramStart)]}");
                _settingModel.ShowCloseAndMinimizeButton = bool.Parse($"{jObject[nameof(SettingJsonModel.ShowCloseAndMinimizeButton)]}");
                _settingModel.ShowXY = bool.Parse($"{jObject[nameof(SettingJsonModel.ShowXY)]}");
                _settingModel.FilterMedicineCode = bool.Parse($"{jObject[nameof(SettingJsonModel.FilterMedicineCode)]}");
                _settingModel.OnlyCanisterIn = bool.Parse($"{jObject[nameof(SettingJsonModel.OnlyCanisterIn)]}");
                _settingModel.WhenCompeletedMoveFile = bool.Parse($"{jObject[nameof(SettingJsonModel.WhenCompeletedMoveFile)]}");
                _settingModel.WhenCompeletedStop = bool.Parse($"{jObject[nameof(SettingJsonModel.WhenCompeletedStop)]}");
                _settingModel.ETCData = jObject[nameof(SettingJsonModel.ETCData)].ToObject<List<ETCInfo>>();
            }
            catch (Exception ex)
            {
                LogService.Exception(ex);
            }
        }
    }
}
