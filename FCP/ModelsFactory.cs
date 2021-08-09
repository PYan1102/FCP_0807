using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FCP.MVVM.Models;

namespace FCP.Factory
{
    public static class ModelsFactory
    {
        public static SettingsModel Settings { get; set; }

        public static SettingsModel GenerateSettings()
        {
            if (Settings == null)
                Settings = new SettingsModel();
            return Settings;
        }
    }
}
