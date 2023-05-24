using Bu.Web.Data.Abstraction;
using Bu.Web.Data.Entities;

namespace Bu.Web.Admin.Tests.Articles.Buho;

[TestClass]
public sealed class TestNews : TestsArticlesBase
{
    [DataTestMethod]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    [DataRow(4)]
    [DataRow(5)]
    [DataRow(6)]
    public void TestNews_(int index)
    {
        int articleDetailId = AddArticleDetail(
            Tests07AreaLayouts.AreaLayoutBuho.Value,
            articleName: $"25th Convocation Ceremony Schedule {index} – BUIC",
            ArticleDetail.ArticleTypes.News,
            articleUniqueId: $"25th-Convocation-Ceremony-Schedule-{index}–BUIC",
            articleShortDescription: $"25th Convocation Ceremony Schedule {index} – BUIC",
            eventStartDate: DateTime.Today.AddDays(index),
            eventEndDate: DateTime.Today.AddDays(index).AddDays(3),
            expiryDateUser: new DateTime(2025, 12, 31, 0, 0, 0, DateTimeKind.Unspecified));
        int articlePageId = AddArticlePage(
              articleDetailId,
              RichTextFormats.Markdown,
              menuText: null,
              pageBody: $"Dear Graduating Students,\r\n\r\n \r\n\r\nTwenty-fifth {index} convocation ceremony of Bahria University Islamabad Campus is scheduled to be conducted as under:\r\n\r\n",
              pageTitle: $"25th Convocation Ceremony Schedule {index} – BUIC",
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
        articleDetail.PostQuery(TestHelper.SuperAdministratorClaimPrincipal);
        UpdateArticleDetail(
            articleDetail,
            articlePageId,
            carouselMediaId: carouselArticleMediaId,
            smallCarouselArticleMediaId: carouselArticleMediaId);
        PublishArticleDetail(articleDetailId);
    }
}
