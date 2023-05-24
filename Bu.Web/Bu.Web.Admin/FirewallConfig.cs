using System.Net;
using Bu.Web.Data;
using Bu.Web.Data.Abstraction;
using Microsoft.Extensions.Caching.Memory;

namespace Bu.Web.Admin;

public sealed class FirewallConfig
{
    public FirewallConfig(Serilog.ILogger logger, IMemoryCache memoryCache, AdminContextProvider adminContextProvider, IWebHostEnvironment environment)
    {
        TimeSpan absoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10);
        if (environment.IsProduction())
        {
            absoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
        }

        this.GetWhitelistedIpAddresses = () => memoryCache.GetOrCreate("WhitelistedIpAddresses", entry =>
        {
            logger.Verbose("Loading Firewall Whitelisted Ip Addresses. {expiryTimeName}: {time} seconds", nameof(absoluteExpirationRelativeToNow), absoluteExpirationRelativeToNow.TotalSeconds);
            entry.AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow;
            using IAdminContext adminContext = adminContextProvider.Create();
            List<string> ipAddresses = adminContext.WhitelistedIpAddresses.Select(ip => ip.IpAddress).ToList();
            return ipAddresses.Select(IPAddress.Parse).ToHashSet();
        }) ?? throw new InvalidOperationException();
    }

    public HashSet<IPAddress> WhitelistedIpAddresses => this.GetWhitelistedIpAddresses();

    private Func<HashSet<IPAddress>> GetWhitelistedIpAddresses { get; }
}