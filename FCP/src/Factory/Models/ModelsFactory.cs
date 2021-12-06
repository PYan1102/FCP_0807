using FCP.MVVM.Models;

namespace FCP.src.Factory.Models
{
    static class ModelsFactory
    {
        private static SettingsPage1Model _SettingsPage1Model;
        private static SettingsPage2Model _SettingsPage2Model;

        public static SettingsPage1Model GenerateSettingsPage1Model()
        {
            if (_SettingsPage1Model == null)
                _SettingsPage1Model = new SettingsPage1Model();
            return _SettingsPage1Model;
        }

        public static SettingsPage2Model GenerateSettingsPage2Model()
        {
            if (_SettingsPage2Model == null)
                _SettingsPage2Model = new SettingsPage2Model();
            return _SettingsPage2Model;
        }

        public static void SetSettingsPage1ModelNull()
        {
            _SettingsPage1Model = null;
        }

        public static void SetSettingsPage2ModelNull()
        {
            _SettingsPage2Model = null;
        }
    }
}
