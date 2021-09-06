using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FCP.MVVM.Factory;
using FCP.MVVM.Factory.ViewModel;
using FCP.MVVM.Models;
using FCP.MVVM.Models.Enum;
using System.Windows.Media;
using System.Windows;

namespace FCP.MVVM.ViewModels
{
    static class RefreshUIPropertyServices
    {
        private static MainWindowViewModel _MainWindowVM { get => MainWindowFactory.GenerateMainWindowViewModel(); }
        private static SettingsModel _SettingsModel { get => SettingsFactory.GenerateSettingsModel(); }
        private static AdvancedSettingsViewModel _AdvancedSettingsVM { get => AdvancedSettingsFactory.GenerateAdvancedSettingsViewModel(); }
        private static SettingsPage1ViewModel _SettingsPage1VM { get => AdvancedSettingsFactory.GenerateSettingsPage1(); }
        private static SettingsPage2ViewModel _SettingsPage2VM { get => AdvancedSettingsFactory.GenerateSettingsPage2(); }
        private static SimpleWindowViewModel _SimpleWindowVM { get => SimpleWindowFactory.GenerateSimpleWindowViewModel(); }

        public static void InitMainWindowUI()
        {
            string BackupPath = $@"{_MainWindowVM.FileBackupPath}\{DateTime.Now:yyyy-MM-dd}";
            _MainWindowVM.SuccessCount = $"{Directory.GetFiles($@"{BackupPath}\Success").Length}";
            _MainWindowVM.FailCount = $"{Directory.GetFiles($@"{BackupPath}\Fail").Length}";
            _MainWindowVM.InputPath1 = _SettingsModel.InputPath1;
            _MainWindowVM.InputPath2 = _SettingsModel.InputPath2;
            _MainWindowVM.InputPath3 = _SettingsModel.InputPath3;
            _MainWindowVM.OutputPath = _SettingsModel.OutputPath;
            _MainWindowVM.IsAutoStartChecked = _SettingsModel.EN_AutoStart;
            _MainWindowVM.WindowX = Properties.Settings.Default.X.ToString();
            _MainWindowVM.WindowY = Properties.Settings.Default.Y.ToString();
            _MainWindowVM.StopEnabled = false;
            _MainWindowVM.OPDToogle1Checked = false;
            _MainWindowVM.OPDToogle2Checked = false;
            _MainWindowVM.OPDToogle3Checked = false;
            _MainWindowVM.OPDToogle4Checked = false;
            _MainWindowVM.StatChecked = _SettingsModel.StatOrBatch == "S";
            _MainWindowVM.BatchChecked = _SettingsModel.StatOrBatch == "B";

        }

        public static void RefrehMainWindowUI(UILayout UI)
        {
            _MainWindowVM.WindowTitle = UI.Title;
            _MainWindowVM.InputPath1Title = UI.IP1Title;
            _MainWindowVM.InputPath2Title = UI.IP2Title;
            _MainWindowVM.InputPath3Title = UI.IP3Title;
            _MainWindowVM.OPDToogle1 = UI.OPDToogle1;
            _MainWindowVM.OPDToogle2 = UI.OPDToogle2;
            _MainWindowVM.OPDToogle3 = UI.OPDToogle3;
            _MainWindowVM.OPDToogle4 = UI.OPDToogle4;
            _MainWindowVM.InputPath1Enabled = UI.IP1Enabled;
            _MainWindowVM.InputPath2Enabled = UI.IP2Enabled;
            _MainWindowVM.InputPath3Enabled = UI.IP3Enabled;
            _MainWindowVM.UDButtonVisibility = UI.UDVisibility;
            _MainWindowVM.OPDToogle1Visibility = UI.OPD1Visibility;
            _MainWindowVM.OPDToogle2Visibility = UI.OPD2Visibility;
            _MainWindowVM.OPDToogle3Visibility = UI.OPD3Visibility;
            _MainWindowVM.OPDToogle4Visibility = UI.OPD4Visibility;
        }

