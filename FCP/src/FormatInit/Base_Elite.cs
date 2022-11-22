using FCP.Models;
using FCP.src.Enum;
using FCP.src.FormatLogic;
using FCP.src.MessageManager;
using Microsoft.Toolkit.Mvvm.Messaging;
using System.Windows;

namespace FCP.src.FormatInit
{
    internal class Base_Elite : ConvertBase
    {
        private FMT_Elite _format;

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
            _format = _format ?? new FMT_Elite();
            var result = _format.DepartmentShunt();
            Result(result, true);
        }

        public override MainUILayoutModel SetUILayout(MainUILayoutModel UI)
        {
            UI.Title = "金鶯診所";
            UI.IP1Enabled = true;
            UI.IP2Enabled = false;
            UI.IP3Enabled = false;
            UI.IP4Enabled = false;
            UI.IP5Enabled = false;
            UI.OPDToogle1 = "輸入1";
            UI.OPDToogle2 = string.Empty;
            UI.OPDToogle3 = string.Empty;
            UI.OPDToogle4 = string.Empty;
            UI.UDVisibility = Visibility.Hidden;
            UI.OPD1Visibility = Visibility.Visible;
            UI.OPD2Visibility = Visibility.Hidden;
            UI.OPD3Visibility = Visibility.Hidden;
            UI.OPD4Visibility = Visibility.Hidden;
            return UI;
        }
    }
}
