using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using FCP.Models;
using System.Windows;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using Helper;
using FCP.src.Enum;
using FCP.src.Factory;
using FCP.src;
using FCP.src.Factory.Models;
using System.Text;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using FCP.Providers;
using FCP.src.MessageManager;
using Microsoft.Toolkit.Mvvm.Messaging;
using System.ComponentModel.DataAnnotations;
using FCP.src.MessageManager.Request;

namespace FCP.ViewModels
{
    class SettingPage1ViewModel : ObservableValidator
    {
        public SettingPage1ViewModel()
        {
            _model = ModelsFactory.GenerateSettingPage1Model();
            _settingModel = ModelsFactory.GenerateSettingModel();

            NormalPack = new RelayCommand(NormalPackFunc);
            FilterAdminCode = new RelayCommand(FilterAdminCodeFunc);
            UseAdminCode = new RelayCommand(UseAdminCodeFunc);
            AddFilterMedicineCode = new RelayCommand(AddFilterMedicineCodeFunc);
            RemoveFilterMedicineCode = new RelayCommand(RemoveFilterMedicineCodeFunc);
            AddFilterAdminCode = new RelayCommand(AddFilterAdminCodeFunc);
            RemoveFilterAdminCode = new RelayCommand(RemoveFilterAdminCodeFunc);
            AddRandom = new RelayCommand(AddRandomFunc);
            RemoveRandom = new RelayCommand(RemoveRandomFunc);

            Init();
        }

        public ICommand NormalPack { get; set; }
        public ICommand FilterAdminCode { get; set; }
        public ICommand UseAdminCode { get; set; }
        public ICommand AddFilterMedicineCode { get; set; }
        public ICommand RemoveFilterMedicineCode { get; set; }
        public ICommand AddFilterAdminCode { get; set; }
        public ICommand RemoveFilterAdminCode { get; set; }
        public ICommand AddRandom { get; set; }
        public ICommand RemoveRandom { get; set; }
        private SettingPage1Model _model;
        private SettingJsonModel _settingModel;

        [Required]
        [RegularExpression("[0-9]+", ErrorMessage = "不可輸入除 0-9 的數字")]
        public int SearchFrequency
        {
            get => _model.SearchFrequency;
            set => SetProperty(_model.SearchFrequency, value, _model, (model, _value) => model.SearchFrequency = _value, true);
        }

        public ObservableCollection<string> Mode
        {
            get => _model.Mode;
            set => SetProperty(_model.Mode, value, _model, (model, _value) => model.Mode = _value);
        }

        public int FormatIndex
        {
            get => _model.FormatIndex;
            set => SetProperty(_model.FormatIndex, value, _model, (model, _value) => model.FormatIndex = _value);
        }

        public bool NormalPackChecked
        {
            get => _model.NormalPackChecked;
            set => SetProperty(_model.NormalPackChecked, value, _model, (model, _value) => model.NormalPackChecked = _value);
        }

        public bool FilterAdminCodeChecked
        {
            get => _model.FilterAdminCodeChecked;
            set => SetProperty(_model.FilterAdminCodeChecked, value, _model, (model, _value) => model.FilterAdminCodeChecked = _value);
        }

        public bool UseAdminCodeChecked
        {
            get => _model.UseAdminCodeChecked;
            set => SetProperty(_model.UseAdminCodeChecked, value, _model, (model, _value) => model.UseAdminCodeChecked = _value);
        }

        public Visibility PackMode
        {
            get => _model.PackModeVisible;
            set => SetProperty(_model.PackModeVisible, value, _model, (model, _value) => model.PackModeVisible = _value);
        }

        public string AdminCode
        {
            get => _model.AdminCode;
            set => SetProperty(_model.AdminCode, value.Trim(), _model, (model, _value) => model.AdminCode = _value);
        }

        public ObservableCollection<string> NeedToFilterAdminCodeList
        {
            get => _model.NeedToFilterAdminCodeList;
            set => SetProperty(_model.NeedToFilterAdminCodeList, value, _model, (model, _value) => model.NeedToFilterAdminCodeList = _value);
        }

        public int NeedToFilterAdminCodeIndex
        {
            get => _model.NeedToFilerAdminCodeIndex;
            set => SetProperty(_model.NeedToFilerAdminCodeIndex, value, _model, (model, _value) => model.NeedToFilerAdminCodeIndex = _value);
        }

        public ObservableCollection<RandomInfo> Random
        {
            get => _model.Random;
            set => SetProperty(_model.Random, value, _model, (model, _value) => model.Random = _value);
        }

