namespace Bu.Web.Admin.Areas.Admin;

[Area(nameof(Admin))]
public abstract class AdminAreaBaseController<TController> : BaseController<AdminAreaBaseController<TController>>
    where TController : AdminAreaBaseController<TController>
{
    protected AdminAreaBaseController(ILogger<AdminAreaBaseController<TController>> logger, AdminContextProvider adminContextProvider)
        : base(logger, adminContextProvider)
    {
    }
}