using Bu.Web.Admin.Areas.Admin.Models.EventLogs;

namespace Bu.Web.Admin.Areas.Admin.Controllers;

[Authorize(nameof(AuthorizationPolicies.Administrator))]
public sealed class EventLogsController : AdminAreaBaseController<EventLogsController>
{
    public EventLogsController(ILogger<AdminAreaBaseController<EventLogsController>> logger, AdminContextProvider adminContextProvider)
        : base(logger, adminContextProvider)
    {
    }

    public IActionResult Index(ListInputModel? input)
    {
        return this.ModelState.IsValid
            ? this.View(nameof(this.Index), new ListModel(this.AdminContextProvider, input ?? new ListInputModel(), this.User))
            : this.View(nameof(this.Index), new ListModel(input ?? new ListInputModel(), this.User));
    }

    [HttpGet]
    public IActionResult Display(int id)
    {
        using IAdminContext adminContext = this.AdminContextProvider.Create();
        var eventLog = adminContext.EventLogs.SingleOrDefault(le => le.EventLogId == id);
        return eventLog == null
            ? this.NotFound()
            : this.View(nameof(this.Display), eventLog);
    }
}