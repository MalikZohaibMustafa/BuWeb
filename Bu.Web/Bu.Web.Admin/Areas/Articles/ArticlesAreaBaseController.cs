namespace Bu.Web.Admin.Areas.Articles;

[Area(nameof(Articles))]
[Authorize(nameof(AuthorizationPolicies.Webmaster))]
public abstract class ArticlesAreaBaseController<TController> : BaseController<ArticlesAreaBaseController<TController>>
    where TController : ArticlesAreaBaseController<TController>
{
    protected ArticlesAreaBaseController(ILogger<ArticlesAreaBaseController<TController>> logger, AdminContextProvider adminContextProvider)
        : base(logger, adminContextProvider)
    {
    }
}