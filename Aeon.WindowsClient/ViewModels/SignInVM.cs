using System;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using Aeon.Base;

namespace Aeon.WindowsClient.ViewModels;

internal class SignInVM : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler? PropertyChanged;

	public TrofCommand Reg => _cmdReg ??= new(() => {
		IsRegister = !IsRegister;
	});

	public TrofCommand Fire => _cmdFire ??= new(async () => {
		var url = @"https://localhost:2366";
		using var http = new HttpClient();
		var model = new LoginModel { Name = NickName, Password = Password };
		if (IsRegister) {
			if (Password == ConfirmPassword)
				await Register(http, url, model);		
			else {
				ErrorMessage = "Пароли не совпадают";
			}
		} 
		else await Login(http, url, model);
		
	}, () => {
		return !string.IsNullOrEmpty(NickName)
		&& Password.Length > 0
		&& (!IsRegister || ConfirmPassword.Length > 0);
	});

	private TrofCommand _cmdReg = null!;
	private TrofCommand _cmdFire = null!;

	// Properties injected with PropertyChanged.Fody //

	public string NickName { get; set; } = "";
	public string Password { private get; set; } = "";
	public string ConfirmPassword { private get; set; } = "";
	public bool IsRegister { get; set; } = false;
	public string ErrorMessage { get; set; } = "";

	public Visibility ConfVisibility => IsRegister ? Visibility.Visible : Visibility.Collapsed;
	public string TrText => IsRegister ? "Регистрация" : "Вход";
	public string ModeButtonText => IsRegister ? "Вход" : "Регистрация";
	public string OkText => "ПЫЩЬ!";



	private async Task Register(HttpClient http, string url, LoginModel model)
	{
		var resp = await HttpClientJsonExtensions.PostAsJsonAsync(http, $@"{url}/api/Account/Register", model);
		if (resp.StatusCode == System.Net.HttpStatusCode.BadRequest) {
			ErrorMessage = await resp.Content.ReadAsStringAsync();
		}
		else if (resp.StatusCode == System.Net.HttpStatusCode.OK) {
			await Login(http, url, model);
		}
	}

	private async Task Login(HttpClient http, string url, LoginModel model)
	{
		try {
			var resp = await HttpClientJsonExtensions.PostAsJsonAsync(http, $@"{url}/api/Account/Login", model);
			var result = await resp.Content.ReadFromJsonAsync<TokenResultVM>();
			if (result!.Ok) {
				ErrorMessage = "Подключение";
				await App.Inst.Connect(result.Token);
			} else ErrorMessage = result.Errors.Aggregate((a, b) => a + "\n" + b);
		} catch (Exception ex) {
			ErrorMessage = ex.Message;
		}
	}
}


public class TrofCommand : ICommand
{
	private Action _execute;
	private Func<bool>? _canExecute;
	public event EventHandler? CanExecuteChanged {
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
	//public void Update() => CanExecuteChanged?.Invoke(this, new());
}

public class TrofCommand<T> : ICommand
{
	private Action<T> _execute;
	private Func<T, bool>? _canExecute;
	public event EventHandler? CanExecuteChanged {
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
	public bool CanExecute(object? parameter) => CanExecute((T?) parameter ?? throw new NotSupportedException());
	public void Execute(object? parameter) => Execute((T?) parameter ?? throw new NotSupportedException());
}
