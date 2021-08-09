using System;
using FCP.MVVM.Models;
using FCP.MVVM.Control;

namespace FCP.MVVM.Factory
{
    public static class SettingsFactory
    {
        private static Control.Settings _SettingsControl { get; set; }
        private static Models.SettingsModel _SettingsModel { get; set; }

        public static Control.Settings GenerateSettingsControl()
        {
            if (_SettingsControl == null)
                _SettingsControl = new Control.Settings();
            return _SettingsControl;
        }

        public static Models.SettingsModel GenerateSettingsModels()
        {
            if (_SettingsModel == null)
                _SettingsModel = new Models.SettingsModel();
            return _SettingsModel;
        }
    }
}
