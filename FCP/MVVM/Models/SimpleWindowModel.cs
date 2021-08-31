using System;
using System.Windows;
using System.Windows.Media;

namespace FCP.MVVM.Models
{
    class SimpleWindowModel
    {
        public bool Enabled { get; set; } = true;
        public bool MultiChecked { get; set; }
        public bool CombiChecked { get; set; }
        public bool MultiEnabled { get; set; }
        public bool CombiEnabled { get; set; }
        public Visibility MultiVisibility { get; set; }
        public Visibility CombiVisibility { get; set; }
        public string OPD { get; set; }
        public SolidColorBrush OPDBackground { get; set; }
        public bool OPDEnalbed { get; set; }
        public float OPDOpacity { get; set; }
        public string UD { get; set; }
        public SolidColorBrush UDBackground { get; set; }
        public bool UDEnalbed { get; set; }
        public float UDOpacity { get; set; }
        public Visibility UDVisibility { get; set; }
        public SolidColorBrush StopBackground { get; set; }
        public bool StopEnalbed { get; set; }
        public float StopOpacity { get; set; }
        public Visibility StatVisibility { get; set; }
        public bool StatChecked { get; set; }
        public Visibility BatchVisibility { get; set; }
        public bool BatchChecked { get; set; }
        public string Log { get; set; }
        public Visibility CloseVisibility { get; set; }
        public Visibility MinimumVisibility { get; set; }
    }
}
