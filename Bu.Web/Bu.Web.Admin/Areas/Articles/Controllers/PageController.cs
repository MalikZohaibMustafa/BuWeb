namespace Bu.Web.Admin.Areas.Articles.Controllers;

[Authorize(nameof(AuthorizationPolicies.Webmaster))]
public sealed class PageController : ArticlesAreaBaseController<PageController>
{
    public PageController(ILogger<PageController> logger, AdminContextProvider adminContextProvider)
        : base(logger, adminContextProvider)
    {
    }

    private IActionResult RedirectToPageDisplay(int articleDetailId, int articlePageId)
    {
        return this.RedirectToAction(nameof(this.Display), "Page", new { area = "Articles", articleDetailId, articlePageId });
    }

    private IActionResult RedirectToArticleDisplay(int articleDetailId)
    {
        return this.RedirectToAction(nameof(this.Display), "Home", new { area = "Articles", id = articleDetailId });
    }

    [HttpGet("[area]/[controller]/[action]/{articleDetailId:int}/{articlePageId:int}")]
    public IActionResult Display(int articleDetailId, int articlePageId)
    {
        if (articleDetailId == default || articlePageId == default)
        {
            return this.BadRequest();
        }

        return this.DisplayPage(nameof(this.Display), articleDetailId, articlePageId);
    }

    [HttpGet]
    public IActionResult New(int id)
    {
        if (id == default)
        {
            return this.BadRequest();
        }

        return this.DisplayPage(nameof(this.New), articleDetailId: id, articlePageId: null);
    }

    [HttpPost]
    public IActionResult Add(ArticlePage model)
    {
        if (model.ArticlePageId != default || model.ArticleDetailId == default)
        {
            return this.BadRequest();
        }

        return this.AddOrUpdatePage(model);
    }

    [HttpGet("[area]/[controller]/[action]/{articleDetailId:int}/{articlePageId:int}")]
    public IActionResult Edit(int articleDetailId, int articlePageId)
    {
        if (articleDetailId == default || articlePageId == default)
        {
            return this.BadRequest();
        }

        return this.DisplayPage(nameof(this.Edit), articleDetailId, articlePageId);
    }

    [HttpPost]
    public IActionResult Update(ArticlePage model)
    {
        if (model.ArticlePageId == default || model.ArticleDetailId == default)
        {
            return this.BadRequest();
        }

        return this.AddOrUpdatePage(model);
    }

    [HttpGet("[area]/[controller]/[action]/{articleDetailId:int}/{articlePageId:int}")]
    public IActionResult Delete(int articleDetailId, int articlePageId)
    {
        if (articleDetailId == default || articlePageId == default)
        {
            return this.BadRequest();
        }

        return this.DisplayPage(nameof(this.Delete), articleDetailId, articlePageId);
    }

    [HttpPost("[area]/[controller]/[action]/{articleDetailId:int}/{articlePageId:int}")]
    public IActionResult DeletePost(int articleDetailId, int articlePageId)
    {
        if (articleDetailId == default || articlePageId == default)
        {
            return this.BadRequest();
        }

        using IAdminContext adminContext = this.AdminContextProvider.Create();
        ArticlePage? articlePage = adminContext.ArticlePages
            .Include(ap => ap.ArticleDetail)
            .SingleOrDefault(ap => ap.ArticlePageId == articlePageId && ap.ArticleDetailId == articleDetailId);
        if (articlePage == null)
        {
            return this.NotFound();
        }

        if (!articlePage.ArticleDetail!.Status.IsPending())
        {
            this.AddPublishedArticleCannotBeEdited(articlePage.ArticleDetail);
            return this.RedirectToPageDisplay(articlePage.ArticleDetailId, articlePage.ArticlePageId);
        }

        if (articlePage.ArticleDetail!.DefaultArticlePageId == articlePage.ArticlePageId)
        {
            articlePage.ArticleDetail.DefaultArticlePageId = null;
        }

        articlePage.ArticleDetail.TrackedEntity.SetLastUpdated(this.UserNow, this.UserId);
        articlePage.ArticleDetail.PreSave(this.User);
        adminContext.ArticlePages.Remove(articlePage);
        adminContext.SaveChanges(this.User);
        this.AddSuccessMessage("Article Page has been deleted.");
        return this.RedirectToArticleDisplay(articlePage.ArticleDetailId);
    }

