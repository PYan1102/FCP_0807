using System;
using FCP.MVVM.ViewModels;

namespace FCP.MVVM.Factory.ViewModel
{
    static class AdvancedSettingsFactory
    {
        private static AdvancedSettingsViewModel _AdvancedSettingsVM { get; set; }
        public static SettingsPage1ViewModel _SettingsPage1VM { get; set; }
        public static SettingsPage2ViewModel _SettingsPage2VM { get; set; }
        public static AdvancedSettings GenerateAdvancedSettings()
        {
            return new AdvancedSettings();
        }

        public static AdvancedSettingsViewModel GenerateAdvancedSettingsViewModel()
        {
            if (_AdvancedSettingsVM == null)
                _AdvancedSettingsVM = new AdvancedSettingsViewModel();
            return _AdvancedSettingsVM;
        }

        public static SettingsPage1ViewModel GenerateSettingsPage1()
        {
            if (_SettingsPage1VM == null)
                _SettingsPage1VM = new SettingsPage1ViewModel();
            return _SettingsPage1VM;
        }

        public static SettingsPage2ViewModel GenerateSettingsPage2()
        {
            if (_SettingsPage2VM == null)
                _SettingsPage2VM = new SettingsPage2ViewModel();
            return _SettingsPage2VM;
        }
    }
}
