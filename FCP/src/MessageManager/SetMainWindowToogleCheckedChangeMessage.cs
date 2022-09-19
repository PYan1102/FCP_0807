using FCP.Models;
using Microsoft.Toolkit.Mvvm.Messaging.Messages;

namespace FCP.src.MessageManager
{
    internal class SetMainWindowToogleCheckedChangeMessage : ValueChangedMessage<MainWindowModel.ToggleModel>
    {
        public SetMainWindowToogleCheckedChangeMessage(MainWindowModel.ToggleModel toogleModel) : base(toogleModel)
        {

        }
    }
}
