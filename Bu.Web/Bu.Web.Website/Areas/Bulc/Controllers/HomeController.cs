namespace Bu.Web.Website.Areas.Bulc.Controllers;

public class HomeController : BulcBaseController
{
    public IActionResult Index()
    {
        return this.View();
    }
}