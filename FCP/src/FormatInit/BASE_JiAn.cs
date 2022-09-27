using FCP.Models;
using FCP.src.Enum;
using FCP.src.FormatLogic;
using FCP.src.MessageManager;
using Microsoft.Toolkit.Mvvm.Messaging;
using System.Windows;

namespace FCP.src.FormatInit
{
    class BASE_JiAn : ConvertBase
    {
        private FMT_JiAn _format;

        public override void Init()
        {
            base.Init();
            WeakReferenceMessenger.Default.Send(new SetMainWindowToogleCheckedChangeMessage(new MainWindowModel.ToggleModel() { Toggle1 = true }));
        }

        public override ActionResult PrepareStart()
        {
            SetFileSearchMode(eFileSearchMode.根據檔名開頭);
            SetOPDRule();
            SetCareRule();
            return base.PrepareStart();
        }

        public override void Converter()
        {
            _format = _format ?? new FMT_JiAn();
            var result = _format.DepartmentShunt();
            Result(result, true);
        }

        public override MainUILayoutModel SetUILayout(MainUILayoutModel UI)
        {
            UI.Title = "吉安醫院";
            UI.IP1Title = "門診";
            UI.IP2Title = "輸入路徑2";
            UI.IP3Title = "機構";
            UI.IP4Title = "輸入路徑4";
            UI.IP5Title = "輸入路徑5";
            UI.IP6Title = "輸入路徑6";
            UI.OPDToogle1 = "門診";
            UI.OPDToogle2 = "";
            UI.OPDToogle3 = "機構";
            UI.OPDToogle4 = "";
            UI.IP1Enabled = true;
            UI.IP2Enabled = false;
            UI.IP3Enabled = true;
            UI.IP4Enabled = false;
            UI.IP5Enabled = false;
            UI.IP6Enabled = false;
            UI.UDVisibility = Visibility.Hidden;
            UI.OPD1Visibility = Visibility.Visible;
            UI.OPD2Visibility = Visibility.Hidden;
            UI.OPD3Visibility = Visibility.Visible;
            UI.OPD4Visibility = Visibility.Hidden;
            return UI;
        }
    }
}
