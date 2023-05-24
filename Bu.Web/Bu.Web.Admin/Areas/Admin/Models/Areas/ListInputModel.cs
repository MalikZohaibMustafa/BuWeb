namespace Bu.Web.Admin.Areas.Admin.Models.Areas;

public sealed class ListInputModel : BaseInputModel, ISort, IPage
{
    public string? SortQueryParameterName => null;

    public string? DescQueryParameterName => null;

    public string? PageIndexQueryParameterName => null;

    public string? PageSizeQueryParameterName => null;

    public string? Sort { get; init; } = nameof(Area.AreaName);

    public bool? Desc { get; init; } = true;

    public int? PageIndex { get; init; }

    public int? PageSize { get; init; }

    [MaxLength(500)]
    [NoLineBreaks]
    public string? Search { get; init; }

    public Area.Statuses? Status { get; init; }
}