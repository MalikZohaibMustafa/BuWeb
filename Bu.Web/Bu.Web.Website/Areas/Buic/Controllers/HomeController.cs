namespace Bu.Web.Website.Areas.Buic.Controllers;

public class HomeController : BuicBaseController
{
    public IActionResult Index()
    {
        return this.View();
    }
}