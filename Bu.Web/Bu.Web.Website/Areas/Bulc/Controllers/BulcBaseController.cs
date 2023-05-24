namespace Bu.Web.Website.Areas.Bulc.Controllers;

[Area("bulc")]
public abstract class BulcBaseController : BaseController
{
    protected sealed override InstituteIds InstituteId => InstituteIds.Bulc;

    protected sealed override string? AreaName => "bulc";

    protected sealed override string AreaPath => "/bulc";
}