        public int RandomIndex
        {
            get => _model.RandomIndex;
            set => SetProperty(_model.RandomIndex, value, _model, (model, _value) => model.RandomIndex = _value);
        }
        public bool MultiChecked
        {
            get => _model.MultiChecked;
            set => SetProperty(_model.MultiChecked, value, _model, (model, _value) => model.MultiChecked = _value);
        }

        public bool CombiChecked
        {
            get => _model.CombiChecked;
            set => SetProperty(_model.CombiChecked, value, _model, (model, _value) => model.CombiChecked = _value);
        }

        public string OutputSpecialAdminCode
        {
            get => _model.OutputSpecialAdminCode;
            set => SetProperty(_model.OutputSpecialAdminCode, value, _model, (model, _value) => model.OutputSpecialAdminCode = _value);
        }

        public string AdminCodeOfCrossDay
        {
            get => _model.AdminCodeOfCrossDay;
            set => SetProperty(_model.AdminCodeOfCrossDay, value.Trim(), _model, (model, _value) => model.AdminCodeOfCrossDay = _value);
        }

        public string MedicineCode
        {
            get => _model.MedicineCode;
            set => SetProperty(_model.MedicineCode, value.Trim(), _model, (model, _value) => model.MedicineCode = _value);
        }

        public ObservableCollection<string> NeedToFilterMedicineCodeList
        {
            get => _model.NeedToFilterMedicineCodeList;
            set => SetProperty(_model.NeedToFilterMedicineCodeList, value, _model, (model, _value) => model.NeedToFilterMedicineCodeList = _value);
        }

        public int NeedToFilterMedicineCodeIndex
        {
            get => _model.NeedToFilterMedicineCodeIndex;
            set => SetProperty(_model.NeedToFilterMedicineCodeIndex, value, _model, (model, _value) => model.NeedToFilterMedicineCodeIndex = _value);
        }

        public void ResetModel()
        {
            WeakReferenceMessenger.Default.Unregister<HasErrorsRequestMessage, string>(this, nameof(SettingPage1ViewModel));
            Init();
        }

        public void NormalPackFunc()
        {
            NormalPackChecked = true;
            FilterAdminCodeChecked = false;
            UseAdminCodeChecked = false;
            PackMode = Visibility.Hidden;
        }

        public void FilterAdminCodeFunc()
        {
            NormalPackChecked = false;
            FilterAdminCodeChecked = true;
            UseAdminCodeChecked = false;
            PackMode = Visibility.Visible;
        }

        public void UseAdminCodeFunc()
        {
            NormalPackChecked = false;
            FilterAdminCodeChecked = false;
            UseAdminCodeChecked = true;
            PackMode = Visibility.Visible;
        }

        public void AddFilterMedicineCodeFunc()
        {
            if (MedicineCode.Length == 0)
            {
                MsgCollection.ShowDialog("藥品代碼欄位為空白，請重新確認", "空白", PackIconKind.Error, ColorProvider.GetSolidColorBrush(eColor.Red));
                FocusedFilterMedicineCode();
                return;
            }
            if (NeedToFilterMedicineCodeList.Contains(MedicineCode))
            {
                MsgCollection.ShowDialog($"該藥品代碼 {MedicineCode} 已建立，請重新確認", "重複", PackIconKind.Error, ColorProvider.GetSolidColorBrush(eColor.Red));
                FocusedFilterMedicineCode();
                SelectedAllMedicineCode();
                return;
            }
            NeedToFilterMedicineCodeList.Add(MedicineCode);
            NeedToFilterMedicineCodeList = ListHelper.ToObservableCollection(NeedToFilterMedicineCodeList.OrderBy(x => x).ToList());
            NeedToFilterMedicineCodeIndex = 0;
            MedicineCode = string.Empty;
            FocusedFilterMedicineCode();
        }

        public void RemoveFilterMedicineCodeFunc()
        {
            if (NeedToFilterMedicineCodeList.Count == 0 || NeedToFilterMedicineCodeIndex == -1)
            {
                return;
            }
            NeedToFilterMedicineCodeList.RemoveAt(NeedToFilterMedicineCodeIndex);
            NeedToFilterMedicineCodeIndex = NeedToFilterMedicineCodeList.Count == 0 ? -1 : 0;
            RefreshedFilterMedicineCode();
        }

