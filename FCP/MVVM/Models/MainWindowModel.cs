using System;
using System.Windows;
using FCP.MVVM.Models.Enum;

namespace FCP.MVVM.Models
{
    class MainWindowModel
    {
        public string WindowTitle { get; set; }
        public string InputPath1Title { get; set; }
        public string InputPath2Title { get; set; }
        public string InputPath3Title { get; set; }
        public string InputPath1 { get; set; }
        public string InputPath2 { get; set; }
        public string InputPath3 { get; set; }
        public bool InputPath1Enabled { get; set; }
        public bool InputPath2Enabled { get; set; }
        public bool InputPath3Enabled { get; set; }
        public string OutputPathTitle { get; set; }
        public string OutputPath { get; set; }
        public bool OutputPathEnabled { get; set; }
        public string OPDButtonContent { get; set; }
        public string UDButtonContent { get; set; }
        public Visibility UDButtonVisibility { get; set; }
        public string OPDToogle1 { get; set; }
        public string OPDToogle2 { get; set; }
        public string OPDToogle3 { get; set; }
        public string OPDToogle4 { get; set; }
        public bool OPDToogle1Checked { get; set; }
        public bool OPDToogle2Checked { get; set; }
        public bool OPDToogle3Checked { get; set; }
        public bool OPDToogle4Checked { get; set; }
        public bool OPDToogle1Enabled { get; set; }
        public bool OPDToogle2Enabled { get; set; }
        public bool OPDToogle3Enabled { get; set; }
        public bool OPDToogle4Enabled { get; set; }
        public Visibility OPDToogle1Visibility { get; set; }
        public Visibility OPDToogle2Visibility { get; set; }
        public Visibility OPDToogle3Visibility { get; set; }
        public Visibility OPDToogle4Visibility { get; set; }
        public bool OPDEnabled { get; set; }
        public bool UDEnabled { get; set; }
        public bool StopEnabled { get; set; }
        public bool SaveEnabled { get; set; }
        public bool StatChecked { get; set; }
        public bool BatchChecked { get; set; }
        public bool StatEnabled { get; set; }
        public bool BatchEnabled { get; set; }
        public Visibility StatVisibility { get; set; }
        public Visibility BatchVisibility { get; set; }
        public bool IsAutoStartChecked { get; set; }
        public string DoseType { get; set; }
        public string SuccessCount { get; set; }
        public string FailCount { get; set; }
        public string WindowX { get; set; }
        public string WindowY { get; set; }
        public bool WindowXEnabled { get; set; }
        public bool WindowYEnabled { get; set; }
        public Visibility WindowXVisibility { get; set; }
        public Visibility WindowYVisibility { get; set; }
        public DoseType PackType { get; set; }
        public Visibility MinimumAndCloseVisibility { get; set; }
    }
}
