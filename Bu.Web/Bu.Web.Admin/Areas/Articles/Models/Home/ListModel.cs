namespace Bu.Web.Admin.Areas.Articles.Models.Home;

public sealed class ListModel : SortedPageDataModel<ArticleDetail, ListInputModel>
{
    public ListModel(ListInputModel input, ClaimsPrincipal user)
        : base(input, user)
    {
    }

    public ListModel(AdminContextProvider adminContextProvider, ListInputModel input, ClaimsPrincipal user)
        : base(adminContextProvider, input, user)
    {
    }

    protected override IQueryable<ArticleDetail> GetFilteredQuery(IAdminContext adminContext)
    {
        IQueryable<ArticleDetail> query = adminContext.GetArticleDetailsLastVersion()
            .Include(ad => ad.Article)
            .Include(ad => ad.Area);

        var search = this.Input.Search?.ToNullIfEmpty();
        if (search != null)
        {
            query = query.Where(ad => ad.ArticleName.Contains(search) || ad.ArticleUniqueId.Contains(search));
        }

        if (this.Input.Status != null)
        {
            query = query.Where(ad => ad.Status == this.Input.Status.Value);
        }

        if (this.Input.AreaId != null)
        {
            query = query.Where(ad => ad.AreaId == this.Input.AreaId.Value);
        }

        if (this.Input.ArticleType != null)
        {
            query = query.Where(ad => ad.ArticleType == this.Input.ArticleType.Value);
        }

        return query;
    }

    protected override IOrderedQueryable<ArticleDetail> PrepareSortedQuery(IQueryable<ArticleDetail> query)
    {
        return this.Input.Sort switch
        {
            nameof(ArticleDetail.ArticleName) => query.OrderBy(this.Input.Desc, ad => ad.ArticleName),
            nameof(ArticleDetail.ArticleType) => query.OrderBy(this.Input.Desc, ad => ad.ArticleType),
            nameof(ArticleDetail.ArticleUniqueId) => query.OrderBy(this.Input.Desc, ad => ad.ArticleUniqueId),
            nameof(ArticleDetail.Status) => query.OrderBy(this.Input.Desc, ad => ad.Status),
            nameof(ArticleDetail.PublishedOnUtc) => query.OrderBy(this.Input.Desc, ad => ad.PublishedOnUtc),
            nameof(ArticleDetail.ExpiryDateUtc) => query.OrderBy(this.Input.Desc, ad => ad.ExpiryDateUtc),
            nameof(ArticleDetail.Area.AreaPath) => query.OrderBy(this.Input.Desc, ad => ad.Area!.AreaPath),
            _ => throw new NotSupportedException(this.Input.Sort),
        };
    }
}