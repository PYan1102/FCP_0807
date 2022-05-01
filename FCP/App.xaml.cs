using Lierda.WPFHelper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FCP
{
    /// <summary>
    /// App.xaml 的互動邏輯
    /// </summary>
    public partial class App : Application
    {
        LierdaCracker cracker = new LierdaCracker();
        protected override void OnStartup(StartupEventArgs e)
        {
            cracker.Cracker(60);
            base.OnStartup(e);
        }
    }
}
