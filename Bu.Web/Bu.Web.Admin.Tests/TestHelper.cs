using System.Security.Claims;
using Bu.AspNetCore.Core.Services;
using Bu.AspNetCore.Mvc;
using Bu.Web.Data;
using Bu.Web.Data.Abstraction;
using Bu.Web.Data.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Serilog;

namespace Bu.Web.Admin.Tests;

internal static class TestHelper
{
    public const int SuperAdministratorUserId = 1;
    public const int ItDirectorateDepartmentId = 1;
    private const string SqlServerConnectionStringName = "Bu.Web";

    static TestHelper()
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        builder.Configuration.AddJsonFile("appsettings.json");
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

        builder.Services.AddLogging();
        IApplicationClock applicationClock = new ApplicationClock();
        DateTimeHelper.InitClock(applicationClock);
        builder.Services.AddSingleton(applicationClock);

        builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        builder.Services.AddSession();
        builder.Services.AddControllersWithViews(options =>
        {
            AuthorizationPolicy policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
            options.Filters.Add(new AuthorizeFilter(policy));
        }).AddSessionStateTempDataProvider();

        BuWebConnectionString = builder.Configuration.GetConnectionString(SqlServerConnectionStringName) ?? throw new InvalidOperationException();
        string fileStorageBasePath = builder.Configuration.GetValue<string>("FileStorageBasePath") ?? throw new InvalidOperationException();
        builder.Services.AddSingleton(new AdminContextProvider(BuWebConnectionString));
        IFileStorage fileStorage = new LocalFileStorage(Log.Logger, fileStorageBasePath);
        builder.Services.AddSingleton(fileStorage);

        WebApplication = builder.Build();
        AdminContextProvider = new AdminContextProvider(BuWebConnectionString);
        FileStorage = fileStorage;
    }

    public static string BuWebConnectionString { get; }

    private static WebApplication WebApplication { get; }

    public static AdminContextProvider AdminContextProvider { get; }

    public static IFileStorage FileStorage { get; }

    public static readonly ClaimsPrincipal SuperAdministratorClaimPrincipal = new ClaimsPrincipal(new ClaimsIdentity(
        new[]
        {
            new Claim("UserId", SuperAdministratorUserId.ToString()),
            new Claim(DateTimeHelper.Clock.UserTimeZoneIdClaimType, TimeZoneInfo.Local.Id),
        }));

    public static DateTime SuperAdministratorUserNow => DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);

    public static DateTime SuperAdministratorUtcNow => DateTime.UtcNow;

    public static ILogger<T> CreateLogger<T>() => WebApplication.Services.GetService<ILoggerFactory>()?.CreateLogger<T>() ?? throw new InvalidOperationException();

    public static IAdminFixedContext AdminFixedContext => new AdminFixedContext(AdminContextProvider);

    public static ClaimsPrincipal SuperAdminUserClaimsPrincipal => new ClaimsPrincipal(new ClaimsIdentity(new[]
    {
        new Claim("UserTimeZoneId", DateTimeHelper.Clock.LocalTimeZone.Id),
        new Claim("UserId", SuperAdministratorUserId.ToString()),
    }));

    public static T CreateController<T>(Func<T> createController)
        where T : Controller
    {
        T controller = createController();
        HttpContext httpContext = new DefaultHttpContext
        {
            User = TestHelper.SuperAdminUserClaimsPrincipal,
        };

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext,
        };

        ITempDataProvider tempDataProvider = Mock.Of<ITempDataProvider>();
        TempDataDictionaryFactory tempDataDictionaryFactory = new TempDataDictionaryFactory(tempDataProvider);
        controller.TempData = tempDataDictionaryFactory.GetTempData(httpContext);
        return controller;
    }
}
