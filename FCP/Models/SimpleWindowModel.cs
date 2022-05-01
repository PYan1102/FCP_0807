using System.Windows;
using System.Windows.Media;

namespace FCP.Models
{
    public sealed class SimpleWindowModel
    {
        public Visibility Visibility { get; set; } = Visibility.Hidden;
        public bool Topmost { get; set; }
        public bool Enabled { get; set; } = false;
        public int Top { get; set; }
        public int Left { get; set; }
        public bool MultiChecked { get; set; }
        public bool CombiChecked { get; set; }
        public bool MultiEnabled { get; set; }
        public bool CombiEnabled { get; set; }
        public Visibility MultiVisibility { get; set; }
        public Visibility CombiVisibility { get; set; }
        public string OPDContent { get; set; } = "開始轉檔F5";
        public SolidColorBrush OPDBackground { get; set; } = new SolidColorBrush(Colors.White);
        public bool OPDEnabled { get; set; } = true;
        public float OPDOpacity { get; set; } = 1;
        public SolidColorBrush UDBackground { get; set; } = new SolidColorBrush(Colors.White);
        public bool UDEnabled { get; set; }
        public float UDOpacity { get; set; } = 1;
        public Visibility UDVisibility { get; set; } = Visibility.Hidden;
        public SolidColorBrush StopBackground { get; set; } = new SolidColorBrush(Colors.White);
        public bool StopEnabled { get; set; }
        public float StopOpacity { get; set; } = 0.5f;
        public Visibility StatVisibility { get; set; }
        public bool StatChecked { get; set; } = true;
        public bool StatEnabled { get; set; } = true;
        public Visibility BatchVisibility { get; set; }
        public bool BatchChecked { get; set; } = false;
        public bool BatchEnabled { get; set; } = true;
        public string Log { get; set; }
        public Visibility CloseVisibility { get; set; }
        public Visibility MinimumVisibility { get; set; }
        public Visibility LogVisibility { get; set; } = Visibility.Hidden;
    }
}
