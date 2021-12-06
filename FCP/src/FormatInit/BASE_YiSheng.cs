using FCP.src.FormatControl;
using FCP.src.Enum;


namespace FCP.src.FormatInit
{
    class BASE_YiSheng : FunctionCollections
    {
        private FMT_YiSheng _YS { get; set; }

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
            if (_YS == null)
                _YS = new FMT_YiSheng();
            var result = _YS.MethodShunt();
            Result(result, true);
        }
    }
}
