using Microsoft.Extensions.Caching.Memory;

namespace Bu.Web.Data.Contexts;

public sealed class AdminCachedContext : CommonCachedContext<IAdminContext, AdminCachedContext.AdminCache>, IAdminCachedContext
{
    private static readonly TimeSpan CacheExpiryTimeSpan = TimeSpan.FromMinutes(5);

    public AdminCachedContext(AdminContextProvider adminContextProvider, IMemoryCache memoryCache)
        : base(adminContextProvider.Create, memoryCache, CacheExpiryTimeSpan)
    {
    }

    public ILookup<string, User> UsersByEmail => this.GetCache().UsersByEmail;

    public ILookup<int, User> UsersByUserId => this.GetCache().UsersByUserId;

    public ILookup<int, UserRole> UserRoles => this.GetCache().UserRoles;

    public IReadOnlyList<UserArea> UserAreas => this.GetCache().UserAreas;

    public sealed class AdminCache : CommonCache, IAdminCachedContext
    {
        public override void Init(IAdminContext context)
        {
            this.UsersByEmail = context.Users.ToLookup(u => u.Email);
            this.UsersByUserId = context.Users.ToLookup(u => u.UserId);
            this.UserAreas = context.UserAreas.ToList();
            this.UserRoles = context.UserRoles.ToLookup(r => r.UserId);
        }

        public ILookup<string, User> UsersByEmail { get; private set; } = null!;

        public ILookup<int, User> UsersByUserId { get; private set; } = null!;

        public IReadOnlyList<UserArea> UserAreas { get; private set; } = null!;

        public ILookup<int, UserRole> UserRoles { get; private set; } = null!;
    }
}