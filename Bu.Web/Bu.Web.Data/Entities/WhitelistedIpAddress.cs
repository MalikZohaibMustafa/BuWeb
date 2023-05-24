using System.Net;

namespace Bu.Web.Data.Entities;

public sealed class WhitelistedIpAddress : BaseTimestampEntity
{
    [Column(Order = 0)]
    public int WhitelistIpAddressId { get; set; }

    [Column(Order = 2)]
    [NoLeadingOrTrailingWhiteSpaces]
    [NoLineBreaks]
    [StringLength(39)]
    [Unicode(false)]
    public string IpAddress { get; set; } = null!;

    public override int Id => this.WhitelistIpAddressId;

    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!IPAddress.TryParse(this.IpAddress, out IPAddress? address))
        {
            yield return new ValidationResult($"'{this.IpAddress}' is not a valid Ip Address.", new[] { nameof(this.IpAddress) });
        }
        else
        {
            this.IpAddress = address.ToString();
        }

        IAdminContext? adminContext = (IAdminContext?)validationContext.GetService(typeof(IAdminContext));
        if (adminContext != null)
        {
            IQueryable<WhitelistedIpAddress> query = adminContext.WhitelistedIpAddresses.Where(ip => ip.WhitelistIpAddressId != this.WhitelistIpAddressId);
            if (query.Any(ip => ip.IpAddress == this.IpAddress))
            {
                yield return new ValidationResult($"{this.GetType().GetDisplayNameForProperty(nameof(this.IpAddress))} '{this.IpAddress}' is already in use.", new[] { nameof(this.IpAddress) });
            }
        }
    }

    public static void OnModelCreating(EntityTypeBuilder<WhitelistedIpAddress> entity, string tableName)
    {
        BaseEntity.OnModelCreating(entity, tableName);
        PrimaryKey(entity, tableName, nameof(WhitelistIpAddressId));
    }
}