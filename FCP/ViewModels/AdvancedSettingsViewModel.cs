using System;
using System.Linq;
using System.Text;
using System.Windows.Input;
using FCP.Core;
using System.Windows.Media;
using FCP.Models;
using System.Windows;
using FCP.Service;
using FCP.src.Factory.ViewModel;
using Helper;
using FCP.src;
using MaterialDesignThemes.Wpf;
using FCP.src.Enum;
using FCP.src.Factory.Models;
using FCP.src.Factory;

namespace FCP.ViewModels
{
    class AdvancedSettingsViewModel : ViewModelBase
    {
        public ICommand Page1 { get; set; }
        public ICommand Page2 { get; set; }
        public ICommand Save { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand Close { get; set; }
        public Action CloseWindow { get; set; }
        private SettingsPage1ViewModel _settingPage1VM;
        private SettingsPage2ViewModel _settingPage2VM;
        private object _currentView;
        private AdvancedSettingsModel _model;
        private Setting _setting;
        private SettingModel _settingModel;
        private SolidColorBrush _yellow = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F4D03F"));
        private SolidColorBrush _blue = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1D6FB1"));
        private SolidColorBrush White = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF"));

        public AdvancedSettingsViewModel()
        {
            Console.WriteLine("A");
            _setting = SettingFactory.GenerateSetting();
            _settingModel = SettingFactory.GenerateSettingModel();
            _model = new AdvancedSettingsModel();
            Page1 = new RelayCommand(() => Page1Func());
            Page2 = new RelayCommand(() => Page2Func());
            Save = new RelayCommand(() => SaveFunc());
            Cancel = new RelayCommand(() => CancelFuncAsync());
            Close = new RelayCommand(() => CloseWindow());

            _settingPage1VM = AdvancedSettingsFactory.GenerateSettingsPage1ViewModel();
            _settingPage2VM = AdvancedSettingsFactory.GenerateSettingsPage2ViewModel();
        }

        public object CurrentView
        {
            get => _currentView;
            set => _currentView = value;
        }

        public SolidColorBrush Page1Backround
        {
            get => _model.Page1Background;
            set => _model.Page1Background = value;
        }

        public SolidColorBrush Page1Foreground
        {
            get => _model.Page1Foreground;
            set => _model.Page1Foreground = value;
        }

        public SolidColorBrush Page2Backround
        {
            get => _model.Page2Background;
            set => _model.Page2Background = value;
        }

        public SolidColorBrush Page2Foreground
        {
            get => _model.Page2Foreground;
            set => _model.Page2Foreground = value;
        }

        public Visibility Visibility
        {
            get => _model.Visibility;
            set => _model.Visibility = value;
        }
        public void Page1Func()
        {
            Page1Backround = _yellow;
            Page1Foreground = White;
            Page2Backround = White;
            Page2Foreground = _blue;
            AdvancedSettingsFactory.ClearSettingsPageViewModel();
            CurrentView = _settingPage1VM;
        }

        public void Page2Func()
        {
            Page1Backround = White;
            Page1Foreground = _blue;
            Page2Backround = _yellow;
            Page2Foreground = White;
            AdvancedSettingsFactory.ClearSettingsPageViewModel();
            CurrentView = _settingPage2VM;
        }

        public void SaveFunc()
        {
            try
            {
                bool isSameFormat = _settingPage1VM.FormatIndex == (int)_settingModel.Format;
                if (!isSameFormat)  //若新設定的轉檔格式與舊的不同，則停止轉檔
                {
                    FormatFactory.GenerateFormat(_settingModel.Format).StopAll();
                }
                eDoseType doseType = _settingPage1VM.MultiChecked ? eDoseType.餐包 : eDoseType.種包;
                ePackMode packMode = _settingPage1VM.FilterAdminCodeChecked ? ePackMode.過濾特殊 : _settingPage1VM.UseAdminCodeChecked ? ePackMode.使用特殊 : ePackMode.正常;

                SettingModel model = _settingModel;
                model.FileExtensionName = _settingPage2VM.FileExtensionName;
                model.Format = (eFormat)_settingPage1VM.FormatIndex;
                model.Speed = Convert.ToInt32(_settingPage1VM.SearchFrequency);
                model.PackMode = packMode;
                model.FilterAdminCode = _settingPage1VM.FilterAdminCodeList.ToList();
                model.ExtraRandom = _settingPage1VM.Random.ToList();
                model.DoseType = doseType;
                model.OutputSpecialAdminCode = _settingPage1VM.OutputSpecialAdminCode;
                model.CutTime = _settingPage1VM.CutTime;
                model.CrossDayAdminCode = _settingPage1VM.AdminCodeOfCrossDay;
                model.FilterMedicineCode = _settingPage1VM.FilterMedicineCodeList.ToList();
                model.UseStatOrBatch = _settingPage2VM.ShowStatAndBatchOptionChecked;
                model.WindowMinimize = _settingPage2VM.MinimizeWindowWhenProgramStartChecked;
                model.ShowWindowOperationButton = _settingPage2VM.ShowCloseAndMinimizeButtonChecked;
                model.ShowXYParameter = _settingPage2VM.ShowXYChecked;
                model.UseFilterMedicineCode = _settingPage2VM.FilterMedicineCodeChecked;
                model.FilterNoCanister = _settingPage2VM.OnlyCanisterInChecked;
                model.MoveSourceFileToBackupDirectoryWhenDone = _settingPage2VM.WhenCompeletedMoveFileChecked;
                model.StopWhenDone = _settingPage2VM.WhenCompeletedStopChecked;
                _setting.Save(model);
                if (!isSameFormat)  //若新設定的轉檔格式與舊的不同，則產生對應的格式
                {
                    MainWindowFactory.GenerateMainWindowViewModel().GenerateCurrentFormat();
                }
                Message.Show("儲存完成", "成功", PackIconKind.Information, KindColors.Information);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                Message.Show(ex.ToString(), "錯誤", PackIconKind.Error, KindColors.Error);
            }
        }

        public async void CancelFuncAsync()
        {
            if (MessageHelper.Question("是否確定取消本次修改?") == System.Windows.Forms.DialogResult.No)
            {
                return;
            }
            AdvancedSettingsFactory.ClearSettingsPageViewModel();
            _settingPage1VM = AdvancedSettingsFactory.GenerateSettingsPage1ViewModel();
            _settingPage2VM = AdvancedSettingsFactory.GenerateSettingsPage2ViewModel();
            Page2Func();
            await System.Threading.Tasks.Task.Delay(1);
            Page1Func();
        }
    }
}