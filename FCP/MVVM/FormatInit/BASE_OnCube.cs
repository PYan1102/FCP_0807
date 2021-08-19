using FCP.MVVM.Models.Enum;
using System;
using FCP.MVVM.FormatControl;

namespace FCP.MVVM.FormatInit
{
    class BASE_OnCube : FunctionCollections
    {
        private FMT_OnCube _OC;
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
            if (_OC == null)
                _OC = new FMT_OnCube();
            var result = _OC.MethodShunt();
            Result(result, true, true);
        }
    }
}
