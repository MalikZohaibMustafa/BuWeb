using Bu.Web.Admin.Areas.Articles.Models.Home;

namespace Bu.Web.Admin.Areas.Articles.Controllers;

[Authorize(nameof(AuthorizationPolicies.Webmaster))]
public sealed class HomeController : ArticlesAreaBaseController<HomeController>
{
    public HomeController(ILogger<HomeController> logger, AdminContextProvider adminContextProvider, IAdminFixedContext adminFixedContext)
        : base(logger, adminContextProvider)
    {
        this.AdminFixedContext = adminFixedContext;
    }

    private IAdminFixedContext AdminFixedContext { get; }

    public IActionResult Index()
    {
        return this.RedirectToAction(nameof(this.List), new ListInputModel());
    }

    public IActionResult List(ListInputModel input)
    {
        if (this.ModelState.IsValid)
        {
            return this.View(nameof(this.List), new ListModel(this.AdminContextProvider, input, this.User));
        }

        return this.View(nameof(this.List), new ListModel(input, this.User));
    }

    [HttpGet]
    public IActionResult Display(int id)
    {
        if (id == default)
        {
            return this.BadRequest();
        }

        return this.DisplayArticle(nameof(this.Display), id, _ => true);
    }

    private IActionResult RedirectToDisplay(int id)
    {
        return this.RedirectToAction(nameof(this.Display), new { id });
    }

    [HttpGet]
    public IActionResult New()
    {
        return this.View(nameof(this.New), new ArticleDetail());
    }

    [HttpPost]
    public IActionResult Add(ArticleDetail model)
    {
        if (model.ArticleDetailId != default || model.ArticleId != default)
        {
            return this.BadRequest();
        }

        model.Status = IPublished.Statuses.Pending;
        return this.AddOrUpdateArticleDetail(model);
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        if (id == default)
        {
            return this.BadRequest();
        }

        return this.DisplayArticle(nameof(this.Edit), id, ad => ad.Status == IPublished.Statuses.Pending);
    }

    [HttpPost]
    public IActionResult Update(ArticleDetail model)
    {
        if (model.ArticleDetailId == default || model.ArticleId == default)
        {
            return this.BadRequest();
        }

        model.Status = IPublished.Statuses.Pending;
        return this.AddOrUpdateArticleDetail(model);
    }

    [HttpGet]
    public IActionResult Pending(int id)
    {
        if (id == default)
        {
            return this.BadRequest();
        }

        return this.DisplayArticle(nameof(this.Pending), id, ad => !ad.Status.IsPublished() && !ad.Status.IsPending());
    }

    [HttpGet]
    public IActionResult Reject(int id)
    {
        if (id == default)
        {
            return this.BadRequest();
        }

        return this.DisplayArticle(nameof(this.Reject), id, ad => !ad.Status.IsPublished() && !ad.Status.IsRejected());
    }

    [HttpGet]
    public IActionResult Publish(int id)
    {
        if (id == default)
        {
            return this.BadRequest();
        }

        return this.DisplayArticle(nameof(this.Publish), id, ad => !ad.Status.IsPublished() && ad.Status.IsPending());
    }

    private IActionResult ChangeStatus(int articleDetailId, IPublished.Statuses newStatus)
    {
        if (articleDetailId == default)
        {
            return this.BadRequest();
        }

        if (newStatus == IPublished.Statuses.Published)
        {
            throw new InvalidOperationException(articleDetailId.ToString());
        }

        using IAdminContext adminContext = this.AdminContextProvider.Create();
        ArticleDetail? articleDetail = adminContext.ArticleDetails
            .SingleOrDefault(ad => ad.ArticleDetailId == articleDetailId);
        if (articleDetail == null)
        {
            return this.NotFound();
        }

        if (articleDetail.Status.IsPublished())
        {
            this.AddWarningMessage("Published Article cannot be changed.");
            return this.RedirectToDisplay(articleDetail.ArticleDetailId);
        }

        if (articleDetail.Status == newStatus)
        {
            this.AddWarningMessage($"Article status is already {newStatus.GetDescription()}.");
            return this.RedirectToDisplay(articleDetail.ArticleDetailId);
        }

        articleDetail.Published.SetPublishedStatus(newStatus, this.UserNow, this.UserId, articleDetail.Remarks);
        articleDetail.Published.SetLastUpdated(this.UserNow, this.UserId);
        articleDetail.PreSave(this.User);
        adminContext.SaveChanges(this.User);
        this.AddSuccessMessage($"Article status is now {articleDetail.Status.GetDescription()}.");
        return this.RedirectToDisplay(articleDetail.ArticleDetailId);
    }

