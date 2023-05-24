namespace Bu.Web.Data.Abstraction;

public interface ICommonFixedContext
{
    public IReadOnlyList<AreaLayout> AreaLayoutsList { get; }

    public IReadOnlyList<Area> AreasList { get; }

    public IReadOnlyList<Department> DepartmentsList { get; }

    public IReadOnlyList<Institute> InstitutesList { get; }

    public IReadOnlyList<InstituteLocation> InstituteLocationsList { get; }

    public Area RootArea { get; }

    public Area GetRootArea(int instituteId)
    {
        if (instituteId == 0)
        {
            return this.RootArea;
        }

        return this.AreasList.Single(a => a.ParentAreaId == this.RootArea.AreaId && a.InstituteId == instituteId && a.InstituteLocationId == null && a.DepartmentId == null);
    }

    public IEnumerable<SelectListItem> GetInstitutes(INamedEntity.Names instituteName, bool appendEmpty = false, string emptyText = "", string emptyValue = "")
    {
        IEnumerable<SelectListItem> items = this.InstitutesList
            .OrderBy(i => i.InstituteId)
            .Cast<INamedEntity>()
            .Select(i => i.ToSelectListItem(instituteName));
        return appendEmpty
            ? new[] { new SelectListItem(emptyText, emptyValue) }.Concat(items)
            : items;
    }

    public IEnumerable<SelectListItem> GetInstituteLocations(INamedEntity.Names locationName, int instituteId, bool appendEmpty = false, string emptyText = "", string emptyValue = "")
    {
        IEnumerable<SelectListItem> items = this.InstituteLocationsList.Where(il => il.InstituteId == instituteId)
            .OrderBy(il => il.InstituteLocationId)
            .Cast<INamedEntity>()
            .Select(i => i.ToSelectListItem(locationName));
        return appendEmpty
            ? new[] { new SelectListItem(emptyText, emptyValue) }.Concat(items)
            : items;
    }

    public IEnumerable<SelectListItem> GetInstituteLocations(INamedEntity.Names instituteName, INamedEntity.Names locationName, int? instituteId, bool appendEmpty = false, string emptyText = "", string emptyValue = "")
    {
        IEnumerable<SelectListItem> items = this.InstituteLocationsList
            .Where(il => instituteId == null || il.InstituteId == instituteId.Value)
            .OrderBy(il => il.InstituteId).ThenBy(il => il.InstituteLocationId)
            .Cast<INamedWithParentEntity>()
            .Select(i => i.ToSelectListItem(instituteName, locationName));
        return appendEmpty
            ? new[] { new SelectListItem(emptyText, emptyValue) }.Concat(items)
            : items;
    }

    public IEnumerable<SelectListItem> GetDepartments(INamedEntity.Names departmentName, int instituteId, bool appendEmpty = false, string emptyText = "", string emptyValue = "")
    {
        IOrderedEnumerable<SelectListItem> items = this.DepartmentsList.Where(il => il.InstituteId == instituteId)
            .Cast<INamedEntity>()
            .Select(d => d.ToSelectListItem(departmentName))
            .OrderBy(i => i.Text);
        return appendEmpty
            ? new[] { new SelectListItem(emptyText, emptyValue) }.Concat(items)
            : items;
    }

    public IEnumerable<SelectListItem> GetDepartments(INamedEntity.Names instituteName, INamedEntity.Names departmentName, int? instituteId, bool appendEmpty = false, string emptyText = "", string emptyValue = "")
    {
        IEnumerable<SelectListItem> items = this.DepartmentsList
            .Where(d => instituteId == null || d.InstituteId == instituteId.Value)
            .Cast<INamedWithParentEntity>()
            .OrderBy(d => d.Parent?.Id).ThenBy(d => d.GetName(departmentName))
            .Select(d => d.ToSelectListItem(instituteName, departmentName));
        return appendEmpty
            ? new[] { new SelectListItem(emptyText, emptyValue) }.Concat(items)
            : items;
    }

    public string GetDepartmentById(int departmentId, INamedEntity.Names departmentName)
    {
        return this.DepartmentsList.Cast<INamedEntity>().Single(d => d.Id == departmentId).GetName(departmentName);
    }

    public string GetDepartmentById(int departmentId, INamedEntity.Names instituteName, INamedEntity.Names departmentName)
    {
        return this.DepartmentsList.Cast<INamedWithParentEntity>().Single(d => d.Id == departmentId).GetName(instituteName, departmentName);
    }

    public IEnumerable<SelectListItem> GetAreas(bool appendEmpty = false, string emptyText = "", string emptyValue = "")
    {
        IEnumerable<SelectListItem> items = this.AreasList
            .OrderBy(a => a.AreaPath)
            .Select(i => new SelectListItem(i.AreaPath, i.AreaId.ToString()));
        return appendEmpty
            ? new[] { new SelectListItem(emptyText, emptyValue) }.Concat(items)
            : items;
    }

    public IEnumerable<SelectListItem> GetAreaLayouts(INamedEntity.Names layoutName, bool appendEmpty = false, string emptyText = "", string emptyValue = "")
    {
        IEnumerable<SelectListItem> items = this.AreaLayoutsList
            .OrderBy(al => al.AreaId)
            .GroupBy(al => al.Area!)
            .SelectMany(g =>
            {
                SelectListGroup selectListGroup = new SelectListGroup { Name = g.Key.AreaPath };
                return g.Cast<INamedEntity>().Select(gg => new SelectListItem
                {
                    Group = selectListGroup,
                    Text = gg.GetName(layoutName),
                    Value = gg.Id.ToString(),
                });
            });
        return appendEmpty
            ? new[] { new SelectListItem(emptyText, emptyValue) }.Concat(items)
            : items;
    }

    public IEnumerable<SelectListItem> GetListItems<TEnum>(bool appendStaticListItem, string staticListItemText = "", string staticListItemValue = "")
        where TEnum : struct, Enum
    {
        Type underlyingType = Enum.GetUnderlyingType(typeof(TEnum));
        IEnumerable<SelectListItem> selectListItems = Enum.GetValues<TEnum>().Select(i => new SelectListItem(i.GetDescription(), Convert.ChangeType(i, underlyingType).ToString()));
        return appendStaticListItem
            ? new[] { new SelectListItem(staticListItemText, staticListItemValue) }.Concat(selectListItems)
            : selectListItems;
    }

    public IEnumerable<SelectListItem> GetYesNoListItem(bool appendStaticListItem, string staticListItemText = "", string staticListItemValue = "")
    {
        IEnumerable<SelectListItem> selectListItems = new[] { new SelectListItem("Yes", true.ToString()), new SelectListItem("No", false.ToString()) };
        return appendStaticListItem
            ? new[] { new SelectListItem(staticListItemText, staticListItemValue) }.Concat(selectListItems)
            : selectListItems;
    }
}