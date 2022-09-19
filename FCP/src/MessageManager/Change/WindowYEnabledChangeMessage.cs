using Microsoft.Toolkit.Mvvm.Messaging.Messages;

namespace FCP.src.MessageManager.Change
{
    class WindowYEnabledChangeMessage : ValueChangedMessage<bool>
    {
        public WindowYEnabledChangeMessage(bool value) : base(value)
        {

        }
    }
}
