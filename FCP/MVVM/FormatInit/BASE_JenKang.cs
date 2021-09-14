using System;
using System.Text;
using System.IO;
using FCP.MVVM.Models.Enum;
using FCP.MVVM.ViewModels.GetConvertFile;
using FCP.MVVM.FormatControl;

namespace FCP.MVVM.FormatInit
{
    class BASE_JenKang : FunctionCollections
    {
        private FMT_JenKang _JK { get; set; }

        public override void Init()
        {
            base.Init();
            MainWindow.Tgl_OPD1.IsChecked = true;
            InitFindFileMode(FindFileModeEnum.根據檔名開頭);
        }

        public override void Save()
        {
            base.Save();
        }

        public override void Stop()
        {
            base.Stop();
        }

        public override void CloseSelf()
        {
            base.CloseSelf();
        }

        public override void ConvertPrepare(bool isOPD)
        {
            base.ConvertPrepare(isOPD);
            SetOPDRule(nameof(DefaultEnum.Default));
            SetIntoProperty(isOPD);
            FindFile.SetUDBatchDefault();
            GetFileAsync();
        }

        public override void SetConvertInformation()
        {
            string content = GetFileContent();
            if (content.Contains("新北護理之家"))
                base.CurrentDepartment = DepartmentEnum.UDBatch;
            base.SetConvertInformation();
            if (_JK == null)
                _JK = new FMT_JenKang();
            var result = _JK.MethodShunt();
            Result(result, true, true);
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


        public override void ProgressBoxClear()
        {
            base.ProgressBoxClear();
        }
    }
}
