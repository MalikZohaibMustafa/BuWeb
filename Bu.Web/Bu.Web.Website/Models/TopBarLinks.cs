using Bu.Web.Website.Models.Controls;

namespace Bu.Web.Website.Models;

public sealed class TopBarLinks
{
    public IEnumerable<Link> Buttons { get; init; } = new List<Link>();

    public IEnumerable<Link> Links { get; init; } = new List<Link>();

    public bool SearchEnabled { get; init; }

    public string SearchDomainUrl { get; init; } = string.Empty;
}