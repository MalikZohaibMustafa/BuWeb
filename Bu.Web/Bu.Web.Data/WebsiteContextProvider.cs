namespace Bu.Web.Data;

public sealed class WebsiteContextProvider : IServiceProvider
{
    private readonly string connectionString;

    public WebsiteContextProvider(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public IWebsiteContext Create()
    {
        WebDbContext webDbContext = new WebDbContext(this.connectionString);
        webDbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        return webDbContext;
    }

    public object? GetService(Type serviceType)
    {
        return serviceType == typeof(IWebsiteContext) ? this.Create() : (object?)null;
    }
}