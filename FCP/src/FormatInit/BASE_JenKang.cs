using System.Text;
using System.IO;
using FCP.src.Enum;
using FCP.src.FormatControl;
using System.Windows;
using FCP.Models;
using Microsoft.Toolkit.Mvvm.Messaging;
using FCP.src.MessageManager;

namespace FCP.src.FormatInit
{
    class BASE_JenKang : FormatBase
    {
        private FMT_JenKang _format;

        public override void Init()
        {
            base.Init();
            WeakReferenceMessenger.Default.Send(new SetMainWindowToogleCheckedChangeMessage(new MainWindowModel.ToogleModel() { Toogle1 = true }));
        }

        public override ActionResult PrepareStart()
        {
            SetFileSearchMode(eFileSearchMode.根據檔名開頭);
            SetOPDRule("A");
            SetBatchRule("UD");
            return base.PrepareStart();
        }

        public override void Converter()
        {
            string content = GetFileContent();
            if (content.Contains("新北護理之家"))
                base.CurrentDepartment = eDepartment.Batch;
            _format = _format ?? new FMT_JenKang();
            var result = _format.DepartmentShunt();
            Result(result, true);
        }

        private string GetFileContent()
        {
            StringBuilder sb = new StringBuilder();
            using (StreamReader sr = new StreamReader(FileInfoModel.SourceFilePath, Encoding.Default))
            {
                sb.Append(sr.ReadToEnd());
            }
            return sb.ToString();
        }

        public override MainUILayoutModel SetUILayout(MainUILayoutModel UI)
        {
            UI.Title = "仁康醫院";
            UI.IP1Title = "門診";
            UI.IP1Enabled = true;
            UI.IP2Enabled = false;
            UI.IP3Enabled = false;
            UI.IP4Enabled = false;
            UI.IP5Enabled = true;
            UI.OPDToogle1 = "門診";
            UI.OPDToogle2 = string.Empty;
            UI.OPDToogle3 = string.Empty;
            UI.OPDToogle4 = string.Empty;
            UI.UDVisibility = Visibility.Visible;
            UI.OPD1Visibility = Visibility.Visible;
            UI.OPD2Visibility = Visibility.Hidden;
            UI.OPD3Visibility = Visibility.Hidden;
            UI.OPD4Visibility = Visibility.Hidden;
            return UI;
        }

    }
}
