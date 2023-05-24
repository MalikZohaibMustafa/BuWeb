namespace Bu.Web.Website.Models;

public class ImageModel
{
    public string CssClass { get; set; } = string.Empty;

    public string Image { get; set; } = string.Empty;

    public string NavigateUrl { get; set; } = string.Empty;

    public string Target { get; set; } = "_self";

    public bool Active { get; set; } = false;
}