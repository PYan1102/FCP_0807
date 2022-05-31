using System.Windows.Media;
using FCP.ViewModels;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using FCP.Views;

namespace FCP.src
{
    internal class MsgCollection
    {
        public static void ShowDialog(object content, object title, PackIconKind kind, SolidColorBrush kindColor)
        {
            MsgViewModel viewModel = App.Current.Services.GetService<MsgViewModel>();
            MsgView view = new MsgView(viewModel);
            viewModel.Init(content, title, kind, kindColor);
            view.ShowDialog();
        }

        public static void ShowDialog(object content)
        {
            MsgViewModel viewModel = App.Current.Services.GetService<MsgViewModel>();
            MsgView view = new MsgView(viewModel);
            viewModel.Init(content);
            view.ShowDialog();
        }
    }
}
