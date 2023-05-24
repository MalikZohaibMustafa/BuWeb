namespace Bu.Web.Data.Abstraction;

public interface IWebsiteCachedContext : ICommonCachedContext
{
    IReadOnlyList<YoutubeVideo> YoutubeVideos { get; }

    IReadOnlyList<EventDetail> EventDetails { get; }

    IReadOnlyList<NewsOrTenderDetail> News { get; }

    IReadOnlyList<NewsOrTenderDetail> Tenders { get; }

    IReadOnlyList<CarouselItem> CarouselItems { get; }

    ILookup<int, EventDetail> AreaEventDetailsLookup { get; }

    ILookup<int, NewsOrTenderDetail> AreaNewsLookup { get; }

    ILookup<int, NewsOrTenderDetail> AreaTendersLookup { get; }

    ILookup<int, CarouselItem> AreaCarouselItemLookup { get; }

    ILookup<int, YoutubeVideo> AreaYoutubeVideosLookup { get; }

    ILookup<string, StaticArticleDetail> StaticArticleDetailsLookup { get; }

    ILookup<string, StaticArticleMedia> StaticArticleMediasLookup { get; }
}