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

namespace FCP
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        BASE_E_DA EDA;
        BASE_JVServer JVS;
        BASE_YiSheng YS;
        BASE_XiaoGang XG;
        BASE_KuangTien KT;
        BASE_HongYen HY;
        BASE_ChuangSheng CS;
        BASE_MinSheng MS;
        BASE_ChangGung_POWDER CG_P;
        BASE_ChangGung CG;
        BASE_TaipeiDetention TPD;
        BASE_JenKang JK;
        MsgB Msg = new MsgB();
        Settings Settings = new Settings();
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
        string UDFormatType = "即時";
        string InputPath1 = "";
        string InputPath2 = "";
        string InputPath3 = "";
        string OutputPath = "";
        const int ShowMainWindow_ID = 100;
        public MainWindow()
        {
            InitializeComponent();
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
                            switch (Settings.Mode)
                            {
                                case (int)Settings.ModeEnum.JVS:
                                    JVS.IconDBClick(null, null);
                                    break;
                                case (int)Settings.ModeEnum.創聖:
                                    CS.IconDBClick(null, null);
                                    break;
                                case (int)Settings.ModeEnum.醫聖:
                                    YS.IconDBClick(null, null);
                                    break;
                                case (int)Settings.ModeEnum.小港醫院:
                                    XG.IconDBClick(null, null);
                                    break;
                                case (int)Settings.ModeEnum.光田OnCube:
                                    KT.IconDBClick(null, null);
                                    break;
                                case (int)Settings.ModeEnum.光田JVS:
                                    KT.IconDBClick(null, null);
                                    break;
                                case (int)Settings.ModeEnum.民生醫院:
                                    MS.IconDBClick(null, null);
                                    break;
                                case (int)Settings.ModeEnum.宏彥診所:
                                    HY.IconDBClick(null, null);
                                    break;
                                case (int)Settings.ModeEnum.義大醫院:
                                    EDA.IconDBClick(null, null);
                                    break;
                                case (int)Settings.ModeEnum.長庚磨粉:
                                    CG_P.IconDBClick(null, null);
                                    break;
                                case (int)Settings.ModeEnum.長庚醫院:
                                    CG.IconDBClick(null, null);
                                    break;
                                case (int)Settings.ModeEnum.台北看守所:
                                    TPD.IconDBClick(null, null);
                                    break;
                                case (int)Settings.ModeEnum.仁康醫院:
                                    JK.IconDBClick(null, null);
                                    break;
                            }
                            handled = true;
                            break;
                    }
                    break;
            }
            return IntPtr.Zero;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Settings.Check();
            Judge(true);
        }

        public void Judge(bool Auto)
        {
            switch(Settings.Mode)
            {
                case (int)Settings.ModeEnum.JVS:
                    JVS = new BASE_JVServer(this, Settings);
                    if(Auto) JVS.AutoStart();
                    break;
                case (int)Settings.ModeEnum.創聖:
                    CS = new BASE_ChuangSheng(this, Settings);
                    if (Auto) CS.AutoStart();
                    break;
                case (int)Settings.ModeEnum.醫聖:
                    YS = new BASE_YiSheng(this, Settings);
                    if (Auto) YS.AutoStart();
                    break;
                case (int)Settings.ModeEnum.小港醫院:
                    XG = new BASE_XiaoGang(this, Settings);
                    if (Auto) XG.AutoStart();
                    break;
                case (int)Settings.ModeEnum.光田OnCube:
                    KT = new BASE_KuangTien(this, Settings);
                    if (Auto) KT.AutoStart();
                    break;
                case (int)Settings.ModeEnum.光田JVS:
                    KT = new BASE_KuangTien(this, Settings);
                    if (Auto) KT.AutoStart();
                    break;
                case (int)Settings.ModeEnum.民生醫院:
                    MS = new BASE_MinSheng(this, Settings);
                    if (Auto) MS.AutoStart();
                    break;
                case (int)Settings.ModeEnum.宏彥診所:
                    HY = new BASE_HongYen(this, Settings);
                    if (Auto) HY.AutoStart();
                    break;
                case (int)Settings.ModeEnum.義大醫院:
                    EDA = new BASE_E_DA(this, Settings);
                    if (Auto) EDA.AutoStart();
                    break;
                case (int)Settings.ModeEnum.長庚磨粉:
                    CG_P = new BASE_ChangGung_POWDER(this, Settings);
                    if (Auto) CG_P.AutoStart();
                    break;
                case (int)Settings.ModeEnum.長庚醫院:
                    CG = new BASE_ChangGung(this, Settings);
                    if (Auto) CG.AutoStart();
                    break;
                case (int)Settings.ModeEnum.台北看守所:
                    TPD = new BASE_TaipeiDetention(this, Settings);
                    if (Auto) TPD.AutoStart();
                    break;
                case (int)Settings.ModeEnum.仁康醫院:
                    JK = new BASE_JenKang(this, Settings);
                    if (Auto) JK.AutoStart();
                    break;
            }
        }

        public void btn_Stop_Click(object sender, RoutedEventArgs e)
        {
            switch (Settings.Mode)
            {
                case (int)Settings.ModeEnum.JVS:
                    JVS.Stop();
                    break;
                case (int)Settings.ModeEnum.創聖:
                    CS.Stop();
                    break;
                case (int)Settings.ModeEnum.醫聖:
                    YS.Stop();
                    break;
                case (int)Settings.ModeEnum.小港醫院:
                    XG.Stop();
                    break;
                case (int)Settings.ModeEnum.光田OnCube:
                    KT.Stop();
                    break;
                case (int)Settings.ModeEnum.光田JVS:
                    KT.Stop();
                    break;
                case (int)Settings.ModeEnum.民生醫院:
                    MS.Stop();
                    break;
                case (int)Settings.ModeEnum.宏彥診所:
                    HY.Stop();
                    break;
                case (int)Settings.ModeEnum.義大醫院:
                    EDA.Stop();
                    break;
                case (int)Settings.ModeEnum.長庚磨粉:
                    CG_P.Stop();
                    break;
                case (int)Settings.ModeEnum.長庚醫院:
                    CG.Stop();
                    break;
                case (int)Settings.ModeEnum.台北看守所:
                    TPD.Stop();
                    break;
                case (int)Settings.ModeEnum.仁康醫院:
                    JK.Stop();
                    break;
            }
        }

        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {
            btn_Save.Focus();
            switch (Settings.Mode)
            {
                case (int)Settings.ModeEnum.JVS:
                    JVS.Save();
                    break;
                case (int)Settings.ModeEnum.創聖:
                    CS.Save();
                    break;
                case (int)Settings.ModeEnum.醫聖:
                    YS.Save();
                    break;
                case (int)Settings.ModeEnum.小港醫院:
                    XG.Save();
                    break;
                case (int)Settings.ModeEnum.光田OnCube:
                    KT.Save();
                    break;
                case (int)Settings.ModeEnum.光田JVS:
                    KT.Save();
                    break;
                case (int)Settings.ModeEnum.民生醫院:
                    MS.Save();
                    break;
                case (int)Settings.ModeEnum.宏彥診所:
                    HY.Save();
                    break;
                case (int)Settings.ModeEnum.義大醫院:
                    EDA.Save();
                    break;
                case (int)Settings.ModeEnum.長庚磨粉:
                    CG_P.Save();
                    break;
                case (int)Settings.ModeEnum.長庚醫院:
                    CG.Save();
                    break;
                case (int)Settings.ModeEnum.台北看守所:
                    TPD.Save();
                    break;
                case (int)Settings.ModeEnum.仁康醫院:
                    JK.Save();
                    break;
            }
        }

        public void ChangeSize_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            switch (Settings.Mode)
            {
                case (int)Settings.ModeEnum.JVS:
                    JVS.ChangeWindow();
                    break;
                case (int)Settings.ModeEnum.創聖:
                    CS.ChangeWindow();
                    break;
                case (int)Settings.ModeEnum.醫聖:
                    YS.ChangeWindow();
                    break;
                case (int)Settings.ModeEnum.小港醫院:
                    XG.ChangeWindow();
                    break;
                case (int)Settings.ModeEnum.光田OnCube:
                    KT.ChangeWindow();
                    break;
                case (int)Settings.ModeEnum.光田JVS:
                    KT.ChangeWindow();
                    break;
                case (int)Settings.ModeEnum.民生醫院:
                    MS.ChangeWindow();
                    break;
                case (int)Settings.ModeEnum.宏彥診所:
                    HY.ChangeWindow();
                    break;
                case (int)Settings.ModeEnum.義大醫院:
                    EDA.ChangeWindow();
                    break;
                case (int)Settings.ModeEnum.長庚磨粉:
                    CG_P.ChangeWindow();
                    break;
                case (int)Settings.ModeEnum.長庚醫院:
                    CG.ChangeWindow();
                    break;
                case (int)Settings.ModeEnum.台北看守所:
                    TPD.ChangeWindow();
                    break;
                case (int)Settings.ModeEnum.仁康醫院:
                    JK.ChangeWindow();
                    break;
            }
        }

        private void AdvancedSettings_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            switch (Settings.Mode)
            {
                case (int)Settings.ModeEnum.JVS:
                    JVS.AdvancedSettingsShow();
                    break;
                case (int)Settings.ModeEnum.創聖:
                    CS.AdvancedSettingsShow();
                    break;
                case (int)Settings.ModeEnum.醫聖:
                    YS.AdvancedSettingsShow();
                    break;
                case (int)Settings.ModeEnum.小港醫院:
                    XG.AdvancedSettingsShow();
                    break;
                case (int)Settings.ModeEnum.光田OnCube:
                    KT.AdvancedSettingsShow();
                    break;
                case (int)Settings.ModeEnum.光田JVS:
                    KT.AdvancedSettingsShow();
                    break;
                case (int)Settings.ModeEnum.民生醫院:
                    MS.AdvancedSettingsShow();
                    break;
                case (int)Settings.ModeEnum.宏彥診所:
                    HY.AdvancedSettingsShow();
                    break;
                case (int)Settings.ModeEnum.義大醫院:
                    EDA.AdvancedSettingsShow();
                    break;
                case (int)Settings.ModeEnum.長庚磨粉:
                    CG_P.AdvancedSettingsShow();
                    break;
                case (int)Settings.ModeEnum.長庚醫院:
                    CG.AdvancedSettingsShow();
                    break;
                case (int)Settings.ModeEnum.台北看守所:
                    TPD.AdvancedSettingsShow();
                    break;
                case (int)Settings.ModeEnum.仁康醫院:
                    JK.AdvancedSettingsShow();
                    break;
            }
        }

        private void btn_ProgressBoxClear_Click(object sender, RoutedEventArgs e)
        {
            switch (Settings.Mode)
            {
                case (int)Settings.ModeEnum.JVS:
                    JVS.ProgressBoxClear();
                    break;
                case (int)Settings.ModeEnum.創聖:
                    CS.ProgressBoxClear();
                    break;
                case (int)Settings.ModeEnum.醫聖:
                    YS.ProgressBoxClear();
                    break;
                case (int)Settings.ModeEnum.小港醫院:
                    XG.ProgressBoxClear();
                    break;
                case (int)Settings.ModeEnum.光田OnCube:
                    KT.ProgressBoxClear();
                    break;
                case (int)Settings.ModeEnum.光田JVS:
                    KT.ProgressBoxClear();
                    break;
                case (int)Settings.ModeEnum.民生醫院:
                    MS.ProgressBoxClear();
                    break;
                case (int)Settings.ModeEnum.宏彥診所:
                    HY.ProgressBoxClear();
                    break;
                case (int)Settings.ModeEnum.義大醫院:
                    EDA.ProgressBoxClear();
                    break;
                case (int)Settings.ModeEnum.長庚磨粉:
                    CG_P.ProgressBoxClear();
                    break;
                case (int)Settings.ModeEnum.長庚醫院:
                    CG.ProgressBoxClear();
                    break;
                case (int)Settings.ModeEnum.台北看守所:
                    TPD.ProgressBoxClear();
                    break;
                case (int)Settings.ModeEnum.仁康醫院:
                    JK.ProgressBoxClear();
                    break;
            }
        }

        public void ClearObject(int Mode)
        {
            switch (Mode)
            {
                case (int)Settings.ModeEnum.JVS:
                    JVS.CloseSelf();
                    JVS = null;
                    break;
                case (int)Settings.ModeEnum.創聖:
                    CS.CloseSelf();
                    CS = null;
                    break;
                case (int)Settings.ModeEnum.醫聖:
                    YS.CloseSelf();
                    YS = null;
                    break;
                case (int)Settings.ModeEnum.小港醫院:
                    XG.CloseSelf();
                    XG = null;
                    break;
                case (int)Settings.ModeEnum.光田OnCube:
                    KT.CloseSelf();
                    KT = null;
                    break;
                case (int)Settings.ModeEnum.光田JVS:
                    KT.CloseSelf();
                    KT = null;
                    break;
                case (int)Settings.ModeEnum.民生醫院:
                    MS.CloseSelf();
                    MS = null;
                    break;
                case (int)Settings.ModeEnum.宏彥診所:
                    HY.CloseSelf();
                    HY = null;
                    break;
                case (int)Settings.ModeEnum.義大醫院:
                    EDA.CloseSelf();
                    EDA = null;
                    break;
                case (int)Settings.ModeEnum.長庚磨粉:
                    CG_P.CloseSelf();
                    CG_P = null;
                    break;
                case (int)Settings.ModeEnum.長庚醫院:
                    CG.CloseSelf();
                    CG = null;
                    break;
                case (int)Settings.ModeEnum.台北看守所:
                    TPD.CloseSelf();
                    TPD = null;
                    break;
                case (int)Settings.ModeEnum.仁康醫院:
                    JK.CloseSelf();
                    JK = null;
                    break;
            }
        }

        private void btn_Minimum_Click(object sender, RoutedEventArgs e)
        {
            switch (Settings.Mode)
            {
                case (int)Settings.ModeEnum.JVS:
                    JVS.AllWindowShowOrHide(false, false, false);
                    break;
                case (int)Settings.ModeEnum.創聖:
                    CS.AllWindowShowOrHide(false, false, false);
                    break;
                case (int)Settings.ModeEnum.醫聖:
                    YS.AllWindowShowOrHide(false, false, false);
                    break;
                case (int)Settings.ModeEnum.小港醫院:
                    XG.AllWindowShowOrHide(false, false, false);
                    break;
                case (int)Settings.ModeEnum.光田OnCube:
                    KT.AllWindowShowOrHide(false, false, false);
                    break;
                case (int)Settings.ModeEnum.光田JVS:
                    KT.AllWindowShowOrHide(false, false, false);
                    break;
                case (int)Settings.ModeEnum.民生醫院:
                    MS.AllWindowShowOrHide(false, false, false);
                    break;
                case (int)Settings.ModeEnum.宏彥診所:
                    HY.AllWindowShowOrHide(false, false, false);
                    break;
                case (int)Settings.ModeEnum.義大醫院:
                    EDA.AllWindowShowOrHide(false, false, false);
                    break;
                case (int)Settings.ModeEnum.長庚磨粉:
                    CG_P.AllWindowShowOrHide(false, false, false);
                    break;
                case (int)Settings.ModeEnum.長庚醫院:
                    CG.AllWindowShowOrHide(false, false, false);
                    break;
                case (int)Settings.ModeEnum.台北看守所:
                    TPD.AllWindowShowOrHide(false, false, false);
                    break;
                case (int)Settings.ModeEnum.仁康醫院:
                    JK.AllWindowShowOrHide(false, false, false);
                    break;
            }
        }  //縮小程式

        public void btn_OPD_Click(object sender, RoutedEventArgs e)
        {
            if ((chk_OPD1.IsChecked | chk_OPD2.IsChecked | chk_OPD3.IsChecked | chk_OPD4.IsChecked) == false)
            {
                Msg.Show("沒有勾選任一個轉檔位置", "位置未勾選", "Error", Msg.Color.Error);
                return;
            }    
            switch (Settings.Mode)
            {
                case (int)Settings.ModeEnum.JVS:
                    JVS.ConvertPrepare(0);
                    break;
                case (int)Settings.ModeEnum.創聖:
                    CS.ConvertPrepare(0);
                    break;
                case (int)Settings.ModeEnum.醫聖:
                    YS.ConvertPrepare(0);
                    break;
                case (int)Settings.ModeEnum.小港醫院:
                    XG.ConvertPrepare(0);
                    break;
                case (int)Settings.ModeEnum.光田OnCube:
                    KT.ConvertPrepare(0);
                    break;
                case (int)Settings.ModeEnum.光田JVS:
                    KT.ConvertPrepare(0);
                    break;
                case (int)Settings.ModeEnum.民生醫院:
                    MS.ConvertPrepare(0);
                    break;
                case (int)Settings.ModeEnum.宏彥診所:
                    HY.ConvertPrepare(0);
                    break;
                case (int)Settings.ModeEnum.義大醫院:
                    EDA.ConvertPrepare(0);
                    break;
                case (int)Settings.ModeEnum.長庚磨粉:
                    CG_P.ConvertPrepare(0);
                    break;
                case (int)Settings.ModeEnum.長庚醫院:
                    CG.ConvertPrepare(0);
                    break;
                case (int)Settings.ModeEnum.台北看守所:
                    TPD.ConvertPrepare(0);
                    break;
                case (int)Settings.ModeEnum.仁康醫院:
                    JK.ConvertPrepare(0);
                    break;
            }
        }

        public void btn_UD_Click(object sender, RoutedEventArgs e)
        {
            switch (Settings.Mode)
            {
                case (int)Settings.ModeEnum.小港醫院:
                    XG.ConvertPrepare(1);
                    break;
                case (int)Settings.ModeEnum.光田OnCube:
                    KT.ConvertPrepare(1);
                    break;
                case (int)Settings.ModeEnum.民生醫院:
                    MS.ConvertPrepare(1);
                    break;
                case (int)Settings.ModeEnum.義大醫院:
                    EDA.ConvertPrepare(1);
                    break;
                case (int)Settings.ModeEnum.長庚醫院:
                    CG.ConvertPrepare(1);
                    break;
                case (int)Settings.ModeEnum.仁康醫院:
                    JK.ConvertPrepare(1);
                    break;
            }
        }

        private void btn_Close_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }  //關閉程式

        private void grd_Title_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }  //拖曳視窗

        private void txtb_InputPath1_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (btn_Stop.IsEnabled == false)
            {
                txt_InputPath1.Text = "";
                InputPath1 = "";
            }
        }

        private void txtb_InputPath2_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (btn_Stop.IsEnabled == false)
            {
                txt_InputPath2.Text = "";
                InputPath2 = "";
            }
        }

        private void txtb_InputPath3_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (btn_Stop.IsEnabled == false)
            {
                txt_InputPath3.Text = "";
                InputPath3 = "";
            }

        }

        private void txtb_OutputPath_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (btn_Stop.IsEnabled == false)
            {
                txt_OutputPath.Text = "";
                OutputPath = "";
            }
        }

        private void OPD_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !btn_Stop.IsEnabled;
        }

        private void OPD_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            btn_OPD_Click(null, null);
        }

        private void Stop_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = btn_Stop.IsEnabled ? true : false;
        }

        private void Stop_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            btn_Stop_Click(null, null);
        }

        private void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = btn_Save.IsEnabled ? true : false;
        }

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            btn_Save_Click(null, null);
        }

        private void Exit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            btn_Close_Click(null, null);
        }

        private void UD_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !(btn_Stop.IsEnabled & btn_UD.Visibility == Visibility.Visible);
        }

        private void UD_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            btn_UD_Click(null, null);
        }

        private void InputPath1_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = btn_InputPath1.IsEnabled;
        }

        private void InputPath1_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            btn_InputPath1_Click(null, null);
        }

        private void btn_InputPath1_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog OFD = new System.Windows.Forms.FolderBrowserDialog();
            if (OFD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txt_InputPath1.Text = OFD.SelectedPath;
                InputPath1 = OFD.SelectedPath;
            }
        }

        private void InputPath2_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = btn_InputPath2.IsEnabled;
        }

        private void InputPath2_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            btn_InputPath2_Click(null, null);
        }

        private void btn_InputPath2_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog OFD = new System.Windows.Forms.FolderBrowserDialog();
            if (OFD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txt_InputPath2.Text = OFD.SelectedPath;
                InputPath2 = OFD.SelectedPath;
            }
        }

        private void InputPath3_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = btn_InputPath3.IsEnabled;
        }

        private void InputPath3_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            btn_InputPath3_Click(null, null);
        }

        private void btn_InputPath3_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog OFD = new System.Windows.Forms.FolderBrowserDialog();
            if (OFD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txt_InputPath3.Text = OFD.SelectedPath;
                InputPath3 = OFD.SelectedPath;
            }
        }

        private void OutputPath_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = btn_OutputPath.IsEnabled ? true : false;
        }

        private void OutputPath_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            btn_OutputPath_Click(null, null);
        }

        private void btn_OutputPath_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog OFD = new System.Windows.Forms.FolderBrowserDialog();
            if (OFD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txt_OutputPath.Text = OFD.SelectedPath;
                OutputPath = OFD.SelectedPath;
            }
        }

        private void ChangeSize_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !btn_Stop.IsEnabled;
        }

        private void AdvancedSettings_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !btn_Stop.IsEnabled;
        }

        private void ClearListBox_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            btn_ProgressBoxClear_Click(null, null);
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
            UDFormatType = Type;
            if (Type == "即時")
                rdo_Stat.IsChecked = true;
            else
                rdo_Batch.IsChecked = true;

        }  //SmallForm切換即時與長期時，連帶更改主視窗的即時與長期

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            switch(Settings.Mode)
            {
                case (int)Settings.ModeEnum.小港醫院:
                    XG.Stop();
                    break;
            }
        }
    }
}
