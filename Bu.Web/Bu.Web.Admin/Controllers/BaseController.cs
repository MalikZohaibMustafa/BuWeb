namespace Bu.Web.Admin.Controllers;

[Authorize]
[AutoValidateAntiforgeryToken]
public abstract class BaseController<TController> : Controller
    where TController : BaseController<TController>
{
    private readonly Lazy<DateTime> lazyUtcNow = new Lazy<DateTime>(() => DateTimeHelper.UtcNow);

    protected DateTime UtcNow => this.lazyUtcNow.Value;

    protected DateTime UserNow => this.lazyUtcNow.Value.ToUserDateTimeFromUtc(this.User);

    protected BaseController(ILogger<TController> logger, AdminContextProvider adminContextProvider)
    {
        this.Logger = logger;
        this.AdminContextProvider = adminContextProvider;
    }

    protected ILogger<TController> Logger { get; }

    protected AdminContextProvider AdminContextProvider { get; }

    protected int UserId => this.User.FindFirstValue(nameof(this.UserId))!.ToInt();
}