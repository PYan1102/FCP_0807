using System;
using FCP.MVVM.Models;
using FCP.MVVM.FormatControl;

namespace FCP.MVVM.FormatInit
{
    class BASE_FangDing : FunctionCollections
    {
        private FMT_FangDing _FangDing { get; set; }

        public override void Init()
        {
            base.Init();
            MainWindow.Tgl_OPD1.IsChecked = true;
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
            Loop_OPD(0, 0, "");
        }

        public override void Loop_OPD(int Start, int Length, string Content)
        {
            base.Loop_OPD(Start, Length, Content);
        }

        public override void Loop_UD(int Start, int Length, string Content)
        {
            base.Loop_UD(Start, Length, Content);
        }

        public override void SetConvertInformation()
        {
            base.SetConvertInformation();
            if (_FangDing == null)
                _FangDing = new FMT_FangDing();
            var result = _FangDing.MethodShunt();
            Result(result, true, true);
        }

        public override void ProgressBoxClear()
        {
            base.ProgressBoxClear();
        }
    }
}
