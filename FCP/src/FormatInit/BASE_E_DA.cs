using FCP.src.FormatLogic;
using FCP.src.Enum;
using System.Windows;
using FCP.Models;
using Microsoft.Toolkit.Mvvm.Messaging;
using FCP.src.MessageManager;

namespace FCP.src.FormatInit
{
    class BASE_E_DA : ConvertBase
    {
        private FMT_E_DA _format;

        public override void Init()
        {
            base.Init();
            WeakReferenceMessenger.Default.Send(new SetMainWindowToogleCheckedChangeMessage(new MainWindowModel.ToggleModel() { Toggle1 = true }));
        }

        public override ActionResult PrepareStart()
        {
            SetFileSearchMode(eFileSearchMode.根據檔名開頭);
            SetBatchRule();
            return base.PrepareStart();
        }

        public override void Converter()
        {
            _format = _format ?? new FMT_E_DA();
            var result = _format.DepartmentShunt();
            Result(result, true);
        }

        public override MainUILayoutModel SetUILayout(MainUILayoutModel UI)
        {
            UI.Title = "義大醫院";
            UI.IP1Enabled = false;
            UI.IP2Enabled = false;
            UI.IP3Enabled = false;
            UI.IP4Enabled = false;
            UI.IP5Enabled = true;
            UI.OPDToogle1 = string.Empty;
            UI.OPDToogle2 = string.Empty;
            UI.OPDToogle3 = string.Empty;
            UI.OPDToogle4 = string.Empty;
            UI.UDVisibility = Visibility.Visible;
            UI.OPD1Visibility = Visibility.Hidden;
            UI.OPD2Visibility = Visibility.Hidden;
            UI.OPD3Visibility = Visibility.Hidden;
            UI.OPD4Visibility = Visibility.Hidden;
            return UI;
        }
    }
}
