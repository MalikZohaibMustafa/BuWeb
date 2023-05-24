namespace Bu.Web.Website.Controllers;

public class BuhoBaseController : BaseController
{
    protected sealed override InstituteIds InstituteId => InstituteIds.Buho;

    protected sealed override string? AreaName => null;

    protected sealed override string AreaPath => "/";
}