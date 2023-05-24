namespace Bu.Web.Website.Models;

public sealed class FooterLinks
{
    public Links Left { get; set; } = new Links();

    public Links Right { get; set; } = new Links();

    public sealed class Links
    {
        public string HeaderHtml { get; set; } = string.Empty;

        public IEnumerable<Link> List { get; set; } = Array.Empty<Link>();
    }
}