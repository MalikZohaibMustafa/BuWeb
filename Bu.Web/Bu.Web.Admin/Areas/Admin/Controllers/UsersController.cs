using Bu.Web.Admin.Areas.Admin.Models.Users;

namespace Bu.Web.Admin.Areas.Admin.Controllers;

[Authorize(nameof(AuthorizationPolicies.SuperAdministrator))]
public sealed class UsersController : AdminAreaBaseController<UsersController>
{
    public UsersController(ILogger<UsersController> logger, AdminContextProvider adminContextProvider)
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
        return this.DisplayUser(nameof(this.Display), id);
    }

    [HttpGet]
    public IActionResult Areas(int id)
    {
        return this.DisplayUser(nameof(this.Areas), id);
    }

    [HttpGet]
    public IActionResult New()
    {
        return this.View(nameof(this.New), new UserInputModel());
    }

    [HttpPost]
    public IActionResult Add(UserInputModel model)
    {
        if (model.UserId != default)
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

        return this.DisplayUser(nameof(this.Edit), id);
    }

    [HttpPost]
    public IActionResult Update(UserInputModel model)
    {
        if (model.UserId == default)
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

        return this.DisplayUser(nameof(this.Delete), id);
    }

    [HttpPost]
    public IActionResult DeletePost(int id)
    {
        if (id == default)
        {
            return this.BadRequest();
        }

        using IAdminContext adminContext = this.AdminContextProvider.Create();
        User? user = adminContext.Users.Include(u => u.UserRoles).SingleOrDefault(u => u.UserId == id);
        if (user == null)
        {
            return this.NotFound();
        }

        adminContext.UserRoles.RemoveRange(user.UserRoles!);
        adminContext.Users.Remove(user);
        adminContext.SaveChanges(this.User);
        this.AddSuccessMessage("User has been deleted.");
        return this.RedirectToAction("Index", "Users", new { area = "Admin" });
    }

    private IActionResult DisplayUser(string viewName, int id)
    {
        using IAdminContext adminContext = this.AdminContextProvider.Create();
        User? user = adminContext.Users
            .Include(u => u.UserRoles)
            .Include(u => u.UserAreas)
            .SingleOrDefault(u => u.UserId == id);
        if (user == null)
        {
            return this.NotFound();
        }

        user.PostQuery(this.User);
        user.UserRoles!.ForEach(r => r.PostQuery(this.User));
        user.UserAreas!.ForEach(ua => ua.PostQuery(this.User));

        var superAdministrator = user.UserRoles!.SingleOrDefault(r => r.Role == UserRole.Roles.SuperAdministrator);
        var administrator = user.UserRoles.SingleOrDefault(r => r.Role == UserRole.Roles.Administrator);
        var webmaster = user.UserRoles.SingleOrDefault(r => r.Role == UserRole.Roles.Webmaster);
        UserInputModel inputModel = new UserInputModel
        {
            UserId = user.UserId,
            Name = user.Name,
            Email = user.Email,
            DepartmentId = user.DepartmentId,
            MobileNo = user.MobileNo,
            PhoneNo = user.PhoneNo,
            ExpiryDateUser = user.ExpiryDateUser,
            Status = user.Status,
            SuperAdministrator = superAdministrator != null,
            SuperAdministratorExpiryDateUser = superAdministrator?.ExpiryDateUser,
            SuperAdministratorStatus = superAdministrator?.Status,
            Administrator = administrator != null,
            AdministratorExpiryDateUser = administrator?.ExpiryDateUser,
            AdministratorStatus = administrator?.Status,
            Webmaster = webmaster != null,
            WebmasterExpiryDateUser = webmaster?.ExpiryDateUser,
            WebmasterStatus = webmaster?.Status,
            UserAreas = user.UserAreas!.Where(ua => ua.UserId == user.UserId).ToList(),
        };

        return this.View(viewName, inputModel);
    }

    private IActionResult AddOrUpdate(UserInputModel model)
    {
        var add = model.UserId == default;
        if (!this.ModelState.IsValid)
        {
            return this.View(add ? nameof(this.New) : nameof(this.Edit), model);
        }

        using IAdminContext adminContext = this.AdminContextProvider.Create();
        User? user;
        if (add)
        {
            user = adminContext.Users.Add(new User
            {
                UserId = default,
                CreatedOnUtc = this.UtcNow,
                CreatedByUserId = this.UserId,
                UserRoles = new List<UserRole>(),
            }).Entity;
        }
        else
        {
            user = adminContext.Users.Include(u => u.UserRoles).SingleOrDefault(u => u.UserId == model.UserId);
            if (user == null)
            {
                return this.NotFound();
            }
        }

        user.Name = model.Name;
        user.Email = model.Email;
        user.DepartmentId = model.DepartmentId;
        user.MobileNo = model.MobileNo;
        user.PhoneNo = model.PhoneNo;
        user.ExpiryDateUser = model.ExpiryDateUser;
        user.Status = model.Status;
        user.LastUpdatedOnUtc = this.UtcNow;
        user.LastUpdatedByUserId = this.UserId;

        if (model.SuperAdministrator)
        {
            UserRole? userRole = user.UserRoles!.SingleOrDefault(r => r.Role == UserRole.Roles.SuperAdministrator);
            if (userRole == null)
            {
                userRole = new UserRole
                {
                    UserRoleId = default,
                    UserId = default,
                    Role = UserRole.Roles.SuperAdministrator,
                    CreatedOnUtc = this.UtcNow,
                    CreatedByUserId = this.UserId,
                };

                user.UserRoles!.Add(userRole);
            }

            userRole.ExpiryDateUser = model.SuperAdministratorExpiryDateUser!.Value;
            userRole.Status = model.SuperAdministratorStatus!.Value;
            userRole.LastUpdatedByUserId = this.UserId;
            userRole.LastUpdatedOnUtc = this.UtcNow;
        }
        else
        {
            user.UserRoles!.RemoveAll(r => r.Role == UserRole.Roles.SuperAdministrator);
        }

        if (model.Administrator)
        {
            UserRole? userRole = user.UserRoles!.SingleOrDefault(r => r.Role == UserRole.Roles.Administrator);
            if (userRole == null)
            {
                userRole = new UserRole
                {
                    UserRoleId = default,
                    UserId = default,
                    Role = UserRole.Roles.Administrator,
                    CreatedOnUtc = this.UtcNow,
                    CreatedByUserId = this.UserId,
                };

                user.UserRoles!.Add(userRole);
            }

            userRole.ExpiryDateUser = model.AdministratorExpiryDateUser!.Value;
            userRole.Status = model.AdministratorStatus!.Value;
            userRole.LastUpdatedByUserId = this.UserId;
            userRole.LastUpdatedOnUtc = this.UtcNow;
        }
        else
        {
            user.UserRoles!.RemoveAll(r => r.Role == UserRole.Roles.Administrator);
        }

        if (model.Webmaster)
        {
            UserRole? userRole = user.UserRoles!.SingleOrDefault(r => r.Role == UserRole.Roles.Webmaster);
            if (userRole == null)
            {
                userRole = new UserRole
                {
                    UserRoleId = default,
                    UserId = default,
                    Role = UserRole.Roles.Webmaster,
                    CreatedOnUtc = this.UtcNow,
                    CreatedByUserId = this.UserId,
                };

                user.UserRoles!.Add(userRole);
            }

            userRole.ExpiryDateUser = model.WebmasterExpiryDateUser!.Value;
            userRole.Status = model.WebmasterStatus!.Value;
            userRole.LastUpdatedByUserId = this.UserId;
            userRole.LastUpdatedOnUtc = this.UtcNow;
        }
        else
        {
            user.UserRoles!.RemoveAll(r => r.Role == UserRole.Roles.Webmaster);
        }

        List<ValidationResult> validationResults = new List<ValidationResult>();
        user.Validate(validationResults, adminContext);
        foreach (UserRole userRole in user.UserRoles!)
        {
            userRole.Validate(validationResults, adminContext);
        }

        if (validationResults.IsNotValid())
        {
            this.ModelState.AddValidationResults(validationResults);
            return this.View(add ? nameof(this.New) : nameof(this.Edit), model);
        }

        adminContext.SaveChanges(this.User);
        this.AddSuccessMessage(add ? "User has been added." : "User has been updated.");
        return this.RedirectToAction("Index", "Users", new { area = "Admin" });
    }
}