using System;
using FCP.MVVM.Models.Enum;
using FCP.MVVM.SQL;

namespace FCP.MVVM.FormatInit
{
    class BASE_KuangTien : FunctionCollections
    {
        public string StatOrBatch { get; set; }
        private FMT_KuangTien _KT { get; set; }

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
            if (SettingsModel.Mode == Format.光田醫院TOC)
            {
                if (SettingsModel.DoseMode == DoseMode.種包)
                {
                    //沙鹿
                    //SQLQuery.NonQuery(@"update PrintFormItem set DeletedYN=1 where RawID in (120180,120195)");

                    //大甲
                    SQLQuery.NonQuery(@"update PrintFormItem set DeletedYN=1 where RawID in (120156,120172)");
                }
                else
                {
                    //沙鹿
                    //SQLQuery.NonQuery(@"update PrintFormItem set DeletedYN=0 where RawID in (120180,120195)");

                    //大甲
                    SQLQuery.NonQuery(@"update PrintFormItem set DeletedYN=0 where RawID in (120156,120172)");
                }
            }
            else if (SettingsModel.Mode == Format.光田醫院TJVS)  //磨粉
            {
                base.ConvertPrepare(isOPD);
                Loop_OPD(0, 0, "");
                return;
            }
            base.ConvertPrepare(isOPD);
            if (isOPD)
                Loop_OPD(0, 0, "");
            else
            {
                if (base.WD._IsStat)
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

        public override void SetConvertInformation()
        {
            base.SetConvertInformation();
            if (_KT == null)
                _KT = new FMT_KuangTien();
            _KT.StatOrBatch = this.StatOrBatch;
            var result = _KT.MethodShunt();
            Result(result, true, true);
        }

        public override void ProgressBoxClear()
        {
            base.ProgressBoxClear();
        }
    }
}
