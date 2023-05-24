namespace Bu.Web.Data;

public sealed class AdminContextProvider : IServiceProvider
{
    private readonly string connectionString;

    public AdminContextProvider(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public IAdminContext Create()
    {
        return new WebDbContext(this.connectionString);
    }

    public object? GetService(Type serviceType)
    {
        return serviceType == typeof(IAdminContext) ? this.Create() : (object?)null;
    }
}