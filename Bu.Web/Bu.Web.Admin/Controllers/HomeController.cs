using Microsoft.Graph;
using Microsoft.Identity.Web;

namespace Bu.Web.Admin.Controllers;

[Authorize]
public sealed class HomeController : BaseController<HomeController>
{
    private readonly GraphServiceClient graphServiceClient;

    public HomeController(ILogger<HomeController> logger, AdminContextProvider adminContextProvider, GraphServiceClient graphServiceClient)
        : base(logger, adminContextProvider)
    {
        this.graphServiceClient = graphServiceClient;
    }

    public IActionResult Index()
    {
        return this.View(nameof(this.Index));
    }

    [HttpGet]
    [Route(nameof(Photo))]
    [AuthorizeForScopes(Scopes = new[] { "user.read" })]
    [ResponseCache(Duration = 1 * 24 * 60 * 60, Location = ResponseCacheLocation.Any, NoStore = false)]
    public async Task<IActionResult> Photo()
    {
        var photoStream = await this.graphServiceClient.Me.Photo.Content.Request().GetAsync();
        this.Response.Headers.ContentDisposition = $"inline;filename={this.User.Identity?.Name ?? "Photo"}.jfif";
        return new FileStreamResult(photoStream, "application/octet-stream");
    }

    [HttpGet]
    [Route("About")]
    public IActionResult About()
    {
        return this.View();
    }

    [Authorize(nameof(AuthorizationPolicies.SuperAdministrator))]
    public IActionResult TestException()
    {
        throw new InvalidOperationException("Get - Testing Exception Logging.", new Exception("Inner Exception."));
    }

    [Authorize(nameof(AuthorizationPolicies.SuperAdministrator))]
    [HttpPost]
    public IActionResult TestExceptionPost()
    {
        throw new InvalidOperationException("Post - Testing Exception Logging.", new Exception("Inner Exception."));
    }

    [HttpHead]
    [Authorize(nameof(AuthorizationPolicies.SuperAdministrator))]
    public IActionResult HttpHeadOnlyMethod()
    {
        return this.RedirectToAction(nameof(this.About));
    }

    [Authorize(nameof(AuthorizationPolicies.SuperAdministrator))]
    public IActionResult? NullReturn()
    {
        return null;
    }

    [Authorize("NoSuchPolicy")]
    public IActionResult TestUnauthorisedRole()
    {
        return this.RedirectToAction(nameof(this.About));
    }

    [Authorize("NoSuchPolicy")]
    [HttpPost]
    public IActionResult TestUnauthorisedRolePost()
    {
        return this.RedirectToAction(nameof(this.About));
    }

    [Authorize(nameof(AuthorizationPolicies.SuperAdministrator))]
    public IActionResult TestLogCritical()
    {
        this.Logger.LogCritical(1000, "Test LogCritical() with EventId");
        this.Logger.LogCritical("Test LogCritical() without EventId");
        return this.RedirectAfterLogging();
    }

    [Authorize(nameof(AuthorizationPolicies.SuperAdministrator))]
    public IActionResult TestLogError()
    {
        this.Logger.LogError(2000, "Test LogError() with EventId");
        this.Logger.LogError("Test LogError() without EventId");
        return this.RedirectAfterLogging();
    }

    [Authorize(nameof(AuthorizationPolicies.SuperAdministrator))]
    public IActionResult TestLogException()
    {
        try
        {
            throw new InvalidOperationException("TestLogException testing.", new Exception("Inner Exception Message"));
        }
        catch (Exception exc)
        {
            this.Logger.LogError(1000, exc, "Test LogError() with exception and EventId");
            this.Logger.LogError(exc, "Test LogError() with exception and without EventId");
        }

        return this.RedirectAfterLogging();
    }

    [Authorize(nameof(AuthorizationPolicies.SuperAdministrator))]
    public IActionResult TestLogWarning()
    {
        this.Logger.LogWarning(3000, "Test LogWarning() with EventId");
        this.Logger.LogWarning("Test LogWarning() without EventId");
        return this.RedirectAfterLogging();
    }

    [Authorize(nameof(AuthorizationPolicies.SuperAdministrator))]
    public IActionResult TestLogInformation()
    {
        this.Logger.LogInformation(4000, "Test LogInformation() with EventId");
        this.Logger.LogInformation("Test LogInformation() without EventId");
        return this.RedirectAfterLogging();
    }

    [Authorize(nameof(AuthorizationPolicies.SuperAdministrator))]
    public IActionResult TestLogDebug()
    {
        this.Logger.LogDebug(5000, "Test LogDebug() with EventId");
        this.Logger.LogDebug("Test LogDebug() without EventId");
        return this.RedirectAfterLogging();
    }

    [Authorize(nameof(AuthorizationPolicies.SuperAdministrator))]
    public IActionResult TestLogTrace()
    {
        this.Logger.LogTrace(6000, "Test LogTrace() with EventId");
        this.Logger.LogTrace("Test LogTrace() without EventId");
        return this.RedirectAfterLogging();
    }

    private IActionResult RedirectAfterLogging()
    {
        return this.RedirectToAction("About", "Home");
    }
}