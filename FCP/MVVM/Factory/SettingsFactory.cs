using FCP.MVVM.Models;
using FCP.MVVM.Control;

namespace FCP.MVVM.Factory
{
    public static class SettingsFactory
    {
        private static Settings _SettingsControl { get; set; }
        private static SettingsModel _SettingsModel { get; set; }


        public static Settings GenerateSettingsControl()
        {
            if (_SettingsControl == null)
                _SettingsControl = new Settings();
            return _SettingsControl;
        }

        public static SettingsModel GenerateSettingsModels()
        {
            if (_SettingsModel == null)
                _SettingsModel = new SettingsModel();
            return _SettingsModel;
        }
        
        public static AdvancedSettings GenerateAdvancesSettings()
        {
            return new AdvancedSettings();
        }
    }
}
