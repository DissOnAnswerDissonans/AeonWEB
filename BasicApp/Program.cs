using Aeon.Core;
using DrawingCLI;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.CodeAnalysis.Rename;
using AeonServer;
using Aeon.Base;

namespace Aeon.BasicApp
{
	internal class Program
	{
		public static readonly Colors[] PlayerColors  = new Colors[] {
			new() { Color = ConsoleColor.Blue, BGColor = ConsoleColor.Black },
			new() { Color = ConsoleColor.DarkYellow, BGColor = ConsoleColor.Black }
		};

		private static async Task Main(string[] args)
		{
			//Console.Clear();
			//Console.SetWindowSize(80, 25);
			//Console.SetBufferSize(80, 25);
			//Console.Title = "Aeon";

			//Console.ResetColor();
			//Print.Pos(3, 1, "Hello Aeon!");
			//DrawTitle();
			//Console.ReadKey();

			//Console.ResetColor();
			//Console.Clear();

			var url = @"https://localhost:2366";

			var http = new HttpClient();
			
			Console.WriteLine("Press R to register");
			var k = Console.ReadKey();

			if (k.Key == ConsoleKey.R) 
			{
				Console.WriteLine("Регистрация");
				Console.Write("Name: ");
				var name = Console.ReadLine();
				Console.Write("Pass: ");
				var pass = Console.ReadLine();

				var resp = await HttpClientJsonExtensions.PostAsJsonAsync(http, $@"{url}/api/Account/Register", 
					new LoginModel { Name = name, Password = pass });

				Console.WriteLine($"{resp.StatusCode}: {resp.RequestMessage}");
			}

			string token;

			{
				Console.Write("Name: ");
				var name = Console.ReadLine();
				Console.Write("Pass: ");
				var pass = Console.ReadLine();

				Console.WriteLine("Авторизация...");

				var resp = await HttpClientJsonExtensions.PostAsJsonAsync(http, $@"{url}/api/Account/Login",
					new LoginModel { Name = name, Password = pass });

				Console.WriteLine($"{resp.StatusCode}: {resp.RequestMessage}");
				token = await resp.Content.ReadFromJsonAsync<string>();
			}

			Console.WriteLine("Подключение...");

			HubConnection aeonConnection = new HubConnectionBuilder()
				.WithUrl($@"{url}/aeon", o => o.AccessTokenProvider = () => Task.FromResult(token))
				.Build();

			try {
				
				await aeonConnection.StartAsync();
				Console.WriteLine("Connected");
				await RoomsMenu(aeonConnection);

			}
			catch (Exception ex) {
				Console.WriteLine($"ERR: {ex.Message}\n{ex.StackTrace}");
			}
			finally {
				Console.ReadKey();
				await aeonConnection.StopAsync();
				Console.WriteLine("Disconnected");
				Console.Write("Goodbye Aeon!");
			}
		}

		public static void DrawTitle()
		{
			ColorPic pic = new(64, 8, title2_64x8);
			pic.DrawIn(8, 6);
		}

		private static readonly byte[] title1_28x4 = new byte[] {
			0, 0, 144, 144, 144, 16, 0, 0, 144, 144, 144, 144, 144, 16, 0,
			0, 144, 144, 144, 16, 0, 0, 144, 16, 0, 0, 144, 16, 0, 153, 17, 0, 0, 153, 17, 0, 153,
			17, 0, 0, 0, 0, 0, 153, 17, 0, 0, 153, 17, 0, 153, 145, 16, 0, 153, 17, 0, 153, 25, 9,
			9, 153, 17, 0, 153, 25, 9, 9, 9, 1, 0, 153, 17, 0, 0, 153, 17, 0, 153, 17, 9, 145, 153,
			17, 0, 153, 17, 0, 0, 153, 17, 0, 153, 145, 144, 144, 144, 16, 0, 9, 145, 144, 144, 25,
			1, 0, 153, 17, 0, 0, 153, 17, 0, 0, 0, 0
		};

