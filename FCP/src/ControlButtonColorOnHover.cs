using Microsoft.Xaml.Behaviors;
using System;
using System.Windows.Controls;
using FCP.Providers;
using FCP.src.Enum;
using System.Windows.Media;

namespace System.Windows.Interactivity
{
    internal class ControlButtonColorOnHover : Behavior<FrameworkElement>
    {
        protected override void OnAttached()
        {
            AssociatedObject.MouseEnter += AssociatedObject_MouseEnter;
            AssociatedObject.MouseLeave += AssociatedObject_MouseLeave;
            base.OnAttached();
        }

        void AssociatedObject_MouseLeave(object sender, Input.MouseEventArgs e)
        {
            Button button = sender as Button;
            button.Background = null;
        }

        void AssociatedObject_MouseEnter(object sender, Input.MouseEventArgs e)
        {
            Button button = sender as Button;
            button.Background = ColorProvider.GetSolidColorBrush(eColor.Blue);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.MouseEnter -= AssociatedObject_MouseEnter;
            AssociatedObject.MouseLeave -= AssociatedObject_MouseLeave;
        }
    }
}
