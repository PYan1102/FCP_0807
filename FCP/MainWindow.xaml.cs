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
using FCP.MVVM.Factory.ViewModel;
using FCP.MVVM.FormatInit;
using FCP.MVVM.Models.Enum;
using FCP.MVVM.Factory;
using FCP.MVVM.Models;
using FCP.MVVM.Control;
using FCP.MVVM.ViewModels;
using MaterialDesignThemes.Wpf;
using FCP.MVVM.Dialog;

namespace FCP
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        private FunctionCollections _Format { get; set; }
        private SettingsModel _SettingsModel { get; set; }
        private MainWindowModel _MainWindowModel { get => MainWindowFactory.GenerateMainWindowModel(); }
        private MsgBViewModel _MsgBVM { get; set; }
        private Stopwatch _StopWatch = new Stopwatch();
        public string CurrentWindow { get; set; }
        private string _StatOrBatch = "S";
        private string _InputPath1 = "";
        private string _InputPath2 = "";
        private string _InputPath3 = "";
        private string _OutputPath = "";
        private const int _ShowMainWindow_ID = 100;

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
            RegisterHotKey(helper.Handle, _ShowMainWindow_ID, KeyModifilers.Alt, System.Windows.Forms.Keys.D);
        }  //註冊全域熱鍵

        private void UnregisterHotKey()
        {
            var helper = new System.Windows.Interop.WindowInteropHelper(this);
            UnregisterHotKey(helper.Handle, _ShowMainWindow_ID);
        }  //註銷全域熱鍵

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            switch (msg)
            {
                case WM_HOTKEY:
                    switch (wParam.ToInt32())
                    {
                        case _ShowMainWindow_ID:
                            MainWindowFactory.GenerateMainWindowViewModel().NotifyIconDBClick(null, null);
                            handled = true;
                            break;
                    }
                    break;
            }
            return IntPtr.Zero;
        }

        public MainWindow()
        {
            InitializeComponent();
            MainWindowFactory.MainWindow = this;
            SettingsFactory.GenerateSettingsControl();
            this.DataContext = MainWindowFactory.GenerateMainWindowViewModel();
            _SettingsModel = SettingsFactory.GenerateSettingsModel();
            SimpleWindowFactory.GenerateSimpleWindow();
        }
        
        private void Btn_ProgressBoxClear_Click(object sender, RoutedEventArgs e)
        {
            _Format.ProgressBoxClear();
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
                _MsgBVM.Show(a.ToString(), "錯誤", PackIconKind.Error, KindColors.Error);
            }
        }  //F1開啟Log

        public void ChangeUDFormatType(string Type)
        {
            _StatOrBatch = Type;
            if (Type == "S")
                Rdo_Stat.IsChecked = true;
            else
                Rdo_Batch.IsChecked = true;

        }  //SmallForm切換即時與長期時，連帶更改主視窗的即時與長期
    }
}
