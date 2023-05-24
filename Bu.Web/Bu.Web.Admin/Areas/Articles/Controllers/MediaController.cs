namespace Bu.Web.Admin.Areas.Articles.Controllers;

[Authorize(nameof(AuthorizationPolicies.Webmaster))]
public sealed class MediaController : ArticlesAreaBaseController<MediaController>
{
    private readonly IFileStorage fileStorage;

    public MediaController(ILogger<MediaController> logger, AdminContextProvider adminContextProvider, IFileStorage fileStorage)
        : base(logger, adminContextProvider)
    {
        this.fileStorage = fileStorage;
    }

    private IActionResult RedirectToMediaDisplay(int articleDetailId, int articleMediaId)
    {
        return this.RedirectToAction(nameof(this.Display), "Media", new { area = "Articles", articleDetailId, articleMediaId });
    }

    private IActionResult RedirectToArticleDisplay(int articleDetailId)
    {
        return this.RedirectToAction(nameof(this.Display), "Home", new { area = "Articles", id = articleDetailId });
    }

    [HttpGet("[area]/[controller]/[action]/{articleDetailId:int}/{articleMediaId:int}")]
    public IActionResult Display(int articleDetailId, int articleMediaId)
    {
        if (articleDetailId == default || articleMediaId == default)
        {
            return this.BadRequest();
        }

        return this.DisplayMedia(nameof(this.Display), articleDetailId, articleMediaId);
    }

    [HttpGet]
    public IActionResult New(int id)
    {
        if (id == default)
        {
            return this.BadRequest();
        }

        return this.DisplayMedia(nameof(this.New), articleDetailId: id, articleMediaId: null);
    }

    [HttpPost]
    public IActionResult Add(ArticleMedia model)
    {
        if (model.ArticleMediaId != default || model.ArticleDetailId == default)
        {
            return this.BadRequest();
        }

        return this.AddOrUpdateMedia(model);
    }

    [HttpGet("[area]/[controller]/[action]/{articleDetailId:int}/{articleMediaId:int}")]
    public IActionResult Edit(int articleDetailId, int articleMediaId)
    {
        if (articleDetailId == default || articleMediaId == default)
        {
            return this.BadRequest();
        }

        return this.DisplayMedia(nameof(this.Edit), articleDetailId, articleMediaId);
    }

    [HttpPost]
    public IActionResult Update(ArticleMedia model)
    {
        if (model.ArticleMediaId == default || model.ArticleDetailId == default)
        {
            return this.BadRequest();
        }

        return this.AddOrUpdateMedia(model);
    }

    [HttpGet("[area]/[controller]/[action]/{articleDetailId:int}/{articleMediaId:int}")]
    public IActionResult Delete(int articleDetailId, int articleMediaId)
    {
        if (articleDetailId == default || articleMediaId == default)
        {
            return this.BadRequest();
        }

        return this.DisplayMedia(nameof(this.Delete), articleDetailId, articleMediaId);
    }

    [HttpPost("[area]/[controller]/[action]/{articleDetailId:int}/{articleMediaId:int}")]
    public IActionResult DeletePost(int articleDetailId, int articleMediaId)
    {
        if (articleDetailId == default || articleMediaId == default)
        {
            return this.BadRequest();
        }

        using IAdminContext adminContext = this.AdminContextProvider.Create();
        ArticleMedia? articleMedia = adminContext.ArticleMedias
            .Include(ap => ap.ArticleDetail)
            .SingleOrDefault(ap => ap.ArticleMediaId == articleMediaId && ap.ArticleDetailId == articleDetailId);
        if (articleMedia == null)
        {
            return this.NotFound();
        }

        if (!articleMedia.ArticleDetail!.Status.IsPending())
        {
            this.AddPublishedArticleCannotBeEdited(articleMedia.ArticleDetail);
            return this.RedirectToMediaDisplay(articleMedia.ArticleDetailId, articleMedia.ArticleMediaId);
        }

        var mediaUsingSameFilesQuery = adminContext.ArticleDetails
            .Where(a => a.ArticleId == articleMedia.ArticleDetail.ArticleId)
            .SelectMany(ad => ad.ArticleMedias!)
            .Where(m => m.ArticleMediaGuid == articleMedia.ArticleMediaGuid && m.ArticleMediaId != articleMedia.ArticleMediaId);
        if (!mediaUsingSameFilesQuery.Any())
        {
            var fileName = Path.Combine(articleMedia.GetFileName());
            this.fileStorage.DeleteFile(fileName);
        }

        adminContext.ArticleMedias.Remove(articleMedia);
        adminContext.SaveChanges(this.User);
        this.AddSuccessMessage("Article Media has been deleted.");
        return this.RedirectToArticleDisplay(articleMedia.ArticleDetailId);
    }

    private void AddPublishedArticleCannotBeEdited(ArticleDetail articleDetail)
    {
        this.AddErrorMessage($"{articleDetail.Status.GetDescription()} Article cannot be edited.");
    }

    private (ArticleDetail ArticleDetail, ArticleMedia? ArticleMedia)? GetPageModel(int articleDetailId, int? articleMediaId)
    {
        using IAdminContext adminContext = this.AdminContextProvider.Create();
        ArticleDetail? articleDetail = adminContext.ArticleDetails
            .Include(ad => ad.Area)
            .Include(ad => ad.AreaLayout)
            .Include(ad => ad.Article)
            .Include(ad => ad.ArticleMedias)
            .AsSplitQuery()
            .SingleOrDefault(ad => ad.ArticleDetailId == articleDetailId);
        if (articleDetail == null)
        {
            return null;
        }

        ArticleMedia? articleMedia = null;
        if (articleMediaId != null && articleMediaId.Value != default)
        {
            articleMedia = articleDetail.ArticleMedias!.SingleOrDefault(p => p.ArticleMediaId == articleMediaId.Value);
            if (articleMedia == null)
            {
                return null;
            }
        }

        articleDetail.Article!.PostQuery(this.User);
        return (articleDetail, articleMedia!);
    }

