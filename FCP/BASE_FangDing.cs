using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCP
{
    class BASE_FangDing : FunctionCollections
    {
        private FMT_FangDing _FangDing { get; set; }

        public BASE_FangDing(MainWindow mainWindow, Settings settings)
        {
            Loaded(mainWindow, settings);
        }

        public override void Loaded(MainWindow mw, Settings S)
        {
            base.Loaded(mw, S);
            mw.chk_OPD1.IsChecked = true;
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

        public override void ConvertPrepare(int Mode)
        {
            base.ConvertPrepare(Mode);
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

        public override void Converter()
        {
            base.Converter();
            if (_FangDing == null)
                _FangDing = new FMT_FangDing();
            _FangDing.Load(base.InputPath, base.OutputPath, base.FilePath, base.NowSecond, Settings, base.Log);
            Result(_FangDing.MethodShunt(base.MethodID), true, true);
        }

        public override void Result(string Result, bool NeedMoveFile, bool NeedReminder)
        {
            base.Result(Result, NeedMoveFile, NeedReminder);
        }

        public override void ProgressBoxClear()
        {
            base.ProgressBoxClear();
        }
    }
}
