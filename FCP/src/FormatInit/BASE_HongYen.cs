using System;
using FCP.src.SQL;
using FCP.src.Enum;
using FCP.src.FormatControl;
using Helper;

namespace FCP.src.FormatInit
{
    class BASE_HongYen : FunctionCollections
    {
        private FMT_HongYen _HY { get; set; }

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
            MSSql.RunSQL($"UPDATE Job Set DeletedYN=1 WHERE DeletedYN=0 and LastUpdatedDate between '{DateTime.Now.AddDays(-1):yyyy/MM/dd 00:00:00:000}' and '{DateTime.Now.AddDays(-1):yyyy/MM/dd 23:59:59:999}'");
            GetFileAsync();
        }

        public override void SetConvertInformation()
        {
            base.SetConvertInformation();
            if (_HY == null)
                _HY = new FMT_HongYen();
            var result = _HY.MethodShunt();
            Result(result, true);
        }
    }
}
