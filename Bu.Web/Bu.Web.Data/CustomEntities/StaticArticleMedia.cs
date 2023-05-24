namespace Bu.Web.Data.CustomEntities;

public sealed class StaticArticleMedia
{
    public string AreaPath { get; init; } = string.Empty;

    public string ArticleUniqueId { get; init; } = string.Empty;

    public string MediaUniqueId { get; init; } = string.Empty;

    public DateTime? ExpiryDateUtc { get; init; }

    public bool NotExpired => this.ExpiryDateUtc == null || this.ExpiryDateUtc >= DateTimeHelper.UtcNow;
}