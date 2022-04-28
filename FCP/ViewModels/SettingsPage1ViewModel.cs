using System;
using System.Collections.Generic;
using System.Linq;
using FCP.Core;
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

namespace FCP.ViewModels
{
    class SettingsPage1ViewModel : ViewModelBase
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
        public Action SelectAllFilterAdminCode { get; set; }
        public Action FocusFilterAdminCode { get; set; }
        public Action SelectAllFilterMedicineCode { get; set; }
        public Action FocusFilterMedicineCode { get; set; }
        public Action RefreshRandomDataGridView { get; set; }
        public Action RefreshFilterMedicineCodeComboBox { get; set; }
        private SettingPage1Model _model;
        private SettingModel _settingModel;

        public SettingsPage1ViewModel()
        {
            _settingModel = SettingFactory.GenerateSettingModel();
            _model = ModelsFactory.GenerateSettingPage1Model();
            NormalPack = new RelayCommand(() => NormalPackFunc());
            FilterAdminCode = new RelayCommand(() => FilterAdminCodeFunc());
            UseAdminCode = new RelayCommand(() => UseAdminCodeFunc());
            AddFilterMedicineCode = new RelayCommand(() => AddFilterMedicineCodeFunc());
            RemoveFilterMedicineCode = new RelayCommand(() => RemoveFilterMedicineCodeFunc());
            AddFilterAdminCode = new RelayCommand(() => AddFilterAdminCodeFunc());
            RemoveFilterAdminCode = new RelayCommand(() => RemoveFilterAdminCodeFunc());
            AddRandom = new RelayCommand(() => AddRandomFunc());
            RemoveRandom = new RelayCommand(() => RemoveRandomFunc());

            Init();
        }

        public int SearchFrequency
        {
            get
            {
                Console.WriteLine($"get {_model.SearchFrequency}");
                return _model.SearchFrequency;
                //if (int.TryParse(_model.SearchFrequency.ToString(), out int newValue))
                //{
                //    return _model.SearchFrequency;
                //}
                //else
                //{
                //    return 0;
                //}
            }
            set
            {
                Console.WriteLine($"set {_model.SearchFrequency}");
                _model.SearchFrequency = value;
                //if (int.TryParse(value.ToString(), out int newValue))
                //{
                //    _model.SearchFrequency = value;
                //}
                //else
                //{
                //    _model.SearchFrequency = 0;
                //}
            }
        }

        public ObservableCollection<string> Mode
        {
            get => _model.Mode;
            set => _model.Mode = value;
        }

        public int FormatIndex
        {
            get => _model.FormatIndex;
            set => _model.FormatIndex = value;
        }

        public bool NormalPackChecked
        {
            //get => _Model.NormalPackChecked;
            get
            {
                return _model.NormalPackChecked;
            }
            set
            {
                _model.NormalPackChecked = value;
            }
        }

        public bool FilterAdminCodeChecked
        {
            get => _model.FilterAdminCodeChecked;
            set
            {
                _model.FilterAdminCodeChecked = value;
            }
        }

        public bool UseAdminCodeChecked
        {
            get => _model.UseAdminCodeChecked;
            set
            {
                _model.UseAdminCodeChecked = value;
            }
        }

        public Visibility PackMode
        {
            get => _model.PackModeVisible;
            set
            {
                _model.PackModeVisible = value;
            }
        }

        public string AdminCode
        {
            get => _model.AdminCode;
            set => _model.AdminCode = value.Trim();
        }

        public ObservableCollection<string> FilterAdminCodeList
        {
            get => _model.FilterAdminCodeList;
            set
            {
                _model.FilterAdminCodeList = value;
            }
        }

        public int FilterAdminCodeIndex
        {
            get => _model.FilerAdminCodeIndex;
            set => _model.FilerAdminCodeIndex = value;
        }

        public ObservableCollection<RandomInfo> Random
        {
            get => _model.Random;
            set
            {
                _model.Random = value;
            }
        }

        public int RandomIndex
        {
            get => _model.RandomIndex;
            set
            {
                _model.RandomIndex = value;
            }
        }
        public bool MultiChecked
        {
            get => _model.MultiChecked;
            set
            {
                _model.MultiChecked = value;
            }
        }

        public bool CombiChecked
        {
            get => _model.CombiChecked;
            set
            {
                _model.CombiChecked = value;
            }
        }

        public string OutputSpecialAdminCode
        {
            get => _model.OutputSpecialAdminCode;
            set => _model.OutputSpecialAdminCode = value;
        }

        public string CutTime
        {
            get => _model.CutTime;
            set
            {
                _model.CutTime = value.Trim();
            }
        }

        public string AdminCodeOfCrossDay
        {
            get => _model.AdminCodeOfCrossDay;
            set
            {
                _model.AdminCodeOfCrossDay = value.Trim();
            }
        }

        public string MedicineCode
        {
            get => _model.MedicineCode;
            set
            {
                _model.MedicineCode = value.Trim();
            }
        }

        public ObservableCollection<string> FilterMedicineCodeList
        {
            get => _model.FilterMedicineCodeList;
            set
            {
                _model.FilterMedicineCodeList = value;
            }
        }

        public int FilterMedicineCodeIndex
        {
            get => _model.FilterMedicineCodeIndex;
            set
            {
                _model.FilterMedicineCodeIndex = value;
            }
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
                Message.Show("藥品代碼欄位為空白，請重新確認", "空白", PackIconKind.Error, KindColors.Error);
                FocusFilterMedicineCode();
                return;
            }
            if (FilterMedicineCodeList.Contains(MedicineCode))
            {
                Message.Show($"該藥品代碼 {MedicineCode} 已建立，請重新確認", "重複", PackIconKind.Error, KindColors.Error);
                FocusFilterMedicineCode();
                SelectAllFilterMedicineCode();
                return;
            }
            FilterMedicineCodeList.Add(MedicineCode);
            FilterMedicineCodeList = ListHelper.ToObservableCollection(FilterMedicineCodeList.OrderBy(x => x).ToList());
            FilterMedicineCodeIndex = 0;
            MedicineCode = string.Empty;
            FocusFilterMedicineCode();
        }

        public void RemoveFilterMedicineCodeFunc()
        {
            if (FilterMedicineCodeList.Count == 0 || FilterMedicineCodeIndex == -1)
            {
                return;
            }
            FilterMedicineCodeList.RemoveAt(FilterMedicineCodeIndex);
            FilterMedicineCodeIndex = FilterMedicineCodeList.Count == 0 ? -1 : 0;
            RefreshFilterMedicineCodeComboBox();
        }

        public void AddFilterAdminCodeFunc()
        {
            if (AdminCode.Length == 0)
            {
                Message.Show("頻率欄位為空白，請重新確認", "空白", PackIconKind.Error, KindColors.Error);
                FocusFilterAdminCode();
                return;
            }
            if (FilterAdminCodeList.Contains(AdminCode))
            {
                Message.Show($"該頻率 {AdminCode} 已建立，請重新確認", "重複", PackIconKind.Error, KindColors.Error);
                FocusFilterAdminCode();
                SelectAllFilterAdminCode();
                return;
            }
            FilterAdminCodeList.Add(AdminCode);
            FilterAdminCodeList = ListHelper.ToObservableCollection(FilterAdminCodeList.OrderBy(x => x).ToList());
            FilterAdminCodeIndex = 0;
            AdminCode = string.Empty;
            FocusFilterAdminCode();
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
            CutTime = _settingModel.CutTime;
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
            RefreshRandomDataGridView();
        }
    }
}
