namespace Bu.Web.Data.Abstraction;

public interface ICommonContext : IDisposable
{
    public DbSet<Area> Areas { get; }

    public DbSet<AreaLayout> AreaLayouts { get; }

    public DbSet<Article> Articles { get; }

    public DbSet<ArticleDetail> ArticleDetails { get; }

    public DbSet<ArticleMedia> ArticleMedias { get; }

    public DbSet<ArticlePage> ArticlePages { get; }

    public DbSet<Department> Departments { get; }

    public DbSet<EventLog> EventLogs { get; }

    public DbSet<Institute> Institutes { get; }

    public DbSet<InstituteLocation> InstituteLocations { get; }

    public DbSet<YoutubeVideo> YoutubeVideos { get; }

    public IQueryable<ArticleDetail> GetPublishedArticleDetails(DateTime utcDateTime)
    {
        var query = this.ArticleDetails
            .Where(ad => ad.PublishedOnUtc <= utcDateTime && ad.Status == IPublished.Statuses.Published)
            .Select(ad => new
            {
                ad.ArticleId,
                ad.PublishedOnUtc,
            }).GroupBy(ad => new
            {
                ad.ArticleId,
            })
            .Select(g => new
            {
                g.Key.ArticleId,
                PublishedOnUtc = g.Max(gg => gg.PublishedOnUtc),
            });

        return from ad in this.ArticleDetails
               join q in query on new { ad.ArticleId, ad.PublishedOnUtc } equals new { q.ArticleId, q.PublishedOnUtc }
               where ad.ExpiryDateUtc == null || ad.ExpiryDateUtc.Value >= utcDateTime
               select ad;
    }

    public IQueryable<ArticleDetail> GetArticleDetailsLastVersion()
    {
        var articleDetailIds = this.ArticleDetails.GroupBy(ad => ad.ArticleId).Select(g => g.Max(gg => gg.ArticleDetailId));
        return this.ArticleDetails.Where(ad => articleDetailIds.Contains(ad.ArticleDetailId));
    }
}