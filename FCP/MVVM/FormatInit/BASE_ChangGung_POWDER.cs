using System;
using FCP.MVVM.Models.Enum;
using FCP.MVVM.FormatControl;

namespace FCP.MVVM.FormatInit
{
    class BASE_ChangGung_POWDER : FunctionCollections
    {
        private FMT_ChangGung_POWDER _CG_P { get; set; }

        public override void Init()
        {
            base.Init();
            MainWindow.Tgl_OPD2.IsChecked = true;
            InitFindFileMode(FindFileModeEnum.根據檔名開頭);
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
            SetIntoProperty(isOPD);
            FindFile.SetPowderDefault();
            GetFileAsync();
        }

        public override void SetConvertInformation()
        {
            base.SetConvertInformation();
            if (_CG_P == null)
                _CG_P = new FMT_ChangGung_POWDER();
            var result = _CG_P.MethodShunt();
            Result(result, true, true);
        }

        public override void ProgressBoxClear()
        {
            base.ProgressBoxClear();
        }
    }
}
