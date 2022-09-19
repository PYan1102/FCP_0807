using FCP.Models;
using Microsoft.Toolkit.Mvvm.Messaging.Messages;

namespace FCP.src.MessageManager.Change
{
    internal class MainUIChangeMessage : ValueChangedMessage<MainUILayoutModel>
    {
        public MainUIChangeMessage(MainUILayoutModel model) : base(model)
        {

        }
    }
}