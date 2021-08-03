using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Threading;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Data.SqlClient;

namespace FCP
{
    public abstract class FunctionCollections
    {
        public string SQLInfo = "server=.;database=OnCube;user id=sa;password=jvm5822511";
        public string IP1, IP2, IP3, OP = "";
        public string PackedType;
        public const string FileBackupPath = @"D:\Converter_Backup";
        System.Windows.Forms.NotifyIcon NF = new System.Windows.Forms.NotifyIcon();
        System.Windows.Forms.ContextMenuStrip CM = new System.Windows.Forms.ContextMenuStrip();
        public Log Log = new Log();
        public Settings Settings;
        public SmallForm SF;
        public AdvancedSettings AS;
        MsgB Msg;
        MergeFiles Merge = new MergeFiles();
        CancellationTokenSource cts;
        CancellationTokenSource cts1;
        List<string> IPList = new List<string>();
        public WindowsDo WD;
        int Mode = 0;
        static string SuccessPath, FailPath;
        public string FilePath, InputPath, OutputPath, NowSecond;
        public int MethodID;

        public enum ResultType
        {
            成功 = 0, 失敗 = 1, 全數過濾 = 2, 沒有頻率 = 3
        }

        public enum ModeEnum
        {
            OPD = 0, UD = 1
        }

        //建立MsgBox物件
        public virtual void Loaded(MainWindow mw, Settings S)
        {
            try
            {
                Log.Check();
                Settings = S;
                SF = new SmallForm(mw, S) { Owner = mw };
                AS = new AdvancedSettings(mw, S, Log) { Owner = mw };
                Msg = new MsgB() { Owner = mw };
                WD = new WindowsDo() { mw = mw, Msg = Msg, Settings = S, Log = Log, SF = SF, AS = AS };
                CheckProgramStart();
                CheckFileBackupPath();
                CreateIcon();
                WD.ProcessAction();
                NF.Text = (bool)mw.tgl_AutoStart.IsChecked ? "轉檔程式：轉檔進行中" : "轉檔程式：停止";
                cts1 = new CancellationTokenSource();
                mw.Dispatcher.InvokeAsync(new Action(() => WD.UI_Refresh(cts1)));
            }
            catch (Exception ex)
            {
                Log.Write($"{ex}");
                MessageBox.Show($"{ex}");
            }
        }

        //檢查程式是否已開啟
        public void CheckProgramStart()
        {
            if (Process.GetProcessesByName("FCP").Length >= 2)
            {
                Msg.Show("程式已開啟，請確認工具列", "重複開啟", "Error", Msg.Color.Error);
                Log.Write("程式已開啟，請確認工具列");
                Environment.Exit(0);
            }
        }

        //檢查備份資料夾是否存在
        public static void CheckFileBackupPath()
        {
            string BackupPath = $@"{FileBackupPath}\{DateTime.Now:yyyy-MM-dd}";
            if (!Directory.Exists($@"{BackupPath}\Success")) Directory.CreateDirectory($@"{BackupPath}\Success");
            if (!Directory.Exists($@"{BackupPath}\Fail")) Directory.CreateDirectory($@"{BackupPath}\Fail");
            SuccessPath = $@"{BackupPath}\Success";
            FailPath = $@"{BackupPath}\Fail";
        }

        public void AllWindowShowOrHide(bool b1, bool b2, bool b3)
        {
            WD.AllWindowShowOrHide(b1, b2, b3);
        }

        private void CreateIcon()
        {
            CM = new System.Windows.Forms.ContextMenuStrip();
            CM.Items.Add("離開");
            CM.ItemClicked += CMS_ItemClicked;
            NF.Icon = new System.Drawing.Icon(new Uri("FCP.ico", UriKind.Relative).ToString());
            NF.Visible = true;
            NF.Text = "轉檔狀態：停止";
            NF.ContextMenuStrip = CM;
            NF.DoubleClick += IconDBClick;
        }

        private void CMS_ItemClicked(object sender, System.Windows.Forms.ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.ToString().Equals("離開"))
                Environment.Exit(0);
        }

        public void IconDBClick(object sender, EventArgs e)
        {
            WD.IconDBClick();
        }

        public virtual void AdvancedSettingsShow()
        {
            WD.AdvancedSettingsShow();
        }

