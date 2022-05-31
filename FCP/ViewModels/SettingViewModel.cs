using System;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using FCP.Models;
using System.Windows;
using FCP.Services;
using Helper;
using FCP.src;
using MaterialDesignThemes.Wpf;
using FCP.src.Enum;
using FCP.src.Factory;
using FCP.src.Factory.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using FCP.src.MessageManager;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Messaging;
using FCP.src.Dictionary;

namespace FCP.ViewModels
{
    class SettingViewModel : ObservableRecipient
    {
        public ICommand Page1Loaded { get; set; }
        public ICommand Page2Loaded { get; set; }
        public ICommand Save { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand Close { get; set; }
        private object _currentView;
        private SettingModel _model;
        private SettingJsonModel _settingModel;
        private SettingPage1ViewModel _page1VM;
        private SettingPage2ViewModel _page2VM;

        public SettingViewModel()
        {
            _model = new SettingModel();
            Page1Loaded = new RelayCommand(Page1Func);
            Page2Loaded = new RelayCommand(Page2Func);
            Save = new RelayCommand(SaveFunc);
            Cancel = new RelayCommand(CancelFuncAsync);
            Close = new RelayCommand<Window>(o => o.DialogResult = true);

            _settingModel = ModelsFactory.GenerateSettingModel();
            _page1VM = App.Current.Services.GetService<SettingPage1ViewModel>();
            _page2VM = App.Current.Services.GetService<SettingPage2ViewModel>();
        }

        public object CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public SolidColorBrush Page1Background
        {
            get => _model.Page1Background;
            set => SetProperty(_model.Page1Background, value, _model, (model, _value) => model.Page1Background = _value);
        }

        public SolidColorBrush Page1Foreground
        {
            get => _model.Page1Foreground;
            set => SetProperty(_model.Page1Foreground, value, _model, (model, _value) => model.Page1Foreground = _value);
        }

        public SolidColorBrush Page2Background
        {
            get => _model.Page2Background;
            set => SetProperty(_model.Page2Background, value, _model, (model, _value) => model.Page2Background = _value);
        }

        public SolidColorBrush Page2Foreground
        {
            get => _model.Page2Foreground;
            set => SetProperty(_model.Page2Foreground, value, _model, (model, _value) => model.Page2Foreground = _value);
        }

        public void Page1Func()
        {
            Page1Background = dColor.GetSolidColorBrush(eColor.Yellow);
            Page1Foreground = dColor.GetSolidColorBrush(eColor.White);
            Page2Background = dColor.GetSolidColorBrush(eColor.White);
            Page2Foreground = dColor.GetSolidColorBrush(eColor.Blue);
            CurrentView = _page1VM;
        }

        public void Page2Func()
        {
            Page1Background = dColor.GetSolidColorBrush(eColor.White);
            Page1Foreground = dColor.GetSolidColorBrush(eColor.Blue);
            Page2Background = dColor.GetSolidColorBrush(eColor.Yellow);
            Page2Foreground = dColor.GetSolidColorBrush(eColor.White);
            CurrentView = _page2VM;
        }

        public void SaveFunc()
        {
            try
            {
                var page1VM = _page1VM;
                var page2VM = _page2VM;
                bool isSameFormat = page1VM.FormatIndex == (int)_settingModel.Format;
                if (!isSameFormat)  //若新設定的轉檔格式與舊的不同，則停止轉檔
                {
                    FormatFactory.GenerateFormat(_settingModel.Format).StopAll();
                }
                eDoseType doseType = page1VM.MultiChecked ? eDoseType.餐包 : eDoseType.種包;
                ePackMode packMode = page1VM.FilterAdminCodeChecked ? ePackMode.過濾特殊 : page1VM.UseAdminCodeChecked ? ePackMode.使用特殊 : ePackMode.正常;

                SettingJsonModel model = _settingModel;
                model.FileExtensionName = page2VM.FileExtensionName;
                model.Format = (eFormat)page1VM.FormatIndex;
                model.Speed = Convert.ToInt32(page1VM.SearchFrequency);
                model.PackMode = packMode;
                model.FilterAdminCode = page1VM.FilterAdminCodeList.ToList();
                model.ExtraRandom = page1VM.Random.ToList();
                model.DoseType = doseType;
                model.OutputSpecialAdminCode = page1VM.OutputSpecialAdminCode;
                model.CrossDayAdminCode = page1VM.AdminCodeOfCrossDay;
                model.FilterMedicineCode = page1VM.FilterMedicineCodeList.ToList();
                model.UseStatOrBatch = page2VM.UseStatAndBatchOptionChecked;
                model.WindowMinimize = page2VM.MinimizeWindowWhenProgramStartChecked;
                model.ShowWindowOperationButton = page2VM.ShowCloseAndMinimizeButtonChecked;
                model.ShowXYParameter = page2VM.ShowXYChecked;
                model.UseFilterMedicineCode = page2VM.FilterMedicineCodeChecked;
                model.FilterNoCanister = page2VM.OnlyCanisterInChecked;
                model.MoveSourceFileToBackupDirectoryWhenDone = page2VM.WhenCompeletedMoveFileChecked;
                model.StopWhenDone = page2VM.WhenCompeletedStopChecked;
                Setting.Save(model);
                if (!isSameFormat)  //若新設定的轉檔格式與舊的不同，則產生對應的格式
                {
                    WeakReferenceMessenger.Default.Send(new CommandMessage(), nameof(eCommandCollection.CreateNewFormat));
                }
                MsgCollection.ShowDialog("儲存完成", "成功", PackIconKind.Information, dColor.GetSolidColorBrush(eColor.RoyalBlue));
            }
            catch (Exception ex)
            {
                LogService.Exception(ex);
                MsgCollection.ShowDialog(ex.ToString(), "錯誤", PackIconKind.Error, dColor.GetSolidColorBrush(eColor.Red));
            }
        }

        public void CancelFuncAsync()
        {
            if (MessageHelper.Question("是否確定取消本次修改?") == System.Windows.Forms.DialogResult.No)
            {
                return;
            }
            _page1VM.ResetModel();
            _page2VM.ResetModel();
        }
    }
}