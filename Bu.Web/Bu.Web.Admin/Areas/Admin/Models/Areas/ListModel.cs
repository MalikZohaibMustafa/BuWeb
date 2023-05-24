using Microsoft.EntityFrameworkCore;

namespace Bu.Web.Admin.Areas.Admin.Models.Areas;

public sealed class ListModel : SortedPageDataModel<Area, ListInputModel>
{
    public ListModel(ListInputModel input, ClaimsPrincipal user)
        : base(input, user)
    {
    }

    public ListModel(AdminContextProvider adminContextProvider, ListInputModel input, ClaimsPrincipal user)
        : base(adminContextProvider, input, user)
    {
    }

    protected override IQueryable<Area> GetFilteredQuery(IAdminContext adminContext)
    {
        IQueryable<Area> query = adminContext.Areas
            .Include(a => a.Institute)
            .Include(a => a.InstituteLocation)
            .Include(a => a.Department)
            .Include(a => a.ParentArea)
            .AsQueryable();

        string? search = this.Input.Search?.ToNullIfEmpty();
        if (search != null)
        {
            query = query.Where(a => a.AreaName.Contains(search) || a.AreaPath.Contains(search) || a.ContactOffice.Contains(search));
        }

        if (this.Input.Status != null)
        {
            query = query.Where(u => u.Status == this.Input.Status.Value);
        }

        return query;
    }

    protected override IOrderedQueryable<Area> PrepareSortedQuery(IQueryable<Area> query)
    {
        return this.Input.Sort switch
        {
            nameof(Area.AreaName) => query.OrderBy(this.Input.Desc, a => a.AreaName),
            nameof(Area.AreaPath) => query.OrderBy(this.Input.Desc, a => a.AreaPath),
            nameof(Area.ContactOffice) => query.OrderBy(this.Input.Desc, a => a.ContactOffice),
            nameof(Area.Institute.InstituteAlias) => query.OrderBy(this.Input.Desc, a => a.Institute!.InstituteAlias),
            nameof(Area.InstituteLocation.LocationAlias) => query.OrderBy(this.Input.Desc, a => a.InstituteLocation!.LocationAlias),
            nameof(Area.Department.DepartmentAlias) => query.OrderBy(this.Input.Desc, a => a.Department!.DepartmentAlias),
            nameof(Area.Status) => query.OrderBy(this.Input.Desc, a => a.Status),
            _ => throw new NotSupportedException(this.Input.Sort),
        };
    }
}