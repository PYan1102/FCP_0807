using System;
using FCP.ViewModels;
using FCP.Views;

namespace FCP.src.Factory.ViewModel
{
    static class MsgBFactory
    {
        private static MsgBViewModel _MsgBVM { get; set; }

        public static MsgB GenerateMsgB()
        {
            return new MsgB();
        }


        public static MsgBViewModel GenerateMsgBViewModel()
        {
            if (_MsgBVM == null)
                _MsgBVM = new MsgBViewModel();
            return _MsgBVM;
        }
    }
}
