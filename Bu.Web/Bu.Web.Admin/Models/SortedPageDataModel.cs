namespace Bu.Web.Admin.Models;

public abstract class SortedPageDataModel<T, TInput>
    where T : class
    where TInput : IPage, ISort, IValidatableObject
{
    private readonly Lazy<SortedPageData<T, TInput>> lazySortedPageData;

    protected SortedPageDataModel(TInput input, ClaimsPrincipal user)
    {
        this.Input = input;
        this.User = user;
        this.lazySortedPageData = new Lazy<SortedPageData<T, TInput>>(() => new SortedPageData<T, TInput>(input));
    }

    protected SortedPageDataModel(AdminContextProvider adminContextProvider, TInput input, ClaimsPrincipal user)
        : this(input, user)
    {
        this.lazySortedPageData = new Lazy<SortedPageData<T, TInput>>(() =>
        {
            IEnumerable<ValidationResult> validationResult = input.Validate(new ValidationContext(input));
            if (validationResult.IsNotValid())
            {
                return new SortedPageData<T, TInput>(input);
            }

            using IAdminContext adminContext = adminContextProvider.Create();
            IQueryable<T> query = this.GetFilteredQuery(adminContext);

            SortedPageData<T, TInput> sortedPageData = new SortedPageData<T, TInput>(
                query,
                input,
                this.PrepareSortedQuery,
                maxPagesCount: this.MaxPageCount,
                pageIndexQueryParameterName: $"{nameof(this.Input)}.{nameof(this.Input.PageIndex)}",
                pageSizeQueryParameterName: $"{nameof(this.Input)}.{nameof(this.Input.PageSize)}",
                controlId: this.ControlId);
            if (typeof(T).IsAssignableTo(typeof(IEntity)))
            {
                foreach (IEntity? entity in sortedPageData.Data.Cast<IEntity>())
                {
                    entity.PostQuery(this.User);
                }
            }

            return sortedPageData;
        });
    }

    public TInput Input { get; }

    [BindNever]
    public SortedPageData<T, TInput> PageData => this.lazySortedPageData.Value;

    protected ClaimsPrincipal User { get; }

    protected virtual string? ControlId => null;

    protected virtual int? MaxPageCount => null;

    protected abstract IQueryable<T> GetFilteredQuery(IAdminContext adminContext);

    protected abstract IOrderedQueryable<T> PrepareSortedQuery(IQueryable<T> query);
}