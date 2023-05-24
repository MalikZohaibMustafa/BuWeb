namespace Bu.Web.Website.Controllers;

public class HomeController : BuhoBaseController
{
    public IActionResult Index()
    {
        return this.View();
    }
}