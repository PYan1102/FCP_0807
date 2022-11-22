using System;
using System.Windows;
using System.Text;
using System.Threading.Tasks;
using FCP.src.Factory;
using System.Windows.Input;
using FCP.Models;
using FCP.src.Enum;
using MaterialDesignThemes.Wpf;
using System.Windows.Media;
using System.Windows.Forms;
using FCP.Services;
using Helper;
using System.Diagnostics;
using FCP.src;
using FCP.src.Factory.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using FCP.src.MessageManager;
using Microsoft.Extensions.DependencyInjection;
using FCP.Providers;
using FCP.src.MessageManager.Change;
using FCP.src.MessageManager.Request;
using SqlHelper;

namespace FCP.ViewModels
{
    class MainWindowViewModel : ObservableRecipient
    {
        public MainWindowViewModel()
        {
            _settingModel = ModelsFactory.GenerateSettingModel();

            ShowAdvancedSettings = new RelayCommand(ShowAdvancedSettingsFunc, CanStartConverterOrShowAdvancedSettings);
            _model = new MainWindowModel();
            Loaded = new RelayCommand(LoadedFuncAsync);
            Close = new RelayCommand(() => Environment.Exit(0));
            MinimumWindow = new RelayCommand(() => Visibility = Visibility.Hidden);
            OPD = new RelayCommand(OPDFunc, CanStartConverterOrShowAdvancedSettings);
            UD = new RelayCommand(UDFunc, CanStartConverterOrShowAdvancedSettings);
            Stop = new RelayCommand(StopFunc);
            Save = new RelayCommand(SaveFunc, CanStartConverterOrShowAdvancedSettings);
            SwitchWindow = new RelayCommand(SwitchWindowFunc, CanStartConverterOrShowAdvancedSettings);
            Activate = new RelayCommand(() => Messenger.Send(new ActivateMessage(), nameof(FCP.Views.MainWindowView)));
            SelectFolder = new RelayCommand<string>(s => SelectFolderFunc(s));
            ClearPath = new RelayCommand<string>(s => ClearPathFunc(s), s => CanStartConverterOrShowAdvancedSettings());
            OpenLog = new RelayCommand(OpenLogFunc);
            ClearProgressBox = new RelayCommand(ClearProgressBoxFunc);
        }
        public ICommand ShowAdvancedSettings { get; set; }
        public ICommand OPD { get; set; }
        public ICommand UD { get; set; }
        public ICommand Stop { get; set; }
        public ICommand Save { get; set; }
        public ICommand Close { get; set; }
        public ICommand MinimumWindow { get; set; }
        public ICommand Loaded { get; set; }
        public ICommand SwitchWindow { get; set; }
        public ICommand Activate { get; set; }
        public ICommand SelectFolder { get; set; }
        public ICommand ClearPath { get; set; }
        public ICommand OpenLog { get; set; }
        public ICommand ClearProgressBox { get; set; }

        public Visibility Visibility
        {
            get => _model.Visibility;
            set => SetProperty(_model.Visibility, value, _model, (model, _value) => model.Visibility = _value);
        }

        public bool Focusable
        {
            get => _model.Focusable;
            set => SetProperty(_model.Focusable, value, _model, (model, _value) => model.Focusable = _value);
        }

        public string WindowTitle
        {
            get => _model.WindowTitle;
            set => SetProperty(_model.WindowTitle, value, _model, (model, _value) => model.WindowTitle = _value);
        }

        public string InputDirectory1Title
        {
            get => _model.InputDirectory1Title;
            set => SetProperty(_model.InputDirectory1Title, value, _model, (model, _value) => model.InputDirectory1Title = _value);
        }

        public string InputDirectory2Title
        {
            get => _model.InputDirectory2Title;
            set => SetProperty(_model.InputDirectory2Title, value, _model, (model, _value) => model.InputDirectory2Title = _value);
        }

        public string InputDirectory3Title
        {
            get => _model.InputDirectory3Title;
            set => SetProperty(_model.InputDirectory3Title, value, _model, (model, _value) => model.InputDirectory3Title = _value);
        }

        public string InputDirectory4Title
        {
            get => _model.InputDirectory4Title;
            set => SetProperty(_model.InputDirectory4Title, value, _model, (model, _value) => model.InputDirectory4Title = _value);
        }