		private static readonly byte[] title2_64x8 = new byte[] {
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 96, 96, 96, 96, 96, 96, 96, 96, 96, 96, 0, 0, 0, 0, 0, 0, 64, 64,
			64, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 16, 177, 187, 187, 176, 0, 0, 0, 0, 0, 0, 0, 96, 230,
			238, 238, 238, 238, 238, 238, 238, 14, 14, 14, 0, 0, 0, 0, 64, 196, 204, 204,
			204, 170, 170, 170, 162, 32, 0, 0, 0, 0, 221, 221, 213, 80, 0, 0, 0, 13, 221,
			221, 213, 80, 0, 0, 0, 0, 0, 0, 0, 0, 0, 17, 187, 187, 11, 27, 187, 187, 0, 0,
			0, 0, 0, 96, 230, 238, 238, 14, 14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 64, 196, 204,
			204, 12, 0, 0, 0, 0, 10, 170, 170, 162, 32, 0, 0, 221, 221, 221, 221, 213, 80,
			0, 0, 0, 13, 221, 221, 85, 0, 0, 0, 0, 0, 0, 0, 17, 187, 187, 11, 0, 1, 27,
			187, 187, 0, 0, 0, 0, 102, 238, 238, 238, 230, 230, 230, 230, 96, 0, 0, 0, 0,
			0, 0, 68, 204, 204, 64, 64, 64, 0, 0, 160, 160, 160, 170, 170, 34, 0, 0, 221,
			221, 93, 221, 221, 221, 213, 80, 0, 0, 221, 221, 85, 0, 0, 0, 0, 0, 0, 16,
			177, 187, 187, 176, 176, 176, 177, 187, 187, 176, 0, 0, 0, 102, 238, 238, 238,
			238, 238, 238, 238, 14, 0, 0, 0, 0, 0, 0, 68, 204, 204, 204, 204, 204, 15, 15,
			42, 42, 42, 170, 170, 34, 0, 0, 221, 221, 85, 0, 13, 221, 221, 221, 213, 80,
			221, 221, 85, 0, 0, 0, 0, 0, 0, 17, 187, 187, 11, 11, 11, 11, 11, 27, 187,
			187, 0, 0, 0, 102, 238, 238, 238, 96, 96, 0, 0, 0, 0, 0, 0, 0, 0, 0, 68, 204,
			204, 192, 0, 0, 0, 0, 0, 0, 160, 170, 170, 34, 0, 0, 221, 221, 85, 0, 0, 0,
			13, 221, 221, 221, 221, 221, 85, 0, 0, 0, 0, 0, 16, 177, 187, 187, 176, 0, 0,
			0, 16, 177, 187, 187, 176, 0, 0, 0, 102, 238, 238, 238, 238, 230, 230, 230,
			230, 96, 96, 96, 0, 0, 0, 4, 76, 204, 204, 192, 192, 160, 160, 170, 170, 42,
			2, 0, 0, 0, 13, 221, 221, 213, 80, 0, 0, 0, 13, 221, 221, 221, 85, 0, 0, 0,
			0, 0, 1, 11, 11, 11, 11, 0, 0, 0, 1, 11, 11, 11, 11, 0, 0, 0, 0, 6, 14, 14,
			14, 14, 14, 14, 14, 14, 14, 14, 0, 0, 0, 0, 0, 4, 12, 12, 12, 42, 42, 42, 2,
			0, 0, 0, 0, 0, 0, 0, 13, 13, 13, 5, 0, 0, 0, 0, 13, 13, 5, 0, 0, 0, 0, 0
		};

		static int _choice = 0;

		private static async Task RoomsMenu(HubConnection connection)
		{
			_choice = -2;
			while (true) {
				string[] rooms = await connection.InvokeAsync<string[]>("GetRoomsList");

				do {
					_choice = Math.Clamp(_choice, -2, rooms.Length - 1);

					Console.ResetColor();
					Console.Clear();

					for (int i = -2; i < rooms.Length; i++) {
						Console.BackgroundColor = _choice == i ?
							ConsoleColor.DarkBlue : ConsoleColor.Black;

						Console.WriteLine(i switch {
							-2 => "[New Room]",
							-1 => "[Refresh]",
							_ => rooms[i]
						});
					}
				} while (Input(Console.ReadKey()));

				if (_choice == -2) {
					Console.ResetColor();
					Console.Clear();
					Console.Write("New Room: ");

					await connection.InvokeAsync("CreateRoom", Console.ReadLine());
					return;
				} 
				else if (_choice == -1) continue;
				else {
					await connection.SendAsync("JoinRoom", rooms[_choice]);
					return;
				}
			}
		}



		private static bool Input(ConsoleKeyInfo info)
		{
			switch (info.Key) {
			case ConsoleKey.UpArrow:
				_choice -= 1; break;
			case ConsoleKey.DownArrow:
				_choice += 1; break;
			case ConsoleKey.Enter:
				return false;
			};
			return true;
		}
	}
}