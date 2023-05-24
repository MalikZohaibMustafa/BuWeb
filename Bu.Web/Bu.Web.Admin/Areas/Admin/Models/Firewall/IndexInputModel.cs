namespace Bu.Web.Admin.Areas.Admin.Models.Firewall;

public class IndexInputModel : BaseInputModel
{
    [DisplayName("Whitelisted IP Addresses (One per each line)")]
    [NoLeadingOrTrailingWhiteSpaces]
    [Required]
    public string IpAddresses { get; init; } = string.Empty;
}