using System.Windows;

namespace FCP.Models
{
    public sealed class MainUILayoutModel
    {
        public string Title { get; set; } = "通用格式";
        public string IP1Title { get; set; } = "輸入路徑1";
        public string IP2Title { get; set; } = "輸入路徑2";
        public string IP3Title { get; set; } = "輸入路徑3";
        public string IP4Title { get; set; } = "輸入路徑4";
        public string IP5Title { get; set; } = "Batch";
        public string IP6Title { get; set; } = "Stat";
        public bool IP1Enabled { get; set; } = true;
        public bool IP2Enabled { get; set; } = true;
        public bool IP3Enabled { get; set; } = true;
        public bool IP4Enabled { get; set; } = true;
        public bool IP5Enabled { get; set; } = false;
        public bool IP6Enabled { get; set; } = false;
        public string OPDToogle1 { get; set; } = "輸入1";
        public string OPDToogle2 { get; set; } = "輸入2";
        public string OPDToogle3 { get; set; } = "輸入3";
        public string OPDToogle4 { get; set; } = "輸入4";
        public Visibility UDVisibility { get; set; } = Visibility.Hidden;
        public Visibility OPD1Visibility { get; set; } = Visibility.Visible;
        public Visibility OPD2Visibility { get; set; } = Visibility.Visible;
        public Visibility OPD3Visibility { get; set; } = Visibility.Visible;
        public Visibility OPD4Visibility { get; set; } = Visibility.Visible;
    }
}