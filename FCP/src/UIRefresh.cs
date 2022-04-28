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
        public UILayout UILayout { get; set; }
        private SimpleWindowViewModel _simpleWindowVM;
        private MainWindowViewModel _mainWindowVM;
        private SettingModel _settingsModel;
        private CancellationTokenSource _cts;
        public UIRefresh()
        {
            _simpleWindowVM = SimpleWindowFactory.GenerateSimpleWindowViewModel();
            _mainWindowVM = MainWindowFactory.GenerateMainWindowViewModel();
            _settingsModel = SettingFactory.GenerateSettingModel();
        }
        public async void StartAsync()
        {
            _cts = new CancellationTokenSource();
            while (!_cts.IsCancellationRequested)
            {
                await Task.Delay(500);
                try
                {
                    _simpleWindowVM.SetWindowPosition(Properties.Settings.Default.X, Properties.Settings.Default.Y);
                    if (!_mainWindowVM.StopEnabled)
                    {
                        RefreshUIPropertyServices.RefrehMainWindowUI(UILayout);
                        _mainWindowVM.DoseType = _settingsModel.DoseType.ToString();
                    }
                    _mainWindowVM.StatVisibility = _settingsModel.UseStatOrBatch ? Visibility.Visible : Visibility.Hidden;
                    _mainWindowVM.BatchVisibility = _settingsModel.UseStatOrBatch ? Visibility.Visible : Visibility.Hidden;
                    _mainWindowVM.MinimumAndCloseVisibility = _settingsModel.ShowWindowOperationButton ? Visibility.Visible : Visibility.Hidden;
                    _mainWindowVM.WindowXVisibility = _settingsModel.ShowXYParameter ? Visibility.Visible : Visibility.Hidden;
                    _mainWindowVM.WindowYVisibility = _settingsModel.ShowXYParameter ? Visibility.Visible : Visibility.Hidden;
                }
                catch (Exception ex)
                {
                    Log.Write(ex);
                    Message.Show(ex, "錯誤", PackIconKind.Error, KindColors.Error);
                }
            }
        }

        public void Stop()
        {
            if (_cts != null)
            {
                _cts.Cancel();
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