        public virtual void Stop()
        {
            if (cts != null) cts.Cancel();
            WD.Stop_Control();
            WD.SwitchControlEnabled(true);
            NF.Text = "停止";
        }

        public virtual void Save()
        {
            try
            {
                Properties.Settings.Default.X = WD.X;
                Properties.Settings.Default.Y = WD.Y;
                Properties.Settings.Default.Save();
                Settings.Check();
                Settings.SaveForm1(WD.IP1, WD.IP2, WD.IP3, WD.OP, WD._AutoStart, WD._isStat ? "S" : "B");
                ProgressBoxAdd("儲存成功");
            }
            catch (Exception ex)
            {
                Log.Write($"{ex}");
            }
        }

        public void ChangeWindow()
        {
            WD.ChangeWindow();
        }

        public virtual void ConvertPrepare(int Mode)
        {
            if ((WD.IP1 + WD.IP2 + WD.IP3).Trim() == "")
            {
                if (cts != null) Stop();
                Msg.Show("來源路徑為空白", "路徑空白", "Error", Msg.Color.Error);
                return;
            }
            if (WD.OP.Trim() == "")
            {
                if (cts != null) Stop();
                Msg.Show("輸出路徑為空白", "路徑空白", "Error", Msg.Color.Error);
                return;
            }
            this.Mode = Mode;
            WD.Start_Control(Mode);
            WD.SwitchControlEnabled(false);
            NF.Text = "開始";
            IPList.Clear();
            IPList.Add(WD.IP1);
            IPList.Add(WD.IP2);
            WD.AllWindowShowOrHide(null, null, false);
            cts = null;
            cts = new CancellationTokenSource();
        }

