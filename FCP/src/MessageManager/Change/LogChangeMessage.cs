using Microsoft.Toolkit.Mvvm.Messaging.Messages;

namespace FCP.src.MessageManager.Change
{
    internal class LogChangeMessage : ValueChangedMessage<string>
    {
        public LogChangeMessage(string value) : base(value)
        {
        }
    }
}
