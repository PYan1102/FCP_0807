using System.IO;
using FCP.src.Enum;
using FCP.src.FormatControl;
using FCP.Services;
using System.Windows;
using FCP.Models;

namespace FCP.src.FormatInit
{
    class BASE_MinSheng : FormatBase
    {
        private FMT_MinSheng _format;

        public override void Init()
        {
            base.Init();
            MainWindowVM.OPDToogle1Checked = true;
            MainWindowVM.OPDToogle3Checked = true;
            MainWindowVM.OPDToogle4Checked = true;
        }

        public override void ConvertPrepare()
        {
            base.ConvertPrepare();
            SetFileSearchMode(eFileSearchMode.根據檔名開頭);
            SetOPDRule("N");
            SetCareRule("E");
            SetOtherRule("K");
            SetBatchRule();
            Start();
        }

        public override void Converter()
        {
            base.Converter();
            string fileDate = Path.GetFileNameWithoutExtension(FileInfoModel.SourceFilePath).Substring(1);
            JsonService.JudgeJsonFileIsAlreadyCreated(fileDate);
            _format = _format ?? new FMT_MinSheng();
            _format.Index = JsonService.GetOLEDBIndex(FileInfoModel.Department, fileDate);
            var department = FileInfoModel.Department;
            var result = _format.DepartmentShunt();
            Result(result, false);
            JsonService.UpdateJson(fileDate, department, _format.NewPrescriptionCount);
        }

        public override MainUILayoutModel SetUILayout(MainUILayoutModel UI)
        {
            UI.Title = "民生醫院";
            UI.IP1Title = "門診";
            UI.IP2Title = "輸入路徑2";
            UI.IP3Title = "養護";
            UI.IP4Title = "大寮";
            UI.IP5Title = "UD";
            UI.OPDToogle1 = "門診";
            UI.OPDToogle2 = "";
            UI.OPDToogle3 = "養護";
            UI.OPDToogle4 = "大寮";
            UI.IP1Enabled = true;
            UI.IP2Enabled = false;
            UI.IP3Enabled = true;
            UI.IP4Enabled = true;
            UI.IP5Enabled = true;
            UI.IP6Enabled = false;
            UI.UDVisibility = Visibility.Visible;
            UI.OPD1Visibility = Visibility.Visible;
            UI.OPD2Visibility = Visibility.Hidden;
            UI.OPD3Visibility = Visibility.Visible;
            UI.OPD4Visibility = Visibility.Visible;
            return UI;
        }
    }
}
