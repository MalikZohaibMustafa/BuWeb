namespace Bu.Web.Website.Models.Controls;

public sealed class BootstrapCarouselItem
{
    public bool Active { get; set; }

    public string? Label { get; init; }

    public Func<IHtmlHelper, IHtmlContent>? CaptionHtml { get; init; }

    public string NavigationUrl { get; init; } = string.Empty;

    public Func<IHtmlHelper, IHtmlContent> InnerHtml { get; init; } = _ => new TagBuilder("div");

    public int? Interval { get; init; }
}