        public string InputDirectory5Title
        {
            get => _model.InputDirectory5Title;
            set => SetProperty(_model.InputDirectory5Title, value, _model, (model, _value) => model.InputDirectory5Title = _value);
        }

        public string InputDirectory6Title
        {
            get => _model.InputDirectory6Title;
            set => SetProperty(_model.InputDirectory6Title, value, _model, (model, _value) => model.InputDirectory6Title = _value);
        }

        public string InputDirectory1
        {
            get => _model.InputDirectory1;
            set => SetProperty(_model.InputDirectory1, value, _model, (model, _value) => model.InputDirectory1 = _value);
        }

        public string InputDirectory2
        {
            get => _model.InputDirectory2;
            set => SetProperty(_model.InputDirectory2, value, _model, (model, _value) => model.InputDirectory2 = _value);
        }

        public string InputDirectory3
        {
            get => _model.InputDirectory3;
            set => SetProperty(_model.InputDirectory3, value, _model, (model, _value) => model.InputDirectory3 = _value);
        }


        public string InputDirectory4
        {
            get => _model.InputDirectory4;
            set => SetProperty(_model.InputDirectory4, value, _model, (model, _value) => model.InputDirectory4 = _value);
        }

        public string InputDirectory5
        {
            get => _model.InputDirectory5;
            set => SetProperty(_model.InputDirectory5, value, _model, (model, _value) => model.InputDirectory5 = _value);
        }

        public string InputDirectory6
        {
            get => _model.InputDirectory6;
            set => SetProperty(_model.InputDirectory6, value, _model, (model, _value) => model.InputDirectory6 = _value);
        }

        public bool InputDirectory1Enabled
        {
            get => _model.InputDirectory1Enabled;
            set => SetProperty(_model.InputDirectory1Enabled, value, _model, (model, _value) => model.InputDirectory1Enabled = _value);
        }

        public bool InputDirectory2Enabled
        {
            get => _model.InputDirectory2Enabled;
            set => SetProperty(_model.InputDirectory2Enabled, value, _model, (model, _value) => model.InputDirectory2Enabled = _value);
        }

        public bool InputDirectory3Enabled
        {
            get => _model.InputDirectory3Enabled;
            set => SetProperty(_model.InputDirectory3Enabled, value, _model, (model, _value) => model.InputDirectory3Enabled = _value);
        }

        public bool InputDirectory4Enabled
        {
            get => _model.InputDirectory4Enabled;
            set => SetProperty(_model.InputDirectory4Enabled, value, _model, (model, _value) => model.InputDirectory4Enabled = _value);
        }

        public bool InputDirectory5Enabled
        {
            get => _model.InputDirectory5Enabled;
            set => SetProperty(_model.InputDirectory5Enabled, value, _model, (model, _value) => model.InputDirectory5Enabled = _value);
        }

        public bool InputDirectory6Enabled
        {
            get => _model.InputDirectory6Enabled;
            set => SetProperty(_model.InputDirectory6Enabled, value, _model, (model, _value) => model.InputDirectory6Enabled = _value);
        }

        public string OutputDirectoryTitle
        {
            get => _model.OutputDirectoryTitle;
            set => SetProperty(_model.OutputDirectoryTitle, value, _model, (model, _value) => model.OutputDirectoryTitle = _value);
        }

        public string OutputDirectory
        {
            get => _model.OutputDirectory;
            set => SetProperty(_model.OutputDirectory, value, _model, (model, _value) => model.OutputDirectory = _value);
        }

        public bool OutputDirectoryEnabled
        {
            get => _model.OutputDirectoryEnabled;
            set => SetProperty(_model.OutputDirectoryEnabled, value, _model, (model, _value) => model.OutputDirectoryEnabled = _value);
        }

        public Visibility UDButtonVisibility
        {
            get => _model.UDButtonVisibility;
            set => SetProperty(_model.UDButtonVisibility, value, _model, (model, _value) => model.UDButtonVisibility = _value);
        }

        public string OPDToogle1
        {
            get => _model.OPDToogle1;
            set => SetProperty(_model.OPDToogle1, value, _model, (model, _value) => model.OPDToogle1 = _value);
        }

