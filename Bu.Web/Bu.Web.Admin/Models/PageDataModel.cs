namespace Bu.Web.Admin.Models;

public abstract class PageDataModel<T, TInput>
    where T : class
    where TInput : IPage, IValidatableObject
{
    private readonly Lazy<PageData<T>> lazyPageData;

    protected PageDataModel(TInput input, ClaimsPrincipal user)
    {
        this.Input = input;
        this.User = user;
        this.lazyPageData = new Lazy<PageData<T>>(() => new PageData<T>());
    }

    protected PageDataModel(AdminContextProvider adminContextProvider, TInput input, ClaimsPrincipal user)
        : this(input, user)
    {
        this.lazyPageData = new Lazy<PageData<T>>(() =>
        {
            IEnumerable<ValidationResult> validationResult = input.Validate(new ValidationContext(input));
            if (validationResult.IsNotValid())
            {
                return new PageData<T>();
            }

            using IAdminContext context = adminContextProvider.Create();
            IQueryable<T> query = this.GetFilteredQuery(context);

            PageData<T> pageData = new PageData<T>(
                query,
                input,
                maxPagesCount: this.MaxPageCount,
                pageIndexQueryParameterName: $"{nameof(this.Input)}.{nameof(this.Input.PageIndex)}",
                pageSizeQueryParameterName: $"{nameof(this.Input)}.{nameof(this.Input.PageSize)}",
                controlId: this.ControlId);

            if (typeof(T).IsAssignableTo(typeof(IEntity)))
            {
                foreach (IEntity? entity in pageData.Data.Cast<IEntity>())
                {
                    entity.PostQuery(this.User);
                }
            }

            return pageData;
        });
    }

    public TInput Input { get; }

    [BindNever]
    public PageData<T> PageData => this.lazyPageData.Value;

    protected ClaimsPrincipal User { get; }

    protected virtual string? ControlId => null;

    protected virtual int? MaxPageCount => null;

    protected abstract IQueryable<T> GetFilteredQuery(IAdminContext adminContext);
}