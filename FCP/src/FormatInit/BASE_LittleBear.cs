using FCP.src.Enum;
using FCP.src.FormatControl;

namespace FCP.src.FormatInit
{
    class BASE_LittleBear:FunctionCollections
    {
        private FMT_LittleBear _LB;
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
            if (_LB == null)
                _LB = new FMT_LittleBear();
            var result = _LB.MethodShunt();
            Result(result, true);
        }
    }
}