    private IActionResult DisplayMedia(string viewName, int articleDetailId, int? articleMediaId)
    {
        (ArticleDetail ArticleDetail, ArticleMedia? ArticleMedia)? model = this.GetPageModel(articleDetailId, articleMediaId);
        if (model == null)
        {
            return this.NotFound();
        }

        return this.DisplayMedia(viewName, model.Value);
    }

    private IActionResult DisplayMedia(string viewName, (ArticleDetail ArticleDetail, ArticleMedia? ArticleMedia) model)
    {
        if (model.ArticleMedia == null || model.ArticleMedia.ArticleMediaId == default)
        {
            if (!model.ArticleDetail.Status.IsPending())
            {
                this.AddPublishedArticleCannotBeEdited(model.ArticleDetail);
                return this.RedirectToArticleDisplay(model.ArticleDetail.ArticleDetailId);
            }

            if (model.ArticleMedia == null)
            {
                ArticleMedia articleMedia = new ArticleMedia
                {
                    ArticleMediaId = default,
                    ArticleDetailId = model.ArticleDetail.ArticleDetailId,
                    ArticleDetail = model.ArticleDetail,
                };
                return this.View(viewName, (model.ArticleDetail, articleMedia));
            }
        }

        return this.View(viewName, model);
    }

    private IActionResult DisplayMedia(ArticleMedia existingModel, IEnumerable<ValidationResult> validationResults)
    {
        (ArticleDetail ArticleDetail, ArticleMedia? ArticleMedia)? model = this.GetPageModel(existingModel.ArticleDetailId, existingModel.ArticleMediaId);
        if (model == null)
        {
            return this.NotFound();
        }

        this.ModelState.AddValidationResults(validationResults);

        ArticleMedia articleMedia = model.Value.ArticleMedia ?? new ArticleMedia
        {
            ArticleMediaId = default,
            ArticleDetailId = model.Value.ArticleDetail.ArticleDetailId,
            ArticleDetail = model.Value.ArticleDetail,
        };
        articleMedia.MediaUniqueId = existingModel.MediaUniqueId;
        articleMedia.MediaFileName = existingModel.MediaFileName;
        articleMedia.MediaContentType = existingModel.MediaContentType;
        articleMedia.MediaContentDisposition = existingModel.MediaContentDisposition;

        return this.DisplayMedia(
            existingModel.ArticleMediaId == default ? nameof(this.New) : nameof(this.Edit),
            (model.Value.ArticleDetail, articleMedia));
    }

    private IActionResult AddOrUpdateMedia(ArticleMedia model)
    {
        using IAdminContext adminContext = this.AdminContextProvider.Create();
        var articleDetail = adminContext.ArticleDetails
            .Include(ad => ad.ArticleMedias)
            .SingleOrDefault(ad => ad.ArticleDetailId == model.ArticleDetailId);
        if (articleDetail == null)
        {
            return this.NotFound();
        }

        articleDetail.PostQuery(this.User);

        if (!articleDetail.Status.IsPending())
        {
            this.AddPublishedArticleCannotBeEdited(articleDetail);
            return this.RedirectToArticleDisplay(articleDetail.ArticleDetailId);
        }

        ArticleMedia? articleMedia;
        if (model.ArticleMediaId == default)
        {
            articleMedia = new ArticleMedia
            {
                ArticleMediaGuid = Guid.NewGuid(),
                ArticleDetailId = articleDetail.ArticleDetailId,
            };

            articleMedia.TrackedEntity.SetCreated(this.UserNow, this.UserId);
            articleDetail.ArticleMedias!.Add(articleMedia);
        }
        else
        {
            articleMedia = articleDetail.ArticleMedias!.SingleOrDefault(ap => ap.ArticleMediaId == model.ArticleMediaId);
            if (articleMedia == null)
            {
                return this.NotFound();
            }
        }

        articleMedia.MediaFileName = model.MediaFileName;
        articleMedia.MediaUniqueId = model.MediaUniqueId;
        articleMedia.MediaContentDisposition = model.MediaContentDisposition;
        articleMedia.MediaContentType = model.MediaContentType;
        articleMedia.TrackedEntity.SetLastUpdated(this.UserNow, this.UserId);

        var validationResults = new List<ValidationResult>();
        articleMedia.Validate(validationResults);

        var duplicateUniqueIDsFound = articleDetail.ArticleMedias!.GroupBy(ap => ap.MediaUniqueId.ToLower()).Any(g => g.Count() > 1);
        if (duplicateUniqueIDsFound)
        {
            validationResults.Add(new ValidationResult("Media Unique Id is already in use.", new[] { nameof(ArticleMedia.MediaUniqueId) }));
        }

        if (articleMedia.Id == default && model.FormFile == null)
        {
            validationResults.Add(new ValidationResult("This field is required.", new[] { nameof(model.FormFile) }));
        }

        if (validationResults.IsNotValid())
        {
            return this.DisplayMedia(model, validationResults);
        }

        if (model.FormFile != null)
        {
            this.fileStorage.SaveFile(articleMedia.GetFileName(articleDetail), model.FormFile);
        }

        adminContext.SaveChanges(this.User);
        this.AddSuccessMessage($"Article Media has been {(model.ArticleMediaId == default ? "added" : "updated")}.");
        return this.RedirectToMediaDisplay(articleMedia.ArticleDetailId, articleMedia.ArticleMediaId);
    }
}