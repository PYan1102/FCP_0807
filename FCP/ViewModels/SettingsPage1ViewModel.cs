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
        private SettingsPage1Model _Model;
        private SettingsModel _SettingsModel;

        public SettingsPage1ViewModel()
        {
            _SettingsModel = SettingsFactory.GenerateSettingsModel();
            _Model = ModelsFactory.GenerateSettingsPage1Model();
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
            get => _Model.SearchFrequency;
            set => _Model.SearchFrequency = value;
        }

        public ObservableCollection<string> Mode
        {
            get => _Model.Mode;
            set => _Model.Mode = value;
        }

        public int ModeIndex
        {
            get => _Model.ModeIndex;
            set => _Model.ModeIndex = value;
        }

        public bool NormalPackChecked
        {
            //get => _Model.NormalPackChecked;
            get
            {
                return _Model.NormalPackChecked;
            }
            set
            {
                _Model.NormalPackChecked = value;
            }
        }

        public bool FilterAdminCodeChecked
        {
            get => _Model.FilterAdminCodeChecked;
            set
            {
                _Model.FilterAdminCodeChecked = value;
            }
        }

        public bool UseAdminCodeChecked
        {
            get => _Model.UseAdminCodeChecked;
            set
            {
                _Model.UseAdminCodeChecked = value;
            }
        }

        public Visibility PackMode
        {
            get => _Model.PackModeVisible;
            set
            {
                _Model.PackModeVisible = value;
            }
        }

        public string AdminCode
        {
            get => _Model.AdminCode;
            set
            {
                _Model.AdminCode = value.Trim();
            }
        }

        public ObservableCollection<string> FilterAdminCodeList
        {
            get => _Model.FilterAdminCodeList;
            set
            {
                _Model.FilterAdminCodeList = value;
            }
        }

        public int FilterAdminCodeIndex
        {
            get => _Model.FilerAdminCodeIndex;
            set => _Model.FilerAdminCodeIndex = value;
        }

        public ObservableCollection<RandomInfo> Random
        {
            get => _Model.Random;
            set
            {
                _Model.Random = value;
            }
        }

        public int RandomIndex
        {
            get => _Model.RandomIndex;
            set
            {
                _Model.RandomIndex = value;
            }
        }
        public bool MultiChecked
        {
            get => _Model.MultiChecked;
            set
            {
                _Model.MultiChecked = value;
            }
        }

        public bool CombiChecked
        {
            get => _Model.CombiChecked;
            set
            {
                _Model.CombiChecked = value;
            }
        }

        public string OutputSpecialAdminCode
        {
            get => _Model.OutputSpecialAdminCode;
            set
            {
                _Model.OutputSpecialAdminCode = value;
            }
        }

        public string CutTime
        {
            get => _Model.CutTime;
            set
            {
                _Model.CutTime = value.Trim();
            }
        }

        public string AdminCodeOfCrossDay
        {
            get => _Model.AdminCodeOfCrossDay;
            set
            {
                _Model.AdminCodeOfCrossDay = value.Trim();
            }
        }

        public string MedicineCode
        {
            get => _Model.MedicineCode;
            set
            {
                _Model.MedicineCode = value.Trim();
            }
        }

        public ObservableCollection<string> FilterMedicineCodeList
        {
            get => _Model.FilterMedicineCodeList;
            set
            {
                _Model.FilterMedicineCodeList = value;
            }
        }

        public int FilterMedicineCodeIndex
        {
            get => _Model.FilterMedicineCodeIndex;
            set
            {
                _Model.FilterMedicineCodeIndex = value;
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
            SearchFrequency = _SettingsModel.Speed;
            MultiChecked = _SettingsModel.DoseType == eDoseType.餐包;
            CombiChecked = _SettingsModel.DoseType == eDoseType.種包;
            OutputSpecialAdminCode = string.Empty;
            _SettingsModel.OutputSpecialAdminCode.ForEach(x => OutputSpecialAdminCode += $"{x},");
            OutputSpecialAdminCode = OutputSpecialAdminCode.TrimEnd(',');
            CutTime = _SettingsModel.CutTime;
            AdminCodeOfCrossDay = string.Empty;
            _SettingsModel.CrossDayAdminCode.ForEach(x => AdminCodeOfCrossDay += $"{x},");
            AdminCodeOfCrossDay = AdminCodeOfCrossDay.TrimEnd(',');
            Mode = EnumHelper.ToObservableCollection<eFormat>();
            ModeIndex = (int)_SettingsModel.Mode;
            FilterAdminCodeList = ListHelper.ToObservableCollection(_SettingsModel.FilterAdminCode);
            List<string> randomList = _SettingsModel.ExtraRandom.Split('|').ToList();
            randomList.RemoveAll(x => x.Length == 0);
            Random.Clear();
            foreach (var v in randomList)
            {
                var random = v.Split(':');
                Random.Add(new RandomInfo()
                {
                    No = random[0],
                    JVServer = random[1],
                    OnCube = random[2]
                });
            }
            FilterMedicineCodeList = ListHelper.ToObservableCollection(_SettingsModel.FilterMedicineCode);

            if (_SettingsModel.PackMode == ePackMode.正常)
            {
                NormalPackChecked = true;
                NormalPackFunc();
            }
            else if (_SettingsModel.PackMode == ePackMode.過濾特殊)
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
            Random.ToList().ForEach(x => x.No = Random.IndexOf(x).ToString());
            RefreshRandomDataGridView();
        }
    }

    public class RandomInfo
    {
        public string No { get; set; }
        public string JVServer { get; set; }
        public string OnCube { get; set; }
    }
}
