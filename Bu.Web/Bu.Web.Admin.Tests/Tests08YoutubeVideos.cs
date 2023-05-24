using Bu.Web.Data.Abstraction;
using Bu.Web.Data.Entities;

namespace Bu.Web.Admin.Tests;

[TestClass]
public sealed class Tests08YoutubeVideos
{
    private static ArticleDetail GetAtricleDetail(int areaId, string articleUniqueId)
    {
        using IAdminContext adminContext = TestHelper.AdminContextProvider.Create();
        return adminContext.ArticleDetails.Single(x => x.AreaId == areaId && x.ArticleUniqueId == articleUniqueId);
    }

    public static ArticleDetail ArticleDetailBuhoArticle1 => GetAtricleDetail(Tests06Areas.AreaBuho.Value.Id, "Article1");

    [ClassInitialize]
    public static void CLassInit(TestContext context)
    {
        using IAdminContext adminContext = TestHelper.AdminContextProvider.Create();

        void AddYoutubeVideo(Area area, string youtubeUrl, string remarks)
        {
            YoutubeVideo youtubeVideo = adminContext.YoutubeVideos.Add(new YoutubeVideo
            {
                AreaId = area.AreaId,
                YoutubeUrl = youtubeUrl,
            }).Entity;
            youtubeVideo.Published.SetPublishedStatus(IPublished.Statuses.Published, TestHelper.SuperAdministratorUserNow, TestHelper.SuperAdministratorUserId, remarks);
            youtubeVideo.Published.SetCreated(TestHelper.SuperAdministratorUserNow, TestHelper.SuperAdministratorUserId);
            youtubeVideo.Published.SetExpiry(null);
        }

        AddYoutubeVideo(Tests06Areas.AreaBuho.Value, "https://www.youtube.com/embed/qTfoMGNzrck", "Test remarks for youtube video 1.");
        AddYoutubeVideo(Tests06Areas.AreaBuho.Value, "https://www.youtube.com/embed/OXQOvRJRET4", "Test remarks for youtube video 2.");
        AddYoutubeVideo(Tests06Areas.AreaBuho.Value, "https://www.youtube.com/embed/P1x-xhfzPm0", "Test remarks for youtube video 3.");
        AddYoutubeVideo(Tests06Areas.AreaBuho.Value, "https://www.youtube.com/embed/qTfoMGNzrck&", "Test remarks for youtube video 4.");
        AddYoutubeVideo(Tests06Areas.AreaBuho.Value, "https://www.youtube.com/embed/OXQOvRJRET4&", "Test remarks for youtube video 5.");
        AddYoutubeVideo(Tests06Areas.AreaBuho.Value, "https://www.youtube.com/embed/P1x-xhfzPm0&", "Test remarks for youtube video 6.");
        adminContext.SaveChanges(TestHelper.SuperAdminUserClaimsPrincipal);
    }

    [TestMethod]
    public void AreaDataCheck()
    {
        using IAdminContext adminContext = TestHelper.AdminContextProvider.Create();
        Assert.IsTrue(adminContext.YoutubeVideos.Any());
    }
}