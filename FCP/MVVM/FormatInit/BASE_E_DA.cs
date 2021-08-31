using System;
using FCP.MVVM.FormatControl;

namespace FCP.MVVM.FormatInit
{
    class BASE_E_DA : FunctionCollections
    {
        private FMT_E_DA _EDA { get; set; }

        public override void Init()
        {
            base.Init();
            InitFindFileMode(Models.Enum.FindFileModeEnum.根據檔名開頭);
        }

        public override void ShowAdvancedSettings()
        {
            base.ShowAdvancedSettings();
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
            FindFile.SetUDBatchDefault();
            GetFileAsync();
        }

        public override void SetConvertInformation()
        {
            base.SetConvertInformation();
            if (_EDA == null)
                _EDA = new FMT_E_DA();
            var result = _EDA.MethodShunt();
            Result(result, true, true);
        }

        public override void ProgressBoxClear()
        {
            base.ProgressBoxClear();
        }
    }
}
