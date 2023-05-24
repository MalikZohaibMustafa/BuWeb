using Bu.AspNetCore.Core.Extensions;
using Bu.Web.Data.Abstraction;
using Bu.Web.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bu.Web.Admin.Tests;

[TestClass]
public abstract class TestsArticlesBase
{
    protected static Lazy<ArticleDetail> GetLazyArticleDetail(int areaLayoutId, string articleUnqiueId)
    {
        return new Lazy<ArticleDetail>(() =>
        {
            using IAdminContext adminContext = TestHelper.AdminContextProvider.Create();
            ArticleDetail articleDetail = adminContext.ArticleDetails.Single(x => x.AreaLayoutId == areaLayoutId && x.ArticleUniqueId == articleUnqiueId);
            articleDetail.PostQuery(TestHelper.SuperAdministratorClaimPrincipal);
            return articleDetail;
        });
    }

    [AssertionMethod]
    protected static int AddArticleDetail(
        AreaLayout areaLayout,
        string articleName,
        ArticleDetail.ArticleTypes articleType,
        string articleUniqueId,
        string? articleShortDescription = null,
        DateTime? eventStartDate = null,
        DateTime? eventEndDate = null,
        DateTime? expiryDateUser = null,
        string remarks = "Test Remarks.")
    {
        using Areas.Articles.Controllers.HomeController articleHomeController = TestHelper.CreateController(
            () => new Areas.Articles.Controllers.HomeController(
                TestHelper.CreateLogger<Areas.Articles.Controllers.HomeController>(),
                TestHelper.AdminContextProvider,
                TestHelper.AdminFixedContext));

        IActionResult? actionResult = null;

        RedirectToActionResult GetRedirectToActionResult()
        {
            return actionResult as RedirectToActionResult ?? throw new InvalidOperationException(actionResult.GetType().FullName);
        }

        actionResult = articleHomeController.Add(new ArticleDetail
        {
            AreaId = areaLayout.AreaId,
            AreaLayoutId = areaLayout.AreaLayoutId,
            ArticleName = articleName,
            ArticleType = articleType,
            ArticleUniqueId = articleUniqueId,
            ExpiryDateUser = expiryDateUser,
            Remarks = remarks,
            ArticleShortDescription = articleShortDescription,
            EventStartDate = eventStartDate,
            EventEndDate = eventEndDate,
        });

        return GetRedirectToActionResult().RouteValues?["id"]?.ToString()?.ToInt() ?? throw new InvalidOperationException();
    }

    [AssertionMethod]
    protected static int UpdateArticleDetail(
        ArticleDetail articleDetail,
        int? defaultArticlePageId = null,
        int? carouselMediaId = null,
        int? smallCarouselArticleMediaId = null)
    {
        using Areas.Articles.Controllers.HomeController articleHomeController = TestHelper.CreateController(
            () => new Areas.Articles.Controllers.HomeController(
                TestHelper.CreateLogger<Areas.Articles.Controllers.HomeController>(),
                TestHelper.AdminContextProvider,
                TestHelper.AdminFixedContext));

        IActionResult? actionResult = null;

        RedirectToActionResult GetRedirectToActionResult()
        {
            return actionResult as RedirectToActionResult ?? throw new InvalidOperationException(actionResult.GetType().FullName);
        }

        actionResult = articleHomeController.Update(new ArticleDetail
        {
            ArticleDetailId = articleDetail.ArticleDetailId,
            ArticleId = articleDetail.ArticleId,
            AreaLayoutId = articleDetail.AreaLayoutId,
            ArticleName = articleDetail.ArticleName,
            ArticleUniqueId = articleDetail.ArticleUniqueId,
            ArticleType = articleDetail.ArticleType,
            ExpiryDateUser = articleDetail.ExpiryDateUser,
            DefaultArticlePageId = defaultArticlePageId,
            CarouselArticleMediaId = carouselMediaId,
            SmallCarouselArticleMediaId = smallCarouselArticleMediaId,
            EventStartDate = articleDetail.EventStartDate,
            EventEndDate = articleDetail.EventEndDate,
            ArticleShortDescription = articleDetail.ArticleShortDescription,
            Remarks = articleDetail.Remarks,
        });

        return GetRedirectToActionResult().RouteValues?["id"]?.ToString()?.ToInt() ?? throw new InvalidOperationException();
    }

    [AssertionMethod]
    protected static int AddArticlePage(
        int articleDetailId,
        RichTextFormats pageBodyTextFormat,
        string? menuText,
        string pageBody,
        string pageTitle,
        string pageUniqueId)
    {
        using Areas.Articles.Controllers.PageController pageController = TestHelper.CreateController(
            () => new Areas.Articles.Controllers.PageController(
                TestHelper.CreateLogger<Areas.Articles.Controllers.PageController>(),
                TestHelper.AdminContextProvider));

        IActionResult? actionResult = null;

        RedirectToActionResult GetRedirectToActionResult()
        {
            return actionResult as RedirectToActionResult ?? throw new InvalidOperationException(actionResult.GetType().FullName);
        }

        actionResult = pageController.Add(new ArticlePage
        {
            ArticleDetailId = articleDetailId,
            PageBodyTextFormat = pageBodyTextFormat,
            MenuText = menuText,
            PageBody = pageBody,
            PageTitle = pageTitle,
            PageUniqueId = pageUniqueId,
        });

        return GetRedirectToActionResult().RouteValues?["articlePageId"]?.ToString()?.ToInt() ?? throw new InvalidOperationException();
    }

    [AssertionMethod]
    protected static int AddArticleMedia(
        int articleDetailId,
        Stream stream,
        ContentDisposition contentDisposition,
        ContentTypes mediaContentType,
        string mediaFileName,
        string mediaUniqueId)
    {
        using Areas.Articles.Controllers.MediaController mediaController = TestHelper.CreateController(
            () => new Areas.Articles.Controllers.MediaController(
                TestHelper.CreateLogger<Areas.Articles.Controllers.MediaController>(),
                TestHelper.AdminContextProvider,
                TestHelper.FileStorage));

        IActionResult? actionResult = null;

        RedirectToActionResult GetRedirectToActionResult()
        {
            return actionResult as RedirectToActionResult ?? throw new InvalidOperationException(actionResult.GetType().FullName);
        }

        actionResult = mediaController.Add(new ArticleMedia
        {
            ArticleDetailId = articleDetailId,
            MediaContentDisposition = contentDisposition,
            MediaContentType = mediaContentType,
            MediaUniqueId = mediaUniqueId,
            MediaFileName = mediaFileName,
            FormFile = new FormFile(stream, 0, stream.Length, mediaFileName, mediaFileName),
        });
        return GetRedirectToActionResult().RouteValues?["articleMediaId"]?.ToString()?.ToInt() ?? throw new InvalidOperationException();
    }

    [AssertionMethod]
    protected static int PublishArticleDetail(int articleDetailId)
    {
        using Areas.Articles.Controllers.HomeController articleHomeController = TestHelper.CreateController(
            () => new Areas.Articles.Controllers.HomeController(
                TestHelper.CreateLogger<Areas.Articles.Controllers.HomeController>(),
                TestHelper.AdminContextProvider,
                TestHelper.AdminFixedContext));

        IActionResult? actionResult = null;

        RedirectToActionResult GetRedirectToActionResult()
        {
            return actionResult as RedirectToActionResult ?? throw new InvalidOperationException(actionResult.GetType().FullName);
        }

        actionResult = articleHomeController.PublishPost(articleDetailId);

        return GetRedirectToActionResult().RouteValues?["id"]?.ToString()?.ToInt() ?? throw new InvalidOperationException();
    }
}