        public string OPDToogle2
        {
            get => _model.OPDToogle2;
            set => SetProperty(_model.OPDToogle2, value, _model, (model, _value) => model.OPDToogle2 = _value);
        }

        public string OPDToogle3
        {
            get => _model.OPDToogle3;
            set => SetProperty(_model.OPDToogle3, value, _model, (model, _value) => model.OPDToogle3 = _value);
        }

        public string OPDToogle4
        {
            get => _model.OPDToogle4;
            set => SetProperty(_model.OPDToogle4, value, _model, (model, _value) => model.OPDToogle4 = _value);
        }

        public bool OPDToogle1Checked
        {
            get => _model.OPDToogle1Checked;
            set => SetProperty(_model.OPDToogle1Checked, value, _model, (model, _value) => model.OPDToogle1Checked = _value);
        }
        public bool OPDToogle2Checked
        {
            get => _model.OPDToogle2Checked;
            set => SetProperty(_model.OPDToogle2Checked, value, _model, (model, _value) => model.OPDToogle2Checked = _value);
        }

        public bool OPDToogle3Checked
        {
            get => _model.OPDToogle3Checked;
            set => SetProperty(_model.OPDToogle3Checked, value, _model, (model, _value) => model.OPDToogle3Checked = _value);
        }

        public bool OPDToogle4Checked
        {
            get => _model.OPDToogle4Checked;
            set => SetProperty(_model.OPDToogle4Checked, value, _model, (model, _value) => model.OPDToogle4Checked = _value);
        }

        public bool OPDToogle1Enabled
        {
            get => _model.OPDToogle1Enabled;
            set => SetProperty(_model.OPDToogle1Enabled, value, _model, (model, _value) => model.OPDToogle1Enabled = _value);
        }

        public bool OPDToogle2Enabled
        {
            get => _model.OPDToogle2Enabled;
            set => SetProperty(_model.OPDToogle2Enabled, value, _model, (model, _value) => model.OPDToogle2Enabled = _value);
        }

        public bool OPDToogle3Enabled
        {
            get => _model.OPDToogle3Enabled;
            set => SetProperty(_model.OPDToogle3Enabled, value, _model, (model, _value) => model.OPDToogle3Enabled = _value);
        }

        public bool OPDToogle4Enabled
        {
            get => _model.OPDToogle4Enabled;
            set => SetProperty(_model.OPDToogle4Enabled, value, _model, (model, _value) => model.OPDToogle4Enabled = _value);
        }

        public Visibility OPDToogle1Visibility
        {
            get => _model.OPDToogle1Visibility;
            set => SetProperty(_model.OPDToogle1Visibility, value, _model, (model, _value) => model.OPDToogle1Visibility = _value);
        }

        public Visibility OPDToogle2Visibility
        {
            get => _model.OPDToogle2Visibility;
            set => SetProperty(_model.OPDToogle2Visibility, value, _model, (model, _value) => model.OPDToogle2Visibility = _value);
        }

        public Visibility OPDToogle3Visibility
        {
            get => _model.OPDToogle3Visibility;
            set => SetProperty(_model.OPDToogle3Visibility, value, _model, (model, _value) => model.OPDToogle3Visibility = _value);
        }

        public Visibility OPDToogle4Visibility
        {
            get => _model.OPDToogle4Visibility;
            set => SetProperty(_model.OPDToogle4Visibility, value, _model, (model, _value) => model.OPDToogle4Visibility = _value);
        }

        public bool OPDEnabled
        {
            get => _model.OPDEnabled;
            set => SetProperty(_model.OPDEnabled, value, _model, (model, _value) => model.OPDEnabled = _value);
        }

        public bool UDEnabled
        {
            get => _model.UDEnabled;
            set => SetProperty(_model.UDEnabled, value, _model, (model, _value) => model.UDEnabled = _value);
        }

        public bool StopEnabled
        {
            get => _model.StopEnabled;
            set => SetProperty(_model.StopEnabled, value, _model, (model, _value) => model.StopEnabled = _value);
        }

        public bool SaveEnabled
        {
            get => _model.SaveEnabled;
            set => SetProperty(_model.SaveEnabled, value, _model, (model, _value) => model.SaveEnabled = _value);
        }

