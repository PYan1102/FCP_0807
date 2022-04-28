using System.Windows.Media;
using FCP.ViewModels;
using FCP.src.Factory.ViewModel;
using MaterialDesignThemes.Wpf;

namespace FCP.src
{
    internal class MsgCollection
    {
        private static MsgViewModel _msgVM => MsgFactory.GenerateMsgViewModel();
        public static void Show(object content, object title, PackIconKind kind, Color kindColor) => _msgVM.Show(content, title, kind, kindColor);
        public static void Show(object content) => _msgVM.Show(content);
    }
}
