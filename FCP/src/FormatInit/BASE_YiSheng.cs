using FCP.src.FormatControl;
using FCP.src.Enum;

namespace FCP.src.FormatInit
{
    class BASE_YiSheng : FormatBase
    {
        private FMT_YiSheng _format;

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
            _format = _format ?? new FMT_YiSheng();
            var result = _format.DepartmentShunt();
            Result(result, true);
        }
    }
}