        public bool StatChecked
        {
            get
            {
                _settingModel.StatOrBatch = _model.StatChecked ? eDepartment.Stat : eDepartment.Batch;
                return _model.StatChecked;
            }
            set => SetProperty(_model.StatChecked, value, _model, (model, _value) => model.StatChecked = _value);
        }

        public bool BatchChecked
        {
            get => _model.BatchChecked;
            set => SetProperty(_model.BatchChecked, value, _model, (model, _value) => model.BatchChecked = _value);
        }

        public bool StatEnabled
        {
            get => _model.StatEnabled;
            set => SetProperty(_model.StatEnabled, value, _model, (model, _value) => model.StatEnabled = _value);
        }

        public bool BatchEnabled
        {
            get => _model.BatchEnabled;
            set => SetProperty(_model.BatchEnabled, value, _model, (model, _value) => model.BatchEnabled = _value);
        }

        public bool AutoStartEnabled
        {
            get => _model.AutoStartEnabled;
            set => SetProperty(_model.AutoStartEnabled, value, _model, (model, _value) => model.AutoStartEnabled = _value);
        }

        public Visibility UDTypeVisibility
        {
            get => _model.UDTypeVisibility;
            set => SetProperty(_model.UDTypeVisibility, value, _model, (model, _value) => model.UDTypeVisibility = _value);
        }

        public Visibility SplitEachMealVisibility
        {
            get => _model.SplitEachMealVisibility;
            set => SetProperty(_model.SplitEachMealVisibility, value, _model, (model, _value) => model.SplitEachMealVisibility = _value);
        }

        public bool IsAutoStartChecked
        {
            get => _model.IsAutoStartChecked;
            set => SetProperty(_model.IsAutoStartChecked, value, _model, (model, _value) => model.IsAutoStartChecked = _value);
        }

        public bool IsSplitEachMealChecked
        {
            get => Properties.Settings.Default.IsSplitEachMeal;
            set
            {
                Properties.Settings.Default.IsSplitEachMeal = value;
                Properties.Settings.Default.Save();
            }
        }

        public eDoseType DoseType
        {
            get => _model.DoseType;
            set => SetProperty(_model.DoseType, value, _model, (model, _value) => model.DoseType = _value);
        }

        public Visibility MinimumAndCloseVisibility
        {
            get => _model.MinimumAndCloseVisibility;
            set => SetProperty(_model.MinimumAndCloseVisibility, value, _model, (model, _value) => model.MinimumAndCloseVisibility = _value);
        }

        public float OPDOpacity
        {
            get => _model.OPDOpacity;
            set => SetProperty(_model.OPDOpacity, value, _model, (model, _value) => model.OPDOpacity = _value);
        }

        public float UDOpacity
        {
            get => _model.UDOpacity;
            set => SetProperty(_model.UDOpacity, value, _model, (model, _value) => model.UDOpacity = _value);
        }

        public SolidColorBrush OPDBackground
        {
            get => _model.OPDBacground;
            set => SetProperty(_model.OPDBacground, value, _model, (model, _value) => model.OPDBacground = _value);
        }

        public SolidColorBrush UDBackground
        {
            get => _model.UDBackground;
            set => SetProperty(_model.UDBackground, value, _model, (model, _value) => model.UDBackground = _value);
        }

        public string ProgressBox
        {
            get => _model.ProgressBox;
            set => SetProperty(_model.ProgressBox, value, _model, (model, _value) => model.ProgressBox = _value);
        }

        public object WindowPosition
        {
            get => _windowPosition;
            set => SetProperty(ref _windowPosition, value);
        }

        private ConvertBase _formatBase;
        private SettingJsonModel _settingModel;
        private MainWindowModel _model;
        private object _windowPosition;

        public void OPDFunc()
        {
            CommonModel.CurrentDepartment = eDepartment.OPD;
            _formatBase.SetMainWindowToogleChecked();
            if (!_formatBase.PrepareStart().Success)
            {
                return;
            }
            SwitchUIToStart();
            SwitchMainWindowControlState(false);
        }

        public void UDFunc()
        {
            CommonModel.CurrentDepartment = eDepartment.UD;
            _formatBase.SetMainWindowToogleChecked();
            if (!_formatBase.PrepareStart().Success)
            {
                return;
            }
            SwitchUIToStart();
            SwitchMainWindowControlState(false);
        }

