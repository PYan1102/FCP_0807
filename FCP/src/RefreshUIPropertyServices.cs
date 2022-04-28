using System;
using FCP.src.Factory.ViewModel;
using FCP.Models;
using System.Windows.Media;
using FCP.src.Factory.Models;
using FCP.ViewModels;

namespace FCP.src
{
    static class RefreshUIPropertyServices
    {
        private static MainWindowViewModel _MainWindowVM { get => MainWindowFactory.GenerateMainWindowViewModel(); }
        private static SettingModel _SettingsModel { get => SettingFactory.GenerateSettingModel(); }
        private static SimpleWindowViewModel _SimpleWindowVM { get => SimpleWindowFactory.GenerateSimpleWindowViewModel(); }

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

        public static void InitSimpleWindow()
        {
            

        }
    }
}