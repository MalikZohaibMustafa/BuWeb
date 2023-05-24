using Bu.Web.Data.Entities;

namespace Bu.Web.Admin.Tests.Articles.Buho;

[TestClass]
public sealed class TestAbout : TestsArticlesBase
{
    public static readonly Lazy<ArticleDetail> ArticleDetailAbout = GetLazyArticleDetail(Tests07AreaLayouts.AreaLayoutBuho.Value.Id, "About");

    [ClassInitialize]
    public static void ClassInit(TestContext context)
    {
        AddArticleDetail(
            Tests07AreaLayouts.AreaLayoutBuho.Value,
            articleName: "About Us",
            ArticleDetail.ArticleTypes.Static,
            articleUniqueId: "About");
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        PublishArticleDetail(ArticleDetailAbout.Value.Id);
    }

    [TestMethod]
    public void RectorMessage()
    {
        int articlePageId = AddArticlePage(
              ArticleDetailAbout.Value.Id,
              Data.Abstraction.RichTextFormats.Markdown,
              menuText: "Rector's Welcome Message",
              pageBody: Properties.Resources.Buho_About_RectorMessage,
              pageTitle: "Rector's Welcome Message",
              pageUniqueId: "RectorMessage");

        UpdateArticleDetail(ArticleDetailAbout.Value, articlePageId, null);
    }

    [TestMethod]
    public void WhyBahria()
    {
        int articlePageId = AddArticlePage(
              ArticleDetailAbout.Value.Id,
              Data.Abstraction.RichTextFormats.Markdown,
              menuText: "Why Bahria",
              pageBody: Properties.Resources.Buho_About_WhyBahria,
              pageTitle: "Why Bahria",
              pageUniqueId: "WhyBahria");

        UpdateArticleDetail(ArticleDetailAbout.Value, articlePageId, null);
    }

    [TestMethod]
    public void VisionAndMission()
    {
        int articlePageId = AddArticlePage(
              ArticleDetailAbout.Value.Id,
              Data.Abstraction.RichTextFormats.Markdown,
              menuText: "Vision & Mission",
              pageBody: Properties.Resources.Buho_About_VisionAndMission,
              pageTitle: "Vision & Mission",
              pageUniqueId: "VisionAndMission");

        UpdateArticleDetail(ArticleDetailAbout.Value, articlePageId, null);
    }
}
