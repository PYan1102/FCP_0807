using FCP.Models;
using FCP.Services;

namespace FCP.src.Factory.Models
{
    public static class SettingFactory
    {
        private static Setting _setting { get; set; }
        private static SettingModel _settingModel { get; set; }


        public static Setting GenerateSetting()
        {
            if (_setting == null)
                _setting = new Setting();
            return _setting;
        }

        public static SettingModel GenerateSettingModel()
        {
            if (_settingModel == null)
                _settingModel = new SettingModel();
            return _settingModel;
        }
    }
}
