using FCP.Models;
using FCP.src.MessageManager.Change;
using FCP.src.MessageManager.Request;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Windows;

namespace FCP.ViewModels
{
    class WindowPositionViewModel : ObservableValidator
    {
        public WindowPositionViewModel()
            : this(WeakReferenceMessenger.Default)
        {
        }

        public WindowPositionViewModel(IMessenger messenger)
        {
            _messenger = messenger;
            _model = new WindowPositionModel();
            WindowX = Properties.Settings.Default.X;
            WindowY = Properties.Settings.Default.Y;

            Init();
        }

        private void Init()
        {
            _messenger.Register<WindowPositionVisibilityChangeMessage>(this, (r, m) => Visibility = m.Value);

            _messenger.Register<WindowXEnabledChangeMessage>(this, (r, m) => WindowXEnabled = m.Value);

            _messenger.Register<WindowYEnabledChangeMessage>(this, (r, m) => WindowYEnabled = m.Value);

            _messenger.Register<WindowXRequestMessage>(this, (r, m) => m.Reply(WindowX));

            _messenger.Register<WindowYRequestMessage>(this, (r, m) => m.Reply(WindowY));

            _messenger.Register<HasErrorsRequestMessage, string>(this, nameof(WindowPositionViewModel), (r, m) => m.Reply(this.HasErrors));
        }

        [Required(ErrorMessage = "該欄位不可為空", AllowEmptyStrings = false)]
        [RegularExpression("[0-9]+", ErrorMessage = "該欄位只能為數字")]
        [Range(0, 2048, ErrorMessage = "您只能輸入 0 ~ 2048 之間的數值")]
        public int WindowX
        {
            get => _model.WindowX;
            set
            {
                SetProperty(_model.WindowX, value, _model, (model, _value) => model.WindowX = _value, true);
            }
        }

        [Required(ErrorMessage = "該欄位不可為空")]
        [RegularExpression("[0-9]+", ErrorMessage = "該欄位只能為數字")]
        [Range(0, 2048, ErrorMessage = "您只能輸入 0 ~ 2048 之間的數值")]
        public int WindowY
        {
            get => _model.WindowY;
            set => SetProperty(_model.WindowY, value, _model, (model, _value) => model.WindowY = _value, true);
        }

        public bool WindowXEnabled
        {
            get => _model.WindowXEnabled;
            set => SetProperty(_model.WindowXEnabled, value, _model, (model, _value) => model.WindowXEnabled = _value);
        }

        public bool WindowYEnabled
        {
            get => _model.WindowYEnabled;
            set => SetProperty(_model.WindowYEnabled, value, _model, (model, _value) => model.WindowYEnabled = _value);
        }

        public Visibility Visibility
        {
            get => _model.Visibility;
            set => SetProperty(_model.Visibility, value, _model, (model, _value) => model.Visibility = _value);
        }

        private IMessenger _messenger;
        private WindowPositionModel _model;
    }
}
