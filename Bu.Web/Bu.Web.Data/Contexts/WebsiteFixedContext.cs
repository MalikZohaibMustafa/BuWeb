namespace Bu.Web.Data.Contexts;

public sealed class WebsiteFixedContext : CommonFixedContext<IWebsiteContext>, IWebsiteFixedContext
{
    public WebsiteFixedContext(WebsiteContextProvider websiteContextProvider)
        : base(websiteContextProvider.Create)
    {
    }
}