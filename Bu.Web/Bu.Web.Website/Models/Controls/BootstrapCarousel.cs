using Bu.AspNetCore.Core.Helpers;

namespace Bu.Web.Website.Models.Controls;

public sealed class BootstrapCarousel
{
    public enum DisplayFormats
    {
        SlidesOnly,
        WithControls,
        WithIndicators,
    }

    public enum Rides
    {
        Carousel,
        True,
        False,
    }

    public string? Id { get; set; }

    public bool Fade { get; init; }

    public DisplayFormats DisplayFormat { get; init; } = DisplayFormats.SlidesOnly;

    public Rides Ride { get; init; } = Rides.Carousel;

    public bool DisableTouchSwiping { get; init; }

    public bool Dark { get; init; }

    public List<BootstrapCarouselItem> Items { get; init; } = new List<BootstrapCarouselItem>();

    public IHtmlContent Render(IHtmlHelper htmlHelper)
    {
        if (!this.Items.Any())
        {
            return string.Empty.ToHtmlContent();
        }

        if (string.IsNullOrEmpty(this.Id))
        {
            this.Id = "carousel" + Guid.NewGuid().ToString("N");
        }

        int activeCount = this.Items.Count(i => i.Active);
        if (activeCount == 0)
        {
            this.Items.First().Active = true;
        }
        else if (activeCount > 1)
        {
            foreach (var item in this.Items.Where(i => i.Active).Skip(1))
            {
                item.Active = false;
            }
        }

        TagBuilder div = new TagBuilder("div");
        if (this.Id != null)
        {
            div.MergeAttribute("id", this.Id);
        }

        div.AddCssClass("carousel slide");
        if (this.Fade)
        {
            div.AddCssClass("carousel-fade");
        }

        div.MergeAttribute("data-ride", this.Ride.ToString().ToLowerInvariant());
        if (this.DisableTouchSwiping)
        {
            div.MergeAttribute("data-bs-touch", this.DisableTouchSwiping.ToString().ToLowerInvariant());
        }

        if (this.Dark)
        {
            div.AddCssClass("carousel-dark");
        }

        IHtmlContent RenderItem(BootstrapCarouselItem carouselItem)
        {
            TagBuilder itemDiv = new TagBuilder("div");
            itemDiv.AddCssClass("carousel-item");
            if (carouselItem.Active)
            {
                itemDiv.AddCssClass("active");
            }

            if (carouselItem.Interval != null)
            {
                itemDiv.MergeAttribute("data-bs-interval", carouselItem.Interval.ToString());
            }

            itemDiv.InnerHtml.AppendHtml(carouselItem.InnerHtml(htmlHelper));

            if (carouselItem.CaptionHtml != null)
            {
                TagBuilder captionDiv = new TagBuilder("div");
                captionDiv.AddCssClass("carousel-caption d-none d-md-block");
                captionDiv.InnerHtml.AppendHtml(carouselItem.CaptionHtml(htmlHelper));
                itemDiv.InnerHtml.AppendHtml(captionDiv);
            }

            return itemDiv;
        }

        IHtmlContent GetCarouselInner()
        {
            TagBuilder carouselInner = new TagBuilder("div");
            carouselInner.AddCssClass("carousel-inner");

            foreach (BootstrapCarouselItem item in this.Items)
            {
                carouselInner.InnerHtml.AppendHtml(RenderItem(item));
            }

            return carouselInner;
        }

        IHtmlContent GetButton(bool previous)
        {
            string type = previous ? "Previous" : "Next";

            TagBuilder button = new TagBuilder("button");
            button.AddCssClass($"carousel-control-{type.ToLower()[..4]}");
            button.MergeAttribute("type", "button");
            button.MergeAttribute("data-bs-target", "#" + this.Id);
            button.MergeAttribute("data-bs-slide", type.ToLower()[..4]);

            TagBuilder icon = new TagBuilder("span");
            icon.AddCssClass($"carousel-control-{type.ToLower()[..4]}-icon");
            icon.MergeAttribute("aria-hidden", "true");

            TagBuilder span = new TagBuilder("span");
            span.AddCssClass("visually-hidden");
            span.InnerHtml.Append(type);

            button.InnerHtml.AppendHtml(icon);
            button.InnerHtml.AppendHtml(span);
            return button;
        }

        IHtmlContent GetIndicators()
        {
            TagBuilder indicators = new TagBuilder("div");
            indicators.AddCssClass("carousel-indicators");

            for (var i = 0; i < this.Items.Count; i++)
            {
                var item = this.Items[i];
                TagBuilder button = new TagBuilder("button");
                button.MergeAttribute("type", "button");
                button.MergeAttribute("data-bs-target", "#" + this.Id);
                button.MergeAttribute("data-bs-slide-to", i.ToString());
                if (item.Active)
                {
                    button.AddCssClass("active");
                    button.MergeAttribute("aria-current", "true");
                    if (!string.IsNullOrWhiteSpace(item.Label))
                    {
                        button.MergeAttribute("aria-label", item.Label);
                    }
                }

                indicators.InnerHtml.AppendHtml(button);
            }

            return indicators;
        }

        switch (this.DisplayFormat)
        {
            case DisplayFormats.SlidesOnly:
                div.InnerHtml.AppendHtml(GetCarouselInner());
                break;
            case DisplayFormats.WithControls:
                div.InnerHtml.AppendHtml(GetCarouselInner());
                if (this.Items.Count > 1)
                {
                    div.InnerHtml.AppendHtml(GetButton(false));
                    div.InnerHtml.AppendHtml(GetButton(true));
                }

                break;
            case DisplayFormats.WithIndicators:
                div.InnerHtml.AppendHtml(GetIndicators());
                div.InnerHtml.AppendHtml(GetCarouselInner());
                if (this.Items.Count > 1)
                {
                    div.InnerHtml.AppendHtml(GetButton(false));
                    div.InnerHtml.AppendHtml(GetButton(true));
                }

                break;
            default:
                throw new InvalidOperationException(this.DisplayFormat.ToString());
        }

        return div;
    }
}