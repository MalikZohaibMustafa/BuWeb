using Bu.AspNetCore.Core.Extensions;
using Bu.Web.Data.Abstraction;
using Bu.Web.Data.Entities;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bu.Web.Common;

public static class RenderingEngine
{
    public static IHtmlContent Render(IHtmlHelper html, ArticlePage articlePage)
    {
        return articlePage.PageBodyTextFormat switch
        {
            RichTextFormats.Html => html.Raw(articlePage.PageBody),
            RichTextFormats.Markdown => html.Raw(MarkdownParser.ToHtml(articlePage.PageBody)),
            _ => throw new ArgumentOutOfRangeException(articlePage.PageBodyTextFormat.ToString()),
        };
    }

    public static IHtmlContent RenderMenu(IUrlHelper url, ArticlePage articlePage)
    {
        TagBuilder ul = new TagBuilder("ul");
        ul.AddCssClass("nav flex-column ms-2");
        List<ArticlePage> rootArticlePages = articlePage.ArticleDetail!.ArticlePages!.Where(p => p.ParentArticlePageId == null).OrderBy(p => p.PageSequence).ToList();
        foreach (ArticlePage rootArticlePage in rootArticlePages)
        {
            ul.InnerHtml.AppendHtml(RenderMenuItem(url, rootArticlePage));
        }

        return ul;
    }

    private static IHtmlContent RenderMenuItem(IUrlHelper url, ArticlePage articlePage)
    {
        List<ArticlePage> childArticlePages = articlePage.ArticleDetail!.ArticlePages!.Where(p => p.ParentArticlePageId == articlePage.ArticlePageId).OrderBy(p => p.PageSequence).ToList();
        TagBuilder li = new TagBuilder("li");
        li.AddCssClass("nav-item");
        if (childArticlePages.Count >= 1)
        {
            TagBuilder a = new TagBuilder("a");
            a.AddCssClass("nav-link");
            a.MergeAttribute("href", "#");
            a.MergeAttribute("data-bs-toggle", "collapse");
            a.MergeAttribute("data-bs-target", $"#links_{articlePage.ArticlePageId}");
            a.MergeAttribute("aria-expanded", "true");
            TagBuilder icon = new TagBuilder("i");
            icon.AddCssClass("bi bi-chevron-down me-2");
            a.InnerHtml.AppendHtml(icon);
            _ = a.InnerHtml.Append(articlePage.MenuText ?? articlePage.PageTitle);
            _ = li.InnerHtml.AppendHtml(a);

            TagBuilder ul = new TagBuilder("ul");
            ul.AddCssClass("nav flex-column ms-2 collapse show");
            ul.MergeAttribute("id", $"links_{articlePage.ArticlePageId}");
            foreach (ArticlePage articlePage1 in childArticlePages)
            {
                ul.InnerHtml.AppendHtml(RenderMenuItem(url, articlePage1));
            }

            _ = li.InnerHtml.AppendHtml(ul);
        }
        else
        {
            TagBuilder a = new TagBuilder("a");
            a.AddCssClass("nav-link");
            string href = url.Content(articlePage.ArticleDetail!.Area!.ParentAreaId == null
                ? $"~/Articles/{articlePage.ArticleDetail.ArticleUniqueId.UrlEncode()}/{articlePage.PageUniqueId.UrlEncode()}"
                : $"~/{articlePage.ArticleDetail.Area.AreaName}/Articles/{articlePage.ArticleDetail.ArticleUniqueId.UrlEncode()}/{articlePage.PageUniqueId.UrlEncode()}");
            a.MergeAttribute("href", href);
            _ = a.InnerHtml.Append(articlePage.MenuText ?? articlePage.PageTitle);
            _ = li.InnerHtml.AppendHtml(a);
        }

        return li;
    }
}

/*
 <div class="flex-shrink-0 p-3 bg-white" style="width: 280px;">
    <a href="/" class="d-flex align-items-center pb-3 mb-3 link-dark text-decoration-none border-bottom">
      <svg class="bi pe-none me-2" width="30" height="24"><use xlink:href="#bootstrap"></use></svg>
      <span class="fs-5 fw-semibold">Collapsible</span>
    </a>
    <ul class="list-unstyled ps-0">
      <li class="mb-1">
        <button class="btn btn-toggle d-inline-flex align-items-center rounded border-0" data-bs-toggle="collapse" data-bs-target="#home-collapse" aria-expanded="true">
          Home
        </button>
        <div class="collapse show" id="home-collapse" style="">
          <ul class="btn-toggle-nav list-unstyled fw-normal pb-1 small">
            <li><a href="#" class="link-dark d-inline-flex text-decoration-none rounded">Overview</a></li>
            <li><a href="#" class="link-dark d-inline-flex text-decoration-none rounded">Updates</a></li>
            <li><a href="#" class="link-dark d-inline-flex text-decoration-none rounded">Reports</a></li>
          </ul>
        </div>
      </li>
      <li class="mb-1">
        <button class="btn btn-toggle d-inline-flex align-items-center rounded border-0 collapsed" data-bs-toggle="collapse" data-bs-target="#dashboard-collapse" aria-expanded="false">
          Dashboard
        </button>
        <div class="collapse" id="dashboard-collapse" style="">
          <ul class="btn-toggle-nav list-unstyled fw-normal pb-1 small">
            <li><a href="#" class="link-dark d-inline-flex text-decoration-none rounded">Overview</a></li>
            <li><a href="#" class="link-dark d-inline-flex text-decoration-none rounded">Weekly</a></li>
            <li><a href="#" class="link-dark d-inline-flex text-decoration-none rounded">Monthly</a></li>
            <li><a href="#" class="link-dark d-inline-flex text-decoration-none rounded">Annually</a></li>
          </ul>
        </div>
      </li>
      <li class="mb-1">
        <button class="btn btn-toggle d-inline-flex align-items-center rounded border-0 collapsed" data-bs-toggle="collapse" data-bs-target="#orders-collapse" aria-expanded="false">
          Orders
        </button>
        <div class="collapse" id="orders-collapse">
          <ul class="btn-toggle-nav list-unstyled fw-normal pb-1 small">
            <li><a href="#" class="link-dark d-inline-flex text-decoration-none rounded">New</a></li>
            <li><a href="#" class="link-dark d-inline-flex text-decoration-none rounded">Processed</a></li>
            <li><a href="#" class="link-dark d-inline-flex text-decoration-none rounded">Shipped</a></li>
            <li><a href="#" class="link-dark d-inline-flex text-decoration-none rounded">Returned</a></li>
          </ul>
        </div>
      </li>
      <li class="border-top my-3"></li>
      <li class="mb-1">
        <button class="btn btn-toggle d-inline-flex align-items-center rounded border-0 collapsed" data-bs-toggle="collapse" data-bs-target="#account-collapse" aria-expanded="false">
          Account
        </button>
        <div class="collapse" id="account-collapse">
          <ul class="btn-toggle-nav list-unstyled fw-normal pb-1 small">
            <li><a href="#" class="link-dark d-inline-flex text-decoration-none rounded">New...</a></li>
            <li><a href="#" class="link-dark d-inline-flex text-decoration-none rounded">Profile</a></li>
            <li><a href="#" class="link-dark d-inline-flex text-decoration-none rounded">Settings</a></li>
            <li><a href="#" class="link-dark d-inline-flex text-decoration-none rounded">Sign out</a></li>
          </ul>
        </div>
      </li>
    </ul>
  </div>
 */