using System.IO;
using FCP.ViewModels.GetConvertFile;
using FCP.src.Enum;
using FCP.src.FormatControl;
using FCP.Service;
using System.Windows;

namespace FCP.src.FormatInit
{
    class BASE_MinSheng : FunctionCollections
    {
        private FMT_MinSheng _MS { get; set; }

        public override void Init()
        {
            base.Init();
            MainWindowVM.OPDToogle1Checked = true;
            MainWindowVM.OPDToogle3Checked = true;
            MainWindowVM.OPDToogle4Checked = true;
            InitFindFileMode(eFindFileMode.根據檔名開頭);
        }

        public override void ConvertPrepare(bool isOPD)
        {
            base.ConvertPrepare(isOPD);
            SetOPDRule("N");
            SetCareRule("E");
            SetOtherRule("K");
            SetUDBatchRule(nameof(DefaultEnum.Default));
            SetIntoProperty(isOPD);
            GetFileAsync();
        }

        public override void SetConvertInformation()
        {
            base.SetConvertInformation();
            string fileDate = Path.GetFileNameWithoutExtension(base.FilePath).Substring(1);
            JsonService.JudgeJsonHasBeenCreated(fileDate);
            if (_MS == null)
                _MS = new FMT_MinSheng();
            _MS.Index = JsonService.GetOLEDBIndex(base.CurrentDepartment, fileDate);
            var result = _MS.MethodShunt();
            Result(result, false);
            JsonService.UpdateJson(fileDate, base.CurrentDepartment, _MS.newCount);
        }

        public override UILayout SetUILayout(UILayout UI)
        {
            UI.Title = "民生醫院 > OnCube";
            UI.IP1Title = "輸入路徑1";
            UI.IP2Title = "輸入路徑1";
            UI.IP3Title = "住   院";
            UI.OPDToogle1 = "門診";
            UI.OPDToogle2 = "";
            UI.OPDToogle3 = "養護";
            UI.OPDToogle4 = "大寮";
            UI.IP1Enabled = true;
            UI.IP2Enabled = true;
            UI.IP3Enabled = true;
            UI.UDVisibility = Visibility.Visible;
            UI.OPD1Visibility = Visibility.Visible;
            UI.OPD2Visibility = Visibility.Hidden;
            UI.OPD3Visibility = Visibility.Visible;
            UI.OPD4Visibility = Visibility.Visible;
            return UI;
        }
    }
}
