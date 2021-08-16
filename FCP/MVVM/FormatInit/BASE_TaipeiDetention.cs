using System;
using FCP.MVVM.Models.Enum;
using FCP.MVVM.FormatControl;

namespace FCP.MVVM.FormatInit
{
    class BASE_TaipeiDetention : FunctionCollections
    {
        private FMT_TaipeiDetention _TPD { get; set; }

        public override void Init()
        {
            base.Init();
            MainWindow.Tgl_OPD1.IsChecked = true;
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
            FindFile.SetOPDDefault();
            GetFileAsync();
        }

        public override void SetConvertInformation()
        {
            base.SetConvertInformation();
            if (_TPD == null)
                _TPD = new FMT_TaipeiDetention();
            var result = _TPD.MethodShunt();
            Result(result, true, true);
        }

        public override void ProgressBoxClear()
        {
            base.ProgressBoxClear();
        }
    }
}