    [HttpPost]
    public IActionResult PendingPost(int id)
    {
        return this.ChangeStatus(id, IPublished.Statuses.Pending);
    }

    [HttpPost]
    public IActionResult RejectPost(int id)
    {
        return this.ChangeStatus(id, IPublished.Statuses.Rejected);
    }

    [HttpPost]
    public IActionResult PublishPost(int id)
    {
        if (id == default)
        {
            return this.BadRequest();
        }

        using IAdminContext adminContext = this.AdminContextProvider.Create();
        ArticleDetail? articleDetail = adminContext.ArticleDetails
            .Include(ad => ad.Article!).ThenInclude(a => a.ArticleDetails)
            .Include(ad => ad.ArticleMedias)
            .Include(ad => ad.ArticlePages)
            .Include(ad => ad.AreaLayout!).ThenInclude(al => al.Area)
            .Include(ad => ad.Area)
            .AsSplitQuery()
            .SingleOrDefault(ad => ad.ArticleDetailId == id);
        if (articleDetail == null)
        {
            return this.NotFound();
        }

        if (articleDetail.Status.IsPublished())
        {
            this.AddWarningMessage("Article has already been published.");
            return this.RedirectToDisplay(id);
        }

        articleDetail.Article!.PostQuery(this.User);
        articleDetail.PreSave(this.User);
        List<ValidationResult> validationResults = new List<ValidationResult>();
        articleDetail.Publish(validationResults, adminContext, this.AdminFixedContext, this.UtcNow);
        if (validationResults.IsNotValid())
        {
            return this.DisplayArticle(nameof(this.Display), articleDetail.ArticleDetailId, ad => ad.Published.IsPending);
        }

        articleDetail.Published.SetPublishedStatus(IPublished.Statuses.Published, this.UserNow, this.UserId, articleDetail.Remarks);
        adminContext.SaveChanges(this.User);
        this.AddSuccessMessage("Article has been published.");
        return this.RedirectToDisplay(articleDetail.ArticleDetailId);
    }

    [HttpGet]
    public IActionResult Draft(int id)
    {
        if (id == default)
        {
            return this.BadRequest();
        }

        return this.DisplayArticle(nameof(this.Draft), id, ad => ad.Status.IsPublished() && !ad.Article!.ArticleDetails!.Any(add => add.Status.IsPending()));
    }

