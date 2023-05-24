namespace Bu.Web.Admin.Areas.Articles.Models.Home;

public sealed class ListInputModel : BaseInputModel, ISort, IPage
{
    public string? SortQueryParameterName => null;

    public string? DescQueryParameterName => null;

    public string? PageIndexQueryParameterName => null;

    public string? PageSizeQueryParameterName => null;

    public string? Sort { get; init; } = nameof(ArticleDetail.ArticleName);

    public bool? Desc { get; init; } = true;

    public int? PageIndex { get; init; }

    public int? PageSize { get; init; }

    [MaxLength(EmailAddressLength)]
    [NoLineBreaks]
    public string? Search { get; init; }

    public IPublished.Statuses? Status { get; init; }

    [DisplayName("Area")]
    public int? AreaId { get; init; }

    [DisplayName("Article Type")]
    public ArticleDetail.ArticleTypes? ArticleType { get; init; }
}