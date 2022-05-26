using System;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using Aeon.Base;
using Aeon.WindowsClient;

namespace Aeon.WindowsClient.ViewModels;

internal class SignInVM : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler? PropertyChanged;

	public TrofCommand Reg => _cmdReg ??= new(() => {
		IsRegister = !IsRegister;
	});

	public TrofCommand Fire => _cmdFire ??= new(async () => {
		var url = $@"http://{Address}:{Port}";
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

	public TrofCommand<LoginModel> Debug => _cmdDebug ??= new TrofCommand<LoginModel>(async arg => {
		await Login(new HttpClient(), $@"http://{Address}:{Port}", arg);
	}, arg => true);

	private TrofCommand<LoginModel>? _cmdDebug = null;
	private TrofCommand _cmdReg = null!;
	private TrofCommand _cmdFire = null!;



	// Properties injected with PropertyChanged.Fody //

	public string NickName { get; set; } = "";
	public string Password { private get; set; } = "";
	public string ConfirmPassword { private get; set; } = "";
	public bool IsRegister { get; set; } = false;
	public string ErrorMessage { get; set; } = "";

	public string Address { get; set; } = "45.8.248.157";
	public string Port { get; set; } = "2363";

	public Visibility ConfVisibility => IsRegister ? Visibility.Visible : Visibility.Collapsed;
	public string TrText => IsRegister ? "Регистрация" : "Вход";
	public string ModeButtonText => IsRegister ? "Вход" : "Регистрация";
	public string OkText => "ПЫЩЬ!";


	private async Task Register(HttpClient http, string url, LoginModel model)
	{
		try {
			ErrorMessage = "Регистрация…";
			var resp = await HttpClientJsonExtensions.PostAsJsonAsync(http, $@"{url}/api/Account/Register", model);
			if (resp.StatusCode == System.Net.HttpStatusCode.BadRequest) {
				ErrorMessage = await resp.Content.ReadAsStringAsync();
			}
			else if (resp.StatusCode == System.Net.HttpStatusCode.OK) {
				await Login(http, url, model);
			}
		}
		catch (Exception ex) {
			ErrorMessage = ex.Message + "\n" + ex.InnerException?.Message;
		}
	}

	private async Task Login(HttpClient http, string url, LoginModel model)
	{
		try {
			ErrorMessage = "Подключение…";
			var resp = await HttpClientJsonExtensions.PostAsJsonAsync(http, $@"{url}/api/Account/Login", model);
			var result = await resp.Content.ReadFromJsonAsync<TokenResultVM>();
			if (result!.Ok) {
				await App.Inst.Connect(url, result.Token);
			} else ErrorMessage = result.Errors.Aggregate((a, b) => a + "\n" + b);
		} catch (Exception ex) {
			ErrorMessage = ex.Message + "\n" + ex.InnerException?.Message;
		}
	}
}



