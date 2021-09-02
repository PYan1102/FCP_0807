using System;
using FCP.MVVM.FormatControl;

namespace FCP.MVVM.FormatInit
{
    class BASE_JVServer : FunctionCollections
    {
        private FMT_JVServer _JVS { get; set; }

        public override void Init()
        {
            base.Init();
            MainWindowVM.OPDToogle1Checked = true;
            InitFindFileMode(Models.Enum.FindFileModeEnum.根據檔名開頭);
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
            if (_JVS == null)
                _JVS = new FMT_JVServer();
            var result = _JVS.MethodShunt();
            Result(result, true, true);
        }

        public override void ProgressBoxClear()
        {
            base.ProgressBoxClear();
        }
    }
}
