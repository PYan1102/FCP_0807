using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using FCP.Core;
using FCP.MVVM.Models;
using FCP.MVVM.Factory.ViewModels;

namespace FCP.MVVM.ViewModels
{
    class MsgBViewModel : ViewModelBase
    {
        public ICommand Close { get; set; }
        public ICommand WindowClosed { get; set; }
        public ICommand DragMove { get; set; }
        private MsgBModel _Model;

        [DllImport("User32.dll")]
        public static extern bool MessageBeep(uint uType);

        public MsgBViewModel()
        {
            _Model = new MsgBModel();
            Close = new ObjectRelayCommand(o => ((Window)o).DialogResult = true);
            WindowClosed = new ObjectRelayCommand(o => ((Window)o).IsEnabled = false);
            DragMove = new ObjectRelayCommand(o => ((Window)o).DragMove());
        }

        public string Content
        {
            get => _Model.Content;
            set
            {
                _Model.Content = value;
                OnPropertyChanged();
            }
        }

        public string Title
        {
            get => _Model.Title;
            set
            {
                _Model.Title = value;
                OnPropertyChanged();
            }
        }

        public MaterialDesignThemes.Wpf.PackIconKind Kind
        {
            get => _Model.Kind;
            set
            {
                _Model.Kind = value;
                OnPropertyChanged();
            }
        }

        public Color KindColor
        {
            get => _Model.KindColor;
            set
            {
                _Model.KindColor = value;
                OnPropertyChanged();
            }
        }

        public Visibility Visibility
        {
            get => _Model.Visibility;
            set
            {
                _Model.Visibility = value;
                OnPropertyChanged();
            }
        }

        public bool OKButtonFocus
        {
            get => _Model.OKButtonFocus;
            set
            {
                _Model.OKButtonFocus = value;
                OnPropertyChanged();
            }
        }

        public void Show(string content, string title, MaterialDesignThemes.Wpf.PackIconKind kind, Color kindColor)
        {
            Content = content;
            Title = title;
            Kind = kind;
            KindColor = kindColor;
            var window = MsgBFactory.GenerateMsgB();
            OKButtonFocus = true;
            MessageBeep(1);
            window.ShowDialog();
        }
    }

    static class KindColors
    {
        public static readonly Color Error = Color.FromRgb(255, 82, 85);
        public static readonly Color Warning = Color.FromRgb(225, 219, 96);
        public static readonly Color Information = Color.FromRgb(0, 100, 213);
    }
}