    [HttpPost("[area]/[controller]/[action]/{articleDetailId:int}/{articlePageId:int}")]
    public IActionResult MoveUp(int articleDetailId, int articlePageId)
    {
        if (articleDetailId == default || articlePageId == default)
        {
            return this.BadRequest();
        }

        using IAdminContext adminContext = this.AdminContextProvider.Create();
        ArticleDetail? articleDetail = adminContext.ArticleDetails
            .Include(ad => ad.ArticlePages)
            .SingleOrDefault(ad => ad.ArticleDetailId == articleDetailId);
        if (articleDetail == null)
        {
            return this.NotFound();
        }

        if (!articleDetail.Status.IsPending())
        {
            this.AddPublishedArticleCannotBeEdited(articleDetail);
            return this.RedirectToArticleDisplay(articleDetail.ArticleDetailId);
        }

        List<ArticlePage> allPages = articleDetail.ArticlePages!.ToList();
        ArticlePage? articlePage = allPages.SingleOrDefault(p => p.ArticlePageId == articlePageId);
        if (articlePage == null)
        {
            return this.NotFound();
        }

        ArticlePage? previousArticlePage = allPages.OrderBy(p => p.PageSequence).LastOrDefault(p =>
            p != articlePage
            && p.ParentArticlePageId == articlePage.ParentArticlePageId
            && p.PageSequence <= articlePage.PageSequence);

        if (previousArticlePage != null)
        {
            (articlePage.PageSequence, previousArticlePage.PageSequence) = (previousArticlePage.PageSequence, articlePage.PageSequence);
            articlePage.TrackedEntity.SetLastUpdated(this.UserNow, this.UserId);
            previousArticlePage.TrackedEntity.SetLastUpdated(this.UserNow, this.UserId);
        }

        this.SortPages(articleDetail);
        articleDetail.TrackedEntity.SetLastUpdated(this.UserNow, this.UserId);
        adminContext.SaveChanges(this.User);
        this.AddSuccessMessage("Article Page has been moved up.");
        return this.RedirectToArticleDisplay(articleDetail.ArticleDetailId);
    }

    [HttpPost("[area]/[controller]/[action]/{articleDetailId:int}/{articlePageId:int}")]
    public IActionResult MoveDown(int articleDetailId, int articlePageId)
    {
        if (articleDetailId == default || articlePageId == default)
        {
            return this.BadRequest();
        }

        using IAdminContext adminContext = this.AdminContextProvider.Create();
        ArticleDetail? articleDetail = adminContext.ArticleDetails
            .Include(ad => ad.ArticlePages)
            .SingleOrDefault(ad => ad.ArticleDetailId == articleDetailId);
        if (articleDetail == null)
        {
            return this.NotFound();
        }

        if (!articleDetail.Status.IsPending())
        {
            this.AddPublishedArticleCannotBeEdited(articleDetail);
            return this.RedirectToArticleDisplay(articleDetail.ArticleDetailId);
        }

        List<ArticlePage> allPages = articleDetail.ArticlePages!.ToList();
        ArticlePage? articlePage = allPages.SingleOrDefault(p => p.ArticlePageId == articlePageId);
        if (articlePage == null)
        {
            return this.NotFound();
        }

        ArticlePage? nextArticlePage = allPages.OrderBy(p => p.PageSequence).FirstOrDefault(p =>
            p != articlePage
            && p.ParentArticlePageId == articlePage.ParentArticlePageId
            && p.PageSequence >= articlePage.PageSequence);

        if (nextArticlePage != null)
        {
            (articlePage.PageSequence, nextArticlePage.PageSequence) = (nextArticlePage.PageSequence, articlePage.PageSequence);
            articlePage.TrackedEntity.SetLastUpdated(this.UserNow, this.UserId);
            nextArticlePage.TrackedEntity.SetLastUpdated(this.UserNow, this.UserId);
        }

        this.SortPages(articleDetail);
        articleDetail.TrackedEntity.SetLastUpdated(this.UserNow, this.UserId);
        adminContext.SaveChanges(this.User);
        this.AddSuccessMessage("Article Page has been moved down.");
        return this.RedirectToArticleDisplay(articleDetail.ArticleDetailId);
    }

