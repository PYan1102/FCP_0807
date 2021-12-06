using System.Text;
using System.IO;
using FCP.src.Enum;
using FCP.MVVM.ViewModels.GetConvertFile;
using FCP.src.FormatControl;
using System.Windows;
using System;

namespace FCP.src.FormatInit
{
    class BASE_JenKang : FunctionCollections
    {
        private FMT_JenKang _JK { get; set; }

        public override void Init()
        {
            base.Init();
            MainWindowVM.OPDToogle1Checked = true;
            InitFindFileMode(eFindFileMode.根據檔名開頭);
        }

        public override void ConvertPrepare(bool isOPD)
        {
            base.ConvertPrepare(isOPD);
            SetOPDRule("A");
            SetIntoProperty(isOPD);
            FindFile.SetUDBatch("UD");
            GetFileAsync();
        }

        public override void SetConvertInformation()
        {
            string content = GetFileContent();
            if (content.Contains("新北護理之家"))
                base.CurrentDepartment = eConvertLocation.UDBatch;
            base.SetConvertInformation();
            if (_JK == null)
                _JK = new FMT_JenKang();
            var result = _JK.MethodShunt();
            Result(result, true);
        }

        private string GetFileContent()
        {
            StringBuilder sb = new StringBuilder();
            using (StreamReader sr = new StreamReader(FilePath, Encoding.Default))
            {
                sb.Append(sr.ReadToEnd());
            }
            return sb.ToString();
        }
        public override UILayout SetUILayout(UILayout UI)
        {
            UI.Title = "仁康醫院 > OnCube";
            UI.IP1Title = "門診";
            UI.IP2Title = "輸入路徑2";
            UI.IP3Title = "住院";
            UI.OPDToogle1 = "門診";
            UI.OPDToogle2 = "";
            UI.OPDToogle3 = "";
            UI.OPDToogle4 = "";
            UI.IP1Enabled = true;
            UI.IP2Enabled = false;
            UI.IP3Enabled = true;
            UI.UDVisibility = Visibility.Visible;
            UI.OPD1Visibility = Visibility.Visible;
            UI.OPD2Visibility = Visibility.Hidden;
            UI.OPD3Visibility = Visibility.Hidden;
            UI.OPD4Visibility = Visibility.Hidden;
            return UI;
        }

    }
}
