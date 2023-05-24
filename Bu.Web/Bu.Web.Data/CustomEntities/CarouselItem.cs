namespace Bu.Web.Data.CustomEntities;

public sealed class CarouselItem
{
    public int AreaId { get; init; }

    public string AreaPath { get; init; } = string.Empty;

    public string ArticleName { get; init; } = string.Empty;

    public ArticleDetail.ArticleTypes ArticleType { get; init; } = ArticleDetail.ArticleTypes.Static;

    public string ArticleUniqueId { get; init; } = string.Empty;

    public DateTime? ExpiryDateUtc { get; init; }

    public bool NotExpired => this.ExpiryDateUtc == null || this.ExpiryDateUtc >= DateTimeHelper.UtcNow;
}