    private void AddPublishedArticleCannotBeEdited(ArticleDetail articleDetail)
    {
        this.AddErrorMessage($"{articleDetail.Status.GetDescription()} Article cannot be edited.");
    }

    private (ArticleDetail ArticleDetail, ArticlePage? ArticlePage)? GetPageModel(int articleDetailId, int? articlePageId)
    {
        using IAdminContext adminContext = this.AdminContextProvider.Create();
        ArticleDetail? articleDetail = adminContext.ArticleDetails
            .Include(ad => ad.Area)
            .Include(ad => ad.AreaLayout)
            .Include(ad => ad.Article)
            .Include(ad => ad.ArticleMedias)
            .Include(ad => ad.ArticlePages)
            .AsSplitQuery()
            .SingleOrDefault(ad => ad.ArticleDetailId == articleDetailId);
        if (articleDetail == null)
        {
            return null;
        }

        ArticlePage? articlePage = null;
        if (articlePageId != null)
        {
            articlePage = articleDetail.ArticlePages!.SingleOrDefault(p => p.ArticlePageId == articlePageId.Value);
            if (articlePage == null)
            {
                return null;
            }
        }

        articleDetail.Article!.PostQuery(this.User);
        return (articleDetail, articlePage!);
    }

    private IActionResult DisplayPage(string viewName, int articleDetailId, int? articlePageId)
    {
        (ArticleDetail ArticleDetail, ArticlePage? ArticlePage)? model = this.GetPageModel(articleDetailId, articlePageId);
        if (model == null)
        {
            return this.NotFound();
        }

        return this.DisplayPage(viewName, model.Value);
    }

    private IActionResult DisplayPage(string viewName, (ArticleDetail ArticleDetail, ArticlePage? ArticlePage) model)
    {
        if (model.ArticlePage == null || model.ArticlePage.ArticlePageId == default)
        {
            if (!model.ArticleDetail.Status.IsPending())
            {
                this.AddPublishedArticleCannotBeEdited(model.ArticleDetail);
                return this.RedirectToArticleDisplay(model.ArticleDetail.ArticleDetailId);
            }

            if (model.ArticlePage == null)
            {
                ArticlePage articlePage = new ArticlePage
                {
                    ArticlePageId = default,
                    ArticleDetailId = model.ArticleDetail.ArticleDetailId,
                    ArticleDetail = model.ArticleDetail,
                };
                return this.View(viewName, (model.ArticleDetail, articlePage));
            }
        }

        return this.View(viewName, model);
    }

    private IActionResult DisplayPage(ArticlePage existingModel, IEnumerable<ValidationResult> validationResults)
    {
        (ArticleDetail ArticleDetail, ArticlePage? ArticlePage)? model = this.GetPageModel(existingModel.ArticleDetailId, existingModel.ArticlePageId);
        if (model == null)
        {
            return this.NotFound();
        }

        this.ModelState.AddValidationResults(validationResults);

        ArticlePage articlePage = model.Value.ArticlePage ?? new ArticlePage
        {
            ArticlePageId = default,
            ArticleDetailId = model.Value.ArticleDetail.ArticleDetailId,
            ArticleDetail = model.Value.ArticleDetail,
        };
        articlePage.PageTitle = existingModel.PageTitle;
        articlePage.PageUniqueId = existingModel.PageUniqueId;
        articlePage.MenuText = existingModel.MenuText;
        articlePage.PageBody = existingModel.PageBody;
        articlePage.PageBodyTextFormat = existingModel.PageBodyTextFormat;

        return this.DisplayPage(
            existingModel.ArticlePageId == default ? nameof(this.New) : nameof(this.Edit),
            (model.Value.ArticleDetail, articlePage));
    }

