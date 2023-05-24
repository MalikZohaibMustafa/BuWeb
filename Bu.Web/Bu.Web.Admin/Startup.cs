using Medallion.Threading;
using Medallion.Threading.SqlServer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Serilog;
using EventLog = Bu.Web.Data.Entities.EventLog;

namespace Bu.Web.Admin;

internal sealed class Startup : AspNetCore.Mvc.Startup
{
    private const string SqlServerConnectionStringName = "Bu.Web";

    protected override string ApplicationName => AboutSystem.SystemName;

    protected override IApplicationClock ApplicationClock { get; } = new ApplicationClock
    {
        ApplicationTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time"),
        LocalTimeZone = TimeZoneInfo.Local,
        GetUtcNow = () => DateTime.UtcNow,
        UserTimeZoneIdClaimType = nameof(IApplicationClock.UserTimeZoneIdClaimType),
    };

    protected override IEnumerable<string> AllowedCorsOrigins
    {
        get
        {
            yield return @"https://login.microsoftonline.com/75df096c-8b72-48e4-9b91-cbf79d87ee3a/oauth2/v2.0/authorize?client_id=76b99122-398c-4693-92d3-e544ac9b275f&";
        }
    }

    protected override ServiceLifetime? NonceLifetime => ServiceLifetime.Scoped;

    protected override Func<Nonce?, IReadOnlyDictionary<string, StringValues>> GetCustomHeaders { get; } = nonce =>
    {
        if (nonce == null)
        {
            throw new ArgumentNullException(nameof(nonce));
        }

        List<string[]> sources = new List<string[]>
        {
            new[] { "default-src", "'self'", },
            new[] { "script-src", "'self'", $"'nonce-{nonce.Value}'", "'unsafe-inline'", },
            new[] { "font-src", "'self'", "https://fonts.gstatic.com/", },
            new[] { "style-src", "'self'", "https://fonts.googleapis.com/css2", },
            new[] { "img-src", "'self'", "data:", },
        };

        string contentSecurityPolicy = string.Join("; ", sources.Select(s => string.Join(" ", s)));
        return new Dictionary<string, StringValues>
        {
            [HeaderNames.ContentSecurityPolicy] = contentSecurityPolicy,
        }.AsReadOnly();
    };

    protected override LoggerConfiguration ConfigureSerilog(Serilog.ILogger logger, WebApplicationBuilder builder, LoggerConfiguration loggerConfiguration)
    {
        string connectionString = builder.Configuration.GetConnectionString(SqlServerConnectionStringName) ?? throw new InvalidOperationException();
        return SerilogHelper.ConfigureSerilog(logger, builder, loggerConfiguration, connectionString, EventLog.SourceApplications.Admin);
    }

    protected override void PreConfigureServices(Serilog.ILogger logger, WebApplicationBuilder builder)
    {
        string connectionString = builder.Configuration.GetConnectionString(SqlServerConnectionStringName) ?? throw new InvalidOperationException();
        string fileStorageBasePath = builder.Configuration.GetValue<string>("FileStorageBasePath") ?? throw new InvalidOperationException();
        builder.Services.AddSingleton(Log.Logger);
        SqlServerCache.Configure(logger, builder, connectionString, "dbo", "MemoryCache", true);
        MicrosoftIdentity.Configure(logger, builder, "/AccessDenied", true);
        AdminContextProvider adminContextProvider = new AdminContextProvider(connectionString);
        builder.Services.AddMemoryCache();
        builder.Services.AddSingleton(adminContextProvider);
        builder.Services.AddSingleton<IAdminCachedContext, AdminCachedContext>();
        builder.Services.AddSingleton<IAdminFixedContext, AdminFixedContext>();
        builder.Services.AddSingleton<IClaimsTransformation, RoleClaimsTransformer>();
        builder.Services.AddSingleton<FirewallConfig>();
        builder.Services.AddSingleton<Firewall>();
        builder.Services.AddSingleton<IFileStorage>(new LocalFileStorage(Log.Logger, fileStorageBasePath));

        builder.Services.AddAuthorization(options =>
        {
            foreach (AuthorizationPolicies authorizationPolicy in Enum.GetValues<AuthorizationPolicies>())
            {
                AuthorizationPolicyBuilder policyBuilder = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireClaim(nameof(UserRole.UserId));

                policyBuilder = authorizationPolicy switch
                {
                    AuthorizationPolicies.SuperAdministrator => policyBuilder.RequireRole(nameof(UserRole.Roles.SuperAdministrator)),
                    AuthorizationPolicies.Administrator => policyBuilder.RequireRole(nameof(UserRole.Roles.Administrator)),
                    AuthorizationPolicies.Webmaster => policyBuilder.RequireRole(nameof(UserRole.Roles.Webmaster)),
                    _ => throw new InvalidOperationException(authorizationPolicy.ToString()),
                };

                options.AddPolicy(authorizationPolicy.ToString(), policyBuilder.Build());
            }

            options.AddPolicy(
                "NoSuchPolicy",
                policyBuilder => policyBuilder
                    .RequireAuthenticatedUser()
                    .RequireClaim(nameof(UserRole.UserId))
                    .RequireRole("NoSuchRole"));

            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireClaim(nameof(UserRole.UserId))
                .Build();
        });
    }

    protected override void PostConfigureServices(Serilog.ILogger logger, WebApplicationBuilder builder)
    {
    }

    protected override void ConfigureMvcBuilder(Serilog.ILogger logger, IMvcBuilder mvcBuilder)
    {
        MicrosoftIdentity.Configure(logger, mvcBuilder);
    }

    protected override IDistributedLockProvider GetDistributedLockProvider(Serilog.ILogger logger, WebApplicationBuilder builder)
    {
        string connectionString = builder.Configuration.GetConnectionString(SqlServerConnectionStringName) ?? throw new InvalidOperationException();
        return new SqlDistributedSynchronizationProvider(connectionString);
    }

    protected override void ConfigureInitialMiddlewares(WebApplication app)
    {
        app.UseMiddleware<Firewall>();
        base.ConfigureInitialMiddlewares(app);
    }
}