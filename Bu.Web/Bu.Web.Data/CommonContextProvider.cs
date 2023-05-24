namespace Bu.Web.Data;

public sealed class CommonContextProvider : IServiceProvider
{
    private readonly string connectionString;

    public CommonContextProvider(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public ICommonContext Create()
    {
        return new WebDbContext(this.connectionString);
    }

    public object? GetService(Type serviceType)
    {
        return serviceType == typeof(ICommonContext) ? this.Create() : (object?)null;
    }
}