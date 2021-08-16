using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using FCP.MVVM.FormatControl;


namespace FCP.MVVM.FormatInit
{
    class BASE_YiSheng : FunctionCollections
    {
        private FMT_YiSheng _YS { get; set; }

        public override void Init()
        {
            base.Init();
            MainWindow.Tgl_OPD1.IsChecked = true;
            InitFindFileMode(Models.Enum.FindFileModeEnum.根據檔名開頭);
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
            SetIntoProperty(isOPD);
            FindFile.SetOPDDefault();
            GetFileAsync();
        }

        public override void SetConvertInformation()
        {
            base.SetConvertInformation();
            if (_YS == null)
                _YS = new FMT_YiSheng();
            var result = _YS.MethodShunt();
            Result(result, true, true);
        }

        public override void ProgressBoxClear()
        {
            base.ProgressBoxClear();
        }
    }
}