        public virtual void Loop_OPD(int Start, int Length, string Content)
        {
            if (cts == null)
                return;
            Task.Run(async () =>
            {
                try
                {
                    while (!cts.IsCancellationRequested)
                    {
                        await Task.Delay(Settings.Speed);
                        foreach (string IP in IPList)
                        {
                            int Index = IPList.IndexOf(IP);
                            if (IP.Trim() == "")
                                continue;
                            foreach (string Name in Directory.GetFiles(IP, Settings.DeputyFileName))
                            {
                                bool _Exit = false;
                                if (Content != "")
                                {
                                    string[] FirstWord = Content.Split('|');
                                    foreach (string s in FirstWord)
                                    {
                                        if (s == "")
                                            continue;
                                        if (Path.GetFileNameWithoutExtension(Name).Substring(Start, Length) == s)
                                        {
                                            SetConvertValue(Index, Path.GetDirectoryName(IP), WD.OP, Name);
                                            Converter();
                                            _Exit = true;
                                            break;
                                        }
                                    }
                                    if (_Exit)
                                        break;
                                }
                                else
                                {
                                    SetConvertValue(Index, Path.GetDirectoryName(IP), WD.OP, Name);
                                    Converter();
                                    break;
                                }
                            }
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Write($"{FilePath} {ex}");
                    MessageBox.Show($"{FilePath} {ex}");
                }
            });
        }

        public virtual void Loop_OPD_小港()
        {
            if (cts == null)
                return;
            Task.Run(async () =>
            {
                try
                {
                    while (!cts.IsCancellationRequested)
                    {
                        await Task.Delay(Settings.Speed);
                        foreach (string IP in IPList)
                        {
                            int Index = IPList.IndexOf(IP);
                            if (IP.Trim() == "")
                                continue;
                            foreach (string Name in Directory.GetFiles(IP, Settings.DeputyFileName))
                            {
                                string FileName = Path.GetFileNameWithoutExtension(Name);
                                if (FileName.Contains(" "))  //門診、急診、慢籤
                                {
                                    string[] Split = FileName.Split(' ');
                                    int No = Convert.ToInt32(Split[1]);

                                    if (WD._OPD1 & 1 <= No & No <= 4999)
                                    {
                                        SetConvertValue(Index, Path.GetDirectoryName(IP), WD.OP, Name);
                                        Converter();
                                        break;
                                    }
                                    else if (WD._OPD2 & 6001 <= No & No <= 7999)
                                    {
                                        SetConvertValue(Index, Path.GetDirectoryName(IP), WD.OP, Name);
                                        Converter();
                                        break;
                                    }
                                    else if (WD._OPD3 & 9001 <= No & No < 19999)
                                    {
                                        SetConvertValue(Index, Path.GetDirectoryName(IP), WD.OP, Name);
                                        Converter();
                                        break;
                                    }
                                }
                                else if (WD._OPD4) //磨粉
                                {
                                    SetConvertValue(Index, Path.GetDirectoryName(IP), WD.OP, Name);
                                    Converter();
                                    break;
                                }
                            }
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Write($"{FilePath} {ex}");
                    MessageBox.Show($"{FilePath} {ex}");
                }
            });
        }

        public virtual void Loop_UD(int Start, int Length, string Content)
        {
            if (cts == null)
                return;
            Task.Run(async () =>
            {
                try
                {
                    while (!cts.IsCancellationRequested)
                    {
                        await Task.Delay(Settings.Speed);
                        string IP = WD.IP3;
                        if (IP.Trim() == "")
                            continue;
                        foreach (string Name in Directory.GetFiles(IP, Settings.DeputyFileName))
                        {
                            if (Settings.EN_StatOrBatch)
                            {
                                if (Path.GetFileNameWithoutExtension(Name).Substring(Start, Length) == Content)
                                {
                                    SetConvertValue(2, Path.GetDirectoryName(IP), WD.OP, Name);
                                    Converter();
                                    break;
                                };
                            }
                            else
                            {
                                SetConvertValue(2, Path.GetDirectoryName(IP), WD.OP, Name);
                                Converter();
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Write($"{ex}");
                    MessageBox.Show($"{FilePath} {ex}");
                }
            });
        }

        private void SetConvertValue(int MethodID, string IP, string OP, string File)
        {
            this.MethodID = MethodID;
            InputPath = IP;
            OutputPath = OP;
            FilePath = File;
        }

        public virtual void Converter()
        {
            NowSecond = DateTime.Now.ToString("ss.ffff");
            //Save();
        }

        public virtual void Result(string Result, bool NeedMoveFile, bool NeedReminder)
        {
            string[] ResultSplit = Result.Split('|');
            string R = ResultSplit[1];
            string FileName = $"{Path.GetFileName(FilePath)}_{ DateTime.Now:ss_fff}";
            switch (Convert.ToInt32(ResultSplit[0]))
            {
                case (int)ResultType.成功:
                    if (NeedMoveFile)
                    {
                        File.Move(FilePath, $@"{SuccessPath}\{FileName}.ok");
                        WD.SuccessCountAdd();
                    }
                    ProgressBoxAdd(R);
                    NF.ShowBalloonTip(850, "轉檔成功", R, System.Windows.Forms.ToolTipIcon.None);
                    break;
                case (int)ResultType.全數過濾:
                    if (NeedMoveFile)
                    {
                        File.Move(FilePath, $@"{SuccessPath}\{FileName}.ok");
                        WD.SuccessCountAdd();
                    }
                    if (NeedReminder)
                    {
                        ProgressBoxAdd(R);
                        NF.ShowBalloonTip(850, "全數過濾", R, System.Windows.Forms.ToolTipIcon.None);
                    }
                    break;
                case (int)ResultType.失敗:
                    if (NeedMoveFile)
                    {
                        File.Move(FilePath, $@"{FailPath}\{FileName}.fail");
                        WD.FailCountAdd();
                    }
                    ProgressBoxAdd(R);
                    NF.ShowBalloonTip(850, "轉檔錯誤", R, System.Windows.Forms.ToolTipIcon.Error);
                    break;
                case (int)ResultType.沒有頻率:
                    Stop();
                    ProgressBoxAdd(R);
                    NF.ShowBalloonTip(850, $"缺少頻率", $"{Path.GetFileName(FilePath)} OnCube中缺少該檔案 {R} 的頻率", System.Windows.Forms.ToolTipIcon.Error);
                    break;
            }
            ////Stop();
        }

        public virtual void ProgressBoxClear()
        {
            WD.ProgressBoxClear();
        }

        private void ProgressBoxAdd(string Result)
        {
            WD.ProgressBoxAdd($"{DateTime.Now:HH:mm:ss:fff} {Result}");
        }

        public virtual async void AutoStart()
        {
            await Task.Delay(1000);
            WD.AutoStart();
        }

        public void CMD(string Command)
        {
            Process p = new Process();
            p.StartInfo.FileName = "CMD.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.Start();
            p.StandardInput.WriteLine(Command);
            p.StandardInput.WriteLine("exit");
            p.Close();
        }

        public string[] CMD(List<string> L)
        {
            Process p = new Process();
            p.StartInfo.FileName = "CMD.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.Start();
            L.ForEach(x => p.StandardInput.WriteLine(x));
            p.StandardInput.WriteLine("exit");
            return p.StandardOutput.ReadToEnd().Split('\n');
        }

        public void Query(string Command)
        {
            SqlConnection con = new SqlConnection(SQLInfo);
            con.Open();
            SqlCommand com = new SqlCommand(Command, con);
            com.ExecuteNonQuery();
            com.Dispose();
            con.Close();
        }

        public virtual void CloseSelf()
        {
            Stop();
            cts1.Cancel();
            NF.Dispose();
            CM.Dispose();
            WD = null;
        }

        public string MergeFiles(string In, string FileName, int Start, int Length, string Word)
        {
            Merge.SetValue(In, FileName);
            Merge.Merge(Start, Length, Word);
            return Merge.FileName;
        }

        //移動檔案
        public void MoveFilesIncludeResult(string Result)
        {
            string FolderName = Result == "ok" ? "Success" : "Fail";
            string[] Files = Directory.GetFiles($@"D:\Converter_Backup\{DateTime.Now:yyyy-MM-dd}\Batch");
            foreach (string s in Files)
            {
                File.Move(s, $@"D:\Converter_Backup\{DateTime.Now:yyyy-MM-dd}\{FolderName}\{Path.GetFileNameWithoutExtension(s)}.{Result}");
            }
            if (SF.Visibility == Visibility.Visible)
                SF.StopConverter_button_Click(null, null);
            else
                Stop();
        }
    }

    public class WindowsDo : Window
    {
        public MainWindow mw { get; set; }
        public Settings Settings { get; set; }
        public MsgB Msg { get; set; }
        public Log Log { get; set; }
        public AdvancedSettings AS { get; set; }
        public SmallForm SF { get; set; }
        public string IP1 { get { string A = ""; Dispatcher.Invoke(new Action(() => { A = mw.txt_InputPath1.Text; })); return A; } }
        public string IP2 { get { string A = ""; Dispatcher.Invoke(new Action(() => { A = mw.txt_InputPath2.Text; })); return A; } }
        public string IP3 { get { string A = ""; Dispatcher.Invoke(new Action(() => { A = mw.txt_InputPath3.Text; })); return A; } }
        public string OP { get { string A = ""; Dispatcher.Invoke(new Action(() => { A = mw.txt_OutputPath.Text; })); return A; } }
        public int X { get { int xPoint = 0; Dispatcher.Invoke(new Action(() => { xPoint = Convert.ToInt32(mw.txt_X.Text.Trim()); })); return xPoint; } }
        public int Y { get { int yPoint = 0; Dispatcher.Invoke(new Action(() => { yPoint = Convert.ToInt32(mw.txt_Y.Text.Trim()); })); return yPoint; } }
        public bool _AutoStart { get { bool B = false; Dispatcher.Invoke(new Action(() => { B = (bool)mw.tgl_AutoStart.IsChecked; })); return B; } }
        public bool _isStat { get { bool B = false; Dispatcher.Invoke(new Action(() => { B = (bool)mw.rdo_Stat.IsChecked; })); return B; } }  //true > Stat, false > Batch
        public bool _OPD1 { get { bool B = false; Dispatcher.Invoke(new Action(() => { B = (bool)mw.chk_OPD1.IsChecked; })); return B; } }
        public bool _OPD2 { get { bool B = false; Dispatcher.Invoke(new Action(() => { B = (bool)mw.chk_OPD2.IsChecked; })); return B; } }
        public bool _OPD3 { get { bool B = false; Dispatcher.Invoke(new Action(() => { B = (bool)mw.chk_OPD3.IsChecked; })); return B; } }
        public bool _OPD4 { get { bool B = false; Dispatcher.Invoke(new Action(() => { B = (bool)mw.chk_OPD4.IsChecked; })); return B; } }

        bool isMainWindow = true;  //true > mainwindow, false > smallform

        SolidColorBrush Red = new SolidColorBrush((Color)Color.FromRgb(255, 82, 85));
        SolidColorBrush White = new SolidColorBrush((Color)Color.FromRgb(255, 255, 255));

        public void ProcessAction()
        {
            try
            {
                string BackupPath = $@"{FunctionCollections.FileBackupPath}\{DateTime.Now:yyyy-MM-dd}";
                mw.txtb_Success.Text = $"{Directory.GetFiles($@"{BackupPath}\Success").Length}";
                mw.txtb_Fail.Text = $"{Directory.GetFiles($@"{BackupPath}\Fail").Length}";
                mw.txt_InputPath1.Text = Settings.InputPath1;
                mw.txt_InputPath2.Text = Settings.InputPath2;
                mw.txt_InputPath3.Text = Settings.InputPath3;
                mw.txt_OutputPath.Text = Settings.OutputPath1;
                mw.tgl_AutoStart.IsChecked = Settings.EN_AutoStart;
                mw.txt_X.Text = Properties.Settings.Default.X.ToString();
                mw.txt_Y.Text = Properties.Settings.Default.Y.ToString();
                mw.btn_Stop.IsEnabled = false;
                mw.chk_OPD1.IsChecked = false;
                mw.chk_OPD2.IsChecked = false;
                mw.chk_OPD3.IsChecked = false;
                mw.chk_OPD4.IsChecked = false;
                mw.rdo_Stat.IsChecked = false;
                if (Settings.StatOrBatch == "S") mw.rdo_Stat.IsChecked = true; else mw.rdo_Batch.IsChecked = true;
                if (Settings.EN_WindowMinimumWhenOpen)
                    ChangeWindow();
            }
            catch (Exception a)
            {
                Msg.Show(a.ToString(), "錯誤", "Error", Msg.Color.Error);
                Log.Write(a.ToString());
            }
        }  //佈署控鍵

        public async void UI_Refresh(CancellationTokenSource cts)
        {
            while (!cts.IsCancellationRequested)
            {
                await Task.Delay(500);
                try
                {
                    //string FileVersion = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FileVersion.ToString();  //版本
                    SF.SetFormLocation(Properties.Settings.Default.X, Properties.Settings.Default.Y);
                    if (!(bool)mw.btn_Stop.IsEnabled)
                    {
                        UILayout UI = new UILayout();
                        if (Settings.Mode == (int)Settings.ModeEnum.小港醫院)
                            小港ToOnCube(UI);
                        else if (Settings.Mode == (int)Settings.ModeEnum.光田OnCube)
                            光田ToOnCube(UI);
                        else if (Settings.Mode == (int)Settings.ModeEnum.光田JVS)
                            光田ToJVServer(UI);
                        else if (Settings.Mode == (int)Settings.ModeEnum.民生醫院)
                            民生ToOnCube(UI);
                        else if (Settings.Mode == (int)Settings.ModeEnum.義大醫院)
                            義大ToOnCube(UI);
                        else if (Settings.Mode == (int)Settings.ModeEnum.長庚磨粉)
                            長庚磨粉ToJVServer(UI);
                        else if (Settings.Mode == (int)Settings.ModeEnum.長庚醫院)
                            長庚醫院ToOnCube(UI);
                        else if (Settings.Mode == (int)Settings.ModeEnum.仁康醫院)
                            仁康醫院ToOnCube(UI);
                        else
                            JVServerToOnCube(UI);
                        UI_LayoutSet(UI);
                        mw.PackType_textblock.Text = Settings.DoseMode == "M" ? "餐包" : "種包";
                    }
                    mw.rdo_Stat.Visibility = Settings.EN_StatOrBatch ? Visibility.Visible : Visibility.Hidden;
                    mw.rdo_Batch.Visibility = Settings.EN_StatOrBatch ? Visibility.Visible : Visibility.Hidden;
                    mw.btn_Close.Visibility = Settings.EN_ShowControlButton ? Visibility.Visible : Visibility.Hidden;
                    mw.btn_Minimum.Visibility = Settings.EN_ShowControlButton ? Visibility.Visible : Visibility.Hidden;
                    mw.bd_X.Visibility = Settings.EN_ShowXY ? Visibility.Visible : Visibility.Hidden;
                    mw.bd_Y.Visibility = Settings.EN_ShowXY ? Visibility.Visible : Visibility.Hidden;
                    FunctionCollections.CheckFileBackupPath();
                }
                catch (Exception a)
                {
                    Msg.Show(a.ToString(), "錯誤", "Error", Msg.Color.Error);
                    Log.Write(a.ToString());
                }
            }
        }

        private void UI_LayoutSet(UILayout UI)
        {
            mw.txtb_Title.Text = UI.Title;
            mw.txtb_InputPath1.Text = UI.IP1s;
            mw.txtb_InputPath2.Text = UI.IP2s;
            mw.txtb_InputPath3.Text = UI.IP3s;
            mw.txt_OPD1.Text = UI.OPD1s;
            mw.txt_OPD2.Text = UI.OPD2s;
            mw.txt_OPD3.Text = UI.OPD3s;
            mw.txt_OPD4.Text = UI.OPD4s;
            mw.btn_InputPath1.IsEnabled = UI.IP1b;
            mw.btn_InputPath2.IsEnabled = UI.IP2b;
            mw.btn_InputPath3.IsEnabled = UI.IP3b;
            mw.btn_UD.Visibility = UI.UDv;
            mw.chk_OPD1.Visibility = UI.OPD1v;
            mw.chk_OPD2.Visibility = UI.OPD2v;
            mw.chk_OPD3.Visibility = UI.OPD3v;
            mw.chk_OPD4.Visibility = UI.OPD4v;
        }

        private void JVServerToOnCube(UILayout UI)
        {
            UI.Title = "Normal > OnCube";
            UI.IP1s = "輸入路徑1";
            UI.IP2s = "輸入路徑2";
            UI.IP3s = "輸入路徑3";
            UI.OPD1s = "門診";
            UI.OPD2s = "";
            UI.OPD3s = "";
            UI.OPD4s = "";
            UI.IP1b = true;
            UI.IP2b = true;
            UI.IP3b = true;
            UI.UDv = Visibility.Hidden;
            UI.OPD1v = Visibility.Visible;
            UI.OPD2v = Visibility.Hidden;
            UI.OPD3v = Visibility.Hidden;
            UI.OPD4v = Visibility.Hidden;
        }

        private void 小港ToOnCube(UILayout UI)
        {
            UI.Title = "小港醫院 > OnCube";
            UI.IP1s = "門   診";
            UI.IP2s = "磨   粉";
            UI.IP3s = "住   院";
            UI.OPD1s = "門診";
            UI.OPD2s = "急診";
            UI.OPD3s = "慢籤";
            UI.OPD4s = "磨粉";
            UI.IP1b = true;
            UI.IP2b = true;
            UI.IP3b = true;
            UI.UDv = Visibility.Visible;
            UI.OPD1v = Visibility.Visible;
            UI.OPD2v = Visibility.Visible;
            UI.OPD3v = Visibility.Visible;
            UI.OPD4v = Visibility.Visible;
        }

        private void 光田ToOnCube(UILayout UI)
        {
            UI.Title = "光田醫院 > OnCube";
            UI.IP1s = "門   診";
            UI.IP2s = "輸入路徑2";
            UI.IP3s = "住   院";
            UI.OPD1s = "門診";
            UI.OPD2s = "";
            UI.OPD3s = "";
            UI.OPD4s = "";
            UI.IP1b = true;
            UI.IP2b = false;
            UI.IP3b = true;
            UI.UDv = Visibility.Visible;
            UI.OPD1v = Visibility.Visible;
            UI.OPD2v = Visibility.Hidden;
            UI.OPD3v = Visibility.Hidden;
            UI.OPD4v = Visibility.Hidden;
        }

        private void 光田ToJVServer(UILayout UI)
        {
            UI.Title = "光田醫院 > JVServer";
            UI.IP1s = "輸入路徑1";
            UI.IP2s = "磨   粉";
            UI.IP3s = "輸入路徑3";
            UI.OPD1s = "磨粉";
            UI.OPD2s = "";
            UI.OPD3s = "";
            UI.OPD4s = "";
            UI.IP1b = false;
            UI.IP2b = true;
            UI.IP3b = false;
            UI.UDv = Visibility.Hidden;
            UI.OPD1v = Visibility.Visible;
            UI.OPD2v = Visibility.Hidden;
            UI.OPD3v = Visibility.Hidden;
            UI.OPD4v = Visibility.Hidden;
        }

        private void 民生ToOnCube(UILayout UI)
        {
            UI.Title = "民生醫院 > OnCube";
            UI.IP1s = "門診養護";
            UI.IP2s = "大寮百合";
            UI.IP3s = "住   院";
            UI.OPD1s = "門診";
            UI.OPD2s = "養護";
            UI.OPD3s = "大寮";
            UI.OPD4s = "";
            UI.IP1b = true;
            UI.IP2b = true;
            UI.IP3b = true;
            UI.UDv = Visibility.Visible;
            UI.OPD1v = Visibility.Visible;
            UI.OPD2v = Visibility.Visible;
            UI.OPD3v = Visibility.Visible;
            UI.OPD4v = Visibility.Hidden;
        }

        private void 義大ToOnCube(UILayout UI)
        {
            UI.Title = "義大醫院 > OnCube";
            UI.IP1s = "輸入路徑1";
            UI.IP2s = "輸入路徑2";
            UI.IP3s = "住   院";
            UI.OPD1s = "";
            UI.OPD2s = "";
            UI.OPD3s = "";
            UI.OPD4s = "";
            UI.IP1b = false;
            UI.IP2b = false;
            UI.IP3b = true;
            UI.UDv = Visibility.Visible;
            UI.OPD1v = Visibility.Hidden;
            UI.OPD2v = Visibility.Hidden;
            UI.OPD3v = Visibility.Hidden;
            UI.OPD4v = Visibility.Hidden;
        }

        private void 長庚磨粉ToJVServer(UILayout UI)
        {
            UI.Title = "長庚磨粉 > JVServer";
            UI.IP1s = "磨粉";
            UI.IP2s = "輸入路徑2";
            UI.IP3s = "輸入路徑3";
            UI.OPD1s = "磨粉";
            UI.OPD2s = "";
            UI.OPD3s = "";
            UI.OPD4s = "";
            UI.IP1b = true;
            UI.IP2b = false;
            UI.IP3b = false;
            UI.UDv = Visibility.Hidden;
            UI.OPD1v = Visibility.Visible;
            UI.OPD2v = Visibility.Hidden;
            UI.OPD3v = Visibility.Hidden;
            UI.OPD4v = Visibility.Hidden;
        }

        private void 長庚醫院ToOnCube(UILayout UI)
        {
            UI.Title = "長庚醫院 > OnCube";
            UI.IP1s = "門診";
            UI.IP2s = "藥來速";
            UI.IP3s = "住院";
            UI.OPD1s = "門診";
            UI.OPD2s = "藥來速";
            UI.OPD3s = "";
            UI.OPD4s = "";
            UI.IP1b = true;
            UI.IP2b = true;
            UI.IP3b = true;
            UI.UDv = Visibility.Visible;
            UI.OPD1v = Visibility.Visible;
            UI.OPD2v = Visibility.Visible;
            UI.OPD3v = Visibility.Hidden;
            UI.OPD4v = Visibility.Hidden;
        }

        private void 仁康醫院ToOnCube(UILayout UI)
        {
            UI.Title = "仁康醫院 > OnCube";
            UI.IP1s = "門診";
            UI.IP2s = "輸入路徑2";
            UI.IP3s = "住院";
            UI.OPD1s = "門診";
            UI.OPD2s = "養護";
            UI.OPD3s = "";
            UI.OPD4s = "";
            UI.IP1b = true;
            UI.IP2b = false;
            UI.IP3b = true;
            UI.UDv = Visibility.Visible;
            UI.OPD1v = Visibility.Visible;
            UI.OPD2v = Visibility.Visible;
            UI.OPD3v = Visibility.Hidden;
            UI.OPD4v = Visibility.Hidden;
        }

        //視窗顯示控制
        public void AllWindowShowOrHide(bool? b1, bool? b2, bool? b3)
        {
            if (b1 != null)
                mw.Visibility = (bool)b1 ? Visibility.Visible : Visibility.Hidden;
            if (b2 != null)
                SF.Visibility = (bool)b2 ? Visibility.Visible : Visibility.Hidden;
            if (b3 != null)
                AS.Visibility = (bool)b3 ? Visibility.Visible : Visibility.Hidden;
        }

        //工具列圖示雙擊
        public void IconDBClick()
        {
            if (isMainWindow)
            {
                mw.Visibility = Visibility.Visible;
                mw.Activate();
            }
            else
            {
                SF = SF ?? new SmallForm(mw, Settings) { Owner = mw };
                SF.Visibility = Visibility.Visible;
                SF.Visibility = Visibility.Visible;
                SF.Topmost = true;
            }
        }

        public void ChangeWindow()
        {
            if (isMainWindow)
            {
                SF = SF ?? new SmallForm(mw, Settings) { Owner = mw };
                SF.Initialize();
                SF.ChangeLayout();
                SF.Visibility = Visibility.Visible;
                SF.Topmost = true;
                mw.Visibility = Visibility.Hidden;
                isMainWindow = false;
            }
            else
            {
                SF.Topmost = false;
                SF.Visibility = Visibility.Hidden;
                mw.Visibility = Visibility.Visible;
                mw.Activate();
                isMainWindow = true;
            }
        }

        //MainWindow控建Enabled切換
        public void SwitchControlEnabled(bool b)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                mw.btn_InputPath1.IsEnabled = b;
                mw.btn_InputPath2.IsEnabled = b;
                mw.btn_InputPath3.IsEnabled = b;
                mw.btn_OutputPath.IsEnabled = b;
                mw.tgl_AutoStart.IsEnabled = b;
                mw.rdo_Stat.IsEnabled = b;
                mw.rdo_Batch.IsEnabled = b;
                mw.btn_OPD.IsEnabled = b;
                mw.btn_UD.IsEnabled = b;
                mw.btn_Stop.IsEnabled = !b;
                mw.btn_Save.IsEnabled = b;
                mw.bd_X.IsEnabled = b;
                mw.bd_Y.IsEnabled = b;
                mw.chk_OPD1.IsEnabled = b;
                mw.chk_OPD2.IsEnabled = b;
                mw.chk_OPD3.IsEnabled = b;
                mw.chk_OPD4.IsEnabled = b;
            }));
        }

        //OPD或UD切換
        public void Start_Control(int Mode)
        {
            switch (Mode)
            {
                case (int)FunctionCollections.ModeEnum.OPD:
                    mw.btn_OPD.Background = Red;
                    mw.btn_UD.Opacity = 0.2;
                    break;
                case (int)FunctionCollections.ModeEnum.UD:
                    mw.btn_UD.Background = Red;
                    mw.btn_OPD.Opacity = 0.2;
                    break;
            }
        }

        public void Stop_Control()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                SF.Stop();
                mw.btn_OPD.Opacity = 1;
                mw.btn_UD.Opacity = 1;
                mw.btn_OPD.Background = White;
                mw.btn_UD.Background = White;
            }));
        }

        public void AdvancedSettingsShow()
        {
            try
            {
                AS.Window_Loaded(null, null);
                AS.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                Msg.Show(ex.ToString(), "錯誤", "Error", Msg.Color.Error);
            }
        }

        public void ProgressBoxClear()
        {
            mw.txt_ProgressBox.Clear();
            SF.ProgressBoxClear();
        }

        public void ProgressBoxAdd(string Result)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                mw.txt_ProgressBox.AppendText($"{Result}\n");
                mw.txt_ProgressBox.ScrollToEnd();
                SF.ProgressBoxAdd(Result);
            }));
        }

        public void SuccessCountAdd()
        {
            Dispatcher.InvokeAsync(new Action(() => { mw.txtb_Success.Text = (Convert.ToInt32(mw.txtb_Success.Text) + 1).ToString(); }));
        }

        public void FailCountAdd()
        {
            Dispatcher.InvokeAsync(new Action(() => { mw.txtb_Fail.Text = (Convert.ToInt32(mw.txtb_Fail.Text) + 1).ToString(); }));
        }

        public void AutoStart()
        {
            if (Settings.EN_AutoStart)
                mw.btn_OPD_Click(null, null);
        }
    }

    public class UILayout
    {
        public string Title { get; set; }
        public string IP1s { get; set; }
        public string IP2s { get; set; }
        public string IP3s { get; set; }
        public string OPD1s { get; set; }
        public string OPD2s { get; set; }
        public string OPD3s { get; set; }
        public string OPD4s { get; set; }
        public bool IP1b { get; set; }
        public bool IP2b { get; set; }
        public bool IP3b { get; set; }
        public Visibility UDv { get; set; }
        public Visibility OPD1v { get; set; }
        public Visibility OPD2v { get; set; }
        public Visibility OPD3v { get; set; }
        public Visibility OPD4v { get; set; }
    }
}
