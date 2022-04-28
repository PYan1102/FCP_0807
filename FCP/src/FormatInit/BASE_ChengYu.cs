using FCP.src.Enum;
using FCP.src.FormatControl;

namespace FCP.src.FormatInit
{
    class BASE_ChengYu : FormatBase
    {
        private FMT_ChengYu _CY;
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
            if (_CY == null)
                _CY = new FMT_ChengYu();
            var result = _CY.MethodShunt();
            Result(result, true);
        }
    }
}
