using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FCP.Core
{
    internal class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
