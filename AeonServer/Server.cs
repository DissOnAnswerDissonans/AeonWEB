global using Aeon.Base;
global using AeonServer.Models;
using AeonServer;
using AeonServer.Services;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using static IdentityModel.ClaimComparer;


var builder = WebApplication.CreateBuilder(args);
var srv = builder.Services;

srv.AddDbContext<TrofIdentityDbContext>(o => o.UseNpgsql(builder.Configuration.GetConnectionString("auth")));
srv.AddDefaultIdentity<IdentityUser>(o => { 
	o.Password.RequireNonAlphanumeric = false;
	o.Password.RequireDigit = false;
	o.Password.RequireUppercase = false;
	o.Password.RequireLowercase = false;
}).AddEntityFrameworkStores<TrofIdentityDbContext>();

srv.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o => {
	o.TokenValidationParameters = new() {
		ValidateIssuer = true,
		ValidIssuer = AuthOptions.ISSUER,
		ValidateAudience = true,
		ValidAudience = AuthOptions.AUDIENCE,
		ValidateLifetime = true,
		IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
		ValidateIssuerSigningKey = true
	};
});

srv.AddEndpointsApiExplorer();
srv.AddSwaggerGen();

srv.AddCors();
srv.AddSingleton<ServerState>();
srv.AddSingleton<GameProvider>();
srv.AddSingleton<HeroesProvider>();
srv.AddSingleton<IUserIdProvider, TrofUserIdProvider>();
srv.AddSingleton<IBalanceProvider, DefaultBalanceProvider>();
srv.AddSignalR();

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
	app.UseSwagger();
	app.UseSwaggerUI(o => {
		o.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
		o.RoutePrefix = "";
	});
} else {
	app.UseExceptionHandler("/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<AeonGeneralHub>("/aeon");
app.MapHub<AeonLobbyHub>("/aeon/lobby");
app.MapHub<AeonGameHub>("/aeon/game");

app.Run();



public class AuthOptions
{
	public const string ISSUER = "MyAuthServer"; // издатель токена
	public const string AUDIENCE = "MyAuthClient"; // потребитель токена
	const string KEY = "mysupersecret_secretkey!123";   // ключ для шифрации
	public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
		new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
}