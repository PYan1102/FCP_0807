using FCP.src.FormatControl;
using FCP.src.Enum;

namespace FCP.src.FormatInit
{
    class BASE_JVServer : FormatBase
    {
        private FMT_JVServer _JVS { get; set; }

        public override void Init()
        {
            base.Init();
            MainWindowVM.OPDToogle1Checked = true;
            InitFindFileMode(eFileSearchMode.根據檔名開頭);
        }

        public override void ConvertPrepare(bool isOPD)
        {
            base.ConvertPrepare(isOPD);
            SetIntoProperty(isOPD);
            FindFile.SetOPDDefault();
            Start();
        }

        public override void Converter()
        {
            base.Converter();
            if (_JVS == null)
                _JVS = new FMT_JVServer();
            var result = _JVS.MethodShunt();
            Result(result, true);
        }
    }
}
