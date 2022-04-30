using System.ComponentModel.DataAnnotations;
using System;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;

namespace Aeon.Base
{
	public class LoginModel
	{
		[Required] public string Name { get; set; }

		[Required, UIHint("password")] public string Password { get; set; }

		internal string ReturnUrl { get; set; } = "/";
	}

	public class TokenResultVM
	{
		public string Token { get; set; } = null;
		public string[] Errors { get; set; } = null;

		public bool Ok => Token != null && Errors == null;

		public static TokenResultVM Success(JwtSecurityToken token) =>
			new TokenResultVM() { Token = new JwtSecurityTokenHandler().WriteToken(token) };

		public static TokenResultVM Error(params string[] errors) => new TokenResultVM() { Errors = errors };
	}

	public class AccountInfo
	{
		public string NickName { get; set; }
	}
}
