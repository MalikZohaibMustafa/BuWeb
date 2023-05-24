using System.IO.Compression;
using Bu.AspNetCore.Mvc;
using Bu.Web.Common;
using Bu.Web.Data.Contexts;
using Bu.Web.Website.RouteTransformers;
using Medallion.Threading;
using Microsoft.AspNetCore.ResponseCompression;
using Serilog;

namespace Bu.Web.Website;

public sealed class Startup : AspNetCore.Mvc.Startup
{
    private const string SqlServerConnectionStringName = "Bu.Web";

    protected override string ApplicationName => "Bahria University Website";

    protected override IApplicationClock ApplicationClock { get; } = new ApplicationClock();

    protected override LoggerConfiguration ConfigureSerilog(Serilog.ILogger logger, WebApplicationBuilder builder, LoggerConfiguration loggerConfiguration)
    {
        string connectionString = builder.Configuration.GetConnectionString(SqlServerConnectionStringName) ?? throw new InvalidOperationException();
        return SerilogHelper.ConfigureSerilog(logger, builder, loggerConfiguration, connectionString, EventLog.SourceApplications.Website);
    }

    protected override void PreConfigureServices(Serilog.ILogger logger, WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton(Log.Logger);
        string connectionString = builder.Configuration.GetConnectionString(SqlServerConnectionStringName) ?? throw new InvalidOperationException();
        string fileStorageBasePath = builder.Configuration.GetValue<string>("FileStorageBasePath") ?? throw new InvalidOperationException();
        WebsiteContextProvider websiteContextProvider = new WebsiteContextProvider(connectionString);
        builder.Services.AddSingleton(websiteContextProvider);
        builder.Services.AddSingleton<IWebsiteCachedContext, WebsiteCachedContext>();
        builder.Services.AddSingleton<IWebsiteFixedContext, WebsiteFixedContext>();
        builder.Services.AddSingleton<IFileStorage>(new LocalFileStorage(Log.Logger, fileStorageBasePath));
        builder.Services.AddSingleton<BuRouteValueTransformer>();

        builder.Services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();
        });
        builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Fastest;
        });
        builder.Services.Configure<GzipCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.SmallestSize;
        });
    }

    protected override void ConfigureInitialMiddlewares(WebApplication app)
    {
        app.UseResponseCompression();
    }

    protected override void PostConfigureServices(Serilog.ILogger logger, WebApplicationBuilder builder)
    {
    }

    protected override void ConfigureMvcBuilder(Serilog.ILogger logger, IMvcBuilder mvcBuilder)
    {
    }

    protected override IDistributedLockProvider? GetDistributedLockProvider(Serilog.ILogger logger, WebApplicationBuilder builder)
    {
        return null;
    }

    protected override void RegisterRoutes(WebApplication app)
    {
#pragma warning disable ASP0014 // Suggest using top level route registrations
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapDynamicControllerRoute<BuRouteValueTransformer>("/{part1?}/{part2?}/{part3?}/{part4?}/{part5?}/{part6?}");

            endpoints.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        });
#pragma warning restore ASP0014 // Suggest using top level route registrations
    }
}