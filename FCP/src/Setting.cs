using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using FCP.src.Enum;
using Helper;
using FCP.src.Factory.Models;
using FCP.Models;

namespace FCP.Service
{
    public class Setting
    {
        private string _settingFilePath = $@"{Environment.CurrentDirectory}\Setting.json";
        private SettingModel _settingModel;

        public Setting()
        {
            _settingModel = SettingFactory.GenerateSettingModel();
            JsonHelper.SetJsonPath(_settingFilePath);
            if (!IsSettingFileExists())
                GenerateSettingFile();
            SetNewValueIntoSettingModel();
        }

        public void Save(object obj)
        {
            JsonHelper.Save(JObject.FromObject(obj));
            SetNewValueIntoSettingModel();
        }

        private bool IsSettingFileExists()
        {
            return File.Exists(_settingFilePath);
        }

        private void GenerateSettingFile()
        {
            Save(new SettingModel());
        }

        public void SetNewValueIntoSettingModel()
        {
            var jObject = JsonHelper.Analysis();
            _settingModel.InputPath1 = $"{jObject[nameof(_settingModel.InputPath1)]}";
            _settingModel.InputPath2 = $"{jObject[nameof(_settingModel.InputPath2)]}";
            _settingModel.InputPath3 = $"{jObject[nameof(_settingModel.InputPath3)]}";
            _settingModel.OutputPath = $"{jObject[nameof(_settingModel.OutputPath)]}";
            _settingModel.FileExtensionName = $"{jObject[nameof(_settingModel.FileExtensionName)]}";
            _settingModel.AutoStart = bool.Parse($"{jObject[nameof(_settingModel.AutoStart)]}");
            _settingModel.Format = (eFormat)Enum.Parse(typeof(eFormat), $"{jObject[nameof(_settingModel.Format)]}");
            _settingModel.Speed = Convert.ToInt32($"{jObject[nameof(_settingModel.Speed)]}");
            _settingModel.PackMode = (ePackMode)Enum.Parse(typeof(ePackMode), $"{jObject[nameof(_settingModel.PackMode)]}");
            _settingModel.FilterAdminCode = jObject[nameof(_settingModel.FilterAdminCode)].ToObject<List<string>>();
            _settingModel.ExtraRandom = jObject[nameof(_settingModel.ExtraRandom)].ToObject<List<RandomInfo>>();
            _settingModel.DoseType = (eDoseType)Enum.Parse(typeof(eDoseType), $"{jObject[nameof(_settingModel.DoseType)]}");
            _settingModel.OutputSpecialAdminCode = $"{jObject[nameof(_settingModel.OutputSpecialAdminCode)]}";
            _settingModel.StatOrBatch = (eDepartment)Enum.Parse(typeof(eDepartment), $"{jObject[nameof(_settingModel.StatOrBatch)]}");
            _settingModel.CutTime = $"{jObject[nameof(_settingModel.CutTime)]}";
            _settingModel.CrossDayAdminCode = $"{jObject[nameof(_settingModel.CrossDayAdminCode)]}";
            _settingModel.FilterMedicineCode = jObject[nameof(_settingModel.FilterMedicineCode)].ToObject<List<string>>();
            _settingModel.UseStatOrBatch = bool.Parse($"{jObject[nameof(_settingModel.UseStatOrBatch)]}");
            _settingModel.WindowMinimize = bool.Parse($"{jObject[nameof(_settingModel.WindowMinimize)]}");
            _settingModel.ShowWindowOperationButton = bool.Parse($"{jObject[nameof(_settingModel.ShowWindowOperationButton)]}");
            _settingModel.ShowXYParameter = bool.Parse($"{jObject[nameof(_settingModel.ShowXYParameter)]}");
            _settingModel.UseFilterMedicineCode = bool.Parse($"{jObject[nameof(_settingModel.UseFilterMedicineCode)]}");
            _settingModel.FilterNoCanister = bool.Parse($"{jObject[nameof(_settingModel.FilterNoCanister)]}");
            _settingModel.MoveSourceFileToBackupDirectoryWhenDone = bool.Parse($"{jObject[nameof(_settingModel.MoveSourceFileToBackupDirectoryWhenDone)]}");
            _settingModel.StopWhenDone = bool.Parse($"{jObject[nameof(_settingModel.StopWhenDone)]}");
        }
    }
}
