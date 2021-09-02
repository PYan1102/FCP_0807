using System;
using System.Diagnostics;
using System.IO;
using FCP.MVVM.Models.Enum;
using FCP.MVVM.FormatControl;

namespace FCP.MVVM.FormatInit
{
    class BASE_ChangGung : FunctionCollections
    {
        private FMT_ChangGung _CG { get; set; }

        public override void Init()
        {
            base.Init();
            MainWindow.Tgl_OPD1.IsChecked = true;
            MainWindow.Tgl_OPD4.IsChecked = true;
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
            SetOPDRule("S");
            SetOtherRule("0");
            SetUDBatchRule("udpkg");
            SetIntoProperty(isOPD);
            FindFile.SetUDStatDefault();
            GetFileAsync();
        }

        public override void Loop_OPD(int Start, int Length, string Content)
        {
            base.Loop_OPD(Start, Length, Content);
        }

        public override void Loop_UD(int Start, int Length, string Content)
        {
            base.Loop_UD(Start, Length, Content);
        }

        public override void SetConvertInformation()
        {
            if (!string.IsNullOrEmpty(FilePath))
                MergeFilesAndSetFilePath();
            base.SetConvertInformation();
            _CG = _CG ?? new FMT_ChangGung();
            var result = _CG.MethodShunt();
            Result(result, true, true);
            MoveFile(result.Result);
        }

        private void MoveFile(ConvertResult result)
        {
            if (CurrentDepartment != DepartmentEnum.UDBatch & CurrentDepartment != DepartmentEnum.Other)
                return;
            switch (result)
            {
                case ConvertResult.成功:
                    MoveFilesIncludeResult(true);
                    break;
                case ConvertResult.全數過濾:
                    break;
                case ConvertResult.沒有種包頻率:
                    break;
                case ConvertResult.沒有餐包頻率:
                    break;
                default:
                    MoveFilesIncludeResult(false);
                    break;
            }
        }

        public override void ProgressBoxClear()
        {
            base.ProgressBoxClear();
        }

        private void MergeFilesAndSetFilePath()
        {
            if (FilePath == string.Empty)
                return;
            if (CurrentDepartment == DepartmentEnum.Other)
            {
                base.FilePath = MergeFilesAndGetNewFilePath(Path.GetDirectoryName(base.FilePath), "藥來速", 0, 1, "0");
            }
            else if (!MainWindowVM.StatChecked && CurrentDepartment == DepartmentEnum.UDBatch)
            {
                base.FilePath = MergeFilesAndGetNewFilePath(Path.GetDirectoryName(base.FilePath), "住院批次", 0, 5, "udpkg");
            }
        }
    }
}
