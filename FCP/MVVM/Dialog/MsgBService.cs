using System;
using FCP.MVVM.Factory.ViewModel;

namespace FCP.MVVM.Dialog
{
    public interface IUIWindowDialogService
    {
        bool? ShowDialog(string title, object datacontext);
    }

    public class MsgBService : IUIWindowDialogService
    {
        public bool? ShowDialog(string title, object datacontext)
        {
            var win = MsgBFactory.GenerateMsgB();
            win.Title = title;
            win.DataContext = datacontext;
            return win.ShowDialog();
        }
    }
}
