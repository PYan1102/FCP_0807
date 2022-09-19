using System;
using FCP.src.Enum;
using FCP.src.FormatLogic;
using FCP.Models;
using Microsoft.Toolkit.Mvvm.Messaging;
using FCP.src.MessageManager;

namespace FCP.src.FormatInit
{
    class BASE_HongYen : ConvertBase
    {
        private FMT_HongYen _format;

        public override void Init()
        {
            base.Init();
            WeakReferenceMessenger.Default.Send(new SetMainWindowToogleCheckedChangeMessage(new MainWindowModel.ToggleModel() { Toggle1 = true }));
        }

        public override ActionResult PrepareStart()
        {
            SetFileSearchMode(eFileSearchMode.根據檔名開頭);
            SetOPDRule();
            CommonModel.SqlHelper.Execute($"UPDATE Job Set DeletedYN=1 WHERE DeletedYN=0 and LastUpdatedDate between '{DateTime.Now.AddDays(-1):yyyy/MM/dd 00:00:00:000}' and '{DateTime.Now.AddDays(-1):yyyy/MM/dd 23:59:59:999}'");
            return base.PrepareStart();
        }

        public override void Converter()
        {
            _format = _format ?? new FMT_HongYen();
            var result = _format.DepartmentShunt();
            Result(result, true);
        }
    }
}
