using Microsoft.AspNetCore.Authentication;

namespace Bu.Web.Admin;

public sealed class RoleClaimsTransformer : IClaimsTransformation
{
    private readonly ILogger<RoleClaimsTransformer> logger;
    private readonly IAdminCachedContext adminCachedContext;
    private readonly IApplicationClock applicationClock;

    public RoleClaimsTransformer(ILogger<RoleClaimsTransformer> logger, IAdminCachedContext adminCachedContext, IApplicationClock applicationClock)
    {
        this.adminCachedContext = adminCachedContext;
        this.logger = logger;
        this.applicationClock = applicationClock;
    }

    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity is ClaimsIdentity claimsIdentity && claimsIdentity.Name != null)
        {
            string email = claimsIdentity.Name.ToLowerInvariant();
            User? user = this.adminCachedContext.UsersByEmail[email].SingleOrDefault(); // Email
            if (user != null)
            {
                if (user.Status == User.Statuses.Active)
                {
                    DateTime utcNow = DateTimeHelper.UtcNow;
                    if (user.ExpiryDateUtc == null || user.ExpiryDateUtc >= utcNow)
                    {
                        List<Claim> roleClaims = this.adminCachedContext.UserRoles[user.UserId]
                            .Where(r => r.Status == UserRole.Statuses.Active)
                            .Where(r => r.ExpiryDateUtc == null || r.ExpiryDateUtc >= utcNow)
                            .Select(r => new Claim(claimsIdentity.RoleClaimType, r.Role.ToString()))
                            .ToList();

                        if (roleClaims.Any())
                        {
                            claimsIdentity.AddClaim(new Claim(nameof(user.UserId), user.UserId.ToString(), nameof(Int32)));
                            claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, claimsIdentity.FindFirst("name")!.Value));
                            claimsIdentity.AddClaim(new Claim(ClaimTypes.Email, email));
                            claimsIdentity.AddClaim(new Claim(this.applicationClock.UserTimeZoneIdClaimType, this.applicationClock.ApplicationTimeZone.Id));
                            claimsIdentity.AddClaims(roleClaims);
                        }
                        else
                        {
                            this.logger.LogWarning($"User account '{email}' has no active role(s).");
                        }
                    }
                    else
                    {
                        this.logger.LogWarning($"User account '{email}' is expired on {user.ExpiryDateUtc}.");
                    }
                }
                else
                {
                    this.logger.LogWarning($"User account '{email}' status is '{user.Status}'.");
                }
            }
            else
            {
                this.logger.LogWarning($"User account '{email}' is not registered.");
            }
        }

        return Task.FromResult(principal);
    }
}