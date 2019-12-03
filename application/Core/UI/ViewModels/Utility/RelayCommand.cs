using System;
using System.Windows.Input;

namespace MORR.Core.UI.ViewModels.Utility
{
    public class RelayCommand : ICommand
    {
        private readonly Predicate<object>? canExecute;
        private readonly Action<object> execute;

        public RelayCommand(Action<object> execute, Predicate<object>? canExecute = null)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            return canExecute == null || canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            execute?.Invoke(parameter);
        }
    }
}