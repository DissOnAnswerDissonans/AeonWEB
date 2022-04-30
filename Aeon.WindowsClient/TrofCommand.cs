using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Aeon.WindowsClient;

public class TrofCommand : ICommand
{
	private Action _execute;
	private Func<bool>? _canExecute;
	public event EventHandler CanExecuteChanged {
		add { CommandManager.RequerySuggested += value; }
		remove { CommandManager.RequerySuggested -= value; }
	}

	public TrofCommand(Action execute, Func<bool>? canExecute = null)
	{
		this._execute = execute;
		this._canExecute = canExecute;
	}

	public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;
	public void Execute(object? parameter) => _execute.Invoke();
	public void Execute() => _execute.Invoke();
	//public void Update() => CanExecuteChanged?.Invoke(this, new());
}

public class TrofCommand2 : ICommand
{
	private Action<object> _execute;
	private Func<object, bool>? _canExecute;
	public event EventHandler CanExecuteChanged {
		add { CommandManager.RequerySuggested += value; }
		remove { CommandManager.RequerySuggested -= value; }
	}

	public TrofCommand2(Action<object> execute, Func<object, bool>? canExecute = null)
	{
		this._execute = execute;
		this._canExecute = canExecute;
	}

	public bool CanExecute(object parameter) => _canExecute?.Invoke(parameter) ?? true;
	public void Execute(object parameter) => _execute.Invoke(parameter);
	//public void Update() => CanExecuteChanged?.Invoke(this, new());
}

public class TrofCommand<T> : ICommand
{
	private Action<T> _execute;
	private Func<T, bool>? _canExecute;
	public event EventHandler CanExecuteChanged {
		add { CommandManager.RequerySuggested += value; }
		remove { CommandManager.RequerySuggested -= value; }
	}

	public TrofCommand(Action<T> execute, Func<T, bool>? canExecute = null)
	{
		this._execute = execute;
		this._canExecute = canExecute;
	}

	public bool CanExecute(T parameter) => _canExecute?.Invoke(parameter) ?? true;
	public void Execute(T parameter) => _execute?.Invoke(parameter);
	public bool CanExecute(object? parameter)
	{
		if (parameter is not T tparam) return false;
		return CanExecute(tparam);
	}
	public void Execute(object? parameter)
	{
		if (parameter is not T tparam) return;
		Execute(tparam);
	}
}
