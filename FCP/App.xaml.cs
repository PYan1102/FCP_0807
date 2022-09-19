using FCP.ViewModels;
using Lierda.WPFHelper;
using Microsoft.Extensions.DependencyInjection;
using System;
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

        public IServiceProvider Services { get; set; }
        public new static App Current => (App)Application.Current;
        private LierdaCracker _cracker = new LierdaCracker();

        public static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            services.AddTransient<MainWindowViewModel>();
            services.AddTransient<SettingPage1ViewModel>();
            services.AddTransient<SettingPage2ViewModel>();
            services.AddTransient<SettingPage3ViewModel>();
            services.AddTransient<MsgViewModel>();
            services.AddTransient<SettingViewModel>();
            services.AddTransient<SimpleWindowViewModel>();
            services.AddTransient<WindowPositionViewModel>();

            return services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            _cracker.Cracker(5);
            base.OnStartup(e);
        }
    }
}
