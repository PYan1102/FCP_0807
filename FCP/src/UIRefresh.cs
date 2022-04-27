using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using FCP.Models;
using FCP.ViewModels;
using FCP.src.Factory.Models;
using FCP.src.Factory.ViewModel;
using Helper;
using MaterialDesignThemes.Wpf;

namespace FCP.src
{
    class UIRefresh
    {
        public CancellationTokenSource CTS { get; set; }
        public UILayout UILayout { get; set; }
        private SimpleWindowViewModel _SimpleWindowVM;
        private MainWindowViewModel _MainWindowVM;
        private SettingsModel _SettingsModel;
        public UIRefresh()
        {
            _SimpleWindowVM = SimpleWindowFactory.GenerateSimpleWindowViewModel();
            _MainWindowVM = MainWindowFactory.GenerateMainWindowViewModel();
            _SettingsModel = SettingsFactory.GenerateSettingsModel();
        }
        public async void StartAsync()
        {
            while (!CTS.IsCancellationRequested)
            {
                await Task.Delay(500);
                try
                {
                    _SimpleWindowVM.SetWindowPosition(Properties.Settings.Default.X, Properties.Settings.Default.Y);
                    if (!_MainWindowVM.StopEnabled)
                    {
                        RefreshUIPropertyServices.RefrehMainWindowUI(UILayout);
                        _MainWindowVM.DoseType = _SettingsModel.DoseType.ToString();
                    }
                    _MainWindowVM.StatVisibility = _SettingsModel.EN_StatOrBatch ? Visibility.Visible : Visibility.Hidden;
                    _MainWindowVM.BatchVisibility = _SettingsModel.EN_StatOrBatch ? Visibility.Visible : Visibility.Hidden;
                    _MainWindowVM.MinimumAndCloseVisibility = _SettingsModel.EN_ShowControlButton ? Visibility.Visible : Visibility.Hidden;
                    _MainWindowVM.WindowXVisibility = _SettingsModel.EN_ShowXY ? Visibility.Visible : Visibility.Hidden;
                    _MainWindowVM.WindowYVisibility = _SettingsModel.EN_ShowXY ? Visibility.Visible : Visibility.Hidden;
                }
                catch (Exception ex)
                {
                    Message.Show(ex, "錯誤", PackIconKind.Error, KindColors.Error);
                    Log.Write(ex);
                }
            }
        }

        public void Stop()
        {
            if (CTS != null)
            {
                CTS.Cancel();
            }
        }
    }

    public class UILayout
    {
        public string Title { get; set; }
        public string IP1Title { get; set; }
        public string IP2Title { get; set; }
        public string IP3Title { get; set; }
        public string OPDToogle1 { get; set; }
        public string OPDToogle2 { get; set; }
        public string OPDToogle3 { get; set; }
        public string OPDToogle4 { get; set; }
        public bool IP1Enabled { get; set; }
        public bool IP2Enabled { get; set; }
        public bool IP3Enabled { get; set; }
        public Visibility UDVisibility { get; set; }
        public Visibility OPD1Visibility { get; set; }
        public Visibility OPD2Visibility { get; set; }
        public Visibility OPD3Visibility { get; set; }
        public Visibility OPD4Visibility { get; set; }
    }
}
