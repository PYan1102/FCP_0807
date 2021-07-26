﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FCP
{
    class BASE_JenKang : FunctionCollections
    {
        FMT_JenKang JK;
        public BASE_JenKang(MainWindow mw, Settings s)
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
            if (Mode == (int)ModeEnum.OPD)
                Loop_OPD(0, 0, "");
            else
                Loop_UD(0, 0, "");
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
            string content = GetFileContent();
            if (content.Contains("護理"))
                base.MethodID = 2;
            base.Converter();
            if (JK == null)
                JK = new FMT_JenKang();
            JK.Load(base.InputPath, base.OutputPath, base.FilePath, base.NowSecond, Settings, base.Log);
            Result(JK.MethodShunt(MethodID), true, true);
        }

        private string GetFileContent()
        {
            StringBuilder sb = new StringBuilder();
            using (StreamReader sr = new StreamReader(FilePath, Encoding.Default))
            {
                sb.Append(sr.ReadToEnd());
            }
            return sb.ToString();
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
