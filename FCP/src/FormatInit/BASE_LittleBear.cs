using FCP.src.Enum;
using FCP.src.FormatControl;

namespace FCP.src.FormatInit
{
    class BASE_LittleBear:FormatBase
    {
        private FMT_LittleBear _LB;
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
            if (_LB == null)
                _LB = new FMT_LittleBear();
            var result = _LB.MethodShunt();
            Result(result, true);
        }
    }
}
