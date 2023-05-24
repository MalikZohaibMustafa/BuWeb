namespace Bu.Web.Admin.Areas.Admin.Models.Users;

public sealed class ListInputModel : BaseInputModel, ISort, IPage
{
    public string? SortQueryParameterName => null;

    public string? DescQueryParameterName => null;

    public string? PageIndexQueryParameterName => null;

    public string? PageSizeQueryParameterName => null;

    public string? Sort { get; init; } = nameof(User.Name);

    public bool? Desc { get; init; } = true;

    public int? PageIndex { get; init; }

    public int? PageSize { get; init; }

    [MaxLength(Constants.EmailAddressLength)]
    [NoLineBreaks]
    public string? Search { get; init; }

    public User.Statuses? Status { get; init; }
}