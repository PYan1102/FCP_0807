using FCP.src.Enum;
using FCP.src.FormatControl;
using System.Windows;

namespace FCP.src.FormatInit
{
    class BASE_ChangGung_POWDER : FunctionCollections
    {
        private FMT_ChangGung_POWDER _CG_P { get; set; }

        public override void Init()
        {
            base.Init();
            MainWindowVM.OPDToogle2Checked = true;
            InitFindFileMode(eFindFileMode.根據檔名開頭);
        }

        public override void ConvertPrepare(bool isOPD)
        {
            base.ConvertPrepare(isOPD);
            SetIntoProperty(isOPD);
            FindFile.SetPowderDefault();
            GetFileAsync();
        }

        public override void SetConvertInformation()
        {
            base.SetConvertInformation();
            if (_CG_P == null)
                _CG_P = new FMT_ChangGung_POWDER();
            var result = _CG_P.MethodShunt();
            Result(result, true);
        }

        public override UILayout SetUILayout(UILayout UI)
        {
            UI.Title = "長庚磨粉 > JVServer";
            UI.IP1Title = "磨粉";
            UI.IP2Title = "輸入路徑2";
            UI.IP3Title = "輸入路徑3";
            UI.OPDToogle1 = "";
            UI.OPDToogle2 = "磨粉";
            UI.OPDToogle3 = "";
            UI.OPDToogle4 = "";
            UI.IP1Enabled = true;
            UI.IP2Enabled = false;
            UI.IP3Enabled = false;
            UI.UDVisibility = Visibility.Hidden;
            UI.OPD1Visibility = Visibility.Hidden;
            UI.OPD2Visibility = Visibility.Visible;
            UI.OPD3Visibility = Visibility.Hidden;
            UI.OPD4Visibility = Visibility.Hidden;
            return UI;
        }
    }
}
