using Bu.Web.Admin.Areas.Admin.Models.Firewall;

namespace Bu.Web.Admin.Areas.Admin.Controllers;

[Authorize(nameof(AuthorizationPolicies.SuperAdministrator))]
public sealed class FirewallController : AdminAreaBaseController<FirewallController>
{
    public FirewallController(ILogger<AdminAreaBaseController<FirewallController>> logger, AdminContextProvider adminContextProvider)
        : base(logger, adminContextProvider)
    {
    }

    [HttpGet]
    public IActionResult Index()
    {
        using IAdminContext adminContext = this.AdminContextProvider.Create();
        var ipAddresses = adminContext.WhitelistedIpAddresses.Select(ip => ip.IpAddress).OrderBy(ip => ip).ToList();
        return this.View(nameof(this.Index), new IndexInputModel
        {
            IpAddresses = string.Join(Environment.NewLine, ipAddresses),
        });
    }

    [HttpPost]
    public IActionResult Post(IndexInputModel model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View(nameof(this.Index), model);
        }

        List<IPAddress> ips = new List<IPAddress>();
        foreach (var address in model.IpAddresses.Trim().Split(Environment.NewLine))
        {
            if (IPAddress.TryParse(address, out var ipAddress))
            {
                ips.Add(ipAddress);
            }
            else
            {
                this.ModelState.AddValidationResults(new ValidationResult[]
                {
                    new ValidationResult($"'{address}' is not a valid IP address.", new[] { nameof(model.IpAddresses) }),
                });
                return this.View(nameof(this.Index), model);
            }
        }

        using IAdminContext adminContext = this.AdminContextProvider.Create();
        var whitelistedIpAddresses = adminContext.WhitelistedIpAddresses.ToList();

        foreach (var whitelistedIpAddress in whitelistedIpAddresses)
        {
            if (!ips.Contains(IPAddress.Parse(whitelistedIpAddress.IpAddress)))
            {
                adminContext.WhitelistedIpAddresses.Remove(whitelistedIpAddress);
            }
        }

        foreach (var ipAddress in ips.Where(ip => whitelistedIpAddresses.All(a => a.IpAddress != ip.ToString())))
        {
            adminContext.WhitelistedIpAddresses.Add(new WhitelistedIpAddress
            {
                IpAddress = ipAddress.ToString(),
            });
        }

        adminContext.SaveChanges(this.User);
        this.AddSuccessMessage("Firewall settings have been updated.");
        return this.RedirectToAction(nameof(this.Index));
    }
}