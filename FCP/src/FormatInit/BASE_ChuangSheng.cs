using FCP.src.Enum;
using FCP.src.FormatControl;

namespace FCP.src.FormatInit
{
    class BASE_ChuangSheng : FunctionCollections
    {
        private FMT_ChuangSheng _CS { get; set; }

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
            if (_CS == null)
                _CS = new FMT_ChuangSheng();
            var result = _CS.MethodShunt();
            Result(result, true);
        }
    }
}
