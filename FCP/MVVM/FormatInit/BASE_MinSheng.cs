using System;
using System.IO;
using FCP.MVVM.ViewModels.GetConvertFile;
using FCP.MVVM.Models.Enum;

namespace FCP.MVVM.FormatInit
{
    class BASE_MinSheng : FunctionCollections
    {
        private FMT_MinSheng _MS { get; set; }

        public override void Init()
        {
            base.Init();
            MainWindow.Tgl_OPD1.IsChecked = true;
            MainWindow.Tgl_OPD3.IsChecked = true;
            MainWindow.Tgl_OPD4.IsChecked = true;
            InitFindFileMode(Models.Enum.FindFileModeEnum.根據檔名開頭);
        }

        public override void AdvancedSettingsShow()
        {
            base.AdvancedSettingsShow();
        }

        public override void AutoStart()
        {
            base.AutoStart();
        }

        public override void Save()
        {
            base.Save();
        }

        public override void Stop()
        {
            base.Stop();
        }

        public override void CloseSelf()
        {
            base.CloseSelf();
        }

        public override void ConvertPrepare(bool isOPD)
        {
            base.ConvertPrepare(isOPD);
            SetOPDRule("N");
            SetCareRule("E");
            SetOtherRule("K");
            SetUDBatchRule(nameof(DefaultEnum.Default));
            SetIntoProperty(isOPD);
            GetFileAsync();
        }

        public override void SetConvertInformation()
        {
            base.SetConvertInformation();
            string fileDate = Path.GetFileNameWithoutExtension(base.FilePath).Substring(1);
            JsonService.JudgeJsonHasBeenCreated(fileDate);
            if (_MS == null)
                _MS = new FMT_MinSheng();
            _MS.Index = JsonService.GetOLEDBIndex(base.CurrentDepartment, fileDate);
            var result = _MS.MethodShunt();
            Result(result, false, false);
            JsonService.UpdateJson(fileDate, base.CurrentDepartment, _MS.newCount);
        }

        public override void ProgressBoxClear()
        {
            base.ProgressBoxClear();
        }
    }
}
