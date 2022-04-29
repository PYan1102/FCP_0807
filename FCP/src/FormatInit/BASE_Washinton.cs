using FCP.src.FormatControl;
using FCP.src.Enum;

namespace FCP.src.FormatInit
{
    class BASE_Washinton : FormatBase
    {
        private FMT_Washinton _JVS { get; set; }

        public override void Init()
        {
            base.Init();
            MainWindowVM.OPDToogle1Checked = true;
        }

        public override void ConvertPrepare()
        {
            base.ConvertPrepare();
            SetFileSearchMode(eFileSearchMode.根據檔名開頭);
            SetOPDRule();
            Start();
        }

        public override void Converter()
        {
            base.Converter();
            if (_JVS == null)
                _JVS = new FMT_Washinton();
            var result = _JVS.MethodShunt();
            Result(result, true);
        }
    }
}
