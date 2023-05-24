using Microsoft.AspNetCore.Mvc;

namespace Bu.Web.Data.CustomEntities;

public sealed class NewsOrTenderDetail
{
    public int ArticleId { get; set; }

    public int ArticleDetailId { get; set; }

    public ArticleDetail.ArticleTypes ArticleType { get; set; }

    public string ArticleName { get; set; } = string.Empty;

    public string? ArticleShortDescription { get; internal set; }

    public int AreaId { get; set; }

    public string AreaPath { get; set; } = string.Empty;

    public DateTime PublishedOnUtc { get; set; }

    [BindNever]
    public DateTime PublishedOnUser { get; set; }

    public DateTime? ExpiryDateUtc { get; set; }

    [BindNever]
    public DateTime? ExpiryDateUser { get; set; }

    public string GetUrl(IUrlHelper url)
    {
        return "#";
    }
}