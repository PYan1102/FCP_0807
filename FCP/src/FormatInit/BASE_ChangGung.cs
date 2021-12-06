using System.IO;
using FCP.src.Enum;
using FCP.src.FormatControl;
using System.Windows;

namespace FCP.src.FormatInit
{
    class BASE_ChangGung : FunctionCollections
    {
        private FMT_ChangGung _CG { get; set; }

        public override void Init()
        {
            base.Init();
            MainWindowVM.OPDToogle1Checked = true;
            MainWindowVM.OPDToogle4Checked = true;
            InitFindFileMode(eFindFileMode.根據檔名開頭);
        }

        public override void ConvertPrepare(bool isOPD)
        {
            base.ConvertPrepare(isOPD);
            SetOPDRule("S");
            SetOtherRule("0");
            SetUDBatchRule("udpkg");
            SetIntoProperty(isOPD);
            FindFile.SetUDStatDefault();
            GetFileAsync();
        }

        public override void SetConvertInformation()
        {
            if (!string.IsNullOrEmpty(FilePath))
                MergeFilesAndSetFilePath();
            base.SetConvertInformation();
            _CG = _CG ?? new FMT_ChangGung();
            var result = _CG.MethodShunt();
            Result(result, true);
            MoveFile(result.Result);
        }

        private void MoveFile(eConvertResult result)
        {
            if (CurrentDepartment != eConvertLocation.UDBatch & CurrentDepartment != eConvertLocation.Other)
                return;
            switch (result)
            {
                case eConvertResult.成功:
                    MoveFilesIncludeResult(true);
                    break;
                case eConvertResult.全數過濾:
                    break;
                case eConvertResult.沒有種包頻率:
                    break;
                case eConvertResult.沒有餐包頻率:
                    break;
                default:
                    MoveFilesIncludeResult(false);
                    break;
            }
        }

        private void MergeFilesAndSetFilePath()
        {
            if (FilePath == string.Empty)
                return;
            if (CurrentDepartment == eConvertLocation.Other)
            {
                base.FilePath = MergeFilesAndGetNewFilePath(Path.GetDirectoryName(base.FilePath), "藥來速", 0, 1, "0");
            }
            else if (!MainWindowVM.StatChecked && CurrentDepartment == eConvertLocation.UDBatch)
            {
                base.FilePath = MergeFilesAndGetNewFilePath(Path.GetDirectoryName(base.FilePath), "住院批次", 0, 5, "udpkg");
            }
        }

        public override UILayout SetUILayout(UILayout UI)
        {
            UI.Title = "長庚醫院 > OnCube";
            UI.IP1Title = "門診";
            UI.IP2Title = "藥來速";
            UI.IP3Title = "住院";
            UI.OPDToogle1 = "門診";
            UI.OPDToogle2 = "";
            UI.OPDToogle3 = "";
            UI.OPDToogle4 = "藥來速";
            UI.IP1Enabled = true;
            UI.IP2Enabled = true;
            UI.IP3Enabled = true;
            UI.UDVisibility = Visibility.Visible;
            UI.OPD1Visibility = Visibility.Visible;
            UI.OPD2Visibility = Visibility.Hidden;
            UI.OPD3Visibility = Visibility.Hidden;
            UI.OPD4Visibility = Visibility.Visible;
            return UI;
        }
    }
}
