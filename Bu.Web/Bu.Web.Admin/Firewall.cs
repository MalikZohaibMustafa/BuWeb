using Bu.Web.Admin.Properties;

namespace Bu.Web.Admin;

public sealed class Firewall : IMiddleware
{
    private readonly FirewallConfig firewallConfig;
    private readonly Serilog.ILogger logger;

    public Firewall(Serilog.ILogger logger, FirewallConfig firewallConfig)
    {
        this.logger = logger;
        this.firewallConfig = firewallConfig;
    }

    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var ipAddress = context.Connection.RemoteIpAddress;
        if (this.firewallConfig.WhitelistedIpAddresses.Any() && ipAddress != null && this.firewallConfig.WhitelistedIpAddresses.Contains(ipAddress))
        {
            return next(context);
        }
        else
        {
            this.logger.Warning("Traffic from ip address {IpAddress} has been blocked. Url: {Url}", ipAddress?.ToString() ?? "N/A", context.GetDisplayUrl());
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            context.Response.ContentType = "text/html; charset=utf-8";
            context.Response.Headers.CacheControl = "no-store,no-cache";
            return context.Response.WriteAsync(Resources.ForbiddenHtml);
        }
    }
}