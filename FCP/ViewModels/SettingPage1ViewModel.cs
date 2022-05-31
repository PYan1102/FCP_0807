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
using FCP.src.Dictionary;
using FCP.src.MessageManager;

namespace FCP.ViewModels
{
    class SettingPage1ViewModel : ObservableRecipient
    {
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

        public int SearchFrequency
        {
            get => _model.SearchFrequency;
            set => SetProperty(_model.SearchFrequency, value, _model, (model, _value) => model.SearchFrequency = _value);
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

        public ObservableCollection<string> FilterAdminCodeList
        {
            get => _model.FilterAdminCodeList;
            set => SetProperty(_model.FilterAdminCodeList, value, _model, (model, _value) => model.FilterAdminCodeList = _value);
        }

        public int FilterAdminCodeIndex
        {
            get => _model.FilerAdminCodeIndex;
            set => SetProperty(_model.FilerAdminCodeIndex, value, _model, (model, _value) => model.FilerAdminCodeIndex = _value);
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

        public ObservableCollection<string> FilterMedicineCodeList
        {
            get => _model.FilterMedicineCodeList;
            set => SetProperty(_model.FilterMedicineCodeList, value, _model, (model, _value) => model.FilterMedicineCodeList = _value);
        }

        public int FilterMedicineCodeIndex
        {
            get => _model.FilterMedicineCodeIndex;
            set => SetProperty(_model.FilterMedicineCodeIndex, value, _model, (model, _value) => model.FilterMedicineCodeIndex = _value);
        }

        public void ResetModel()
        {
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
                MsgCollection.ShowDialog("藥品代碼欄位為空白，請重新確認", "空白", PackIconKind.Error, dColor.GetSolidColorBrush(eColor.Red));
                FocusedFilterMedicineCode();
                return;
            }
            if (FilterMedicineCodeList.Contains(MedicineCode))
            {
                MsgCollection.ShowDialog($"該藥品代碼 {MedicineCode} 已建立，請重新確認", "重複", PackIconKind.Error, dColor.GetSolidColorBrush(eColor.Red));
                FocusedFilterMedicineCode();
                SelectedAllMedicineCode();
                return;
            }
            FilterMedicineCodeList.Add(MedicineCode);
            FilterMedicineCodeList = ListHelper.ToObservableCollection(FilterMedicineCodeList.OrderBy(x => x).ToList());
            FilterMedicineCodeIndex = 0;
            MedicineCode = string.Empty;
            FocusedFilterMedicineCode();
        }

        public void RemoveFilterMedicineCodeFunc()
        {
            if (FilterMedicineCodeList.Count == 0 || FilterMedicineCodeIndex == -1)
            {
                return;
            }
            FilterMedicineCodeList.RemoveAt(FilterMedicineCodeIndex);
            FilterMedicineCodeIndex = FilterMedicineCodeList.Count == 0 ? -1 : 0;
            RefreshedFilterMedicineCode();
        }

        public void AddFilterAdminCodeFunc()
        {
            if (AdminCode.Length == 0)
            {
                MsgCollection.ShowDialog("頻率欄位為空白，請重新確認", "空白", PackIconKind.Error, dColor.GetSolidColorBrush(eColor.Red));
                FocusedFilterAdminCode();

                return;
            }
            if (FilterAdminCodeList.Contains(AdminCode))
            {
                MsgCollection.ShowDialog($"該頻率 {AdminCode} 已建立，請重新確認", "重複", PackIconKind.Error, dColor.GetSolidColorBrush(eColor.Red));
                FocusedFilterAdminCode();
                SelectedAllAdminCode();
                return;
            }
            FilterAdminCodeList.Add(AdminCode);
            FilterAdminCodeList = ListHelper.ToObservableCollection(FilterAdminCodeList.OrderBy(x => x).ToList());
            FilterAdminCodeIndex = 0;
            AdminCode = string.Empty;
            FocusedFilterAdminCode();
        }

        public void RemoveFilterAdminCodeFunc()
        {
            if (FilterAdminCodeList.Count == 0 || FilterAdminCodeIndex == -1)
            {
                return;
            }
            FilterAdminCodeList.RemoveAt(FilterAdminCodeIndex);
            FilterAdminCodeIndex = FilterAdminCodeList.Count == 0 ? -1 : 0;
        }

        private void Init()
        {
            SearchFrequency = _settingModel.Speed;
            MultiChecked = _settingModel.DoseType == eDoseType.餐包;
            CombiChecked = _settingModel.DoseType == eDoseType.種包;
            OutputSpecialAdminCode = _settingModel.OutputSpecialAdminCode;
            AdminCodeOfCrossDay = _settingModel.CrossDayAdminCode;
            Mode = EnumHelper.ToObservableCollection<eFormat>();
            FormatIndex = (int)_settingModel.Format;
            FilterAdminCodeList = ListHelper.ToObservableCollection(_settingModel.FilterAdminCode);
            Random = ListHelper.ToObservableCollection(_settingModel.ExtraRandom);
            FilterMedicineCodeList = ListHelper.ToObservableCollection(_settingModel.FilterMedicineCode);

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
            Random.ToList().ForEach(x => x.No = Random.IndexOf(x).ToString());  //重新編排No
            RefreshedRandomataGridView();
        }

        private void FocusedFilterAdminCode()
        {
            Messenger.Send(new View_FocusedElementMessage(), "Txt_FilterAdminCode");
        }

        private void FocusedFilterMedicineCode()
        {
            Messenger.Send(new View_FocusedElementMessage(), "Txt_FilterMedicineCode");
        }

        private void SelectedAllAdminCode()
        {
            Messenger.Send(new View_SelectedAllMessage(), "Txt_FilterAdminCode");
        }

        private void SelectedAllMedicineCode()
        {
            Messenger.Send(new View_SelectedAllMessage(), "Txt_FilterMedicineCode");
        }

        private void RefreshedRandomataGridView()
        {
            Messenger.Send(new View_RefresedhElementMessage(), "Dg_RandomSetting");
        }

        private void RefreshedFilterMedicineCode()
        {
            Messenger.Send(new View_RefresedhElementMessage(), "Cbo_FilterMedicineCode");
        }
    }
}
