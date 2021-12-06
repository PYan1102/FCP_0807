using System;
using System.Linq;
using System.Text;
using System.Windows.Input;
using FCP.Core;
using System.Windows.Media;
using FCP.MVVM.Models;
using System.Windows;
using FCP.MVVM.Control;
using FCP.src.Factory.ViewModel;
using Helper;
using FCP.src;
using MaterialDesignThemes.Wpf;
using FCP.src.Enum;
using FCP.src.Factory.Models;
using FCP.src.Factory;

namespace FCP.MVVM.ViewModels
{
    class AdvancedSettingsViewModel : ViewModelBase
    {
        public ICommand Page1 { get; set; }
        public ICommand Page2 { get; set; }
        public ICommand Save { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand Close { get; set; }
        public Action CloseWindow { get; set; }
        private SettingsPage1ViewModel _SettingsPage1VM;
        private SettingsPage2ViewModel _SettingsPage2VM;
        private object _CurrentView;
        private AdvancedSettingsModel _Model;
        private Settings _Settings;
        private SettingsModel _SettingsModel;
        private SolidColorBrush Yellow = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F4D03F"));
        private SolidColorBrush Blue = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1D6FB1"));
        private SolidColorBrush White = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF"));

        public AdvancedSettingsViewModel()
        {
            _Settings = SettingsFactory.GenerateSettingsControl();
            _SettingsModel = SettingsFactory.GenerateSettingsModel();
            _Model = new AdvancedSettingsModel();
            Page1 = new RelayCommand(() => Page1Func());
            Page2 = new RelayCommand(() => Page2Func());
            Save = new RelayCommand(() => SaveFunc());
            Cancel = new RelayCommand(() => CancelFuncAsync());
            Close = new RelayCommand(() => CloseWindow());

            _SettingsPage1VM = AdvancedSettingsFactory.GenerateSettingsPage1ViewModel();
            _SettingsPage2VM = AdvancedSettingsFactory.GenerateSettingsPage2ViewModel();
        }

        public object CurrentView
        {
            get => _CurrentView;
            set => _CurrentView = value;
        }

        public SolidColorBrush Page1Backround
        {
            get => _Model.Page1Background;
            set => _Model.Page1Background = value;
        }

        public SolidColorBrush Page1Foreground
        {
            get => _Model.Page1Foreground;
            set => _Model.Page1Foreground = value;
        }

        public SolidColorBrush Page2Backround
        {
            get => _Model.Page2Background;
            set => _Model.Page2Background = value;
        }

        public SolidColorBrush Page2Foreground
        {
            get => _Model.Page2Foreground;
            set => _Model.Page2Foreground = value;
        }

        public Visibility Visibility
        {
            get => _Model.Visibility;
            set => _Model.Visibility = value;
        }
        public void Page1Func()
        {
            Page1Backround = Yellow;
            Page1Foreground = White;
            Page2Backround = White;
            Page2Foreground = Blue;
            AdvancedSettingsFactory.ClearSettingsPageViewModel();
            CurrentView = _SettingsPage1VM;
        }

        public void Page2Func()
        {
            Page1Backround = White;
            Page1Foreground = Blue;
            Page2Backround = Yellow;
            Page2Foreground = White;
            AdvancedSettingsFactory.ClearSettingsPageViewModel();
            CurrentView = _SettingsPage2VM;
        }

        public void SaveFunc()
        {
            try
            {
                bool isSameFormat = _SettingsPage1VM.ModeIndex == (int)_SettingsModel.Mode;
                if (!isSameFormat)
                {
                    FormatFactory.GenerateFormat(_SettingsModel.Mode).CloseSelf();  //old
                }
                eDoseType doseType = _SettingsPage1VM.MultiChecked ? eDoseType.餐包 : eDoseType.種包;
                ePackMode packMode;
                if (_SettingsPage1VM.FilterAdminCodeChecked)
                    packMode = ePackMode.過濾特殊;
                else if (_SettingsPage1VM.UseAdminCodeChecked)
                    packMode = ePackMode.使用特殊;
                else
                    packMode = ePackMode.正常;
                string filterAdminCode = "";
                _SettingsPage1VM.FilterAdminCodeList.ToList().ForEach(x => filterAdminCode += $"{x},");
                string filterMedicineCode = "";
                _SettingsPage1VM.FilterMedicineCodeList.ToList().ForEach(x => filterMedicineCode += $"{x},");
                StringBuilder sb = new StringBuilder();
                _SettingsPage1VM.Random.ToList().ForEach(x => sb.Append($"{x.No}:{x.JVServer}:{x.OnCube}|"));
                string extraRandom = sb.ToString().TrimEnd('|');
                _Settings.SaveAdvancedSettings(
                    (eFormat)_SettingsPage1VM.ModeIndex,
                    Convert.ToInt32(_SettingsPage1VM.SearchFrequency),
                    packMode,
                    filterAdminCode.TrimEnd(','),
                    extraRandom,
                    doseType,
                    _SettingsPage1VM.OutputSpecialAdminCode,
                    _SettingsPage1VM.CutTime,
                    _SettingsPage1VM.AdminCodeOfCrossDay,
                    filterMedicineCode.TrimEnd(','),
                    _SettingsPage2VM.FileExtensionName,
                    _SettingsPage2VM.ShowStatAndBatchOptionChecked,
                    _SettingsPage2VM.MinimizeWindowWhenProgramStartChecked,
                    _SettingsPage2VM.ShowCloseAndMinimizeButtonChecked,
                    _SettingsPage2VM.ShowXYChecked,
                    _SettingsPage2VM.FilterMedicineCodeChecked,
                    _SettingsPage2VM.OnlyCanisterInChecked,
                    _SettingsPage2VM.WhenCompeletedMoveFileChecked,
                    _SettingsPage2VM.WhenCompeletedStopChecked);
                if (!isSameFormat)
                {
                    MainWindowFactory.GenerateMainWindowViewModel().JudgeCurrentFormat();  //new
                }
                Message.Show("儲存完成", "成功", PackIconKind.Information, KindColors.Information);
            }
            catch (Exception ex)
            {
                Message.Show(ex.ToString(), "錯誤", PackIconKind.Error, KindColors.Error);
                Log.Write(ex);
            }
        }

        public async void CancelFuncAsync()
        {
            if (MessageHelper.Question("是否確定取消本次修改?") == System.Windows.Forms.DialogResult.No)
            {
                return;
            }
            AdvancedSettingsFactory.ClearSettingsPageViewModel();
            _SettingsPage1VM = AdvancedSettingsFactory.GenerateSettingsPage1ViewModel();
            _SettingsPage2VM = AdvancedSettingsFactory.GenerateSettingsPage2ViewModel();
            Page2Func();
            await System.Threading.Tasks.Task.Delay(1);
            Page1Func();
        }
    }
}