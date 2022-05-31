using FCP.ViewModels;
using Lierda.WPFHelper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Data;
using System.Linq;
using System.Windows;

namespace FCP
{
    /// <summary>
    /// App.xaml 的互動邏輯
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Services = ConfigureServices();
            this.InitializeComponent();
        }

        public new static App Current => (App)Application.Current;
        public IServiceProvider Services { get; set; }

        public static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            services.AddTransient<MainWindowViewModel>();
            services.AddTransient<SettingPage1ViewModel>();
            services.AddTransient<SettingPage2ViewModel>();
            services.AddTransient<MsgViewModel>();
            services.AddTransient<SettingViewModel>();
            services.AddTransient<SimpleWindowViewModel>();

            return services.BuildServiceProvider();
        }

        //LierdaCracker cracker = new LierdaCracker();
        //protected override void OnStartup(StartupEventArgs e)
        //{
        //    cracker.Cracker(60);
        //    base.OnStartup(e);
        //}
    }
}
