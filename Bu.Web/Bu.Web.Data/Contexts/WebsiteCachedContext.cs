namespace Bu.Web.Data.Contexts;

public sealed class WebsiteCachedContext : CommonCachedContext<IWebsiteContext, WebsiteCachedContext.WebsiteCache>, IWebsiteCachedContext
{
    private static readonly TimeSpan CacheExpiryTimeSpan = TimeSpan.FromMinutes(5);

    public WebsiteCachedContext(WebsiteContextProvider websiteContextProvider, IMemoryCache memoryCache)
        : base(websiteContextProvider.Create, memoryCache, CacheExpiryTimeSpan)
    {
    }

    public IReadOnlyList<YoutubeVideo> YoutubeVideos => this.GetCache().YoutubeVideos;

    public IReadOnlyList<EventDetail> EventDetails => this.GetCache().EventDetails;

    public IReadOnlyList<NewsOrTenderDetail> News => this.GetCache().News;

    public IReadOnlyList<NewsOrTenderDetail> Tenders => this.GetCache().Tenders;

    public IReadOnlyList<CarouselItem> CarouselItems => this.GetCache().CarouselItems;

    public ILookup<int, YoutubeVideo> AreaYoutubeVideosLookup => this.GetCache().AreaYoutubeVideosLookup;

    public ILookup<int, EventDetail> AreaEventDetailsLookup => this.GetCache().AreaEventDetailsLookup;

    public ILookup<int, NewsOrTenderDetail> AreaNewsLookup => this.GetCache().AreaNewsLookup;

    public ILookup<int, NewsOrTenderDetail> AreaTendersLookup => this.GetCache().AreaTendersLookup;

    public ILookup<int, CarouselItem> AreaCarouselItemLookup => this.GetCache().AreaCarouselItemLookup;

    public ILookup<string, StaticArticleDetail> StaticArticleDetailsLookup => this.GetCache().StaticArticleDetailsLookup;

    public ILookup<string, StaticArticleMedia> StaticArticleMediasLookup => this.GetCache().StaticArticleMediasLookup;