    [HttpPost]
    public IActionResult DraftPost(int id)
    {
        using IAdminContext adminContext = this.AdminContextProvider.Create();
        ArticleDetail? previousArticleDetail = adminContext.ArticleDetails
            .Include(ad => ad.ArticlePages)
            .Include(ad => ad.ArticleMedias)
            .SingleOrDefault(ad => ad.ArticleDetailId == id);
        if (previousArticleDetail?.Status.IsPublished() != true)
        {
            return this.NotFound();
        }

        previousArticleDetail.PostQuery(this.User);

        ArticleDetail articleDetail = adminContext.ArticleDetails.Add(new ArticleDetail
        {
            AreaId = previousArticleDetail.AreaId,
            AreaLayoutId = previousArticleDetail.AreaLayoutId,
            ArticleName = previousArticleDetail.ArticleName,
            ArticleUniqueId = previousArticleDetail.ArticleUniqueId,
            ArticleId = previousArticleDetail.ArticleId,
            ArticleType = previousArticleDetail.ArticleType,
            PreviousArticleDetailId = previousArticleDetail.ArticleDetailId,
            ArticlePages = previousArticleDetail.ArticlePages!.Select(previousArticlePage =>
            {
                ArticlePage articlePage = new ArticlePage
                {
                    MenuText = previousArticlePage.MenuText,
                    PageBodyTextFormat = previousArticlePage.PageBodyTextFormat,
                    PageBody = previousArticlePage.PageBody,
                    PageSequence = previousArticlePage.PageSequence,
                    PageTitle = previousArticlePage.PageTitle,
                    PageUniqueId = previousArticlePage.PageUniqueId,
                };
                articlePage.TrackedEntity.SetCreated(previousArticlePage.CreatedOnUser, previousArticlePage.CreatedByUserId);
                articlePage.TrackedEntity.SetLastUpdated(previousArticlePage.LastUpdatedOnUser, previousArticlePage.LastUpdatedByUserId);
                return articlePage;
            }).ToList(),
            ArticleMedias = previousArticleDetail.ArticleMedias!.Select(previousArticleMedia =>
            {
                ArticleMedia articleMedia = new ArticleMedia
                {
                    ArticleMediaGuid = previousArticleMedia.ArticleMediaGuid,
                    MediaContentDisposition = previousArticleMedia.MediaContentDisposition,
                    MediaContentType = previousArticleMedia.MediaContentType,
                    MediaFileName = previousArticleMedia.MediaFileName,
                    MediaUniqueId = previousArticleMedia.MediaUniqueId,
                };
                articleMedia.TrackedEntity.SetLastUpdated(previousArticleMedia.LastUpdatedOnUser, previousArticleMedia.LastUpdatedByUserId);
                articleMedia.TrackedEntity.SetCreated(previousArticleMedia.CreatedOnUser, previousArticleMedia.CreatedByUserId);
                return articleMedia;
            }).ToList(),
        }).Entity;

        articleDetail.Published.SetExpiry(previousArticleDetail.ExpiryDateUser);
        articleDetail.Published.SetCreated(previousArticleDetail.CreatedOnUser, previousArticleDetail.CreatedByUserId);
        articleDetail.Published.SetLastUpdated(this.UtcNow.ToUserDateTimeFromUtc(this.User), this.UserId);
        articleDetail.Published.SetPublishedStatus(IPublished.Statuses.Pending, this.UserNow, this.UserId, previousArticleDetail.Remarks);

        adminContext.SaveChanges(this.User);

        if (previousArticleDetail.DefaultArticlePageId != null)
        {
            ArticlePage defaultArticlePage = previousArticleDetail.ArticlePages!.Single(ap => ap.ArticlePageId == previousArticleDetail.DefaultArticlePageId.Value);
            articleDetail.DefaultArticlePageId = articleDetail.ArticlePages!.Single(ap => ap.PageUniqueId == defaultArticlePage.PageUniqueId).ArticlePageId;
        }

        if (previousArticleDetail.CarouselArticleMediaId != null)
        {
            ArticleMedia carouselArticleMedia = previousArticleDetail.ArticleMedias!.Single(am => am.ArticleMediaId == previousArticleDetail.CarouselArticleMediaId.Value);
            articleDetail.CarouselArticleMediaId = articleDetail.ArticleMedias!.Single(am => am.ArticleMediaGuid == carouselArticleMedia.ArticleMediaGuid).ArticleMediaId;
        }

        foreach (ArticlePage articlePage in articleDetail.ArticlePages!)
        {
            ArticlePage previousArticlePage = previousArticleDetail.ArticlePages!.Single(ap => ap.PageUniqueId == articlePage.PageUniqueId);
            if (previousArticlePage.ParentArticlePage != null)
            {
                articlePage.ParentArticlePageId = articleDetail.ArticlePages.Single(ap => ap.PageUniqueId == previousArticlePage.ParentArticlePage.PageUniqueId).ArticlePageId;
            }
        }

        adminContext.SaveChanges(this.User);

        this.AddSuccessMessage("Draft has been created.");
        return this.RedirectToDisplay(articleDetail.ArticleDetailId);
    }

