using FCP.Models;
using FCP.Service;

namespace FCP.src.Factory.Models
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

        public static SettingsModel GenerateSettingsModel()
        {
            if (_SettingsModel == null)
                _SettingsModel = new SettingsModel();
            return _SettingsModel;
        }
    }
}
