using Microsoft.Toolkit.Mvvm.Messaging.Messages;
using System.Windows;

namespace FCP.src.MessageManager.Change
{
    class WindowPositionVisibilityChangeMessage : ValueChangedMessage<Visibility>
    {
        public WindowPositionVisibilityChangeMessage(Visibility visibility) : base(visibility)
        {

        }
    }
}
