namespace Bu.Web.Website.Controllers;

[AllowAnonymous]
public abstract class BaseController : Controller
{
    protected abstract InstituteIds InstituteId { get; }

    protected abstract string? AreaName { get; }

    protected abstract string AreaPath { get; }
}