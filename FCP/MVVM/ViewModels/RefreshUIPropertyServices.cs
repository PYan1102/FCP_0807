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

namespace FCP.MVVM.ViewModels
{
    static class RefreshUIPropertyServices
    {
        private static MainWindowViewModel _MainWindowVM { get => MainWindowFacotry.GenerateMainWindowViewModel(); }
        private static SettingsModel _SettingsModel { get => SettingsFactory.GenerateSettingsModel(); }
        private static AdvancedSettingsViewModel _AdvancedSettingsVM { get=> AdvancedSettingsFactory.GenerateAdvancedSettingsViewModel(); }
        private static SettingsPage1ViewModel _SettingsPage1VM { get=> AdvancedSettingsFactory.GenerateSettingsPage1(); }
        private static SettingsPage2ViewModel _SettingsPage2VM { get => AdvancedSettingsFactory.GenerateSettingsPage2(); }

        public static void InitMainWindowUI()
        {
            string BackupPath = $@"{FunctionCollections.FileBackupPath}\{DateTime.Now:yyyy-MM-dd}";
            _MainWindowVM.SuccessCount= $"{Directory.GetFiles($@"{BackupPath}\Success").Length}";
            _MainWindowVM.FailCount= $"{Directory.GetFiles($@"{BackupPath}\Fail").Length}";
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
            _MainWindowVM.IsAutoStartChecked = b;
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
    }
}