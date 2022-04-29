using FCP.src.Enum;
using FCP.src.FormatControl;

namespace FCP.src.FormatInit
{
    class BASE_OnCube : FormatBase
    {
        private FMT_OnCube _OC;
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
            if (_OC == null)
                _OC = new FMT_OnCube();
            var result = _OC.MethodShunt();
            Result(result, true);
        }
    }
}
