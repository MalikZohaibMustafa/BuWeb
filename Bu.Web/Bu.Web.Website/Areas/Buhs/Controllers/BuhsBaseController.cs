namespace Bu.Web.Website.Areas.Buhs.Controllers;

[Area("buhs")]
public abstract class BuhsBaseController : BaseController
{
    protected sealed override string AreaPath => "/buhs";

    protected sealed override string? AreaName => "buhs";

    protected override InstituteIds InstituteId => InstituteIds.Buhs;
}