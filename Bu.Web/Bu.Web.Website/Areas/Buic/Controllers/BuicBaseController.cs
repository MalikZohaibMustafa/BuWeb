namespace Bu.Web.Website.Areas.Buic.Controllers;

[Area("buic")]
public abstract class BuicBaseController : BaseController
{
    protected sealed override string AreaPath => "/buic";

    protected sealed override string? AreaName => "buic";

    protected override InstituteIds InstituteId => InstituteIds.Buic;
}