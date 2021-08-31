using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;
using FCP.MVVM.Models.Enum;
using FCP.MVVM.FormatControl;
using FCP.MVVM.SQL;

namespace FCP.MVVM.FormatInit
{
    class BASE_XiaoGang : FunctionCollections
    {
        private FMT_XiaoGang _XG { get; set; }
        private string _FullFileName { get; set; }

        public enum KeyModifilers
        {
            None = 0, Alt = 1, Ctrl = 2, Shift = 3
        }
        [DllImport("user32.dll", EntryPoint = "keybd_event", SetLastError = true)]
        public static extern void keybd_event(System.Windows.Forms.Keys bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        public BASE_XiaoGang()
        {

        }

        public override void Init()
        {
            base.Init();
            MainWindow.Tgl_OPD1.IsChecked = true;
            MainWindow.Tgl_OPD2.IsChecked = true;
            MainWindow.Tgl_OPD3.IsChecked = true;
            MainWindow.Tgl_OPD4.IsChecked = true;
        }

        public override void ShowAdvancedSettings()
        {
            base.ShowAdvancedSettings();
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
            //CMD(@"net use /del O:");
            //CMD(@"net use /del P:");
            //CMD(@"net use /del Z:");
        }

        public override void CloseSelf()
        {
            base.CloseSelf();
        }

        public override void ConvertPrepare(bool isOPD)
        {
            if (isOPD)
            {
                if (base.WD._OPD4)
                    CMD(@"net use P: \\192.168.11.134\Powder_OPD");
                if (base.WD._OPD1 || base.WD._OPD2 || base.WD._OPD3)
                    CMD(@"net use O: \\192.168.11.134\Pack");
            }
            else
                CMD(@"net use Z: \\192.168.11.134\udmachine\1");
            base.ConvertPrepare(isOPD);
            if (isOPD)
                Loop_OPD_小港();
            else
            {
                if (base.WD._IsStat)
                    Loop_UD(0, 1, "O");
                else
                    Loop_UD(0, 1, "B");
            }
        }

        public override void Loop_OPD_小港()
        {
            SQLQuery.NonQuery(@"update OCSMapping set ItemStartPosition=1,ItemDataLength=20 where OCSMapping.RawID=20116
                                            update OCSMapping set ItemDataLength=30 where OCSMapping.RawID=20150
                                            update OCSMapping set ItemStartPosition=872 where OCSMapping.RawID=20150
                                            update OCSMapping set ItemStartPosition=135,ItemDataLength=20 where OCSMapping.RawID=20122
                                            update PrintFormItem set PrintFormItemName='Patient - Name' where printformitem.RawID=120201
                                            update PrintFormItem set PrintElementID=223 where printformitem.RawID=120201
                                            update PrintFormItem set PrintFormItemName='Patient - Name' where printformitem.RawID=120200
                                            update PrintFormItem set PrintElementID=223 where printformitem.RawID=120200
                                            update PrintMetaDataElement set DeletedYN='false' where RawID=60354
                                            update SettingValue set Value='True' where RawID=71");
            SwitchOnCubePage();
            base.Loop_OPD_小港();
        }

        public override void Loop_UD(int Start, int Length, string Content)
        {
            SQLQuery.NonQuery(@"update OCSMapping set ItemStartPosition=782,ItemDataLength=30 where OCSMapping.RawID=20116
                                            update OCSMapping set ItemDataLength=20 where OCSMapping.RawID=20150
                                            update OCSMapping set ItemStartPosition=1 where OCSMapping.RawID=20150
                                            update OCSMapping set ItemStartPosition=542,ItemDataLength=20 where OCSMapping.RawID=20122
                                            update PrintFormItem set PrintFormItemName='PackOrderMedicineRefV - Medicine_ETC14' where printformitem.RawID=120201
                                            update PrintFormItem set PrintElementID=743 where printformitem.RawID=120201
                                            update PrintFormItem set PrintFormItemName='PackOrderMedicineRefV - Medicine_ETC14' where printformitem.RawID=120200
                                            update PrintFormItem set PrintElementID=743 where printformitem.RawID=120200");
            if (base.WD._IsStat)
                SQLQuery.NonQuery("update SettingValue set Value='True' where RawID=71");
            else
                SQLQuery.NonQuery("update SettingValue set Value='False' where RawID=71");
            SwitchOnCubePage();
            base.Loop_UD(Start, Length, Content);
        }

        public override void SetConvertInformation()
        {
            //MethodID = 2為住院路徑
            //若為住院長期才整合多檔案到一個檔案內，並取得該整合檔案的檔名
            if (base.CurrentDepartment == DepartmentEnum.UDBatch & !base.WD._IsStat)
            {
                FilesAllInOne();
                GetBatchFile();

                //更新FunctionsCollections內的FilePath值
                //若不加此條，會引發在Result要移檔案時出現找不到一開始取得的檔案
                //因為程式會先在Loop找到B開頭的檔案才進行上面的整合及取得整合檔案的檔名
                //參考 GetBatchFile()
                base.FilePath = _FullFileName;
            }
            base.SetConvertInformation();
            if (_XG == null)
                _XG = new FMT_XiaoGang();
            var result = _XG.MethodShunt();
            Result(result, true, true);
            MoveFile(result.Result);
        }

        private void MoveFile(ConvertResult result)
        {
            if (!WD._IsStat)
            {
                switch (result)
                {
                    case ConvertResult.成功:
                        MoveFilesIncludeResult(true);
                        break;
                    case ConvertResult.全數過濾 | ConvertResult.沒有種包頻率 | ConvertResult.沒有餐包頻率:
                        break;
                    default:
                        MoveFilesIncludeResult(false);
                        break;
                }
            }
        }

        public override void ProgressBoxClear()
        {
            base.ProgressBoxClear();
        }

        private void SwitchOnCubePage()
        {
            foreach (Process p in Process.GetProcessesByName("OnCube"))
            {
                SetForegroundWindow(p.MainWindowHandle);
            }
            Thread.Sleep(500);
            keybd_event(System.Windows.Forms.Keys.F12, 0, 0, 0);
            Thread.Sleep(20);
            keybd_event(System.Windows.Forms.Keys.F12, 0, 2, 0);
            Thread.Sleep(500);
            keybd_event(System.Windows.Forms.Keys.F11, 0, 0, 0);
            Thread.Sleep(20);
            keybd_event(System.Windows.Forms.Keys.F11, 0, 2, 0);
        }

        //多個檔案整合成單一檔案
        private void FilesAllInOne()
        {
            Process p = new Process();
            p.StartInfo.FileName = "MultipleFilesInOneFile.exe";
            p.StartInfo.UseShellExecute = false;
            p.Start();
            p.WaitForExit();
            p.Close();
        }

        //取得整合完的檔案檔名
        private void GetBatchFile()
        {
            WD.MainWindow.Dispatcher.Invoke(new Action(() =>
            {
                string FilePath = SettingsModel.InputPath3;
                List<string> L = new List<string>() { $"cd {FilePath}", $"{FilePath.Substring(0, 2)}", "dir /b" };
                string[] FilesList = CMD(L);
                foreach (string s in FilesList)
                {
                    if (s.Contains("Batch"))
                    {
                        _FullFileName = $@"{FilePath}\{s.Trim()}";
                        return;
                    }
                }
                _FullFileName = "NO";
            }));
        }
    }
}