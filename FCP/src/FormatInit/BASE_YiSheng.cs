﻿using FCP.src.FormatLogic;
using FCP.src.Enum;
using Microsoft.Toolkit.Mvvm.Messaging;
using FCP.src.MessageManager;
using FCP.Models;

namespace FCP.src.FormatInit
{
    class BASE_YiSheng : ConvertBase
    {
        private FMT_YiSheng _format;

        public override void Init()
        {
            base.Init();
            WeakReferenceMessenger.Default.Send(new SetMainWindowToogleCheckedChangeMessage(new MainWindowModel.ToggleModel() { Toggle1 = true }));
        }

        public override ActionResult PrepareStart()
        {
            SetFileSearchMode(eFileSearchMode.根據檔名開頭);
            SetOPDRule();
            return base.PrepareStart();
        }

        public override void Converter()
        {
            _format = _format ?? new FMT_YiSheng();
            var result = _format.DepartmentShunt();
            Result(result, true);
        }
    }
}
