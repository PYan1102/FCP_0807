using FCP.src.FormatControl;
using FCP.src.Enum;
using System.Windows;

namespace FCP.src.FormatInit
{
    class BASE_E_DA : FormatBase
    {
        private FMT_E_DA _EDA { get; set; }

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
            if (_EDA == null)
                _EDA = new FMT_E_DA();
            var result = _EDA.MethodShunt();
            Result(result, true);
        }

        public override UILayout SetUILayout(UILayout UI)
        {
            UI.Title = "義大醫院 > OnCube";
            UI.IP1Title = "輸入路徑1";
            UI.IP2Title = "輸入路徑2";
            UI.IP3Title = "住   院";
            UI.OPDToogle1 = "";
            UI.OPDToogle2 = "";
            UI.OPDToogle3 = "";
            UI.OPDToogle4 = "";
            UI.IP1Enabled = false;
            UI.IP2Enabled = false;
            UI.IP3Enabled = true;
            UI.UDVisibility = Visibility.Visible;
            UI.OPD1Visibility = Visibility.Hidden;
            UI.OPD2Visibility = Visibility.Hidden;
            UI.OPD3Visibility = Visibility.Hidden;
            UI.OPD4Visibility = Visibility.Hidden;
            return UI;
        }
    }
}
