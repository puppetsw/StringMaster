using System;
using System.Diagnostics;
using System.Windows.Input;

namespace StringMaster;

public class RelayCommand<T> : ICommand
{
    private readonly Predicate<T> _canExecute;

    private readonly Action<T> _execute;

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    protected RelayCommand(Action<T> execute, Predicate<T> canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public void Execute(object parameter)
    {
        _execute((T)parameter);
    }

    [DebuggerStepThrough]
    public bool CanExecute(object parameter)
    {
        return _canExecute == null || _canExecute((T)parameter);
    }
}

public class RelayCommand : RelayCommand<object>
{
    public RelayCommand(Action execute) : this(execute, () => true)
    {
    }

    public RelayCommand(Action execute, Func<bool> canExecute) :
        base(_ => execute(), _ => canExecute())
    {
    }
}