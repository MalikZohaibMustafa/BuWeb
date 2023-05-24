using Bu.Web.Data.Abstraction;
using Bu.Web.Data.Entities;

namespace Bu.Web.Admin.Tests.Articles.Buho;

[TestClass]
public sealed class TestEvents : TestsArticlesBase
{
    [TestMethod]
    public void TestEvent1()
    {
        int articleDetailId = AddArticleDetail(
            Tests07AreaLayouts.AreaLayoutBuho.Value,
            articleName: "25th Convocation Ceremony Schedule – BUIC",
            ArticleDetail.ArticleTypes.Event,
            articleUniqueId: "25th-Convocation-Ceremony-Schedule–BUIC",
            articleShortDescription: "25th Convocation Ceremony Schedule – BUIC",
            eventStartDate: DateTime.Today,
            eventEndDate: DateTime.Today.AddDays(3));
        int articlePageId = AddArticlePage(
              articleDetailId,
              RichTextFormats.Markdown,
              menuText: null,
              pageBody: "Dear Graduating Students,\r\n\r\n \r\n\r\nTwenty-fifth convocation ceremony of Bahria University Islamabad Campus is scheduled to be conducted as under:\r\n\r\n",
              pageTitle: "25th Convocation Ceremony Schedule – BUIC",
              pageUniqueId: "Schedule");
        using Stream image = File.OpenRead("Carousel-1920x600px.jpg");
        int carouselArticleMediaId = AddArticleMedia(
            articleDetailId,
            image,
            ContentDisposition.Inline,
            ContentTypes.Jpeg,
            "Banner.jpeg",
            "Banner");

        using IAdminContext context = TestHelper.AdminContextProvider.Create();
        var articleDetail = context.ArticleDetails.Single(x => x.ArticleDetailId == articleDetailId);
        UpdateArticleDetail(
            articleDetail,
            articlePageId,
            carouselMediaId: carouselArticleMediaId,
            smallCarouselArticleMediaId: carouselArticleMediaId);
        PublishArticleDetail(articleDetailId);
    }

    [TestMethod]
    public void TestEvent2()
    {
        int articleDetailId = AddArticleDetail(
            Tests07AreaLayouts.AreaLayoutBuho.Value,
            articleName: "25th Convocation Ceremony Schedule 2 – BUIC",
            ArticleDetail.ArticleTypes.Event,
            articleUniqueId: "25th-Convocation-Ceremony-Schedule-2–BUIC",
            articleShortDescription: "25th Convocation Ceremony Schedule – BUIC",
            eventStartDate: DateTime.Today,
            eventEndDate: DateTime.Today.AddDays(3));
        int articlePageId = AddArticlePage(
              articleDetailId,
              RichTextFormats.Markdown,
              menuText: null,
              pageBody: "Dear Graduating Students,\r\n\r\n \r\n\r\nTwenty-fifth convocation ceremony of Bahria University Islamabad Campus is scheduled to be conducted as under:\r\n\r\n",
              pageTitle: "25th Convocation Ceremony Schedule – BUIC",
              pageUniqueId: "Schedule");
        using Stream image = File.OpenRead("Carousel-1920x600px.jpg");
        int carouselArticleMediaId = AddArticleMedia(
            articleDetailId,
            image,
            ContentDisposition.Inline,
            ContentTypes.Jpeg,
            "Banner.jpeg",
            "Banner");

        using IAdminContext context = TestHelper.AdminContextProvider.Create();
        ArticleDetail articleDetail = context.ArticleDetails.Single(x => x.ArticleDetailId == articleDetailId);
        UpdateArticleDetail(
            articleDetail,
            articlePageId,
            carouselMediaId: carouselArticleMediaId,
            smallCarouselArticleMediaId: carouselArticleMediaId);
        PublishArticleDetail(articleDetailId);
    }
}
