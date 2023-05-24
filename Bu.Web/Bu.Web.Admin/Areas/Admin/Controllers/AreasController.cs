using Bu.Web.Admin.Areas.Admin.Models.Areas;

namespace Bu.Web.Admin.Areas.Admin.Controllers;

[Authorize(nameof(AuthorizationPolicies.Administrator))]
public sealed class AreasController : AdminAreaBaseController<AreasController>
{
    public AreasController(ILogger<AreasController> logger, AdminContextProvider adminContextProvider)
        : base(logger, adminContextProvider)
    {
    }

    public IActionResult Index()
    {
        return this.RedirectToAction(nameof(this.List), new ListInputModel());
    }

    public IActionResult List(ListInputModel input)
    {
        if (this.ModelState.IsValid)
        {
            return this.View(nameof(this.List), new ListModel(this.AdminContextProvider, input, this.User));
        }

        return this.View(nameof(this.List), new ListModel(input, this.User));
    }

    [HttpGet]
    public IActionResult Display(int id)
    {
        return this.DisplayArea(nameof(this.Display), id);
    }

    [HttpGet]
    public IActionResult New()
    {
        return this.View(nameof(this.New), new Area());
    }

    [HttpPost]
    public IActionResult Add(Area model)
    {
        if (model.AreaId != default)
        {
            return this.BadRequest();
        }

        return this.AddOrUpdate(model);
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        if (id == default)
        {
            return this.BadRequest();
        }

        return this.DisplayArea(nameof(this.Edit), id);
    }

    [HttpPost]
    public IActionResult Update(Area model)
    {
        if (model.AreaId == default)
        {
            return this.BadRequest();
        }

        return this.AddOrUpdate(model);
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        if (id == default)
        {
            return this.BadRequest();
        }

        return this.DisplayArea(nameof(this.Delete), id);
    }

    private bool IsAreaInUse(int areaId, IAdminContext adminContext)
    {
        return adminContext.AreaLayouts.Any(al => al.AreaId == areaId)
                         || adminContext.UserAreas.Any(ua => ua.AreaId == areaId)
                         || adminContext.YoutubeVideos.Any(yv => yv.AreaId == areaId);
    }

    [HttpPost]
    public IActionResult DeletePost(int id)
    {
        if (id == default)
        {
            return this.BadRequest();
        }

        using IAdminContext adminContext = this.AdminContextProvider.Create();
        Area? area = adminContext.Areas.SingleOrDefault(a => a.AreaId == id);
        if (area == null)
        {
            return this.NotFound();
        }

        if (this.IsAreaInUse(area.AreaId, adminContext))
        {
            return this.DisplayArea(nameof(this.Delete), area.Id, new List<ValidationResult>
            {
                new ValidationResult("Area cannot be deleted due to child records."),
            });
        }

        adminContext.Areas.Remove(area);
        adminContext.SaveChanges(this.User);
        this.AddSuccessMessage("Area has been deleted.");
        return this.RedirectToAction("Index", "Areas", new { area = "Admin" });
    }

    private IActionResult DisplayArea(string viewName, int id, List<ValidationResult>? validationResults = null, Area? userInput = null)
    {
        using IAdminContext adminContext = this.AdminContextProvider.Create();
        Area? area = adminContext.Areas.SingleOrDefault(a => a.AreaId == id);
        if (area == null)
        {
            return this.NotFound();
        }

        area.PostQuery(this.User);

        if (validationResults != null)
        {
            this.AddErrorMessages(validationResults);
        }

        if (userInput != null)
        {
            area.AreaName = userInput.AreaName;
        }

        return this.View(viewName, area);
    }

    private IActionResult RedirectToAreas()
    {
        return this.RedirectToAction("Index", "Areas", new { area = "Admin" });
    }

    private IActionResult AddOrUpdate(Area model)
    {
        var add = model.AreaId == default;
        if (!this.ModelState.IsValid)
        {
            return this.View(add ? nameof(this.New) : nameof(this.Edit), model);
        }

        using IAdminContext adminContext = this.AdminContextProvider.Create();
        List<ValidationResult> validationResults = new List<ValidationResult>();
        Area? area;
        if (add)
        {
            if (model.ParentAreaId == null)
            {
                validationResults.Add(new ValidationResult("Area cannot be added at a root level."));
            }

            area = adminContext.Areas.Add(new Area
            {
                ParentAreaId = model.ParentAreaId,
                InstituteId = model.InstituteId,
            }).Entity;
            area.TrackedEntity.SetCreated(this.UserNow, this.UserId);
        }
        else
        {
            area = adminContext.Areas.SingleOrDefault(a => a.AreaId == model.AreaId);
            if (area == null)
            {
                return this.NotFound();
            }

            area.PostQuery(this.User);
            if (area.ParentAreaId == null)
            {
                validationResults.Add(new ValidationResult("Root Level Area cannot be modified."));
            }
        }

        bool areaInUse = !add && this.IsAreaInUse(area.AreaId, adminContext);

        if (area.AreaName != model.AreaName)
        {
            if (areaInUse)
            {
                validationResults.Add(new ValidationResult($"{typeof(Area).GetDisplayNameForProperty(nameof(area.AreaName))} cannot be modified because child record exists."));
            }

            area.AreaName = model.AreaName;
        }

        if (area.ParentAreaId != model.ParentAreaId)
        {
            if (areaInUse)
            {
                validationResults.Add(new ValidationResult($"{typeof(Area).GetDisplayNameForProperty(nameof(area.ParentAreaId))} cannot be modified because child record exists."));
            }

            area.ParentAreaId = model.ParentAreaId;
        }

        area.InstituteId = model.InstituteId;
        area.InstituteLocationId = model.InstituteLocationId;
        area.DepartmentId = model.DepartmentId;
        area.ContactOffice = model.ContactOffice;
        area.Status = model.Status;
        area.TrackedEntity.SetLastUpdated(this.UserNow, this.UserId);
        area.PreSave(this.User);

        area.Validate(validationResults, adminContext);

        if (validationResults.IsNotValid())
        {
            if (add)
            {
                this.ModelState.AddValidationResults(validationResults);
                return this.View(nameof(this.New), model);
            }

            return this.DisplayArea(nameof(this.Edit), area.AreaId, validationResults, model);
        }

        adminContext.SaveChanges(this.User);
        this.AddSuccessMessage(add ? "Area has been added." : "Area has been updated.");
        return this.RedirectToAreas();
    }
}