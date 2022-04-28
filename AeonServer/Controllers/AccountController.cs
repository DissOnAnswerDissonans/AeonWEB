using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Aeon.Base;

namespace AeonServer.Controllers;
[Route("api/[controller]/[action]")]
[ApiController, Authorize]
public class AccountController : ControllerBase
{
	private UserManager<IdentityUser> _users;
	private SignInManager<IdentityUser> _signin;
	//private readonly ILogger<AccountController> _logger;
	public AccountController
		(UserManager<IdentityUser> userM, SignInManager<IdentityUser> signIn)
	{
		_users = userM; _signin = signIn;
	}

	[AllowAnonymous, HttpPost]
	public async Task<IActionResult> Login(LoginModel loginVM)
	{
		try {
			IdentityUser user = await _users.FindByNameAsync(loginVM.Name);
			if (user != null) {
				await _signin.SignOutAsync();
				var result = await _signin
				.PasswordSignInAsync(user, loginVM.Password, false, false);
				if (result.Succeeded) {
					var claims = await _signin.ClaimsFactory.CreateAsync(user);
					var token = new JwtSecurityToken(AuthOptions.ISSUER, AuthOptions.AUDIENCE,
					claims: claims.Claims, expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
					signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
					);
					return new JsonResult(TokenResultVM.Success(token));
				}
			}
			return new JsonResult(TokenResultVM.Error("Неверное имя или пароль"));
		} catch (Exception ex) {
			return new JsonResult(TokenResultVM.Error(ex.Message, ex.InnerException?.Message, ex.StackTrace));
		}
	}

	[AllowAnonymous, HttpPost]
	public async Task<IActionResult> Register(LoginModel model)
	{
		IdentityUser user = _users.FindByNameAsync(model.Name).Result;
		if (user is not null) return BadRequest("That user already exists");
		user = new IdentityUser(model.Name);
		IdentityResult? res = await _users.CreateAsync(user, model.Password);
		if (res.Succeeded)
		{
			return Ok();
		}
		return BadRequest(res.Errors.Select(e => e.Description));
	}
}
