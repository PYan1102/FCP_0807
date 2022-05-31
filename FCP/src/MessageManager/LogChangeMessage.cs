using Microsoft.Toolkit.Mvvm.Messaging.Messages;
using System.Text;

namespace FCP.src.MessageManager
{
    internal class LogChangeMessage : ValueChangedMessage<StringBuilder>
    {
        public LogChangeMessage(StringBuilder sb) : base(sb)
        {

        }
    }
}