        public void StopFunc()
        {
            _formatBase.Stop();
            SwitchUIToStop();
            SwitchMainWindowControlState(true);
        }

        public void SaveFunc()
        {
            if (HasAnyErrorsMessage())
            {
                MsgCollection.ShowDialog("可能有欄位處於未通過驗證的狀態，請確認各欄位是否符合驗證規則", "有未驗證成功的欄位", PackIconKind.Error, ColorProvider.GetSolidColorBrush(eColor.Red));
                return;
            }
            Properties.Settings.Default.X = Messenger.Send(new WindowXRequestMessage());
            Properties.Settings.Default.Y = Messenger.Send(new WindowYRequestMessage());
            Properties.Settings.Default.Save();
            SettingJsonModel model = _settingModel;
            model.InputDirectory1 = InputDirectory1;
            model.InputDirectory2 = InputDirectory2;
            model.InputDirectory3 = InputDirectory3;
            model.InputDirectory4 = InputDirectory4;
            model.InputDirectory5 = InputDirectory5;
            model.InputDirectory6 = InputDirectory6;
            model.OutputDirectory = OutputDirectory;
            model.AutoStart = IsAutoStartChecked;
            Setting.Save(model);
            MsgCollection.ShowDialog("設定儲存成功", "成功", PackIconKind.Information, ColorProvider.GetSolidColorBrush(eColor.RoyalBlue));
        }

        public void SwitchWindowFunc()
        {
            Visibility = Visibility.Hidden;
            CommonModel.WindowType = eWindowType.SimpleWindow;
            Messenger.Send(new ShowDialogMessage(), nameof(FCP.Views.SimpleWindowView));
        }

        public void NotifyIconDBClick()
        {
            if (CommonModel.WindowType == eWindowType.SimpleWindow)
            {
                Messenger.Send(new VisibilityMessage(), nameof(SimpleWindowViewModel));
            }
            else
            {
                Visibility = Visibility.Visible;
                Messenger.Send(new ActivateMessage(), nameof(FCP.Views.MainWindowView));
            }
        }

        protected override void OnActivated()
        {
            base.OnActivated();

            Messenger.Register<MainUIChangeMessage>(this, (r, m) => RefreshMainWindowUI(m.Value));

            Messenger.Register<SetMainWindowToogleCheckedChangeMessage>(this, (r, m) => SetToogleChecked(m.Value));

            Messenger.Register<CommandMessage, string>(this, nameof(eCommandCollection.CreateNewFormat), (r, m) => NewFormat());

            Messenger.Register<GetMainWindowToogleCheckedRequestMessage>(this, (r, m) =>
            {
                m.Reply(new MainWindowModel.ToggleModel()
                {
                    Toggle1 = OPDToogle1Checked,
                    Toggle2 = OPDToogle2Checked,
                    Toggle3 = OPDToogle3Checked,
                    Toggle4 = OPDToogle4Checked
                });
            });

            Messenger.Register<VisibilityMessage, string>(this, nameof(MainWindowViewModel), (r, m) =>
            {
                Visibility = Visibility.Visible;
                Focusable = true;
            });

            Messenger.Register<HasErrorsRequestMessage, string>(this, nameof(MainWindowViewModel), (r, m) => m.Reply(false));

            Messenger.Register<LogChangeMessage>(this, (r, m) =>
            {
                ProgressBox += $"{m.Value}\n";
            });

            Messenger.Register<OpreationMessage, string>(this, nameof(eOpreation.OPD), (r, m) => OPDFunc());

            Messenger.Register<OpreationMessage, string>(this, nameof(eOpreation.UD), (r, m) => UDFunc());

            Messenger.Register<OpreationMessage, string>(this, nameof(eOpreation.Stop), (r, m) => StopFunc());
        }

        private bool HasAnyErrorsMessage()
        {
            if (Messenger.Send(new HasErrorsRequestMessage(), nameof(MainWindowViewModel)))
            {
                return true;
            }
            if (Messenger.Send(new HasErrorsRequestMessage(), nameof(WindowPositionViewModel)))
            {
                return true;
            }
            return false;
        }

