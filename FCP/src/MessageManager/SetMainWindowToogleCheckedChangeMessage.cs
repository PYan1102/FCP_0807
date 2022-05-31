using FCP.Models;
using Microsoft.Toolkit.Mvvm.Messaging.Messages;

namespace FCP.src.MessageManager
{
    internal class SetMainWindowToogleCheckedChangeMessage : ValueChangedMessage<MainWindowModel.ToogleModel>
    {
        public SetMainWindowToogleCheckedChangeMessage(MainWindowModel.ToogleModel toogleModel) : base(toogleModel)
        {

        }
    }
}
