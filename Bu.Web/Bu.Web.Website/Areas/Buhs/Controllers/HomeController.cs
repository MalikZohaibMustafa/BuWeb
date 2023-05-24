namespace Bu.Web.Website.Areas.Buhs.Controllers;

public class HomeController : BuhsBaseController
{
    public IActionResult Index()
    {
        return this.View();
    }
}