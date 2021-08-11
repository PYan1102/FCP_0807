using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace FCP.MVVM.FormatInit
{
    class BASE_MinSheng : FunctionCollections
    {
        FMT_MinSheng MS;
        DoJson doJson = new DoJson();
        public BASE_MinSheng()
        {

        }

        public override void Init()
        {
            MainWindow.Tgl_OPD1.IsChecked = true;
            MainWindow.Tgl_OPD2.IsChecked = true;
            MainWindow.Tgl_OPD3.IsChecked = true;
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

        public override void ConvertPrepare(bool isOPD)
        {
            base.ConvertPrepare(isOPD);
            if (isOPD)
            {
                string Filter = WD._OPD1 ? "N|" : "";
                Filter += WD._OPD2 ? "E|" : "";
                Filter += WD._OPD3 ? "K" : "";
                Loop_OPD(0, 1, Filter);
            }
            else
            {
                Loop_UD(0, 0, "");
            }
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
            base.SetConvertInformation();
            string Date = Path.GetFileNameWithoutExtension(base.FilePath).Substring(1);
            string FirstFileName = Path.GetFileNameWithoutExtension(base.FilePath).Substring(0, 1);
            int Index = FirstFileName == "N" ? 0 : FirstFileName == "E" ? 1 : FirstFileName == "K" ? 2 : 3;
            doJson.JudgeJson(Date);
            if (MS == null)
                MS = new FMT_MinSheng();
            MS.Index = doJson.GetIndex(Index, Date);
            var result = MS.MethodShunt();
            Result(result, false, false);
            doJson.UpdateJson(Date, Index, MS.newCount);
        }

        public override void ProgressBoxClear()
        {
            base.ProgressBoxClear();
        }
    }
}