    private IActionResult DisplayArticle(string viewName, int id, Func<ArticleDetail, bool> filter, ArticleDetail? cloneValuesFromArticleDetail = null)
    {
        using IAdminContext adminContext = this.AdminContextProvider.Create();
        ArticleDetail? articleDetail = adminContext.ArticleDetails
            .Include(ad => ad.Article).ThenInclude(a => a!.ArticleDetails)
            .Include(ad => ad.Area)
            .Include(ad => ad.AreaLayout)
            .Include(ad => ad.ArticleMedias)
            .Include(ad => ad.ArticlePages)
            .AsSplitQuery()
            .SingleOrDefault(ad => ad.ArticleDetailId == id);
        if (articleDetail == null || !filter(articleDetail))
        {
            return this.NotFound();
        }

        if (cloneValuesFromArticleDetail != null)
        {
            articleDetail.AreaLayoutId = cloneValuesFromArticleDetail.AreaLayoutId;
            articleDetail.ArticleName = cloneValuesFromArticleDetail.ArticleName;
            articleDetail.ArticleUniqueId = cloneValuesFromArticleDetail.ArticleUniqueId;
            articleDetail.ArticleType = cloneValuesFromArticleDetail.ArticleType;
            articleDetail.PublishedOnUser = cloneValuesFromArticleDetail.PublishedOnUser;
            articleDetail.PublishedOnUtc = cloneValuesFromArticleDetail.PublishedOnUser.ToUtcFromUserDateTime(this.User);
            articleDetail.ExpiryDateUser = cloneValuesFromArticleDetail.ExpiryDateUser;
            articleDetail.ExpiryDateUtc = cloneValuesFromArticleDetail.ExpiryDateUser?.ToUtcFromUserDateTime(this.User);
            articleDetail.DefaultArticlePageId = cloneValuesFromArticleDetail.DefaultArticlePageId;
            articleDetail.CarouselArticleMediaId = cloneValuesFromArticleDetail.CarouselArticleMediaId;
            articleDetail.Remarks = cloneValuesFromArticleDetail.Remarks;
        }

        articleDetail.Article!.PostQuery(this.User);

        if (articleDetail.Published.IsPending && viewName is nameof(this.Display) or nameof(this.Publish))
        {
            List<ValidationResult> validationResults = new List<ValidationResult>();
            articleDetail.Publish(validationResults, adminContext, this.AdminFixedContext, this.UtcNow);
            if (validationResults.IsNotValid())
            {
                this.ModelState.AddValidationResults(validationResults);
            }
        }

        return this.View(viewName, articleDetail);
    }

    internal IActionResult AddOrUpdateArticleDetail(ArticleDetail model)
    {
        using IAdminContext adminContext = this.AdminContextProvider.Create();
        ArticleDetail? articleDetail;
        if (model.ArticleId == default && model.ArticleDetailId == default)
        {
            model.Status = IPublished.Statuses.Pending;
            articleDetail = new ArticleDetail
            {
                AreaId = adminContext.AreaLayouts.Single(al => al.AreaLayoutId == model.AreaLayoutId).AreaId,
            };

            Article article = new Article
            {
                CreatedByUserId = this.UserId,
                CreatedOnUtc = this.UtcNow,
                ArticleDetails = new List<ArticleDetail>
                {
                    articleDetail,
                },
            };
            articleDetail.Published.SetCreated(this.UserNow, this.UserId);
            articleDetail.Published.SetPublishedStatus(model.Status, model.PublishedOnUser, this.UserId, string.Empty);
            adminContext.Articles.Add(article);
        }
        else
        {
            articleDetail = adminContext.ArticleDetails
                .SingleOrDefault(ad => ad.ArticleDetailId == model.ArticleDetailId && ad.ArticleId == model.ArticleId);
            if (articleDetail?.Published.IsPending != true)
            {
                return this.NotFound();
            }

            articleDetail.PostQuery(this.User);

            articleDetail.DefaultArticlePageId = model.DefaultArticlePageId;
            articleDetail.CarouselArticleMediaId = model.CarouselArticleMediaId;
        }

        articleDetail.AreaLayoutId = model.AreaLayoutId;
        articleDetail.ArticleUniqueId = model.ArticleUniqueId;
        articleDetail.ArticleName = model.ArticleName;
        articleDetail.ArticleType = model.ArticleType;
        articleDetail.EventStartDate = model.EventStartDate;
        articleDetail.EventEndDate = model.EventEndDate;
        articleDetail.ExpiryDateUser = model.ExpiryDateUser;
        articleDetail.SmallCarouselArticleMediaId = model.SmallCarouselArticleMediaId;
        articleDetail.ArticleShortDescription = model.ArticleShortDescription;
        articleDetail.Published.SetExpiry(model.ExpiryDateUser);
        articleDetail.Published.SetPublishedStatus(model.Status, model.PublishedOnUser, this.UserId, model.Remarks);
        articleDetail.Published.SetLastUpdated(this.UserNow, this.UserId);
        articleDetail.PreSave(this.User);

        List<ValidationResult> validationResults = new List<ValidationResult>();
        if (!articleDetail.Validate(validationResults))
        {
            this.ModelState.AddValidationResults(validationResults);
            if (articleDetail.ArticleDetailId == default)
            {
                return this.View(model);
            }
            else
            {
                return this.DisplayArticle(nameof(this.Edit), articleDetail.ArticleDetailId, ad => ad.Published.IsPending, model);
            }
        }

        adminContext.SaveChanges(this.User);
        this.AddSuccessMessage("Article has been updated.");
        return this.RedirectToDisplay(articleDetail.ArticleDetailId);
    }
}