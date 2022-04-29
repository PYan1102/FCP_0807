using FCP.src.Enum;
using FCP.src.FormatControl;

namespace FCP.src.FormatInit
{
    class BASE_TaipeiDetention : FormatBase
    {
        private FMT_TaipeiDetention _TPD { get; set; }

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
            if (_TPD == null)
                _TPD = new FMT_TaipeiDetention();
            var result = _TPD.MethodShunt();
            Result(result, true);
        }
    }
}
