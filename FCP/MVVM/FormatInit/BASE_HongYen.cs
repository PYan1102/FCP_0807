using System;
using FCP.MVVM.SQL;
using FCP.MVVM.Models.Enum;
using FCP.MVVM.FormatControl;

namespace FCP.MVVM.FormatInit
{
    class BASE_HongYen : FunctionCollections
    {
        private FMT_HongYen _HY { get; set; }

        public override void Init()
        {
            base.Init();
            MainWindow.Tgl_OPD1.IsChecked = true;
            InitFindFileMode(FindFileModeEnum.根據檔名開頭);
        }

        public override void Save()
        {
            base.Save();
        }

        public override void Stop()
        {
            base.Stop();
        }

        public override void CloseSelf()
        {
            base.CloseSelf();
        }

        public override void ConvertPrepare(bool isOPD)
        {
            base.ConvertPrepare(isOPD);
            SetIntoProperty(isOPD);
            FindFile.SetOPDDefault();
            SQLQuery.NonQuery($"UPDATE Job Set DeletedYN=1 WHERE DeletedYN=0 and LastUpdatedDate between '{DateTime.Now.AddDays(-1):yyyy/MM/dd 00:00:00:000}' and '{DateTime.Now.AddDays(-1):yyyy/MM/dd 23:59:59:999}'");
            GetFileAsync();
        }

        public override void SetConvertInformation()
        {
            base.SetConvertInformation();
            if (_HY == null)
                _HY = new FMT_HongYen();
            var result = _HY.MethodShunt();
            Result(result, true, true);
        }


        public override void ProgressBoxClear()
        {
            base.ProgressBoxClear();
        }
    }
}
