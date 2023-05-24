namespace Bu.Web.Website.Areas.Bukc.Controllers;

public class HomeController : BukcBaseController
{
    public IActionResult Index()
    {
        return this.View();
    }
}