using System;
using System.Diagnostics;
using System.IO;
using FCP.MVVM.Models.Enum;

namespace FCP.MVVM.FormatInit
{
    class BASE_ChangGung : FunctionCollections
    {
        FMT_ChangGung CG = new FMT_ChangGung();
        string FullFileName = "";

        public BASE_ChangGung()
        {

        }

        public override void Loaded()
        {
            base.Loaded();
            MainWindow.Tgl_OPD1.IsChecked = true;
            MainWindow.Tgl_OPD2.IsChecked = true;
        }

        public override void AdvancedSettingsShow()
        {
            base.AdvancedSettingsShow();
        }

        public override void AutoStart()
        {
            base.AutoStart();
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

        public override void ConvertPrepare(int Mode)
        {
            base.ConvertPrepare(Mode);
            if (Mode == (int)ModeEnum.OPD)
            {
                string Filter = base.WD._OPD1 ? "S|" : "";
                Filter += base.WD._OPD2 ? "0" : "";
                Loop_OPD(0, 1, Filter);
            }
            else if (base.WD._isStat)
                Loop_UD(0, 1, "6");
            else
                Loop_UD(0, 5, "udpkg");

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
            if (base.MethodID == 1)
            {
                base.FilePath = MergeFiles(Path.GetDirectoryName(base.FilePath), "藥來速", 0, 1, "0");
            }
            else if (!WD._isStat && base.MethodID == 2)
            {
                base.FilePath = MergeFiles(Path.GetDirectoryName(base.FilePath), "住院批次", 0, 5, "udpkg");
                Debug.WriteLine(base.FilePath);
            }
            base.SetConvertInformation();
            CG = CG ?? new FMT_ChangGung();
            var result = CG.MethodShunt();
            Result(result, true, true);
            MoveFile(result.Result);
        }

        private void MoveFile(ConvertResult result)
        {
            switch (result)
            {
                case ConvertResult.成功:
                    MoveFilesIncludeResult("ok");
                    break;
                case ConvertResult.失敗:
                    MoveFilesIncludeResult("fail");
                    break;
            }
        }

        public override void ProgressBoxClear()
        {
            base.ProgressBoxClear();
        }
    }
}
