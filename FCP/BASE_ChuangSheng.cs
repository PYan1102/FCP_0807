﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace FCP
{
    class BASE_ChuangSheng : FunctionCollections
    {
        FMT_ChuangSheng CS;
        public BASE_ChuangSheng(MainWindow mw, Settings s)
        {
            Loaded(mw, s);
        }

        public override void Loaded(MainWindow mw, Settings s)
        {
            base.Loaded(mw, s);
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
            if (CS == null)
                CS = new FMT_ChuangSheng();
            CS.Load(base.InputPath, base.OutputPath, base.FilePath, base.NowSecond, Settings, base.Log);
            Result(CS.MethodShunt(null), true, true);
        }

        public override void Result(string Result, bool NeedMoveFile, bool NeedReminder)
        {
            base.Result(Result, NeedMoveFile ,NeedReminder);
        }

        public override void ProgressBoxClear()
        {
            base.ProgressBoxClear();
        }
    }
}
