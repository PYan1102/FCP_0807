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
        private static MainWindowViewModel _mainWindowVM { get => MainWindowFactory.GenerateMainWindowViewModel(); }
        private static SettingModel _settingModel { get => SettingFactory.GenerateSettingModel(); }
        private static SimpleWindowViewModel _simpleWindowVM { get => SimpleWindowFactory.GenerateSimpleWindowViewModel(); }

        public static void RefrehMainWindowUI(UILayout UI)
        {
            _mainWindowVM.WindowTitle = UI.Title;
            _mainWindowVM.InputDirectory1Title = UI.IP1Title;
            _mainWindowVM.InputDirectory2Title = UI.IP2Title;
            _mainWindowVM.InputDirectory3Title = UI.IP3Title;
            _mainWindowVM.InputDirectory4Title = UI.IP4Title;
            _mainWindowVM.InputDirectory5Title = UI.IP5Title;
            _mainWindowVM.InputDirectory6Title = UI.IP6Title;
            _mainWindowVM.OPDToogle1 = UI.OPDToogle1;
            _mainWindowVM.OPDToogle2 = UI.OPDToogle2;
            _mainWindowVM.OPDToogle3 = UI.OPDToogle3;
            _mainWindowVM.OPDToogle4 = UI.OPDToogle4;
            _mainWindowVM.InputDirectory1Enabled = UI.IP1Enabled;
            _mainWindowVM.InputDirectory2Enabled = UI.IP2Enabled;
            _mainWindowVM.InputDirectory3Enabled = UI.IP3Enabled;
            _mainWindowVM.InputDirectory4Enabled = UI.IP4Enabled;
            _mainWindowVM.InputDirectory5Enabled = UI.IP5Enabled;
            _mainWindowVM.InputDirectory6Enabled = UI.IP6Enabled;
            _mainWindowVM.UDButtonVisibility = UI.UDVisibility;
            _mainWindowVM.OPDToogle1Visibility = UI.OPD1Visibility;
            _mainWindowVM.OPDToogle2Visibility = UI.OPD2Visibility;
            _mainWindowVM.OPDToogle3Visibility = UI.OPD3Visibility;
            _mainWindowVM.OPDToogle4Visibility = UI.OPD4Visibility;
        }

        public static void SwitchMainWindowControlState(bool b)
        {
            _mainWindowVM.InputDirectory1Enabled = b;
            _mainWindowVM.InputDirectory2Enabled = b;
            _mainWindowVM.InputDirectory3Enabled = b;
            _mainWindowVM.InputDirectory4Enabled = b;
            _mainWindowVM.InputDirectory5Enabled = b;
            _mainWindowVM.InputDirectory6Enabled = b;
            _mainWindowVM.OutputDirectoryEnabled = b;
            _mainWindowVM.StatEnabled = b;
            _mainWindowVM.BatchEnabled = b;
            _mainWindowVM.OPDEnabled = b;
            _mainWindowVM.UDEnabled = b;
            _mainWindowVM.StopEnabled = !b;
            _mainWindowVM.SaveEnabled = b;
            _mainWindowVM.WindowXEnabled = b;
            _mainWindowVM.WindowYEnabled = b;
            _mainWindowVM.OPDToogle1Enabled = b;
            _mainWindowVM.OPDToogle2Enabled = b;
            _mainWindowVM.OPDToogle3Enabled = b;
            _mainWindowVM.OPDToogle4Enabled = b;
        }

        public static void SwitchUIStateForStart()
        {
            if (CommonModel.CurrentDepartment == Enum.eDepartment.OPD)
            {
                try
                {
                    _mainWindowVM.OPDBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF5255"));
                    _mainWindowVM.UDOpacity = 0.2F;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            else
            {
                _mainWindowVM.UDBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF5255"));
                _mainWindowVM.OPDOpacity = 0.2F;
            }
        }

        public static void SwitchUIStateForStop()
        {
            _mainWindowVM.OPDOpacity = 1;
            _mainWindowVM.UDOpacity = 1;
            _mainWindowVM.OPDBackground = new SolidColorBrush(Colors.White);
            _mainWindowVM.UDBackground = new SolidColorBrush(Colors.White);
        }

        public static void InitSimpleWindow()
        {


        }
    }
}