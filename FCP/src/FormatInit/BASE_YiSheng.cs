using FCP.src.FormatControl;
using FCP.src.Enum;


namespace FCP.src.FormatInit
{
    class BASE_YiSheng : FormatBase
    {
        private FMT_YiSheng _YS { get; set; }

        public override void Init()
        {
            base.Init();
            MainWindowVM.OPDToogle1Checked = true;
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
            if (_YS == null)
                _YS = new FMT_YiSheng();
            var result = _YS.MethodShunt();
            Result(result, true);
        }
    }
}
