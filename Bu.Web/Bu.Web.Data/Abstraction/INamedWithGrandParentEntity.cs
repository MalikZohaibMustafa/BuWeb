using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bu.Web.Data.Abstraction;

public interface INamedWithGrandParentEntity : INamedWithParentEntity
{
    public INamedWithParentEntity? GrandParent { get; }

    public string GetName(Names grandParentName, Names parentName, Names name)
    {
        return this.GrandParent == null ? this.GetName(parentName, name) : $"{this.GrandParent.GetName(grandParentName)}/{this.GetName(parentName, name)}";
    }

    public SelectListItem ToSelectListItem(Names grandParentName, Names parentName, Names name)
    {
        return new SelectListItem(this.GetName(grandParentName, parentName, name), this.Id.ToString());
    }
}