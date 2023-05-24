namespace Bu.Web.Data;

public sealed class WebDbContextFactory : IDesignTimeDbContextFactory<WebDbContext>
{
    public WebDbContext CreateDbContext(string[] args)
    {
        return new WebDbContext(new SqlConnectionStringBuilder
        {
            DataSource = ".",
            InitialCatalog = "Bu.Web",
            IntegratedSecurity = true,
            TrustServerCertificate = true,
        }.ConnectionString);
    }
}