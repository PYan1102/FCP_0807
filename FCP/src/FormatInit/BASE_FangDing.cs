using FCP.src.Enum;
using FCP.src.FormatControl;

namespace FCP.src.FormatInit
{
    class BASE_FangDing : FunctionCollections
    {
        private FMT_FangDing _FangDing { get; set; }

        public override void Init()
        {
            base.Init();
            MainWindowVM.OPDToogle1Checked = true;
            InitFindFileMode(eFindFileMode.根據檔名開頭);
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
            if (_FangDing == null)
                _FangDing = new FMT_FangDing();
            var result = _FangDing.MethodShunt();
            Result(result, true);
        }
    }
}
