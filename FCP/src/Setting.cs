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
        private static SettingModel _settingModel => ModelsFactory.GenerateSettingModel();

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
            Save(new SettingModel());
        }

        public static void SetNewValueIntoSettingModel()
        {
            var jObject = JsonHelper.Analysis();
            _settingModel.InputDirectory1 = $"{jObject[nameof(SettingModel.InputDirectory1)]}";
            _settingModel.InputDirectory2 = $"{jObject[nameof(SettingModel.InputDirectory2)]}";
            _settingModel.InputDirectory3 = $"{jObject[nameof(SettingModel.InputDirectory3)]}";
            _settingModel.InputDirectory4 = $"{jObject[nameof(SettingModel.InputDirectory4)]}";
            _settingModel.InputDirectory5 = $"{jObject[nameof(SettingModel.InputDirectory5)]}";
            _settingModel.InputDirectory6 = $"{jObject[nameof(SettingModel.InputDirectory6)]}";
            _settingModel.OutputDirectory = $"{jObject[nameof(SettingModel.OutputDirectory)]}";
            _settingModel.FileExtensionName = $"{jObject[nameof(SettingModel.FileExtensionName)]}";
            _settingModel.AutoStart = bool.Parse($"{jObject[nameof(SettingModel.AutoStart)]}");
            _settingModel.Format = (eFormat)Enum.Parse(typeof(eFormat), $"{jObject[nameof(SettingModel.Format)]}");
            _settingModel.Speed = Convert.ToInt32($"{jObject[nameof(SettingModel.Speed)]}");
            _settingModel.PackMode = (ePackMode)Enum.Parse(typeof(ePackMode), $"{jObject[nameof(SettingModel.PackMode)]}");
            _settingModel.FilterAdminCode = jObject[nameof(SettingModel.FilterAdminCode)].ToObject<List<string>>();
            _settingModel.ExtraRandom = jObject[nameof(SettingModel.ExtraRandom)].ToObject<List<RandomInfo>>();
            _settingModel.DoseType = (eDoseType)Enum.Parse(typeof(eDoseType), $"{jObject[nameof(SettingModel.DoseType)]}");
            _settingModel.OutputSpecialAdminCode = $"{jObject[nameof(SettingModel.OutputSpecialAdminCode)]}";
            _settingModel.StatOrBatch = (eDepartment)Enum.Parse(typeof(eDepartment), $"{jObject[nameof(SettingModel.StatOrBatch)]}");
            _settingModel.CutTime = $"{jObject[nameof(SettingModel.CutTime)]}";
            _settingModel.CrossDayAdminCode = $"{jObject[nameof(SettingModel.CrossDayAdminCode)]}";
            _settingModel.FilterMedicineCode = jObject[nameof(SettingModel.FilterMedicineCode)].ToObject<List<string>>();
            _settingModel.UseStatOrBatch = bool.Parse($"{jObject[nameof(SettingModel.UseStatOrBatch)]}");
            _settingModel.WindowMinimize = bool.Parse($"{jObject[nameof(SettingModel.WindowMinimize)]}");
            _settingModel.ShowWindowOperationButton = bool.Parse($"{jObject[nameof(SettingModel.ShowWindowOperationButton)]}");
            _settingModel.ShowXYParameter = bool.Parse($"{jObject[nameof(SettingModel.ShowXYParameter)]}");
            _settingModel.UseFilterMedicineCode = bool.Parse($"{jObject[nameof(SettingModel.UseFilterMedicineCode)]}");
            _settingModel.FilterNoCanister = bool.Parse($"{jObject[nameof(SettingModel.FilterNoCanister)]}");
            _settingModel.MoveSourceFileToBackupDirectoryWhenDone = bool.Parse($"{jObject[nameof(SettingModel.MoveSourceFileToBackupDirectoryWhenDone)]}");
            _settingModel.StopWhenDone = bool.Parse($"{jObject[nameof(SettingModel.StopWhenDone)]}");
        }
    }
}
