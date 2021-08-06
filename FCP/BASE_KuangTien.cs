using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Data.SqlClient;

namespace FCP
{
    class BASE_KuangTien : FunctionCollections
    {
        public string StatOrBatch { get; set; }
        private FMT_KuangTien _KT;
        public BASE_KuangTien(MainWindow mw, Settings s)
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
            if (Settings.Mode == (int)Settings.ModeEnum.光田OnCube)
            {
                if (Settings.DoseMode == "C")
                {
                    //沙鹿
                    //Query(@"update PrintFormItem set DeletedYN=1 where RawID in (120180,120195)");

                    //大甲
                    Query(@"update PrintFormItem set DeletedYN=1 where RawID in (120156,120172)");
                }
                else
                {
                    //沙鹿
                    //Query(@"update PrintFormItem set DeletedYN=0 where RawID in (120180,120195)");

                    //大甲
                    Query(@"update PrintFormItem set DeletedYN=0 where RawID in (120156,120172)");
                }
            }
            else if (Settings.Mode == (int)Settings.ModeEnum.光田JVS)  //磨粉
            {
                base.ConvertPrepare(Mode);
                Loop_OPD(0, 0, "");
                return;
            }
            base.ConvertPrepare(Mode);
            if (Mode == (int)ModeEnum.OPD)
                Loop_OPD(0, 0, "");
            else
            {
                if (base.WD._isStat)
                    Loop_UD(0, 7, "uds3200");
                else
                    Loop_UD(0, 7, "uds9100");
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
            if (_KT == null)
                _KT = new FMT_KuangTien();
            _KT.StatOrBatch = this.StatOrBatch;
            _KT.Load(base.InputPath, base.OutputPath, base.FilePath, base.NowSecond, Settings, base.Log);
            Result(_KT.MethodShunt(base.MethodID), true, true);
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
