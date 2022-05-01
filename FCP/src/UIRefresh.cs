using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using FCP.Models;
using FCP.ViewModels;
using FCP.src.Factory.ViewModel;
using Helper;
using MaterialDesignThemes.Wpf;
using FCP.src.Factory.Models;

namespace FCP.src
{
    class UIRefresh
    {
        public MainUILayoutModel UILayout { get; set; }
        private SimpleWindowViewModel _simpleWindowVM;
        private MainWindowViewModel _mainWindowVM;
        private SettingModel _settingModel;
        private CancellationTokenSource _cts;
        public UIRefresh()
        {
            _simpleWindowVM = SimpleWindowFactory.GenerateSimpleWindowViewModel();
            _mainWindowVM = MainWindowFactory.GenerateMainWindowViewModel();
            _settingModel = ModelsFactory.GenerateSettingModel();
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
                        _mainWindowVM.DoseType = _settingModel.DoseType.ToString();
                    }
                    _mainWindowVM.StatVisibility = _settingModel.UseStatOrBatch ? Visibility.Visible : Visibility.Hidden;
                    _mainWindowVM.BatchVisibility = _settingModel.UseStatOrBatch ? Visibility.Visible : Visibility.Hidden;
                    _mainWindowVM.MinimumAndCloseVisibility = _settingModel.ShowWindowOperationButton ? Visibility.Visible : Visibility.Hidden;
                    _mainWindowVM.WindowXVisibility = _settingModel.ShowXYParameter ? Visibility.Visible : Visibility.Hidden;
                    _mainWindowVM.WindowYVisibility = _settingModel.ShowXYParameter ? Visibility.Visible : Visibility.Hidden;
                }
                catch (Exception ex)
                {
                    Log.Write(ex);
                    MsgCollection.Show(ex, "錯誤", PackIconKind.Error, KindColors.Error);
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
}
