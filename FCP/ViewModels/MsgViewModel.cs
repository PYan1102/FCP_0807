using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using FCP.Models;
using FCP.Providers;
using FCP.src.Enum;
using MaterialDesignThemes.Wpf;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace FCP.ViewModels
{
    public class MsgViewModel : ObservableRecipient
    {
        public ICommand Close { get; set; }
        private MsgModel _model;

        [DllImport("User32.dll")]
        public static extern bool MessageBeep(uint uType);

        public MsgViewModel()
        {
            _model = new MsgModel();
            Close = new RelayCommand<Window>(o => o.DialogResult = true);
        }

        public string Content
        {
            get => _model.Content;
            set => SetProperty(_model.Content, value, _model, (model, _value) => model.Content = _value);
        }

        public string Title
        {
            get => _model.Title;
            set => SetProperty(_model.Title, value, _model, (model, _value) => model.Title = _value);
        }

        public PackIconKind Kind
        {
            get => _model.Kind;
            set => SetProperty(_model.Kind, value, _model, (model, _value) => model.Kind = _value);
        }

        public SolidColorBrush KindColor
        {
            get => _model.KindColor;
            set => SetProperty(_model.KindColor, value, _model, (model, _value) => model.KindColor = _value);
        }

        public bool OKButtonFocus
        {
            get => _model.OKButtonFocus;
            set => SetProperty(_model.OKButtonFocus, value, _model, (model, _value) => model.OKButtonFocus = _value);
        }

        public void Init(object content, object title, PackIconKind kind, SolidColorBrush kindColor)
        {
            Content = content.ToString();
            Title = title.ToString();
            Kind = kind;
            KindColor = kindColor;
            OKButtonFocus = true;
            MessageBeep(1);
        }

        public void Init(object content)
        {
            Content = content.ToString();
            Title = string.Empty;
            Kind = PackIconKind.Information;
            KindColor = ColorProvider.GetSolidColorBrush(eColor.RoyalBlue);
            OKButtonFocus = true;
            MessageBeep(1);
        }
    }
}