        private void NewFormat()
        {
            _formatBase = FormatFactory.GenerateNewFormat(_settingModel.Format);
            CommonModel.SqlHelper.ConnectionStr = $"{_settingModel.Format}".Contains("OC") || $"{_settingModel.Format}" == "JVS" ? CommonModel.OnCubeSqlConnection : CommonModel.JVServerSqlConnection;
            CommonModel.SqlHelper.InitDb();
            _formatBase.Init();
        }

        private async void LoadedFuncAsync()
        {
            Log.Path = $@"{Environment.CurrentDirectory}\Log";
            CheckProgramIsAlreadyOpen();
            SwitchMainWindowControlState(true);
            NotifyIconHelper.Init(Properties.Resources.FCP, "轉檔");
            NotifyIconHelper.DoubleClickAction += NotifyIconDBClick;
            CommonModel.NotifyIcon = NotifyIconHelper.NotifyIcon;
            NewFormat();
            Init();
            if (IsAutoStartChecked)
            {
                await Task.Delay(500);
                OPDFunc();
            }
            if (_settingModel.MinimizeWindowWhenProgramStart)
            {
                SwitchWindowFunc();
            }
        }

        //檢查程式是否已開啟
        private void CheckProgramIsAlreadyOpen()
        {
            if (Process.GetProcessesByName("FCP").Length > 1)
            {
                Log.Write("程式已開啟，請確認工具列");
                MsgCollection.ShowDialog("程式已開啟，請確認工具列", "重複開啟", PackIconKind.Error, ColorProvider.GetSolidColorBrush(eColor.Red));
                Environment.Exit(0);
            }
        }

        private void ShowAdvancedSettingsFunc()
        {
            Messenger.Send(new ShowDialogMessage(), nameof(FCP.Views.SettingView));
        }

        private void SelectFolderFunc(string name)
        {
            string path = string.Empty;
            using (FolderBrowserDialog folder = new FolderBrowserDialog())
            {
                if (folder.ShowDialog() == DialogResult.OK)
                {
                    path = folder.SelectedPath;
                }
            }
            switch (name)
            {
                case "IP1":
                    InputDirectory1 = path;
                    break;
                case "IP2":
                    InputDirectory2 = path;
                    break;
                case "IP3":
                    InputDirectory3 = path;
                    break;
                case "IP4":
                    InputDirectory4 = path;
                    break;
                case "IP5":
                    InputDirectory5 = path;
                    break;
                case "IP6":
                    InputDirectory6 = path;
                    break;
                case "OP":
                    OutputDirectory = path;
                    break;
                default:
                    break;
            }
        }

        private void ClearPathFunc(string name)
        {
            switch (name)
            {
                case "IP1":
                    InputDirectory1 = string.Empty;
                    break;
                case "IP2":
                    InputDirectory2 = string.Empty;
                    break;
                case "IP3":
                    InputDirectory3 = string.Empty;
                    break;
                case "IP4":
                    InputDirectory4 = string.Empty;
                    break;
                case "IP5":
                    InputDirectory5 = string.Empty;
                    break;
                case "IP6":
                    InputDirectory6 = string.Empty;
                    break;
                default:
                    break;
            }
        }

        private void Init()
        {
            InputDirectory1 = _settingModel.InputDirectory1;
            InputDirectory2 = _settingModel.InputDirectory2;
            InputDirectory3 = _settingModel.InputDirectory3;
            InputDirectory4 = _settingModel.InputDirectory4;
            InputDirectory5 = _settingModel.InputDirectory5;
            InputDirectory6 = _settingModel.InputDirectory6;
            OutputDirectory = _settingModel.OutputDirectory;
            IsAutoStartChecked = _settingModel.AutoStart;
            StopEnabled = false;
            StatChecked = _settingModel.StatOrBatch == eDepartment.Stat;
            BatchChecked = _settingModel.StatOrBatch == eDepartment.Batch;
            WindowPosition = App.Current.Services.GetService<WindowPositionViewModel>();
        }

        private bool CanStartConverterOrShowAdvancedSettings()
        {
            if (_formatBase == null)
            {
                return true;
            }
            return !_formatBase.IsStart;
        }

        private void ClearProgressBoxFunc()
        {
            ProgressBox = string.Empty;
        }

