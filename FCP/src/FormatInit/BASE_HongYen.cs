using System;
using FCP.src.Enum;
using FCP.src.FormatControl;
using FCP.Models;

namespace FCP.src.FormatInit
{
    class BASE_HongYen : FormatBase
    {
        private FMT_HongYen _format;

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
            CommonModel.SqlHelper.Execute($"UPDATE Job Set DeletedYN=1 WHERE DeletedYN=0 and LastUpdatedDate between '{DateTime.Now.AddDays(-1):yyyy/MM/dd 00:00:00:000}' and '{DateTime.Now.AddDays(-1):yyyy/MM/dd 23:59:59:999}'");
            Start();
        }

        public override void Converter()
        {
            base.Converter();
            _format = _format ?? new FMT_HongYen();
            var result = _format.MethodShunt();
            Result(result, true);
        }
    }
}
