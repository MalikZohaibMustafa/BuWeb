namespace Bu.Web.Data.Abstraction;

public interface IEntity : IValidatableObject
{
    public int Id { get; }

    public void PreSave(ClaimsPrincipal claimsPrincipal);

    public void PostQuery(ClaimsPrincipal claimsPrincipal);
}