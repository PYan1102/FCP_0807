using System;
using System.Windows;
using FCP.MVVM.Models.Enum;

namespace FCP.MVVM.Models
{
    class MainWindowModel
    {
        public string Title { get; set; }
        public string InputPath1Title { get; set; }
        public string InputPath2Title { get; set; }
        public string InputPath3Title { get; set; }
        public string OutputPathTitle { get; set; }
        public string InputPath1 { get; set; }
        public string InputPath2 { get; set; }
        public string InputPath3 { get; set; }
        public bool InputPath1Enable { get; set; }
        public bool InputPath2Enable { get; set; }
        public bool InputPath3Enable { get; set; }
        public string OutputPath { get; set; }
        public string OPDContent { get; set; }
        public string UDContent { get; set; }
        public Visibility UDVisibility { get; set; }
        public string OPD1 { get; set; }
        public string OPD2 { get; set; }
        public string OPD3 { get; set; }
        public string OPD4 { get; set; }
        public Visibility OPD1Visibility { get; set; }
        public Visibility OPD2Visibility { get; set; }
        public Visibility OPD3Visibility { get; set; }
        public Visibility OPD4Visibility { get; set; }
        public bool IsStat { get; set; }
        public bool IsBatch { get; set; }
        public DoseType PackType { get; set; }
        public bool IsOPD { get; set; } = false;
    }
}
