using System.Windows.Media;
using FCP.MVVM.ViewModels;
using FCP.src.Factory.ViewModel;
using MaterialDesignThemes.Wpf;

namespace FCP.src
{
    static class Message
    {
        public static void Show(object content, object title, PackIconKind kind, Color kindColor) => _MsgBVM.Show(content, title, kind, kindColor);
        public static void Show(object content) => _MsgBVM.Show(content);
        private static MsgBViewModel _MsgBVM=> MsgBFactory.GenerateMsgBViewModel();
    }
}