    private IActionResult AddOrUpdatePage(ArticlePage model)
    {
        using IAdminContext adminContext = this.AdminContextProvider.Create();
        ArticleDetail? articleDetail = adminContext.ArticleDetails
            .Include(ad => ad.ArticlePages)
            .SingleOrDefault(ad => ad.ArticleDetailId == model.ArticleDetailId);
        if (articleDetail == null)
        {
            return this.NotFound();
        }

        articleDetail.PostQuery(this.User);

        if (!articleDetail.Published.IsPending)
        {
            this.AddPublishedArticleCannotBeEdited(articleDetail);
            return this.RedirectToArticleDisplay(articleDetail.ArticleDetailId);
        }

        ArticlePage? articlePage;
        if (model.ArticlePageId == default)
        {
            articlePage = new ArticlePage
            {
                ArticleDetailId = articleDetail.ArticleDetailId,
                PageSequence = articleDetail.ArticlePages!.Any() ? articleDetail.ArticlePages!.Max(ap => ap.PageSequence) + 1 : 0,
            };

            articlePage.TrackedEntity.SetCreated(this.UserNow, this.UserId);
            articleDetail.ArticlePages!.Add(articlePage);
        }
        else
        {
            articlePage = articleDetail.ArticlePages!.SingleOrDefault(ap => ap.ArticlePageId == model.ArticlePageId);
            if (articlePage == null)
            {
                return this.NotFound();
            }

            articlePage.PostQuery(this.User);
        }

        articlePage.PageTitle = model.PageTitle;
        articlePage.MenuText = model.MenuText;
        articlePage.PageBody = model.PageBody;
        articlePage.PageUniqueId = model.PageUniqueId;
        articlePage.ParentArticlePageId = model.ParentArticlePageId;
        articlePage.PageBodyTextFormat = model.PageBodyTextFormat;
        articlePage.TrackedEntity.SetLastUpdated(this.UserNow, this.UserId);
        articleDetail.TrackedEntity.SetLastUpdated(this.UserNow, this.UserId);
        List<ValidationResult> validationResults = new List<ValidationResult>();
        articlePage.Validate(validationResults);

        bool duplicateUniqueIDsFound = articleDetail.ArticlePages!.GroupBy(ap => ap.PageUniqueId.ToLower()).Any(g => g.Count() > 1);
        if (duplicateUniqueIDsFound)
        {
            validationResults.Add(new ValidationResult("Circular Reference detected.", new[] { nameof(articlePage.ParentArticlePageId) }));
        }

        if (validationResults.IsValid() && !this.SortPages(articleDetail))
        {
            validationResults.Add(new ValidationResult("Circular Reference detected.", new[] { nameof(articlePage.ParentArticlePageId) }));
        }

        if (validationResults.IsNotValid())
        {
            return this.DisplayPage(model, validationResults);
        }

        articlePage.PreSave(this.User);
        articleDetail.PreSave(this.User);
        adminContext.SaveChanges(this.User);
        this.AddSuccessMessage($"Article Page has been {(model.ArticlePageId == default ? "added" : "updated")}.");
        return this.RedirectToPageDisplay(articlePage.ArticleDetailId, articlePage.ArticlePageId);
    }

    private bool SortPages(ArticleDetail articleDetail)
    {
        List<ArticlePage>? allPages = articleDetail.ArticlePages!.OrderBy(p => p.ParentArticlePageId).ThenBy(p => p.PageSequence).ToList();
        List<ArticlePage> processed = new List<ArticlePage>();

        bool Sort(List<ArticlePage> pages)
        {
            int pageSequence = default;
            foreach (ArticlePage articlePage in pages)
            {
                if (processed.Contains(articlePage))
                {
                    return false;
                }

                if (articlePage.PageSequence != pageSequence)
                {
                    articlePage.PageSequence = pageSequence;
                    articlePage.TrackedEntity.SetLastUpdated(this.UserNow, this.UserId);
                    articleDetail.TrackedEntity.SetLastUpdated(this.UserNow, this.UserId);
                }

                pageSequence++;
                processed.Add(articlePage);
                List<ArticlePage> childPages = allPages.Where(p => p.ParentArticlePageId == articlePage.ArticlePageId).ToList();
                if (childPages.Any() && !Sort(childPages))
                {
                    return false;
                }
            }

            return true;
        }

        return Sort(allPages.Where(p => p.ParentArticlePageId == null).ToList());
    }
}