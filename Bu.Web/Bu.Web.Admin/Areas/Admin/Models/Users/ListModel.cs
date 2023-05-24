namespace Bu.Web.Admin.Areas.Admin.Models.Users;

public sealed class ListModel : SortedPageDataModel<User, ListInputModel>
{
    public ListModel(ListInputModel input, ClaimsPrincipal user)
        : base(input, user)
    {
    }

    public ListModel(AdminContextProvider adminContextProvider, ListInputModel input, ClaimsPrincipal user)
        : base(adminContextProvider, input, user)
    {
    }

    protected override IQueryable<User> GetFilteredQuery(IAdminContext adminContext)
    {
        IQueryable<User> query = adminContext.Users.Include(u => u.UserRoles);

        var search = this.Input.Search?.ToNullIfEmpty();
        if (search != null)
        {
            query = query.Where(u => u.Name.Contains(search) || u.Email.Contains(search));
        }

        if (this.Input.Status != null)
        {
            query = query.Where(u => u.Status == this.Input.Status.Value);
        }

        return query;
    }

    protected override IOrderedQueryable<User> PrepareSortedQuery(IQueryable<User> query)
    {
        return this.Input.Sort switch
        {
            nameof(Data.Entities.User.Name) => query.OrderBy(this.Input.Desc, u => u.Name),
            nameof(Data.Entities.User.Email) => query.OrderBy(this.Input.Desc, u => u.Email),
            nameof(Data.Entities.User.Status) => query.OrderBy(this.Input.Desc, u => u.Status),
            nameof(Data.Entities.User.DepartmentId) => query.OrderBy(this.Input.Desc, u => u.DepartmentId),
            nameof(Data.Entities.User.ExpiryDateUtc) => query.OrderBy(this.Input.Desc, u => u.ExpiryDateUtc),
            nameof(Data.Entities.User.ExpiryDateUser) => query.OrderBy(this.Input.Desc, u => u.ExpiryDateUtc),
            _ => throw new NotSupportedException(this.Input.Sort),
        };
    }
}