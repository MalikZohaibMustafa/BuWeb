namespace Bu.Web.Data.Abstraction;

public interface INamedWithParentEntity : INamedEntity
{
    public INamedEntity? Parent { get; }

    public string GetName(Names parentName, Names name)
    {
        return this.Parent == null ? this.GetName(name) : $"{this.Parent.GetName(parentName)}/{this.GetName(name)}";
    }

    public SelectListItem ToSelectListItem(Names parentName, Names name)
    {
        return new SelectListItem(this.GetName(parentName, name), this.Id.ToString());
    }
}