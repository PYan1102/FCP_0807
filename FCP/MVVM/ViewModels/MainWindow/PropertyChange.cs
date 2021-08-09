using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using FCP.Core;
using FCP.MVVM.Models;
using FCP.MVVM.Models.Enum;

namespace FCP.MVVM.ViewModels.MainWindow
{
    class PropertyChange : ViewModelBase
    {
        private MainWindowModel _MainWindowModel { get; set; }
        public PropertyChange()
        {
            _MainWindowModel = new MainWindowModel();
        }
        public string Title { get => _MainWindowModel.Title; set => _MainWindowModel.Title = value; }
        public string InputPath1Title { get => _MainWindowModel.InputPath1Title; set => _MainWindowModel.InputPath1Title = value; }
        public string InputPath2Title { get => _MainWindowModel.InputPath2Title; set => _MainWindowModel.InputPath2Title = value; }
        public string InputPath3Title { get => _MainWindowModel.InputPath3Title; set => _MainWindowModel.InputPath3Title = value; }
        public string OutputPathTitle { get => _MainWindowModel.OutputPathTitle; set => _MainWindowModel.OutputPathTitle = value; }
        public string InputPath1 { get => _MainWindowModel.InputPath1; set => _MainWindowModel.InputPath1 = value; }
        public string InputPath2 { get => _MainWindowModel.InputPath2; set => _MainWindowModel.InputPath2 = value; }
        public string InputPath3 { get => _MainWindowModel.InputPath3; set => _MainWindowModel.InputPath3 = value; }
        public bool InputPath1Enable { get => _MainWindowModel.InputPath1Enable; set => _MainWindowModel.InputPath1Enable = value; }
        public bool InputPath2Enable { get => _MainWindowModel.InputPath2Enable; set => _MainWindowModel.InputPath2Enable = value; }
        public bool InputPath3Enable { get => _MainWindowModel.InputPath3Enable; set => _MainWindowModel.InputPath3Enable = value; }
        public string OutputPath { get => _MainWindowModel.OutputPath; set => _MainWindowModel.OutputPath = value; }
        public string OPDContent { get => _MainWindowModel.OPDContent; set => _MainWindowModel.OPDContent = value; }
        public string UDContent { get => _MainWindowModel.UDContent; set => _MainWindowModel.UDContent = value; }
        public Visibility UDVisibility { get => _MainWindowModel.UDVisibility; set => _MainWindowModel.UDVisibility = value; }
        public string OPD1 { get => _MainWindowModel.OPD1; set => _MainWindowModel.OPD1 = value; }
        public string OPD2 { get => _MainWindowModel.OPD2; set => _MainWindowModel.OPD2 = value; }
        public string OPD3 { get => _MainWindowModel.OPD3; set => _MainWindowModel.OPD3 = value; }
        public string OPD4 { get => _MainWindowModel.OPD4; set => _MainWindowModel.OPD4 = value; }
        public Visibility OPD1Visibility { get => _MainWindowModel.OPD1Visibility; set => _MainWindowModel.OPD1Visibility = value; }
        public Visibility OPD2Visibility { get => _MainWindowModel.OPD2Visibility; set => _MainWindowModel.OPD2Visibility = value; }
        public Visibility OPD3Visibility { get => _MainWindowModel.OPD3Visibility; set => _MainWindowModel.OPD3Visibility = value; }
        public Visibility OPD4Visibility { get => _MainWindowModel.OPD4Visibility; set => _MainWindowModel.OPD4Visibility = value; }
        public bool IsStat { get => _MainWindowModel.IsStat; set => _MainWindowModel.IsStat = value; }
        public bool IsBatch { get => _MainWindowModel.IsBatch; set => _MainWindowModel.IsBatch = value; }
        public DoseMode PackType { get => _MainWindowModel.PackType; set => _MainWindowModel.PackType = value; }
    }
}
