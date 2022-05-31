using FCP.Models;
using Microsoft.Toolkit.Mvvm.Messaging.Messages;

namespace FCP.src.MessageManager
{
    internal class UpdateMainUIChangeMessage : ValueChangedMessage<MainUILayoutModel>
    {
        public UpdateMainUIChangeMessage(MainUILayoutModel model) : base(model)
        {

        }
    }
}