using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using FCP.Core;
using FCP.Models;
using FCP.src.Factory.ViewModel;
using MaterialDesignThemes.Wpf;

namespace FCP.ViewModels
{
    class MsgViewModel : ViewModelBase
    {
        public ICommand Close { get; set; }
        public ICommand WindowClosed { get; set; }
        public ICommand DragMove { get; set; }
        private MsgModel _model;

        [DllImport("User32.dll")]
        public static extern bool MessageBeep(uint uType);

        public MsgViewModel()
        {
            _model = new MsgModel();
            Close = new ObjectRelayCommand(o => ((Window)o).DialogResult = true);
            WindowClosed = new ObjectRelayCommand(o => ((Window)o).IsEnabled = false);
            DragMove = new ObjectRelayCommand(o => ((Window)o).DragMove());
        }

        public string Content
        {
            get => _model.Content;
            set => _model.Content = value;
        }

        public string Title
        {
            get => _model.Title;
            set => _model.Title = value;
        }

        public PackIconKind Kind
        {
            get => _model.Kind;
            set => _model.Kind = value;
        }

        public Color KindColor
        {
            get => _model.KindColor;
            set => _model.KindColor = value;
        }

        public Visibility WindowVisibility
        {
            get => _model.WindowVisibility;
            set => _model.WindowVisibility = value;
        }

        public bool OKButtonFocus
        {
            get => _model.OKButtonFocus;
            set => _model.OKButtonFocus = value;
        }

        public void Show(object content, object title, PackIconKind kind, Color kindColor)
        {
            Content = content.ToString();
            Title = title.ToString();
            Kind = kind;
            KindColor = kindColor;
            var window = MsgFactory.GenerateMsg();
            OKButtonFocus = true;
            MessageBeep(1);
            window.ShowDialog();
            window.Close();
        }

        public void Show(object content)
        {
            Content = content.ToString();
            Title = string.Empty;
            Kind = PackIconKind.Information;
            KindColor = KindColors.Information;
            var window = MsgFactory.GenerateMsg();
            OKButtonFocus = true;
            MessageBeep(1);
            window.ShowDialog();
            window.Close();
        }
    }

    static class KindColors
    {
        public static readonly Color Error = Color.FromRgb(255, 82, 85);
        public static readonly Color Warning = Color.FromRgb(225, 219, 96);
        public static readonly Color Information = Color.FromRgb(0, 100, 213);
    }
}
