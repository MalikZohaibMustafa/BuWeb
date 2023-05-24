namespace Bu.Web.Website.Controllers;

public sealed class ArticlesController : BaseController
{
    public ArticlesController(WebsiteContextProvider websiteContextProvider, IApplicationClock applicationClock)
    {
        this.WebsiteContextProvider = websiteContextProvider;
        this.UtcNow = applicationClock.UtcNow;
    }

    private WebsiteContextProvider WebsiteContextProvider { get; }

    private DateTime UtcNow { get; }

    protected override InstituteIds InstituteId => throw new InvalidOperationException();

    protected override string? AreaName => null;

    protected override string AreaPath => Area.RootPath;

    private ArticlePage? GetPublishedArticlePage(
        IWebsiteContext websiteContext,
        int areaId,
        string articleUniqueId,
        string? pageUniqueId,
        ArticleDetail.ArticleTypes articleType)
    {
        ArticleDetail? articleDetail = websiteContext.GetPublishedArticleDetails(this.UtcNow)
            .Include(ad => ad.ArticlePages)
            .Include(ap => ap.AreaLayout)
            .Include(ap => ap.Area)
            .SingleOrDefault(ad => ad.ArticleUniqueId == articleUniqueId && ad.ArticleType == articleType && ad.AreaId == areaId);

        if (articleDetail == null)
        {
            return null;
        }

        if (!string.IsNullOrWhiteSpace(pageUniqueId))
        {
            return articleDetail.ArticlePages!.SingleOrDefault(ap => ap.PageUniqueId.Equals(pageUniqueId, StringComparison.InvariantCultureIgnoreCase));
        }

        if (articleDetail.DefaultArticlePageId != null)
        {
            return articleDetail.ArticlePages!.SingleOrDefault(ap => ap.ArticlePageId == articleDetail.DefaultArticlePageId.Value);
        }

        return articleDetail.ArticlePages!.Where(ap => ap.ParentArticlePageId == null).MinBy(ap => ap.PageSequence);
    }

    private IActionResult GetPublishedArticleMedia(
        IWebsiteContext websiteContext,
        IFileStorage fileStorage,
        int areaId,
        string articleUniqueId,
        ArticleDetail.ArticleTypes articleType,
        string mediaUniqueId,
        ContentDisposition? contentDisposition)
    {
        ArticleMedia? articleMedia = websiteContext.GetPublishedArticleDetails(this.UtcNow)
            .Where(ad => ad.ArticleUniqueId == articleUniqueId && ad.ArticleType == articleType && ad.AreaId == areaId)
            .SelectMany(ad => ad.ArticleMedias!)
            .Include(am => am.ArticleDetail!).ThenInclude(ad => ad.Area!)
            .SingleOrDefault(am => am.MediaUniqueId == mediaUniqueId);

        if (articleMedia == null)
        {
            return this.NotFound();
        }

        contentDisposition ??= articleMedia.MediaContentDisposition ?? articleMedia.MediaContentType.GetContentDisposition();
        string fileName = articleMedia.GetFileName();
        byte[] bytes = fileStorage.ReadFileBytes(fileName);
        this.Response.Headers.Add("Content-Disposition", $"{contentDisposition.Value.ToString().ToLowerInvariant()};filename={articleMedia.MediaFileName};");
        return new FileContentResult(bytes, articleMedia.MediaContentType.GetMimeType());
    }

    public IActionResult GetPublishedArticlePage(
        int areaId,
        string articleUniqueId,
        ArticleDetail.ArticleTypes articleType,
        string? pageUniqueId)
    {
        using IWebsiteContext websiteContext = this.WebsiteContextProvider.Create();
        ArticlePage? articlePage = this.GetPublishedArticlePage(websiteContext, areaId, articleUniqueId, pageUniqueId, articleType);
        if (articlePage == null)
        {
            return this.NotFound();
        }

        AreaLayout.LayoutTypes layoutType = articlePage.ArticleDetail!.AreaLayout!.LayoutType;
        return layoutType switch
        {
            AreaLayout.LayoutTypes.NoMenu => this.View("ArticlePage", articlePage),
            AreaLayout.LayoutTypes.LeftMenu => this.View("ArticlePageLeftMenu", articlePage),
            AreaLayout.LayoutTypes.RightMenu => this.View("ArticlePageRightMenu", articlePage),
            _ => throw new InvalidOperationException(layoutType.ToString()),
        };
    }

    public IActionResult GetPublishedArticleMedia(
        [FromServices] IFileStorage fileStorage,
        int areaId,
        string articleUniqueId,
        ArticleDetail.ArticleTypes articleType,
        string mediaUniqueId,
        ContentDisposition? contentDisposition)
    {
        using IWebsiteContext websiteContext = this.WebsiteContextProvider.Create();
        return this.GetPublishedArticleMedia(websiteContext, fileStorage, areaId, articleUniqueId, articleType, mediaUniqueId, contentDisposition);
    }

    public IActionResult GetPublishedArticleCarousel(
        [FromServices] IFileStorage fileStorage,
        int areaId,
        string articleUniqueId,
        ArticleDetail.ArticleTypes articleType)
    {
        using IWebsiteContext websiteContext = this.WebsiteContextProvider.Create();
        var articleMedia = websiteContext.GetPublishedArticleDetails(this.UtcNow)
            .Where(ad => ad.ArticleUniqueId == articleUniqueId && ad.ArticleType == articleType && ad.AreaId == areaId && ad.CarouselArticleMediaId != null)
            .Select(ad => new
            {
                ad.CarouselArticleMedia!.MediaUniqueId,
            })
            .SingleOrDefault();

        if (articleMedia == null)
        {
            return this.NotFound();
        }

        return this.GetPublishedArticleMedia(websiteContext, fileStorage, areaId, articleUniqueId, articleType, articleMedia.MediaUniqueId, ContentDisposition.Inline);
    }

    public IActionResult GetPublishedArticleSmallCarousel(
        [FromServices] IFileStorage fileStorage,
        int areaId,
        string articleUniqueId,
        ArticleDetail.ArticleTypes articleType)
    {
        using IWebsiteContext websiteContext = this.WebsiteContextProvider.Create();
        var articleMedia = websiteContext.GetPublishedArticleDetails(this.UtcNow)
            .Where(ad => ad.ArticleUniqueId == articleUniqueId && ad.ArticleType == articleType && ad.AreaId == areaId && ad.SmallCarouselArticleMediaId != null)
            .Select(ad => new
            {
                ad.SmallCarouselArticleMedia!.MediaUniqueId,
            })
            .SingleOrDefault();

        if (articleMedia == null)
        {
            return this.NotFound();
        }

        return this.GetPublishedArticleMedia(websiteContext, fileStorage, areaId, articleUniqueId, articleType, articleMedia.MediaUniqueId, ContentDisposition.Inline);
    }
}