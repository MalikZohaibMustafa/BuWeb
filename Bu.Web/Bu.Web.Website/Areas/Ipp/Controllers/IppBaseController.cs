namespace Bu.Web.Website.Areas.Ipp.Controllers;

[Area("ipp")]
public abstract class IppBaseController : BaseController
{
    protected sealed override InstituteIds InstituteId => InstituteIds.Ipp;

    protected sealed override string? AreaName => "ipp";

    protected sealed override string AreaPath => "/ipp";
}