        public void AddFilterAdminCodeFunc()
        {
            if (AdminCode.Length == 0)
            {
                MsgCollection.ShowDialog("頻率欄位為空白，請重新確認", "空白", PackIconKind.Error, ColorProvider.GetSolidColorBrush(eColor.Red));
                FocusedFilterAdminCode();

                return;
            }
            if (NeedToFilterAdminCodeList.Contains(AdminCode))
            {
                MsgCollection.ShowDialog($"該頻率 {AdminCode} 已建立，請重新確認", "重複", PackIconKind.Error, ColorProvider.GetSolidColorBrush(eColor.Red));
                FocusedFilterAdminCode();
                SelectedAllAdminCode();
                return;
            }
            NeedToFilterAdminCodeList.Add(AdminCode);
            NeedToFilterAdminCodeList = ListHelper.ToObservableCollection(NeedToFilterAdminCodeList.OrderBy(x => x).ToList());
            NeedToFilterAdminCodeIndex = 0;
            AdminCode = string.Empty;
            FocusedFilterAdminCode();
        }

        public void RemoveFilterAdminCodeFunc()
        {
            if (NeedToFilterAdminCodeList.Count == 0 || NeedToFilterAdminCodeIndex == -1)
            {
                return;
            }
            NeedToFilterAdminCodeList.RemoveAt(NeedToFilterAdminCodeIndex);
            NeedToFilterAdminCodeIndex = NeedToFilterAdminCodeList.Count == 0 ? -1 : 0;
        }

        private void Init()
        {
            WeakReferenceMessenger.Default.Register<HasErrorsRequestMessage, string>(this, nameof(SettingPage1ViewModel), (r, m) => m.Reply(this.HasErrors));
            SearchFrequency = _settingModel.Speed;
            MultiChecked = _settingModel.DoseType == eDoseType.餐包;
            CombiChecked = _settingModel.DoseType == eDoseType.種包;
            OutputSpecialAdminCode = _settingModel.OutputSpecialAdminCode;
            AdminCodeOfCrossDay = _settingModel.CrossDayAdminCode;
            Mode = EnumHelper.ToObservableCollection<eFormat>();
            FormatIndex = (int)_settingModel.Format;
            NeedToFilterAdminCodeList = ListHelper.ToObservableCollection(_settingModel.NeedToFilterAdminCode);
            Random = ListHelper.ToObservableCollection(_settingModel.ExtraRandom);
            NeedToFilterMedicineCodeList = ListHelper.ToObservableCollection(_settingModel.NeedToFilterMedicineCode);

            if (_settingModel.PackMode == ePackMode.正常)
            {
                NormalPackChecked = true;
                NormalPackFunc();
            }
            else if (_settingModel.PackMode == ePackMode.過濾特殊)
            {
                FilterAdminCodeChecked = true;
                FilterAdminCodeFunc();
            }
            else
            {
                UseAdminCodeChecked = true;
                UseAdminCodeFunc();
            }
        }

        private void AddRandomFunc()
        {
            Random.Add(new RandomInfo()
            {
                No = $"{Random.Count}",
                JVServer = string.Empty,
                OnCube = string.Empty
            });
        }

        private void RemoveRandomFunc()
        {
            if (Random.Count == 0 || RandomIndex == -1)
            {
                return;
            }
            Random.RemoveAt(RandomIndex);
            RandomIndex = RandomIndex == 0 ? -1 : 0;
            Random.ToList().ForEach(x => x.No = Random.IndexOf(x).ToString());  //重新編排No.
            RefreshedRandomataGridView();
        }

        private void FocusedFilterAdminCode()
        {
            WeakReferenceMessenger.Default.Send(new View_FocusedElementMessage(), "Txt_FilterAdminCode");
        }

        private void FocusedFilterMedicineCode()
        {
            WeakReferenceMessenger.Default.Send(new View_FocusedElementMessage(), "Txt_FilterMedicineCode");
        }

        private void SelectedAllAdminCode()
        {
            WeakReferenceMessenger.Default.Send(new View_SelectedAllMessage(), "Txt_FilterAdminCode");
        }

        private void SelectedAllMedicineCode()
        {
            WeakReferenceMessenger.Default.Send(new View_SelectedAllMessage(), "Txt_FilterMedicineCode");
        }

        private void RefreshedRandomataGridView()
        {
            WeakReferenceMessenger.Default.Send(new View_RefresedhElementMessage(), "Dg_RandomSetting");
        }

        private void RefreshedFilterMedicineCode()
        {
            WeakReferenceMessenger.Default.Send(new View_RefresedhElementMessage(), "Cbo_FilterMedicineCode");
        }
    }
}
