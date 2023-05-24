namespace Bu.Web.Website.Models;

public class Announcement
{
    public bool Active { get; set; }

    public string TitleHtml { get; set; } = string.Empty;

    public string BodyHtml { get; set; } = string.Empty;

    public string NavigateUrl { get; set; } = string.Empty;

    public string Target { get; set; } = "_self";
}