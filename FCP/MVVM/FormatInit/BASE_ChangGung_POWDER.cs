﻿using System;

namespace FCP.MVVM.FormatInit
{
    class BASE_ChangGung_POWDER : FunctionCollections
    {
        FMT_ChangGung_POWDER CG_P;
        public BASE_ChangGung_POWDER()
        {

        }

        public override void Init()
        {
            base.Init();
            MainWindow.Tgl_OPD2.IsChecked = true;
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
            if (CG_P == null)
                CG_P = new FMT_ChangGung_POWDER();
            var result = CG_P.MethodShunt();
            Result(result, true, true);
        }

        public override void ProgressBoxClear()
        {
            base.ProgressBoxClear();
        }
    }
}
