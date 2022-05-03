using FCP.src.Enum;
using FCP.src.FormatControl;

namespace FCP.src.FormatInit
{
    class BASE_FangDing : FormatBase
    {
        private FMT_FangDing _format;

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
            _format = _format ?? new FMT_FangDing();
            var result = _format.MethodShunt();
            Result(result, true);
        }
    }
}
