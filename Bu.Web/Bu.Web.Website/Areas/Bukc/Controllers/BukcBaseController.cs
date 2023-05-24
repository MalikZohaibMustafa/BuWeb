namespace Bu.Web.Website.Areas.Bukc.Controllers;

[Area("bukc")]
public abstract class BukcBaseController : BaseController
{
    protected sealed override InstituteIds InstituteId => InstituteIds.Bukc;

    protected sealed override string? AreaName => "bukc";

    protected sealed override string AreaPath => "/bukc";
}