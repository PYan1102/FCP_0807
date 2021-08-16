using System;
using FCP.MVVM.Models.Enum;
using FCP.MVVM.SQL;
using FCP.MVVM.FormatControl;
using FCP.MVVM.ViewModels.GetConvertFile;
using System.Diagnostics;

namespace FCP.MVVM.FormatInit
{
    class BASE_KuangTien : FunctionCollections
    {
        private Stopwatch sw = new Stopwatch();
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
                SetIntoProperty(isOPD);
                FindFile.SetPowderDefault();
                GetFileAsync();
                return;
            }
            base.ConvertPrepare(isOPD);
            SetOPDRule(nameof(DefaultEnum.Default));
            SetUDStatRule("uds3200");
            SetUDBatchRule("uds9100");
            SetIntoProperty(isOPD);
            GetFileAsync();
        }

        public override void SetConvertInformation()
        {
            sw.Restart();
            base.SetConvertInformation();
            if (_KT == null)
                _KT = new FMT_KuangTien();
            _KT.StatOrBatch = this.StatOrBatch;
            var result = _KT.MethodShunt();
            Result(result, true, true);
            Console.WriteLine(sw.ElapsedMilliseconds);
        }

        public override void ProgressBoxClear()
        {
            base.ProgressBoxClear();
        }
    }
}
