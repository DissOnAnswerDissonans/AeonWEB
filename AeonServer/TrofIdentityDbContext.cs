using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.Extensions.Options;
using Duende.IdentityServer.EntityFramework.Options;

namespace AeonServer;

public class TrofIdentityDbContext : ApiAuthorizationDbContext<IdentityUser>
{
	public TrofIdentityDbContext(DbContextOptions<TrofIdentityDbContext> options, IOptions<OperationalStoreOptions> options1)
		: base(options, options1) => Database.EnsureCreated();
}
