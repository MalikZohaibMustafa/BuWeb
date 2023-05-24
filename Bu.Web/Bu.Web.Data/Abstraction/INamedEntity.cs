namespace Bu.Web.Data.Abstraction;

public interface INamedEntity
{
    public enum Names
    {
        Name,
        ShortName,
        Alias,
    }

    public int Id { get; }

    public string Name { get; }

    public string ShortName { get; }

    public string Alias { get; }

    public string GetName(Names name)
    {
        return name switch
        {
            Names.Name => this.Name,
            Names.ShortName => this.ShortName,
            Names.Alias => this.Alias,
            _ => throw new ArgumentOutOfRangeException(nameof(name), name, null),
        };
    }

    public SelectListItem ToSelectListItem(Names name)
    {
        return new SelectListItem(this.GetName(name), this.Id.ToString());
    }
}