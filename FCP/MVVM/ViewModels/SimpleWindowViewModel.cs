using System;
using System.Windows;
using System.Windows.Media;
using FCP.Core;
using FCP.MVVM.Models;

namespace FCP.MVVM.ViewModels
{
    class SimpleWindowViewModel : ViewModelBase
    {
        private SimpleWindowModel _Model;

        public SimpleWindowViewModel()
        {
            _Model = new SimpleWindowModel();
        }

        public bool Enabled
        {
            get => _Model.Enabled;
            set => _Model.Enabled = value;
        }

        public bool MultiChecked
        {
            get => _Model.MultiChecked;
            set => _Model.MultiChecked = value;
        }

        public bool CombiChecked
        {
            get => _Model.CombiChecked;
            set => _Model.CombiChecked = value;
        }

        public bool MultiEnabled
        {
            get => _Model.MultiEnabled;
            set => _Model.MultiEnabled = value;
        }

        public bool CombiEnabled
        {
            get => _Model.CombiEnabled;
            set => _Model.CombiEnabled = value;
        }

        public Visibility MultiVisibility
        {
            get => _Model.MultiVisibility;
            set => _Model.MultiVisibility = value;
        }

        public Visibility CombiVisibility
        {
            get => _Model.CombiVisibility;
            set => _Model.CombiVisibility = value;
        }

        public string OPD
        {
            get => _Model.OPD;
            set => _Model.OPD = value;
        }

        public SolidColorBrush OPDBackground
        {
            get => _Model.OPDBackground;
            set => _Model.OPDBackground = value;
        }

        public bool OPDEnalbed
        {
            get => _Model.OPDEnalbed;
            set => _Model.OPDEnalbed = value;
        }

        public float OPDOpacity
        {
            get => _Model.OPDOpacity;
            set => _Model.OPDOpacity = value;
        }

        public string UD
        {
            get => _Model.UD;
            set => _Model.UD = value;
        }

        public SolidColorBrush UDBackground
        {
            get => _Model.UDBackground;
            set => _Model.UDBackground = value;
        }

        public bool UDEnalbed
        {
            get => _Model.UDEnalbed;
            set => _Model.UDEnalbed = value;
        }

        public float UDOpacity
        {
            get => _Model.UDOpacity;
            set => _Model.UDOpacity = value;
        }

        public Visibility UDVisibility
        {
            get => _Model.UDVisibility;
            set => _Model.UDVisibility = value;
        }

        public SolidColorBrush StopBackground
        {
            get => _Model.StopBackground;
            set => _Model.StopBackground = value;
        }

        public bool StopEnalbed
        {
            get => _Model.StopEnalbed;
            set => _Model.StopEnalbed = value;
        }

        public float StopOpacity
        {
            get => _Model.StopOpacity;
            set => _Model.StopOpacity = value;
        }

        public Visibility StatVisibility
        {
            get => _Model.StatVisibility;
            set => _Model.StatVisibility = value;
        }

        public bool StatChecked
        {
            get => _Model.StatChecked;
            set => _Model.StatChecked = value;
        }

        public Visibility BatchVisibility
        {
            get => _Model.BatchVisibility;
            set => _Model.BatchVisibility = value;
        }

        public bool BatchChecked
        {
            get => _Model.BatchChecked;
            set => _Model.BatchChecked = value;
        }

        public string Log
        {
            get => _Model.Log;
            set => _Model.Log = value;
        }

        public Visibility CloseVisibility
        {
            get => _Model.CloseVisibility;
            set => _Model.CloseVisibility = value;
        }

        public Visibility MinimumVisibility
        {
            get => _Model.MinimumVisibility;
            set => _Model.MinimumVisibility = value;
        }

        public void ChangeLayout()
        {

        }
    }
}
