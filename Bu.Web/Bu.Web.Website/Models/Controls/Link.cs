namespace Bu.Web.Website.Models.Controls;

public class Link
{
    public string InnerHtml { get; set; } = string.Empty;

    public string NavigateUrl { get; set; } = string.Empty;

    public LinkTargets LinkTarget { get; set; } = LinkTargets.Self;

    public string Target => this.LinkTarget switch
    {
        LinkTargets.Self => "_self",
        LinkTargets.Blank => "_blank",
        _ => throw new InvalidOperationException(this.LinkTarget.ToString()),
    };
}