namespace Bu.Web.Admin.Models;

public abstract class BaseInputModel : IValidatableObject
{
    public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        yield break;
    }
}