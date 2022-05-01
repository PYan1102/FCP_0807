using System.Windows;

namespace FCP.Models
{
    public sealed class MainUILayoutModel
    {
        public string Title { get; set; }
        public string IP1Title { get; set; }
        public string IP2Title { get; set; }
        public string IP3Title { get; set; }
        public string IP4Title { get; set; }
        public string IP5Title { get; set; }
        public string IP6Title { get; set; }
        public string OPDToogle1 { get; set; }
        public string OPDToogle2 { get; set; }
        public string OPDToogle3 { get; set; }
        public string OPDToogle4 { get; set; }
        public bool IP1Enabled { get; set; }
        public bool IP2Enabled { get; set; }
        public bool IP3Enabled { get; set; }
        public bool IP4Enabled { get; set; }
        public bool IP5Enabled { get; set; }
        public bool IP6Enabled { get; set; }
        public Visibility UDVisibility { get; set; }
        public Visibility OPD1Visibility { get; set; }
        public Visibility OPD2Visibility { get; set; }
        public Visibility OPD3Visibility { get; set; }
        public Visibility OPD4Visibility { get; set; }
    }
}
