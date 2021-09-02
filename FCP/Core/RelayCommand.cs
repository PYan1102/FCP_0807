using System;
using System.Windows.Input;

namespace FCP.Core
{
    class ObjectRelayCommand : ICommand
    {
        private Action<object> _Execute;
        private Func<object, bool> _CanExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public ObjectRelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _Execute = execute;
            _CanExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _CanExecute == null || _CanExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _Execute(parameter);
        }
    }

    class RelayCommand : ICommand
    {
        private Action _Execute;
        private Func<bool> _CanExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _Execute = execute;
            _CanExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (_CanExecute != null)
            {
                Console.WriteLine(_CanExecute.Method);
            }
            return _CanExecute == null || _CanExecute();
        }

        public void Execute(object parameter)
        {
            _Execute();
        }
    }
}
