using FCP.Models;
using FCP.src.Enum;
using FCP.src.FormatLogic;
using FCP.src.MessageManager;
using Microsoft.Toolkit.Mvvm.Messaging;
using System.Windows;

namespace FCP.src.FormatInit
{
    internal class BASE_JianTong : ConvertBase
    {
        private FMT_JianTong _format;

        public override void Init()
        {
            base.Init();
            WeakReferenceMessenger.Default.Send(new SetMainWindowToogleCheckedChangeMessage(new MainWindowModel.ToggleModel() { Toggle1 = true, Toggle2 = true, Toggle3 = true }));
        }

        public override ActionResult PrepareStart()
        {
            SetFileSearchMode(eFileSearchMode.根據檔名開頭);
            SetOPDRule();
            SetPowderRule();
            SetCareRule();
            return base.PrepareStart();
        }

        public override void Converter()
        {
            _format = _format ?? new FMT_JianTong();
            var result = _format.DepartmentShunt();
            Result(result, true);
        }

        public override MainUILayoutModel SetUILayout(MainUILayoutModel UI)
        {
            UI.Title = "健通藥局";
            return UI;
        }
    }
}
