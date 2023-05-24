using Microsoft.AspNetCore.Mvc.Routing;

namespace Bu.Web.Website.RouteTransformers;

internal sealed class BuRouteValueTransformer : DynamicRouteValueTransformer
{
    private const string AreaIdName = "areaId";
    private const string ArticleUniqueIdName = "articleUniqueId";
    private const string PageUniqueIdName = "pageUniqueId";
    private const string MediaUniqueIdName = "mediaUniqueId";
    private const string ArticleTypeName = "articleType";
    private const string ControllerName = "controller";
    private const string ActionName = "action";

    public BuRouteValueTransformer(
        IWebsiteFixedContext websiteFixedContext,
        WebsiteContextProvider websiteContextProvider)
    {
        this.WebsiteFixedContext = websiteFixedContext;
        this.WebsiteContextProvider = websiteContextProvider;
    }

    private IWebsiteFixedContext WebsiteFixedContext { get; }

    private WebsiteContextProvider WebsiteContextProvider { get; }

    private ValueTask<RouteValueDictionary> CheckArticle(
        RouteValueDictionary values,
        int areaId,
        ArticleDetail.ArticleTypes articleType,
        string articleUniqueId,
        string? part2,
        string? part3)
    {
        using IWebsiteContext websiteContext = this.WebsiteContextProvider.Create();
        IQueryable<ArticleDetail> articleQuery = websiteContext.GetPublishedArticleDetails(DateTimeHelper.UtcNow)
            .Where(ad => ad.ArticleUniqueId == articleUniqueId && ad.ArticleType == articleType && ad.AreaId == areaId);

        if (part2 != null)
        {
            switch (part2.ToLower())
            {
                case "carousel" when part3 == null:
                    if (articleQuery.Any(ad => ad.CarouselArticleMediaId != null))
                    {
                        values[AreaIdName] = areaId;
                        values[ArticleUniqueIdName] = articleUniqueId;
                        values[ArticleTypeName] = articleType;
                        values[ControllerName] = "Articles";
                        values[ActionName] = nameof(ArticlesController.GetPublishedArticleCarousel);
                        return ValueTask.FromResult(values);
                    }

                    break;
                case "smallcarousel" when part3 == null:
                    if (articleQuery.Any(ad => ad.SmallCarouselArticleMediaId != null))
                    {
                        values[AreaIdName] = areaId;
                        values[ArticleUniqueIdName] = articleUniqueId;
                        values[ArticleTypeName] = articleType;
                        values[ControllerName] = "Articles";
                        values[ActionName] = nameof(ArticlesController.GetPublishedArticleSmallCarousel);
                        return ValueTask.FromResult(values);
                    }

                    break;
                case "media" when part3 != null:
                    if (articleQuery.SelectMany(ad => ad.ArticleMedias!).Any(am => am.MediaUniqueId == part3))
                    {
                        values[AreaIdName] = areaId;
                        values[ArticleUniqueIdName] = articleUniqueId;
                        values[ArticleTypeName] = articleType;
                        values[MediaUniqueIdName] = part3;
                        values[ControllerName] = "Articles";
                        values[ActionName] = nameof(ArticlesController.GetPublishedArticleMedia);
                        return ValueTask.FromResult(values);
                    }

                    break;
                default:
                    if (articleQuery.SelectMany(ad => ad.ArticlePages!).Any(ap => ap.PageUniqueId == part2))
                    {
                        values[AreaIdName] = areaId;
                        values[ArticleUniqueIdName] = articleUniqueId;
                        values[ArticleTypeName] = articleType;
                        values[PageUniqueIdName] = part2;
                        values[ControllerName] = "Articles";
                        values[ActionName] = nameof(ArticlesController.GetPublishedArticlePage);
                        return ValueTask.FromResult(values);
                    }

                    break;
            }
        }
        else
        {
            if (articleQuery.SelectMany(ad => ad.ArticlePages!).Any())
            {
                values[AreaIdName] = areaId;
                values[ArticleUniqueIdName] = articleUniqueId;
                values[ArticleTypeName] = articleType;
                values[PageUniqueIdName] = null;
                values[ControllerName] = "Articles";
                values[ActionName] = nameof(ArticlesController.GetPublishedArticlePage);
                return ValueTask.FromResult(values);
            }
        }

        return ValueTask.FromResult(values);
    }

    private ValueTask<RouteValueDictionary> Article(RouteValueDictionary values, int areaId, string? part1, string? part2, string? part3, string? part4)
    {
#pragma warning disable S2234 // Parameters should be passed in the correct order
        Area? nestedArea = this.WebsiteFixedContext.AreasList.SingleOrDefault(a => a.ParentAreaId == areaId && a.AreaName == part1);
        if (nestedArea != null)
        {
            return this.Article(values, nestedArea.AreaId, part2, part3, part4, null);
        }

        if (part1 != null)
        {
            return part1.ToLower() switch
            {
                "article" when part2 is not null => this.CheckArticle(values, areaId, ArticleDetail.ArticleTypes.Dynamic, part2, part3, part4),
                "news" when part2 is not null => this.CheckArticle(values, areaId, ArticleDetail.ArticleTypes.News, part2, part3, part4),
                "tender" when part2 is not null => this.CheckArticle(values, areaId, ArticleDetail.ArticleTypes.Tender, part2, part3, part4),
                "event" when part2 is not null => this.CheckArticle(values, areaId, ArticleDetail.ArticleTypes.Event, part2, part3, part4),
                _ => this.CheckArticle(values, areaId, ArticleDetail.ArticleTypes.Static, part1, part2, part3),
            };
        }
#pragma warning restore S2234 // Parameters should be passed in the correct order

        return ValueTask.FromResult(values);
    }

    public override ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext, RouteValueDictionary values)
    {
        string? part1 = values["part1"] as string;
        string? part2 = values["part2"] as string;
        string? part3 = values["part3"] as string;
        string? part4 = values["part4"] as string;
        int rootAreaId = this.WebsiteFixedContext.RootArea.AreaId;
        return this.Article(values, rootAreaId, part1, part2, part3, part4);
    }
}
