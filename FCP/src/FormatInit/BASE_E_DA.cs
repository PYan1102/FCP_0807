using FCP.src.FormatControl;
using FCP.src.Enum;
using System.Windows;
using FCP.Models;

namespace FCP.src.FormatInit
{
    class BASE_E_DA : FormatBase
    {
        private FMT_E_DA _format;

        public override void Init()
        {
            base.Init();
            MainWindowVM.OPDToogle1Checked = true;
        }

        public override void ConvertPrepare()
        {
            base.ConvertPrepare();
            SetFileSearchMode(eFileSearchMode.根據檔名開頭);
            SetBatchRule();
            Start();
        }

        public override void Converter()
        {
            base.Converter();
            _format = _format ?? new FMT_E_DA();
            var result = _format.MethodShunt();
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
