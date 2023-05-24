namespace Bu.Web.Website.Models.Controls;

public readonly struct Icon
{
    public Icon(string iconCssClass)
    {
        this.IconCssClass = iconCssClass;
    }

    public string IconCssClass { get; init; }

    public string Html => $"<i class=\"{this.IconCssClass}\"></i>";

    public static implicit operator string(Icon icon) => icon.Html;

    public static explicit operator Icon(string iconCssClass) => new Icon(iconCssClass);

    public override string ToString()
    {
        return this.Html;
    }
}