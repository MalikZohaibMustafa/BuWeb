using Microsoft.EntityFrameworkCore;

namespace Bu.Web.Admin.Areas.Admin.Models.Users;

public class UserInputModel : BaseInputModel
{
    public int UserId { get; set; }

    [NoLeadingOrTrailingWhiteSpaces]
    [NoLineBreaks]
    [Required]
    [StringLength(Constants.PersonNameLength, MinimumLength = 3)]
    [DisplayName("Name")]
    public string Name { get; set; } = string.Empty;

    [NoLeadingOrTrailingWhiteSpaces]
    [NoLineBreaks]
    [Required]
    [StringLength(Constants.EmailAddressLength, MinimumLength = 5)]
    [EmailAddress]
    [DisplayName("Email")]
    public string Email { get; set; } = string.Empty;

    [DisplayName("Department")]
    public int DepartmentId { get; set; }

    [DisplayName("Mobile No.")]
    [NoLeadingOrTrailingWhiteSpaces]
    [NoLineBreaks]
    [StringLength(MobileNumberLength)]
    [Unicode(MobileNumberUnicode)]
    public string? MobileNo { get; set; } = null!;

    [DisplayName("Phone No.")]
    [NoLeadingOrTrailingWhiteSpaces]
    [NoLineBreaks]
    [StringLength(PhoneNumberLength)]
    [Unicode(PhoneNumberUnicode)]
    public string? PhoneNo { get; set; } = null!;

    [DisplayName("Expiry Date")]
    public DateTime? ExpiryDateUser { get; set; }

    [DisplayName("Status")]
    [Required]
    public User.Statuses Status { get; set; }

    public bool SuperAdministrator { get; set; }

    [DisplayName("Super Administrator Expiry Date")]
    public DateTime? SuperAdministratorExpiryDateUser { get; set; }

    [DisplayName("Super Administrator Status")]
    [Required]
    public UserRole.Statuses? SuperAdministratorStatus { get; set; }

    public bool Administrator { get; set; }

    [DisplayName("Administrator Expiry Date")]
    public DateTime? AdministratorExpiryDateUser { get; set; }

    [DisplayName("Administrator Status")]
    public UserRole.Statuses? AdministratorStatus { get; set; }

    public bool Webmaster { get; set; }

    [DisplayName("Webmaster Expiry Date")]
    public DateTime? WebmasterExpiryDateUser { get; set; }

    [DisplayName("Webmaster Status")]
    public UserRole.Statuses? WebmasterStatus { get; set; }

    public List<UserArea> UserAreas { get; set; } = new List<UserArea>();

    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        foreach (var validationResult in base.Validate(validationContext))
        {
            yield return validationResult;
        }

        if (this.SuperAdministrator)
        {
            if (this.SuperAdministratorExpiryDateUser == null)
            {
                yield return new ValidationResult("The Expiry Date field is required.", new[] { nameof(this.SuperAdministratorExpiryDateUser) });
            }

            if (this.SuperAdministratorStatus == null)
            {
                yield return new ValidationResult("The Status field is required.", new[] { nameof(this.SuperAdministratorStatus) });
            }
        }

        if (this.Administrator)
        {
            if (this.AdministratorExpiryDateUser == null)
            {
                yield return new ValidationResult("The Expiry Date field is required.", new[] { nameof(this.AdministratorExpiryDateUser) });
            }

            if (this.AdministratorStatus == null)
            {
                yield return new ValidationResult("The Status field is required.", new[] { nameof(this.AdministratorStatus) });
            }
        }

        if (this.Webmaster)
        {
            if (this.WebmasterExpiryDateUser == null)
            {
                yield return new ValidationResult("The Expiry Date field is required.", new[] { nameof(this.WebmasterExpiryDateUser) });
            }

            if (this.WebmasterStatus == null)
            {
                yield return new ValidationResult("The Status field is required.", new[] { nameof(this.WebmasterStatus) });
            }
        }
    }
}