namespace Bu.Web.Data.CustomEntities;

public sealed class EventDetail
{
    public int AreaId { get; init; }

    public string AreaPath { get; init; } = string.Empty;

    public string ArticleUniqueId { get; init; } = string.Empty;

    public DateTime? StartDate { get; init; }

    public DateTime? EndDate { get; init; }

    public string ArticleName { get; init; } = string.Empty;

    public string? ShortDescription { get; init; }

    public string SmallCarouselUrl => ArticleDetail.GetArticleSmallCarouselMediaUrlWithTilde(this.AreaPath, ArticleDetail.ArticleTypes.Event, this.ArticleUniqueId);

    public string ArticleUrl => ArticleDetail.GetArticleUrlWithTilde(this.AreaPath, ArticleDetail.ArticleTypes.Event, this.ArticleUniqueId);
}