using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.ComponentModel;
using FCP.MVVM.Factory.ViewModels;
using FCP.MVVM.ViewModels.MainWindow;
using FCP.MVVM.FormatInit;
using FCP.MVVM.Models.Enum;
using FCP.MVVM.Factory;
using FCP.MVVM.Models;
using FCP.MVVM.Control;

namespace FCP
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        private BASE_E_DA _EDA { get; set; }
        private BASE_JVServer _JVServer { get; set; }
        private BASE_YiSheng _YiSheng { get; set; }
        private BASE_XiaoGang _XiaoGang { get; set; }
        private BASE_KuangTien _KangTien { get; set; }
        private BASE_HongYen _HongYen { get; set; }
        private BASE_ChuangSheng _ChuangSheng { get; set; }
        private BASE_MinSheng _MinSheng { get; set; }
        private BASE_ChangGung_POWDER _ChangGungPowder { get; set; }
        private BASE_ChangGung _ChangGung { get; set; }
        private BASE_TaipeiDetention _TaipeiDetention { get; set; }
        private BASE_JenKang _JengKang { get; set; }
        private BASE_FangDing _FangDing { get; set; }
        private FunctionCollections _Format { get; set; }
        private MsgB Msg = new MsgB();
        private PropertyChange _PropertyChange { get; set; }
        private SettingsModel _SettingsModel { get; set; }
        private Settings _Settings { get; set; }
        Log log = new Log();
        DoJson doJson = new DoJson();
        Stopwatch sw = new Stopwatch();
        Color Warning = Color.FromRgb(225, 219, 96);
        SolidColorBrush Red = new SolidColorBrush((Color)Color.FromRgb(255, 82, 85));
        SolidColorBrush White = new SolidColorBrush((Color)Color.FromRgb(255, 255, 255));
        List<string> SettingsList = new List<string>();
        List<string> HospitalList = new List<string>() { "小港ToOnCube", "光田ToOnCube", "光田ToJVServer", "民生ToOnCube", "義大ToOnCube" };
        List<string> FilePath = new List<string>();
        StringBuilder logw = new StringBuilder();
        public string CurrentWindow;
        string StatOrBatch = "S";
        string InputPath1 = "";
        string InputPath2 = "";
        string InputPath3 = "";
        string OutputPath = "";
        const int ShowMainWindow_ID = 100;
        public MainWindow()
        {
            InitializeComponent();
            _PropertyChange = MainWindowFacotry.GeneratePropertyChange();
            this.DataContext = _PropertyChange;
            _Settings = SettingsFactory.GenerateSettingsControl();
            _SettingsModel = SettingsFactory.GenerateSettingsModels();
        }

        public enum KeyModifilers
        {
            None = 0, Alt = 1, Ctrl = 2, Shift = 3
        }

        [DllImport("user32.dll", EntryPoint = "keybd_event", SetLastError = true)]
        public static extern void keybd_event(System.Windows.Forms.Keys bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("User32.dll")]
        private static extern bool RegisterHotKey([In] IntPtr hWnd, [In] int id, [In] KeyModifilers fsModifiers, [In] System.Windows.Forms.Keys vk);

        [DllImport("User32.dll")]
        private static extern bool UnregisterHotKey([In] IntPtr hWnd, [In] int id);

        [DllImport("User32.dll")]

        public static extern bool MessageBeep(uint uType);

        private System.Windows.Interop.HwndSource _source;

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var helper = new System.Windows.Interop.WindowInteropHelper(this);
            _source = System.Windows.Interop.HwndSource.FromHwnd(helper.Handle);
            _source.AddHook(HwndHook);
            RegisterHotKey();
        }

        protected override void OnClosed(EventArgs e)
        {
            _source.RemoveHook(HwndHook);
            _source = null;
            UnregisterHotKey();
            base.OnClosed(e);
        }

        private void RegisterHotKey()
        {
            var helper = new System.Windows.Interop.WindowInteropHelper(this);
            RegisterHotKey(helper.Handle, ShowMainWindow_ID, KeyModifilers.Alt, System.Windows.Forms.Keys.D);
        }  //註冊全域熱鍵

        private void UnregisterHotKey()
        {
            var helper = new System.Windows.Interop.WindowInteropHelper(this);
            UnregisterHotKey(helper.Handle, ShowMainWindow_ID);
        }  //註銷全域熱鍵

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            switch (msg)
            {
                case WM_HOTKEY:
                    switch (wParam.ToInt32())
                    {
                        case ShowMainWindow_ID:
                            _Format.IconDBClick(null, null);
                            handled = true;
                            break;
                    }
                    break;
            }
            return IntPtr.Zero;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            Judge(true);
        }

        public void Judge(bool Auto)
        {
            _Format = FormatFactory.GenerateFormat(_SettingsModel.Mode);
            _Format.SetWindow(this);
            _Format.Loaded();
            if (Auto) _Format.AutoStart();
        }

        public void Btn_Stop_Click(object sender, RoutedEventArgs e)
        {
            _Format.Stop();
        }

        private void Btn_Save_Click(object sender, RoutedEventArgs e)
        {
            Btn_Save.Focus();
            _Format.Save();
        }

        public void ChangeSize_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _Format.ChangeWindow();
        }

        private void AdvancedSettings_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _Format.AdvancedSettingsShow();
        }

        private void Btn_ProgressBoxClear_Click(object sender, RoutedEventArgs e)
        {
            _Format.ProgressBoxClear();
        }

        public void ClearObject()
        {
            _Format.CloseSelf();
        }

        private void Btn_Minimum_Click(object sender, RoutedEventArgs e)
        {
            _Format.AllWindowShowOrHide(false, false, false);
        }  //縮小程式

        public void Btn_OPD_Click(object sender, RoutedEventArgs e)
        {
            if ((Tgl_OPD1.IsChecked | Tgl_OPD2.IsChecked | Tgl_OPD3.IsChecked | Tgl_OPD4.IsChecked) == false)
            {
                Msg.Show("沒有勾選任一個轉檔位置", "位置未勾選", "Error", Msg.Color.Error);
                return;
            }
            _Format.ConvertPrepare(0);
        }

        public void Btn_UD_Click(object sender, RoutedEventArgs e)
        {
            _Format.ConvertPrepare(1);
        }

        private void Btn_Close_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }  //關閉程式

        private void Gd_Title_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }  //拖曳視窗

        private void Txtb_InputPath1_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Btn_Stop.IsEnabled == false)
            {
                Txt_InputPath1.Text = "";
                InputPath1 = "";
            }
        }

        private void Txtb_InputPath2_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Btn_Stop.IsEnabled == false)
            {
                Txt_InputPath2.Text = "";
                InputPath2 = "";
            }
        }

        private void Txtb_InputPath3_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Btn_Stop.IsEnabled == false)
            {
                Txt_InputPath3.Text = "";
                InputPath3 = "";
            }

        }

        private void Txtb_OutputPath_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Btn_Stop.IsEnabled == false)
            {
                Txt_OutputPath.Text = "";
                OutputPath = "";
            }
        }

        private void OPD_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !Btn_Stop.IsEnabled;
        }

        private void OPD_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Btn_OPD_Click(null, null);
        }

        private void Stop_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Btn_Stop.IsEnabled ? true : false;
        }

        private void Stop_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Btn_Stop_Click(null, null);
        }

        private void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Btn_Save.IsEnabled ? true : false;
        }

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Btn_Save_Click(null, null);
        }

        private void Exit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Btn_Close_Click(null, null);
        }

        private void UD_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !(Btn_Stop.IsEnabled & Btn_UD.Visibility == Visibility.Visible);
        }

        private void UD_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Btn_UD_Click(null, null);
        }

        private void InputPath1_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Btn_InputPath1.IsEnabled;
        }

        private void InputPath1_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Btn_InputPath1_Click(null, null);
        }

        private void Btn_InputPath1_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog OFD = new System.Windows.Forms.FolderBrowserDialog();
            if (OFD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Txt_InputPath1.Text = OFD.SelectedPath;
                InputPath1 = OFD.SelectedPath;
            }
        }

        private void InputPath2_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Btn_InputPath2.IsEnabled;
        }

        private void InputPath2_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Btn_InputPath2_Click(null, null);
        }

        private void Btn_InputPath2_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog OFD = new System.Windows.Forms.FolderBrowserDialog();
            if (OFD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Txt_InputPath2.Text = OFD.SelectedPath;
                InputPath2 = OFD.SelectedPath;
            }
        }

        private void InputPath3_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Btn_InputPath3.IsEnabled;
        }

        private void InputPath3_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Btn_InputPath3_Click(null, null);
        }

        private void Btn_InputPath3_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog OFD = new System.Windows.Forms.FolderBrowserDialog();
            if (OFD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Txt_InputPath3.Text = OFD.SelectedPath;
                InputPath3 = OFD.SelectedPath;
            }
        }

        private void OutputPath_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Btn_OutputPath.IsEnabled ? true : false;
        }

        private void OutputPath_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Btn_OutputPath_Click(null, null);
        }

        private void Btn_OutputPath_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog OFD = new System.Windows.Forms.FolderBrowserDialog();
            if (OFD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Txt_OutputPath.Text = OFD.SelectedPath;
                OutputPath = OFD.SelectedPath;
            }
        }

        private void ChangeSize_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !Btn_Stop.IsEnabled;
        }

        private void AdvancedSettings_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !Btn_Stop.IsEnabled;
        }

        private void ClearListBox_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Btn_ProgressBoxClear_Click(null, null);
        }

        private void Log_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                Process.Start($@"D:\FCP\LOG\{DateTime.Now:yyyy-MM-dd}\Error_Log.txt");
            }
            catch (Exception a)
            {
                Msg.Show(a.ToString(), "錯誤", "Error", Colors.Red);
            }
        }  //F1開啟Log

        public void ChangeUDFormatType(string Type)
        {
            StatOrBatch = Type;
            if (Type == "S")
                Rdo_Stat.IsChecked = true;
            else
                Rdo_Batch.IsChecked = true;

        }  //SmallForm切換即時與長期時，連帶更改主視窗的即時與長期

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            switch (_SettingsModel.Mode)
            {
                case Format.小港醫院TOC:
                    _XiaoGang.Stop();
                    break;
            }
        }
    }
}
