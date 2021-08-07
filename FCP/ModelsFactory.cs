using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCP.Factory
{
    public static class ModelsFactory
    {
        public static Models.Settings Settings { get; set; }

        public static Models.Settings GenerateSettings()
        {
            if (Settings == null)
                Settings = new Models.Settings();
            return Settings;
        }
    }
}
