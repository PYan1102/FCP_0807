using FCP.Models;

namespace FCP.src.Factory.Models
{
    static class ModelsFactory
    {
        private static SettingPage1Model _settingsPage1Model;
        private static SettingPage2Model _settingPage2Model;
        private static SettingJsonModel _settingModel;

        public static SettingPage1Model GenerateSettingPage1Model()
        {
            _settingsPage1Model = _settingsPage1Model ?? new SettingPage1Model();
            return _settingsPage1Model;
        }

        public static SettingPage2Model GenerateSettingPage2Model()
        {
            _settingPage2Model = _settingPage2Model ?? new SettingPage2Model();
            return _settingPage2Model;
        }

        public static SettingJsonModel GenerateSettingModel()
        {
            _settingModel = _settingModel ?? new SettingJsonModel();
            return _settingModel;
        }

        public static void SetSettingPage1ModelNull()
        {
            _settingsPage1Model = null;
        }

        public static void SetSettingPage2ModelNull()
        {
            _settingPage2Model = null;
        }
    }
}
