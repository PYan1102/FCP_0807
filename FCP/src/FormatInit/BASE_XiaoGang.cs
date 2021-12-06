using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;
using FCP.src.Enum;
using FCP.src.FormatControl;
using Helper;
using System.Windows;

namespace FCP.src.FormatInit
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

        public override void Init()
        {
            base.Init();
            MainWindowVM.OPDToogle1Checked = true;
            MainWindowVM.OPDToogle2Checked = true;
            MainWindowVM.OPDToogle3Checked = true;
            MainWindowVM.OPDToogle4Checked = true;
        }

        public override void Stop()
        {
            base.Stop();
            //CMD(@"net use /del O:");
            //CMD(@"net use /del P:");
            //CMD(@"net use /del Z:");
        }

        public override void ConvertPrepare(bool isOPD)
        {
            if (isOPD)
            {
                if (base.MainWindowVM.OPDToogle4Checked)
                    CMD.ExecuteCommand(@"net use P: \\192.168.11.134\Powder_OPD");
                if (base.MainWindowVM.OPDToogle1Checked || base.MainWindowVM.OPDToogle2Checked || base.MainWindowVM.OPDToogle3Checked)
                    CMD.ExecuteCommand(@"net use O: \\192.168.11.134\Pack");
            }
            else
                CMD.ExecuteCommand(@"net use Z: \\192.168.11.134\udmachine\1");
            base.ConvertPrepare(isOPD);
            if (isOPD)
                Loop_OPD_小港();
            else
            {
                if (base.MainWindowVM.StatChecked)
                    Loop_UD(0, 1, "O");
                else
                    Loop_UD(0, 1, "B");
            }
        }

        public override void Loop_OPD_小港()
        {
            MSSql.RunSQL(@"update OCSMapping set ItemStartPosition=1,ItemDataLength=20 where OCSMapping.RawID=20116
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
            MSSql.RunSQL(@"update OCSMapping set ItemStartPosition=782,ItemDataLength=30 where OCSMapping.RawID=20116
                           update OCSMapping set ItemDataLength=20 where OCSMapping.RawID=20150
                           update OCSMapping set ItemStartPosition=1 where OCSMapping.RawID=20150
                           update OCSMapping set ItemStartPosition=542,ItemDataLength=20 where OCSMapping.RawID=20122
                           update PrintFormItem set PrintFormItemName='PackOrderMedicineRefV - Medicine_ETC14' where printformitem.RawID=120201
                           update PrintFormItem set PrintElementID=743 where printformitem.RawID=120201
                           update PrintFormItem set PrintFormItemName='PackOrderMedicineRefV - Medicine_ETC14' where printformitem.RawID=120200
                                            update PrintFormItem set PrintElementID=743 where printformitem.RawID=120200");
            if (base.MainWindowVM.StatChecked)
                MSSql.RunSQL("update SettingValue set Value='True' where RawID=71");
            else
                MSSql.RunSQL("update SettingValue set Value='False' where RawID=71");
            SwitchOnCubePage();
            base.Loop_UD(Start, Length, Content);
        }

        public override void SetConvertInformation()
        {
            //MethodID = 2為住院路徑
            //若為住院長期才整合多檔案到一個檔案內，並取得該整合檔案的檔名
            if (base.CurrentDepartment == eConvertLocation.UDBatch & !base.MainWindowVM.StatChecked)
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
            Result(result, true);
            MoveFile(result.Result);
        }

        private void MoveFile(eConvertResult result)
        {
            if (!base.MainWindowVM.StatChecked)
            {
                switch (result)
                {
                    case eConvertResult.成功:
                        MoveFilesIncludeResult(true);
                        break;
                    case eConvertResult.全數過濾 | eConvertResult.沒有種包頻率 | eConvertResult.沒有餐包頻率:
                        break;
                    default:
                        MoveFilesIncludeResult(false);
                        break;
                }
            }
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
            string FilePath = SettingsModel.InputPath3;
            List<string> list = new List<string>() { $"cd {FilePath}", $"{FilePath.Substring(0, 2)}", "dir /b" };
            string[] filesList = CMD.ExecuteCommands(list);
            foreach (string s in filesList)
            {
                if (s.Contains("Batch"))
                {
                    _FullFileName = $@"{FilePath}\{s.Trim()}";
                    return;
                }
            }
            _FullFileName = "NO";
        }

        public override UILayout SetUILayout(UILayout UI)
        {
            UI.Title = "小港醫院 > OnCube";
            UI.IP1Title = "門   診";
            UI.IP2Title = "磨   粉";
            UI.IP3Title = "住   院";
            UI.OPDToogle1 = "門診";
            UI.OPDToogle2 = "磨粉";
            UI.OPDToogle3 = "慢籤";
            UI.OPDToogle4 = "急診";
            UI.IP1Enabled = true;
            UI.IP2Enabled = true;
            UI.IP3Enabled = true;
            UI.UDVisibility = Visibility.Visible;
            UI.OPD1Visibility = Visibility.Visible;
            UI.OPD2Visibility = Visibility.Visible;
            UI.OPD3Visibility = Visibility.Visible;
            UI.OPD4Visibility = Visibility.Visible;
            return UI;
        }
    }
}