        private void OpenLogFunc()
        {
            try
            {
                Process.Start($@"{Environment.CurrentDirectory}\Log\{DateTime.Now:yyyy-MM-dd}\Log_{DateTime.Now:HH}.txt");
            }
            catch
            {
                MsgCollection.ShowDialog("目前尚未產生Log檔", "錯誤", PackIconKind.Error, ColorProvider.GetSolidColorBrush(eColor.Red));
            }
        }

        private void RefreshMainWindowUI(MainUILayoutModel UI)
        {
            if (_formatBase.IsStart)
            {
                return;
            }
            WindowTitle = UI.Title;
            InputDirectory1Title = UI.IP1Title;
            InputDirectory2Title = UI.IP2Title;
            InputDirectory3Title = UI.IP3Title;
            InputDirectory4Title = UI.IP4Title;
            InputDirectory5Title = UI.IP5Title;
            InputDirectory6Title = UI.IP6Title;
            OPDToogle1 = UI.OPDToogle1;
            OPDToogle2 = UI.OPDToogle2;
            OPDToogle3 = UI.OPDToogle3;
            OPDToogle4 = UI.OPDToogle4;
            InputDirectory1Enabled = UI.IP1Enabled;
            InputDirectory2Enabled = UI.IP2Enabled;
            InputDirectory3Enabled = UI.IP3Enabled;
            InputDirectory4Enabled = UI.IP4Enabled;
            InputDirectory5Enabled = UI.IP5Enabled;
            InputDirectory6Enabled = UI.IP6Enabled;
            UDButtonVisibility = UI.UDVisibility;
            OPDToogle1Visibility = UI.OPD1Visibility;
            OPDToogle2Visibility = UI.OPD2Visibility;
            OPDToogle3Visibility = UI.OPD3Visibility;
            OPDToogle4Visibility = UI.OPD4Visibility;
            DoseType = _settingModel.DoseType == eDoseType.餐包 ? eDoseType.餐包 : eDoseType.種包;
            UDTypeVisibility = _settingModel.UseStatAndBatchOption ? Visibility.Visible : Visibility.Hidden;
            MinimumAndCloseVisibility = _settingModel.ShowCloseAndMinimizeButton ? Visibility.Visible : Visibility.Hidden;
            SplitEachMealVisibility = _settingModel.Format == eFormat.JVS ? Visibility.Visible : Visibility.Hidden;
            Messenger.Send(new WindowPositionVisibilityChangeMessage(_settingModel.ShowXY ? Visibility.Visible : Visibility.Hidden));
        }

        private void SwitchMainWindowControlState(bool b)
        {
            InputDirectory1Enabled = b;
            InputDirectory2Enabled = b;
            InputDirectory3Enabled = b;
            InputDirectory4Enabled = b;
            InputDirectory5Enabled = b;
            InputDirectory6Enabled = b;
            OutputDirectoryEnabled = b;
            StatEnabled = b;
            BatchEnabled = b;
            OPDEnabled = b;
            UDEnabled = b;
            StopEnabled = !b;
            SaveEnabled = b;
            AutoStartEnabled = b;
            Messenger.Send(new WindowXEnabledChangeMessage(b));
            Messenger.Send(new WindowYEnabledChangeMessage(b));
            OPDToogle1Enabled = b;
            OPDToogle2Enabled = b;
            OPDToogle3Enabled = b;
            OPDToogle4Enabled = b;
        }

        private void SwitchUIToStop()
        {
            OPDOpacity = 1;
            UDOpacity = 1;
            OPDBackground = ColorProvider.GetSolidColorBrush(eColor.White);
            UDBackground = ColorProvider.GetSolidColorBrush(eColor.White);
        }

        private void SwitchUIToStart()
        {
            if (CommonModel.CurrentDepartment == eDepartment.OPD)
            {
                OPDBackground = ColorProvider.GetSolidColorBrush(eColor.Red);
                UDOpacity = 0.2F;
            }
            else
            {
                UDBackground = ColorProvider.GetSolidColorBrush(eColor.Red);
                OPDOpacity = 0.2F;
            }
        }

        private void SetToogleChecked(MainWindowModel.ToggleModel model)
        {
            OPDToogle1Checked = model.Toggle1;
            OPDToogle2Checked = model.Toggle2;
            OPDToogle3Checked = model.Toggle3;
            OPDToogle4Checked = model.Toggle4;
        }
    }
}