namespace Bu.Web.Website.Areas.Ipp.Controllers;

public class HomeController : IppBaseController
{
    public IActionResult Index()
    {
        return this.View();
    }
}