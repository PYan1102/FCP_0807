using System;
using System.Windows;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Input;
using FCP.src.Factory.ViewModel;
using FCP.Models;
using FCP.ViewModels;
using FCP.src;
using Helper;
using FCP.src.Factory.Models;

namespace FCP.Views
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        private FunctionCollections _Format { get; set; }
        private SettingModel _SettingsModel { get; set; }
        private MainWindowModel _MainWindowModel { get => MainWindowFactory.GenerateMainWindowModel(); }
        private Stopwatch _StopWatch = new Stopwatch();
        public string CurrentWindow { get; set; }
        private string _StatOrBatch = "S";
        private const int _ShowMainWindow = 100;

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
            RegisterHotKey(helper.Handle, _ShowMainWindow, KeyModifilers.Alt, System.Windows.Forms.Keys.D);
        }  //註冊全域熱鍵

        private void UnregisterHotKey()
        {
            var helper = new System.Windows.Interop.WindowInteropHelper(this);
            UnregisterHotKey(helper.Handle, _ShowMainWindow);
        }  //註銷全域熱鍵

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            switch (msg)
            {
                case WM_HOTKEY:
                    switch (wParam.ToInt32())
                    {
                        case _ShowMainWindow:
                            MainWindowFactory.GenerateMainWindowViewModel().NotifyIconDBClick();
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
            WindowOwner.MainWindowOwner = this;
            SettingFactory.GenerateSetting();
            this.DataContext = MainWindowFactory.GenerateMainWindowViewModel();
            _SettingsModel = SettingFactory.GenerateSettingModel();
            SimpleWindowFactory.GenerateSimpleWindow();
            var vm = this.DataContext as MainWindowViewModel;
            vm.ActivateWindow += ActivateWindow;
            MSSql.SqlInfo = CommonModel.SqlConnection;
        }

        private void ActivateWindow()
        {
            this.Activate();
        }

        public void ChangeUDFormatType(string Type)
        {
            _StatOrBatch = Type;
            if (Type == "S")
                Rdo_Stat.IsChecked = true;
            else
                Rdo_Batch.IsChecked = true;

        }  //SmallForm切換即時與長期時，連帶更改主視窗的即時與長期

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
