using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace FCP
{
    class BASE_MinSheng : FunctionCollections
    {
        FMT_MinSheng MS;
        DoJson doJson = new DoJson();
        public BASE_MinSheng(MainWindow mw, Settings s)
        {
            Loaded(mw, s);
        }

        public override void Loaded(MainWindow mw, Settings s)
        {
            base.Loaded(mw, s);
            mw.chk_OPD1.IsChecked = true;
            mw.chk_OPD2.IsChecked = true;
            mw.chk_OPD3.IsChecked = true;
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
            switch(Mode)
            {
                case (int)ModeEnum.OPD:
                    string Filter = WD._OPD1 ? "N|" : "";
                    Filter += WD._OPD2 ? "E|" : "";
                    Filter += WD._OPD3 ? "K" : "";
                    Loop_OPD(0, 1, Filter);
                    break;
                case (int)ModeEnum.UD:
                    Loop_UD(0, 0, "");
                    break;
            }
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
            string Date = Path.GetFileNameWithoutExtension(base.FilePath).Substring(1);
            string FirstFileName = Path.GetFileNameWithoutExtension(base.FilePath).Substring(0, 1);
            int Index = FirstFileName == "N" ? 0 : FirstFileName == "E" ? 1 : FirstFileName == "K" ? 2 : 3;
            doJson.JudgeJson(Date);
            if (MS == null)
                MS = new FMT_MinSheng();
            MS.Index = doJson.GetIndex(Index, Date);
            MS.Load(base.InputPath, base.OutputPath, base.FilePath, base.NowSecond, Settings, base.Log);
            Result(MS.MethodShunt(base.MethodID), false, false);
            doJson.UpdateJson(Date, Index, MS.newCount);
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
