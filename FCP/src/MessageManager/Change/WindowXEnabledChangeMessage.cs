using Microsoft.Toolkit.Mvvm.Messaging.Messages;

namespace FCP.src.MessageManager.Change
{
    class WindowXEnabledChangeMessage : ValueChangedMessage<bool>
    {
        public WindowXEnabledChangeMessage(bool value) : base(value)
        {

        }
    }
}
