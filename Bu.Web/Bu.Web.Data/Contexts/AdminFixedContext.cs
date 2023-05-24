namespace Bu.Web.Data.Contexts;

public sealed class AdminFixedContext : CommonFixedContext<IAdminContext>, IAdminFixedContext
{
    public AdminFixedContext(AdminContextProvider adminContextProvider)
        : base(adminContextProvider.Create)
    {
    }
}