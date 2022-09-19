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
using FCP.Providers;
using FCP.src.MessageManager.Request;
using System.Text;
using System.Collections.Generic;

namespace FCP.ViewModels
{
    class SettingViewModel : ObservableRecipient
    {
        public ICommand Page1Loaded { get; set; }
        public ICommand Page2Loaded { get; set; }
        public ICommand Page3Loaded { get; set; }
        public ICommand Save { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand Close { get; set; }
        private object _currentView;
        private SettingModel _model;
        private SettingJsonModel _settingModel;
        private SettingPage1ViewModel _page1VM;
        private SettingPage2ViewModel _page2VM;
        private SettingPage3ViewModel _page3VM;

        public SettingViewModel()
        {
            _model = new SettingModel();
            Page1Loaded = new RelayCommand(Page1Func);
            Page2Loaded = new RelayCommand(Page2Func);
            Page3Loaded = new RelayCommand(Page3Func);
            Save = new RelayCommand(SaveFunc);
            Cancel = new RelayCommand(CancelFunc);
            Close = new RelayCommand<Window>(o => o.DialogResult = true);

            _settingModel = ModelsFactory.GenerateSettingModel();
            _page1VM = App.Current.Services.GetService<SettingPage1ViewModel>();
            _page2VM = App.Current.Services.GetService<SettingPage2ViewModel>();
            _page3VM = App.Current.Services.GetService<SettingPage3ViewModel>();
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

        public SolidColorBrush Page3Background
        {
            get => _model.Page3Background;
            set => SetProperty(_model.Page3Background, value, _model, (model, _value) => model.Page3Background = _value);
        }

        public SolidColorBrush Page3Foreground
        {
            get => _model.Page3Foreground;
            set => SetProperty(_model.Page3Foreground, value, _model, (model, _value) => model.Page3Foreground = _value);
        }

        public void Page1Func()
        {
            Page1Background = ColorProvider.GetSolidColorBrush(eColor.Yellow);
            Page1Foreground = ColorProvider.GetSolidColorBrush(eColor.White);
            Page2Background = ColorProvider.GetSolidColorBrush(eColor.White);
            Page2Foreground = ColorProvider.GetSolidColorBrush(eColor.Blue);
            Page3Background = ColorProvider.GetSolidColorBrush(eColor.White);
            Page3Foreground = ColorProvider.GetSolidColorBrush(eColor.Blue);
            CurrentView = _page1VM;
        }

        public void Page2Func()
        {
            Page1Background = ColorProvider.GetSolidColorBrush(eColor.White);
            Page1Foreground = ColorProvider.GetSolidColorBrush(eColor.Blue);
            Page2Background = ColorProvider.GetSolidColorBrush(eColor.Yellow);
            Page2Foreground = ColorProvider.GetSolidColorBrush(eColor.White);
            Page3Background = ColorProvider.GetSolidColorBrush(eColor.White);
            Page3Foreground = ColorProvider.GetSolidColorBrush(eColor.Blue);
            CurrentView = _page2VM;
        }

        public void Page3Func()
        {
            Page1Background = ColorProvider.GetSolidColorBrush(eColor.White);
            Page1Foreground = ColorProvider.GetSolidColorBrush(eColor.Blue);
            Page2Background = ColorProvider.GetSolidColorBrush(eColor.White);
            Page2Foreground = ColorProvider.GetSolidColorBrush(eColor.Blue);
            Page3Background = ColorProvider.GetSolidColorBrush(eColor.Yellow);
            Page3Foreground = ColorProvider.GetSolidColorBrush(eColor.White);
            CurrentView = _page3VM;
        }

        public void SaveFunc()
        {
            try
            {
                if (HasAnyErrorsMessage())
                {
                    MsgCollection.ShowDialog("可能有欄位處於未通過驗證的狀態，請確認各欄位是否符合驗證規則", "有未驗證成功的欄位", PackIconKind.Error, ColorProvider.GetSolidColorBrush(eColor.Red));
                    return;
                }
                var page1VM = _page1VM;
                var page2VM = _page2VM;
                var page3VM = _page3VM;
                bool isSameFormat = page1VM.FormatIndex == (int)_settingModel.Format;
                if (!isSameFormat)  //若新設定的轉檔格式與舊的不同，則停止轉檔
                {
                    FormatFactory.GenerateNewFormat(_settingModel.Format).StopAll();
                }
                eDoseType doseType = page1VM.MultiChecked ? eDoseType.餐包 : eDoseType.種包;
                ePackMode packMode = page1VM.FilterAdminCodeChecked ? ePackMode.過濾特殊 : page1VM.UseAdminCodeChecked ? ePackMode.使用特殊 : ePackMode.正常;
                List<ETCInfo> etcInfo = new List<ETCInfo>();
                foreach (var v in page3VM.ETCData)
                {
                    etcInfo.Add(new ETCInfo()
                    {
                        ETCIndex = v.ETCIndex,
                        PrescriptionParameterIndex = v.PrescriptionParameterIndex,
                        Format = v.Format
                    });
                }
                SettingJsonModel model = _settingModel;
                model.FileExtensionName = page2VM.FileExtensionName;
                model.Format = (eFormat)page1VM.FormatIndex;
                model.Speed = Convert.ToInt32(page1VM.SearchFrequency);
                model.PackMode = packMode;
                model.NeedToFilterAdminCode = page1VM.NeedToFilterAdminCodeList.ToList();
                model.ExtraRandom = page1VM.Random.ToList();
                model.DoseType = doseType;
                model.OutputSpecialAdminCode = page1VM.OutputSpecialAdminCode;
                model.CrossDayAdminCode = page1VM.AdminCodeOfCrossDay;
                model.NeedToFilterMedicineCode = page1VM.NeedToFilterMedicineCodeList.ToList();
                model.UseStatAndBatchOption = page2VM.UseStatAndBatchOptionChecked;
                model.MinimizeWindowWhenProgramStart = page2VM.MinimizeWindowWhenProgramStartChecked;
                model.ShowCloseAndMinimizeButton = page2VM.ShowCloseAndMinimizeButtonChecked;
                model.ShowXY = page2VM.ShowXYChecked;
                model.FilterMedicineCode = page2VM.FilterMedicineCodeChecked;
                model.OnlyCanisterIn = page2VM.OnlyCanisterInChecked;
                model.WhenCompeletedMoveFile = page2VM.WhenCompeletedMoveFileChecked;
                model.WhenCompeletedStop = page2VM.WhenCompeletedStopChecked;
                model.IgnoreAdminCodeIfNotInOnCube = page2VM.IgnoreAdminCodeIfNotInOnCubeChecked;
                model.ETCData = etcInfo;
                Setting.Save(model);
                if (!isSameFormat)  //若新設定的轉檔格式與舊的不同，則產生對應的格式
                {
                    Messenger.Send(new CommandMessage(), nameof(eCommandCollection.CreateNewFormat));
                }
                MsgCollection.ShowDialog("儲存完成", "成功", PackIconKind.Information, ColorProvider.GetSolidColorBrush(eColor.RoyalBlue));
            }
            catch (Exception ex)
            {
                LogService.Exception(ex);
                MsgCollection.ShowDialog(ex.ToString(), "錯誤", PackIconKind.Error, ColorProvider.GetSolidColorBrush(eColor.Red));
            }
        }

        public void CancelFunc()
        {
            if (MessageHelper.Question("是否確定取消本次修改?") == System.Windows.Forms.DialogResult.No)
            {
                return;
            }
            _page1VM.ResetModel();
            _page2VM.ResetModel();
            _page3VM.ResetModel();
        }

        protected override void OnDeactivated()
        {
            WeakReferenceMessenger.Default.Unregister<HasErrorsRequestMessage, string>(_page1VM, nameof(SettingPage1ViewModel));
            WeakReferenceMessenger.Default.Unregister<HasErrorsRequestMessage, string>(_page2VM, nameof(SettingPage2ViewModel));

            base.OnDeactivated();
        }

        private bool HasAnyErrorsMessage()
        {
            if (Messenger.Send(new HasErrorsRequestMessage(), nameof(SettingPage1ViewModel)))
            {
                return true;
            }
            if (Messenger.Send(new HasErrorsRequestMessage(), nameof(SettingPage2ViewModel)))
            {
                return true;
            }
            return false;
        }
    }
}