using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Bu.Web.Data.Abstraction;

public interface IAdminContext : IWebsiteContext
{
    public DbSet<User> Users { get; }

    public DbSet<UserRole> UserRoles { get; }

    public DbSet<UserArea> UserAreas { get; }

    public DbSet<WhitelistedIpAddress> WhitelistedIpAddresses { get; }

    public DatabaseFacade Database { get; }

    public int SaveChanges(ClaimsPrincipal claimsPrincipal);
}