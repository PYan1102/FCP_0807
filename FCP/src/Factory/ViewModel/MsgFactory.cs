using System;
using FCP.ViewModels;
using FCP.Views;

namespace FCP.src.Factory.ViewModel
{
    static class MsgFactory
    {
        private static MsgViewModel _msgVM { get; set; }

        public static Msg GenerateMsg()
        {
            return new Msg();
        }


        public static MsgViewModel GenerateMsgViewModel()
        {
            if (_msgVM == null)
                _msgVM = new MsgViewModel();
            return _msgVM;
        }
    }
}
