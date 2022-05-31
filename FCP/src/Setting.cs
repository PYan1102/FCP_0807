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

        public static void SetNewValueIntoSettingModel()
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
            _settingModel.FilterAdminCode = jObject[nameof(SettingJsonModel.FilterAdminCode)].ToObject<List<string>>();
            _settingModel.ExtraRandom = jObject[nameof(SettingJsonModel.ExtraRandom)].ToObject<List<RandomInfo>>();
            _settingModel.DoseType = (eDoseType)Enum.Parse(typeof(eDoseType), $"{jObject[nameof(SettingJsonModel.DoseType)]}");
            _settingModel.OutputSpecialAdminCode = $"{jObject[nameof(SettingJsonModel.OutputSpecialAdminCode)]}";
            _settingModel.StatOrBatch = (eDepartment)Enum.Parse(typeof(eDepartment), $"{jObject[nameof(SettingJsonModel.StatOrBatch)]}");
            _settingModel.CrossDayAdminCode = $"{jObject[nameof(SettingJsonModel.CrossDayAdminCode)]}";
            _settingModel.FilterMedicineCode = jObject[nameof(SettingJsonModel.FilterMedicineCode)].ToObject<List<string>>();
            _settingModel.UseStatOrBatch = bool.Parse($"{jObject[nameof(SettingJsonModel.UseStatOrBatch)]}");
            _settingModel.WindowMinimize = bool.Parse($"{jObject[nameof(SettingJsonModel.WindowMinimize)]}");
            _settingModel.ShowWindowOperationButton = bool.Parse($"{jObject[nameof(SettingJsonModel.ShowWindowOperationButton)]}");
            _settingModel.ShowXYParameter = bool.Parse($"{jObject[nameof(SettingJsonModel.ShowXYParameter)]}");
            _settingModel.UseFilterMedicineCode = bool.Parse($"{jObject[nameof(SettingJsonModel.UseFilterMedicineCode)]}");
            _settingModel.FilterNoCanister = bool.Parse($"{jObject[nameof(SettingJsonModel.FilterNoCanister)]}");
            _settingModel.MoveSourceFileToBackupDirectoryWhenDone = bool.Parse($"{jObject[nameof(SettingJsonModel.MoveSourceFileToBackupDirectoryWhenDone)]}");
            _settingModel.StopWhenDone = bool.Parse($"{jObject[nameof(SettingJsonModel.StopWhenDone)]}");
        }
    }
}