    public sealed class WebsiteCache : CommonCache, IWebsiteCachedContext
    {
        public override void Init(IWebsiteContext context)
        {
            DateTime now = DateTimeHelper.UtcNow;

            List<YoutubeVideo> youtubeVideos = context.YoutubeVideos.ToList();
            this.EventDetails = context.ArticleDetails
                .Where(x => x.ArticleType == ArticleDetail.ArticleTypes.Event)
                .Select(x => new EventDetail
                {
                    AreaId = x.AreaId,
                    AreaPath = x.Area!.AreaPath,
                    ArticleName = x.ArticleName,
                    StartDate = x.EventStartDate,
                    EndDate = x.EventEndDate,
                    ArticleUniqueId = x.ArticleUniqueId,
                    ShortDescription = x.ArticleShortDescription,
                })
                .OrderByDescending(x => x.StartDate).ThenBy(x => x.EndDate)
                .ToList();

            List<NewsOrTenderDetail> newOrTenderDetails = context.GetPublishedArticleDetails(now)
                .Select(ad => new NewsOrTenderDetail
                {
                    ArticleId = ad.ArticleId,
                    ArticleDetailId = ad.ArticleDetailId,
                    ArticleType = ad.ArticleType,
                    AreaId = ad.AreaId,
                    AreaPath = ad.Area!.AreaPath,
                    PublishedOnUtc = ad.PublishedOnUtc,
                    ExpiryDateUtc = ad.ExpiryDateUtc,
                    ArticleName = ad.ArticleName,
                    ArticleShortDescription = ad.ArticleShortDescription,
                })
                .ToList();

            this.News = newOrTenderDetails.Where(d => d.ArticleType == ArticleDetail.ArticleTypes.News)
                .OrderByDescending(ad => ad.PublishedOnUtc).ThenBy(ad => ad.ExpiryDateUtc)
                .ToList();

            this.Tenders = newOrTenderDetails.Where(d => d.ArticleType == ArticleDetail.ArticleTypes.Tender)
                .OrderByDescending(ad => ad.PublishedOnUtc).ThenBy(ad => ad.ExpiryDateUtc)
                .ToList();

            this.CarouselItems = context.GetPublishedArticleDetails(now)
                .Where(ad => ad.CarouselArticleMediaId != null)
                .Select(ad => new CarouselItem
                {
                    AreaId = ad.AreaId,
                    ArticleName = ad.Name,
                    ArticleType = ad.ArticleType,
                    ArticleUniqueId = ad.ArticleUniqueId,
                    AreaPath = ad.Area!.AreaPath,
                    ExpiryDateUtc = ad.ExpiryDateUtc,
                }).ToList();

            this.AreaYoutubeVideosLookup = youtubeVideos.ToLookup(x => x.AreaId);

            this.StaticArticleDetailsLookup = context.GetPublishedArticleDetails(now)
                .Where(ad => ad.ArticleType == ArticleDetail.ArticleTypes.Static)
                .Select(ad => new StaticArticleDetail
                {
                    AreaPath = ad.Area!.AreaPath,
                    ArticleUniqueId = ad.ArticleUniqueId,
                    ExpiryDateUtc = ad.ExpiryDateUtc,
                })
                .AsEnumerable()
                .ToLookup(x => $"{x.AreaPath}/{x.ArticleUniqueId}");

            this.StaticArticleMediasLookup = context.GetPublishedArticleDetails(now)
                .Where(ad => ad.ArticleType == ArticleDetail.ArticleTypes.Static)
                .SelectMany(ad => ad.ArticleMedias!)
                .Select(am => new StaticArticleMedia
                {
                    AreaPath = am.ArticleDetail!.Area!.AreaPath,
                    ArticleUniqueId = am.ArticleDetail!.ArticleUniqueId,
                    ExpiryDateUtc = am.ArticleDetail!.ExpiryDateUtc,
                    MediaUniqueId = am.MediaUniqueId,
                })
                .AsEnumerable()
                .ToLookup(x => $"{x.AreaPath}/{x.ArticleUniqueId}/Media/{x.MediaUniqueId}");

            this.AreaEventDetailsLookup = this.EventDetails.ToLookup(nd => nd.AreaId);
            this.AreaNewsLookup = this.News.ToLookup(nd => nd.AreaId);
            this.AreaTendersLookup = this.Tenders.ToLookup(nd => nd.AreaId);
            this.AreaCarouselItemLookup = this.CarouselItems.ToLookup(ci => ci.AreaId);
        }

        public IReadOnlyList<YoutubeVideo> YoutubeVideos { get; private set; } = null!;

        public IReadOnlyList<EventDetail> EventDetails { get; private set; } = null!;

        public IReadOnlyList<NewsOrTenderDetail> News { get; private set; } = null!;

        public IReadOnlyList<NewsOrTenderDetail> Tenders { get; private set; } = null!;

        public IReadOnlyList<CarouselItem> CarouselItems { get; private set; } = null!;

        public ILookup<int, EventDetail> AreaEventDetailsLookup { get; private set; } = null!;

        public ILookup<int, YoutubeVideo> AreaYoutubeVideosLookup { get; private set; } = null!;

        public ILookup<int, NewsOrTenderDetail> AreaNewsLookup { get; private set; } = null!;

        public ILookup<int, NewsOrTenderDetail> AreaTendersLookup { get; private set; } = null!;

        public ILookup<int, CarouselItem> AreaCarouselItemLookup { get; private set; } = null!;

        public ILookup<string, StaticArticleDetail> StaticArticleDetailsLookup { get; private set; } = null!;

        public ILookup<string, StaticArticleMedia> StaticArticleMediasLookup { get; private set; } = null!;
    }
}