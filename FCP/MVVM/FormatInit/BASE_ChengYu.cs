using System;
using FCP.MVVM.Models.Enum;
using FCP.MVVM.FormatControl;

namespace FCP.MVVM.FormatInit
{
    class BASE_ChengYu : FunctionCollections
    {
        private FMT_ChengYu _CY;
        public override void Init()
        {
            base.Init();
            MainWindow.Tgl_OPD1.IsChecked = true;
            InitFindFileMode(FindFileModeEnum.根據檔名開頭);
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
            if (_CY == null)
                _CY = new FMT_ChengYu();
            var result = _CY.MethodShunt();
            Result(result, false, true);
        }
    }
}