        public static void SwitchMainWindowControlState(bool b)
        {
            _MainWindowVM.InputPath1Enabled = b;
            _MainWindowVM.InputPath2Enabled = b;
            _MainWindowVM.InputPath3Enabled = b;
            _MainWindowVM.OutputPathEnabled = b;
            _MainWindowVM.StatEnabled = b;
            _MainWindowVM.BatchEnabled = b;
            _MainWindowVM.OPDEnabled = b;
            _MainWindowVM.UDEnabled = b;
            _MainWindowVM.StopEnabled = !b;
            _MainWindowVM.SaveEnabled = b;
            _MainWindowVM.WindowXEnabled = b;
            _MainWindowVM.WindowYEnabled = b;
            _MainWindowVM.OPDToogle1Enabled = b;
            _MainWindowVM.OPDToogle2Enabled = b;
            _MainWindowVM.OPDToogle3Enabled = b;
            _MainWindowVM.OPDToogle4Enabled = b;
        }

        public static void SwitchUIStateForStart(bool isOPD)
        {
            if (isOPD)
            {
                try
                {
                    _MainWindowVM.OPDBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF5255"));
                    _MainWindowVM.UDOpacity = 0.2F;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            else
            {
                _MainWindowVM.UDBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF5255"));
                _MainWindowVM.OPDOpacity = 0.2F;
            }
        }

        public static void SwitchUIStateForStop()
        {
            _MainWindowVM.OPDOpacity = 1;
            _MainWindowVM.UDOpacity = 1;
            _MainWindowVM.OPDBackground = new SolidColorBrush(Colors.White);
            _MainWindowVM.UDBackground = new SolidColorBrush(Colors.White);
        }


        public static void InitAdvancedSettingsUI()
        {

        }

        public static void InitSettingsPage1UI()
        {
            _SettingsPage1VM.SearchFrequency = _SettingsModel.Speed;

        }

        public static void InitSettingsPage2UI()
        {

        }

        public static void InitSimpleWindow()
        {
            List<Format> hospitalCustomers = new List<Format>() { Format.小港醫院TOC, Format.光田醫院TOC, Format.民生醫院TOC, Format.義大醫院TOC };
            List<Format> powderCustomers = new List<Format>() { Format.光田醫院TJVS, Format.長庚磨粉TJVS};
            if (hospitalCustomers.Contains(_SettingsModel.Mode))
            {
                _SimpleWindowVM.OPDContent = "門 診F5";
                _SimpleWindowVM.UDVisibility = Visibility.Visible;
            }
            _SimpleWindowVM.MultiVisibility = _SettingsModel.Mode == Format.光田醫院TOC ? Visibility.Visible : Visibility.Hidden;
            _SimpleWindowVM.CombiVisibility = _SettingsModel.Mode == Format.光田醫院TOC ? Visibility.Visible : Visibility.Hidden;
            _SimpleWindowVM.MultiChecked = Properties.Settings.Default.DoseType == "M";
            if (powderCustomers.Contains(_SettingsModel.Mode))
            {
                _SimpleWindowVM.OPDContent = "磨 粉F5";
                _SimpleWindowVM.UDVisibility = Visibility.Hidden;
            }
            _SimpleWindowVM.StatVisibility = _SettingsModel.EN_StatOrBatch ? Visibility.Visible : Visibility.Hidden;
            _SimpleWindowVM.BatchVisibility = _SettingsModel.EN_StatOrBatch ? Visibility.Visible : Visibility.Hidden;
            _SimpleWindowVM.CloseVisibility = _SettingsModel.EN_ShowControlButton ? Visibility.Visible : Visibility.Hidden;
            _SimpleWindowVM.MinimumVisibility = _SettingsModel.EN_ShowControlButton ? Visibility.Visible : Visibility.Hidden;
            _SimpleWindowVM.StatChecked = _SettingsModel.StatOrBatch == "S";
            _SimpleWindowVM.StopEnabled = false;
            _SimpleWindowVM.Left = Properties.Settings.Default.X;
            _SimpleWindowVM.Top = Properties.Settings.Default.Y;

        }
    }
}