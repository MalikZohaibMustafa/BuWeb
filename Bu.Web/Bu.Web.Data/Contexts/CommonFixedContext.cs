namespace Bu.Web.Data.Contexts;

public abstract class CommonFixedContext<TContext> : ICommonFixedContext
    where TContext : ICommonContext
{
    protected CommonFixedContext(Func<TContext> contextProvider)
    {
        using ICommonContext commonContext = contextProvider();
        this.AreasList = commonContext.Areas
            .ToList();
        this.AreaLayoutsList = commonContext.AreaLayouts
            .Include(a => a.Area)
            .ToList();
        this.InstitutesList = commonContext.Institutes
            .OrderBy(i => i.InstituteId)
            .ToList();
        this.InstituteLocationsList = commonContext.InstituteLocations
            .Include(il => il.Institute)
            .OrderBy(il => il.InstituteLocationId)
            .ToList();
        this.DepartmentsList = commonContext.Departments
            .Include(d => d.Institute)
            .Include(d => d.InstituteLocation)
            .OrderBy(d => d.DepartmentName)
            .ToList();

        this.RootArea = this.AreasList.Single(a => a.ParentAreaId == null);
    }

    public IReadOnlyList<AreaLayout> AreaLayoutsList { get; }

    public IReadOnlyList<Area> AreasList { get; }

    public IReadOnlyList<Department> DepartmentsList { get; }

    public IReadOnlyList<Institute> InstitutesList { get; }

    public IReadOnlyList<InstituteLocation> InstituteLocationsList { get; }

    public Area RootArea { get; }
}